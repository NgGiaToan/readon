using ReadOn.DbContexts;

namespace ReadOn.Models
{
    public class BookVM
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Language { get; set; }
        public int Total { get; set; }
        public int Available { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public string? Note { get; set; }
        public ApplicationAccount ApplicationAccount { get; set; }
        public Guid AccountId { get; set; }

        public ICollection<LoanDetail> LoanDetails { get; set; }
        public BookVM()
        {
            LoanDetails = new List<LoanDetail>();
        }
    }
}
