using Microsoft.AspNetCore.Identity;

namespace ReadOn.DbContexts
{
    public enum AccountStatus { Active, Inactive}
    public class ApplicationAccount : IdentityUser<Guid>
    {
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? Contact { get; set; }
        public string? Address { get; set; }
        public DateTime Createddate { get; set; } = DateTime.Now;
        public AccountStatus Status { get; set; } = AccountStatus.Active;
        public string? Note { get; set; }

        public ICollection<Loan> Loans { get; set; }
        public ICollection<Book> Books { get; set; }
        public ICollection<LoanPreview> LoanPreviews { get; set; }
        public ICollection<OTP> OTPs { get; set; }
        public ApplicationAccount()
        {
            Loans = new List<Loan>();
            Books = new List<Book>();
            LoanPreviews = new List<LoanPreview>();
            OTPs = new List<OTP>();
            SecurityStamp = Guid.NewGuid().ToString();
        }

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public DateTime? LastLogin { get; set; }
    }
}
