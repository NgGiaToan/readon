using System.Diagnostics.Eventing.Reader;

namespace ReadOn.DbContexts
{
    public class LoanDetail
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public string? Status { get; set; }
        public string? Note { get; set; }
        public Loan Loan { get; set; }
        public Guid LoanId { get; set; }
        public Book Book { get; set; }
        public Guid BookId { get; set; }
    }
}
