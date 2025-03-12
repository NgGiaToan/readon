namespace ReadOn.DbContexts
{
    public class Book
    {
        public Guid  Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Language { get; set; }
        public int Total { get; set; } 
        public int Available { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now; 
        public string? Note { get; set; }

        public ApplicationAccount ApplicationAccount { get; set; }
        public Guid ApplicationAccountId { get; set; }

        public ICollection<LoanDetail> LoanDetails { get; set; }
        public ICollection<LoanPreview> LoanPreviews { get; set; }
        public Book()
        {
            LoanDetails = new List<LoanDetail>();
            LoanPreviews = new List<LoanPreview>(); 
        }
    }
}
