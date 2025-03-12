namespace ReadOn.DbContexts
{
    public enum LoanStatus { Return, Borrow }
    public class Loan
    {
        public Guid Id { get; set; }
        public DateTime Borrowdate { get; set; } = DateTime.Now;
        public DateTime Duedate { get; set; } 
        public DateTime? Returndate { get; set; }
        public LoanStatus Status { get; set; } = LoanStatus.Borrow;
        public string? Note { get; set; }
        public ApplicationAccount ApplicationAccount { get; set; }
        public Guid ApplicationAccountId { get; set; }
        public ICollection<LoanDetail> LoanDetails { get; set; }
        public Loan()
        {
            LoanDetails = new List<LoanDetail>();
            Duedate = Borrowdate.AddDays(14);
        }
    }
}
