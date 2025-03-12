using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using ReadOn.DbContexts;
using ReadOn.Dtos;
using ReadOn.Helpers;
using ReadOn.Shared;
using System.Reflection.Metadata.Ecma335;

namespace ReadOn.Services
{
    public class AccountService: IAccountService
    {
        private readonly MyDbContext _dbContext;
        private readonly UserManager<ApplicationAccount> _userManager;
        public AccountService(MyDbContext dbContext, UserManager<ApplicationAccount> userManager)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }
        public async Task<int> TotalUsers()
        {
            return (await _userManager.GetUsersInRoleAsync(AppRole.User)).Count();
        }

        public async Task<List<AdminDto>> AdminAsync()
        {
            List<AdminDto> itemDto = (await _userManager.GetUsersInRoleAsync(AppRole.Admin))
                .Select(a => new AdminDto
                {
                    Fullname = a.Lastname + a.Firstname,
                    Id = a.Id,
                    Status = a.Status.ToString()
                })
                .ToList();
            return itemDto;
        }

        public async Task<List<UserDto>> UserAsync()
        {
            List<UserDto> itemDto = (await _userManager.GetUsersInRoleAsync(AppRole.User))
                .Select(a => new UserDto
                {
                    Id = a.Id,
                    Name = a.Lastname + a.Firstname,
                    Email = a.Email,
                    Username = a.UserName,
                }).ToList();
            return itemDto;
        }

        public async Task<ViewUserDto> ViewUser(Guid id)
        {
            var user = await _userManager.Users
                .Where(u => u.Id == id)
                .Select( u => new ViewUserDto
                {
                    Id = u.Id,
                    Name = u.Firstname + " "+ u.Lastname,
                    Email = u.Email,
                    Username = u.UserName,
                    Note = u.Note,
                }).FirstOrDefaultAsync();
            return user;
        }

        public async Task<ApiResponse<ApplicationAccount>> CreateUser(CreateUpdateUserDto value, Guid id)
        {
            try
            {
                var checkUsername = await _userManager.FindByNameAsync(value.Username);
                if (checkUsername != null)
                {
                    return new ApiResponse<ApplicationAccount>(false, "The username already exists.", null, 404);
                }

                var account = await _userManager.FindByIdAsync(id.ToString());

                if (account == null)
                {
                    return new ApiResponse<ApplicationAccount>(false, "Id is Incorrect.", null, 404);
                }

                bool isAdmin = await _userManager.IsInRoleAsync(account, AppRole.Admin);

                if (!isAdmin)
                {
                    return new ApiResponse<ApplicationAccount>(false, "User is not an Admin.", null, 403);
                }


                var user = new ApplicationAccount
                {
                    Id = Guid.NewGuid(),
                    UserName = value.Username.Trim(),
                    Firstname = value.Name.Trim(),
                    Lastname = null,
                    Email = value.Email.Trim(),
                    Contact = null,
                    Address = null,
                    Createddate = DateTime.Now,
                    Status = 0,
                    Note = account.Firstname.Trim() + " " + account.Lastname.Trim() ,
                    RefreshToken = null,
                    RefreshTokenExpiryTime = null,
                    LastLogin = null
                    
                };

                var result = await _userManager.CreateAsync(user, value.Password.Trim());

                if (!result.Succeeded)
                {
                    var errors = string.Join("; ", result.Errors.Select(e => e.Description));

                    return new ApiResponse<ApplicationAccount>(false, "Failed to create user: " + errors, null, 500);
                }
                else
                {
                    await _userManager.AddToRoleAsync(user, AppRole.User);
                    return new ApiResponse<ApplicationAccount>(true, "User created successfully.", user, 201);
                }


            }
            catch (Exception ex)
            {
                return new ApiResponse<ApplicationAccount>(false, "An error occurred: " + ex.Message, null, 500);
            }
        }

        public async Task<ApiResponse<ApplicationAccount>> UpdateUser(CreateUpdateUserDto value, Guid id)
        {
            try
            {
                var account = await _userManager.FindByIdAsync(id.ToString());
                if (account == null)
                {
                    return new ApiResponse<ApplicationAccount>(false, "Account not Found.", null, 404);
                }

                var username = await _userManager.FindByNameAsync(value.Username);
                if (username != null) {
                    return new ApiResponse<ApplicationAccount>(false, "The username already exists.", null, 404);
                }

                account.Firstname = value.Name.Trim();
                account.Lastname = null;
                account.Email = value.Email.Trim();
                account.UserName = value.Username.Trim();

                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(account);
                var passwordResult = await _userManager.ResetPasswordAsync(account, resetToken, value.Password.Trim());

                if (!passwordResult.Succeeded)
                {
                    var errors = string.Join("; ", passwordResult.Errors.Select(e => e.Description));
                    return new ApiResponse<ApplicationAccount>(false, "Failed to update password: " + errors, null, 500);
                }

                var updateResult = await _userManager.UpdateAsync(account);
                if (!updateResult.Succeeded)
                {
                    var errors = string.Join("; ", updateResult.Errors.Select(e => e.Description));
                    return new ApiResponse<ApplicationAccount>(false, "Failed to update user: " + errors, null, 500);
                }

                return new ApiResponse<ApplicationAccount>(true, "User updated successfully.", account, 201);
            }

            catch (Exception ex)
            {
                return new ApiResponse<ApplicationAccount>(false, "An error occurred: " + ex.Message, null, 500);
            }
        }

        public async Task<ApiResponse<bool>> DeleteUser(Guid id)
        {
            try
            {
                var account = await _userManager.FindByIdAsync(id.ToString());
                if (account == null )
                {
                    return new ApiResponse<bool>(false, "Account not Found.", false, 404);

                }

                await _userManager.DeleteAsync(account);

                return new ApiResponse<bool>(true, "User Deleted Successfully.", true, 201);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>(false, "An error occurred: " + ex.Message, false, 500);
            }
        }
    }
}
