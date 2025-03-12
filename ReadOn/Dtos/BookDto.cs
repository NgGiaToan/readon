namespace ReadOn.Dtos
{
    public class BookDto
    {
        public Guid Id { get; set; }
        public string Fullname {  get; set; }
        public string Type {  get; set; }
        public string Language { get; set; }
        public string Availability {  get; set; }
    }
}
