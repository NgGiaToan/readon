namespace ReadOn.DbContexts
{
    public class OTP
    {
        public Guid Id { get; set; }
        public ApplicationAccount ApplicationAccount { get; set; }
        public Guid ApplicationAccountId { get; set; }
        public string OTPCode { get; set; }
        public DateTime ExpiryTime { get; set; }

    }
}
