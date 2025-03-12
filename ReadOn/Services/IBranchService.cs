using ReadOn.DbContexts;
using ReadOn.Dtos;
using ReadOn.Shared;

namespace ReadOn.Services
{
    public interface IBranchService
    {
        Task<int> TotalBranch();
        Task<List<ItemDto>> Branches();
        Task<List<BranchDto>> BranchAsync();
        Task<ViewBranchDto> ViewBranch(Guid id);
        Task<ApiResponse<Branch>> CreateBranch(CreateUpdateBranchDto value, Guid id);
        Task<ApiResponse<Branch>> UpdateBranch(CreateUpdateBranchDto value, Guid id);
        Task<ApiResponse<bool>> DeleteBranch(Guid id);

    }
}
