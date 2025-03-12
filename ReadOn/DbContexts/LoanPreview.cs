namespace ReadOn.DbContexts
{
    public class LoanPreview
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ApplicationAccountId { get; set; }
        public Guid BookId { get; set; }
        public ApplicationAccount ApplicationAccount { get; set; }
        public Book Book { get; set; }
    }
}
