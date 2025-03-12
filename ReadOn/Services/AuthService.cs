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
using System.Text;

namespace ReadOn.Services
{
    public class AuthService: IAuthService
    {
        private readonly MyDbContext _dbContext;
        private readonly SignInManager<ApplicationAccount> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationAccount> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        public AuthService(MyDbContext dbContext, 
            IConfiguration configuration, 
            SignInManager<ApplicationAccount> signInManager, 
            UserManager<ApplicationAccount> userManager,
            RoleManager<IdentityRole<Guid>> roleManager
            )
        {
            _signInManager = signInManager;
            _dbContext = dbContext;
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<string> SignInAsync(Signin model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);

            if (user == null || !passwordValid) { 
                return string.Empty;
            }

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var roles = await _userManager.GetRolesAsync(user); 
            foreach ( var role in roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role.ToString()));
            }

            var authenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.Now.AddMinutes(60),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authenKey, SecurityAlgorithms.HmacSha512Signature)
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
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

                if (result.Succeeded) {

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
            catch (Exception ex) {
                return new ApiResponse<bool>(false, "An error occurred: " + ex.Message, false, 500);
            }
        }
    }
}
