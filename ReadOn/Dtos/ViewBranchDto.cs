using System.Runtime.CompilerServices;

namespace ReadOn.Dtos
{
    public class ViewBranchDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ContactNo { get; set; }
        public string Location {  get; set; }
        public string Note { get; set; }
    }
}
