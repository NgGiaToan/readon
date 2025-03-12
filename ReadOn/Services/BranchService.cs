using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using ReadOn.DbContexts;
using ReadOn.Dtos;
using ReadOn.Helpers;
using ReadOn.Shared;

namespace ReadOn.Services
{
    public class BranchService: IBranchService
    {
        private readonly MyDbContext _dbContext;
        private readonly UserManager<ApplicationAccount> _userManager;
        public BranchService(MyDbContext dbContext, UserManager<ApplicationAccount> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<int> TotalBranch()
        {
            var totalBranch = await _dbContext.Branches.CountAsync();
            return totalBranch;
        }

        public async Task<List<ItemDto>> Branches()
        {
            var branches = await _dbContext.Branches
                .Select(b => new ItemDto
                {
                    Fullname = b.Name,
                    Id = b.Id
                })
                .ToListAsync();
            return branches;
        }

        public async Task<List<BranchDto>> BranchAsync()
        {
            var branch = await _dbContext.Branches
                .Select(b => new BranchDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    ContactNo = b.Contact,
                    Location = b.Location,
                }).ToListAsync();
            return branch;
        }

        public async Task<ViewBranchDto> ViewBranch(Guid id)
        {
            var branch = await _dbContext.Branches
                .Where(b => b.Id == id)
                .Select(b => new ViewBranchDto { 
                    Id = b.Id,
                    Name = b.Name,
                    ContactNo = b.Contact,
                    Location = b.Location,
                    Note = b.Note,
                }).FirstOrDefaultAsync();
            return branch;
        }

        public async Task<ApiResponse<Branch>> CreateBranch(CreateUpdateBranchDto value, Guid id)
        {
            try
            {
                var name = await _dbContext.Branches.Where(u => u.Name == value.Name).FirstOrDefaultAsync();
                if (name != null)
                {
                    return new ApiResponse<Branch>(false, "The name already exists.", null, 404);
                }

                var account = await _userManager.FindByIdAsync(id.ToString());

                if (account == null)
                {
                    return new ApiResponse<Branch>(false, "Id is Incorrect.", null, 404);
                }

                bool isAdmin = await _userManager.IsInRoleAsync(account, AppRole.Admin);

                if (!isAdmin)
                {
                    return new ApiResponse<Branch>(false, "User is not an Admin.", null, 403);
                }

                if (account == null) {
                    return new ApiResponse<Branch>(false, "Admin Id not Found.", null, 404);
                }

                var branch = new Branch
                {
                    Id = Guid.NewGuid(),
                    Name = value.Name,
                    Contact = value.ContactNo,
                    Location = value.Location,
                    Status = 0,
                    Note = account.Firstname + " " + account.Lastname,
                };

                _dbContext.Add(branch);
                await _dbContext.SaveChangesAsync();

                return new ApiResponse<Branch>(true, "Branch Created Successfully.", branch, 201);    
            }
            catch (Exception ex)
            {
                return new ApiResponse<Branch>(false, "An Error occurred:" + ex.Message, null, 500);
            }
        }

        public async Task<ApiResponse<Branch>> UpdateBranch(CreateUpdateBranchDto value, Guid id)
        {
            try
            {
                var name = await _dbContext.Branches.Where(u => u.Name == value.Name).FirstOrDefaultAsync();
                if (name != null)
                {
                    return new ApiResponse<Branch>(false, "The name already exists.", null, 404);
                }

                var branch = await _dbContext.Branches.FindAsync(id);
                if (branch == null) {
                    return new ApiResponse<Branch>(false, "Admin Id not Found.", null, 404);
                }

                branch.Name = value.Name;
                branch.Contact = value.ContactNo;
                branch.Location = value.Location;

                await _dbContext.SaveChangesAsync();

                return new ApiResponse<Branch>(true, "Branch Updated Successfully.", branch, 201);
            }
            catch (Exception ex)
            {
                return new ApiResponse<Branch>(false, "An Error Occurred: " + ex.Message, null, 500);
            }
        }

        public async Task<ApiResponse<bool>> DeleteBranch(Guid id)
        {
            try
            {
                var branch = await _dbContext.Branches.FindAsync(id);
                if (branch == null)
                {
                    return new ApiResponse<bool>(false, "Admin Id not Found", false, 404);
                }

                _dbContext.Branches.Remove(branch);
                await _dbContext.SaveChangesAsync();
                return new ApiResponse<bool>(true, "Branch Deleted Successfully.", true, 201);
            }
            catch (Exception ex) {
                return new ApiResponse<bool>(false, "An error occured: " + ex.Message, false, 500);
            }
        }
    }
}
