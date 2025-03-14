using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using ReadOn.DbContexts;
using ReadOn.Helpers;
using ReadOn.Models;
using ReadOn.Shared;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ReadOn.Services
{
    public class AuthService : IAuthService
    {
        private readonly IEmailService _emailService;
        private readonly MyDbContext _dbContext;
        private readonly SignInManager<ApplicationAccount> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationAccount> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly ILoanPreviewService _loanPreviewService;
        public AuthService(MyDbContext dbContext,
            IConfiguration configuration,
            SignInManager<ApplicationAccount> signInManager,
            UserManager<ApplicationAccount> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            IEmailService emailService,
            ILoanPreviewService loanPreviewService
            )
        {
            _signInManager = signInManager;
            _dbContext = dbContext;
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;
            _emailService = emailService;
            _loanPreviewService = loanPreviewService;
        }

        public async Task<(string accessToken, string refreshToken)> SigninAsync(Signin model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);

            if (user == null || !passwordValid)
            {
                throw new UnauthorizedAccessException("Invalid username or password");
            }

            var accessToken = await GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(5);
            await _userManager.UpdateAsync(user);

            return (accessToken, refreshToken);
        }

        public async Task<(string accessToken, string refreshToken)> RefreshTokenAsync(string refreshToken)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
            if (user == null || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                throw new UnauthorizedAccessException("Invalid or expired refresh token");
            }

            var newAccessToken = await GenerateAccessToken(user);
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(5);
            await _userManager.UpdateAsync(user);

            return (newAccessToken, newRefreshToken);
        }

        public async Task<string> GenerateAccessToken(ApplicationAccount user)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role.ToString()));
            }

            var authenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.Now.AddMinutes(20),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authenKey, SecurityAlgorithms.HmacSha512Signature)
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            return Convert.ToBase64String(randomBytes);
        }

        public async Task<bool> LogoutAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return false;
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;

            await _loanPreviewService.ClearPreview(userId);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<ApiResponse<bool>> SignUpAsync(Signup model)
        {
            try
            {
                var checkUsername = await _userManager.FindByNameAsync(model.Username);
                if (checkUsername != null)
                {
                    return new ApiResponse<bool>(false, "The Username Already exists.", false, 404);
                }

                var user = new ApplicationAccount
                {
                    Id = Guid.NewGuid(),
                    UserName = model.Username.Trim(),
                    Firstname = model.Firstname.Trim(),
                    Lastname = model.Lastname.Trim(),
                    Email = model.Email.Trim(),
                    Contact = model.ContactNo.Trim(),
                    Address = null,
                    Createddate = DateTime.Now,
                    Status = 0,
                    Note = null,
                    RefreshToken = null,
                    RefreshTokenExpiryTime = null,
                    LastLogin = null
                };

                var result = await _userManager.CreateAsync(user, model.Password.Trim());

                if (result.Succeeded)
                {

                    if (!await _roleManager.RoleExistsAsync(AppRole.User))
                    {
                        await _roleManager.CreateAsync(new IdentityRole<Guid>(AppRole.User));
                    }

                    await _userManager.AddToRoleAsync(user, AppRole.User);
                    return new ApiResponse<bool>(true, "User created successfully.", true, 201);
                }
                else
                {
                    var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                    return new ApiResponse<bool>(false, "Failed to create user: " + errors, false, 500);
                }

            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>(false, "An error occurred: " + ex.Message, false, 500);
            }
        }

        public async Task<ApiResponse<bool>> ChangePassword(Guid userId, string oldPassword, string newPassword)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());

                var passwordValid = await _userManager.CheckPasswordAsync(user, oldPassword);
                if (passwordValid == false)
                {
                    return new ApiResponse<bool>(false, "Current Password is Incorrect.", false, 404);
                }

                var changePassword = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
                if (!changePassword.Succeeded)
                {
                    return new ApiResponse<bool>(false, "New Password is Incorrect.", false, 404);
                }

                return new ApiResponse<bool>(true, "Change Password Successfully.", true, 201);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>(false, "An error occurred: " + ex.Message, false, 500);
            }
        }

        public async Task<ApiResponse<bool>> SendOTP(string username)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(username);
                if (user == null)
                {
                    return new ApiResponse<bool>(false, "Username is incorrect.", false, 404);
                }

                var otpremove = await _dbContext.OTPs.Where(o => o.ExpiryTime < DateTime.Now || o.ApplicationAccountId == user.Id).ToListAsync();

                _dbContext.OTPs.RemoveRange(otpremove);
                await _dbContext.SaveChangesAsync();

                var otpcode = new Random().Next(100000, 999999).ToString();

                var otp = new OTP
                {
                    Id = Guid.NewGuid(),
                    ApplicationAccountId = user.Id,
                    OTPCode = otpcode,
                    ExpiryTime = DateTime.Now.AddMinutes(5),
                };

                await _dbContext.OTPs.AddAsync(otp);

                await _dbContext.SaveChangesAsync();

                await _emailService.SendEmailAsync(user.Email, otpcode);

                return new ApiResponse<bool>(true, "Send OTP successfully.", true, 201);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>(false, "An error occurred: " + ex.Message, false, 500);
            }
        }

        public async Task<ApiResponse<string>> CheckOTP(string username, string otp)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return new ApiResponse<string>(false, "User is not Found.", null, 404);
            }

            var checkOtp = await _dbContext.OTPs
                .FirstOrDefaultAsync(o => o.OTPCode == otp && o.ExpiryTime > DateTime.Now && o.ApplicationAccountId == user.Id);
            
            if (checkOtp == null)
            {
                return new ApiResponse<string>(false, "OTP is expired or incorrect.", null, 400);
            }

            _dbContext.OTPs.Remove(checkOtp);
            await _dbContext.SaveChangesAsync();

            string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            return new ApiResponse<string>(true, "OTP verified successfully.", resetToken, 201);
        }

        public async Task<ApiResponse<bool>> ResetPassword(string username, string resetToken, string newpassword)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return new ApiResponse<bool>(false, "User is not found.", false, 404);
            }

            var result = await _userManager.ResetPasswordAsync(user, resetToken, newpassword);
            if (!result.Succeeded)
            {
                return new ApiResponse<bool>(false, "Failed to reset password.", false, 500);
            }

            return new ApiResponse<bool>(true, "Password reset Successfully.", true, 201);
        }
    }
}
