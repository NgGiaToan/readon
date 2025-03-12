namespace ReadOn.Dtos
{
    public class BorrowedDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public int Amount { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime Time {  get; set; }
    }
}
