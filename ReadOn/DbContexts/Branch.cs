namespace ReadOn.DbContexts
{
    public enum BranchStatus { Active, Inactive }
    public class Branch
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Contact { get; set; }
        public string Location {  get; set; }
        public BranchStatus Status { get; set; } = BranchStatus.Active;
        public string? Note { get; set; }
    }
}
