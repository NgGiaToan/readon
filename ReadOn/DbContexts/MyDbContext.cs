using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace ReadOn.DbContexts
{
    
    public class MyDbContext : IdentityDbContext<ApplicationAccount, IdentityRole<Guid>, Guid>
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }
        #region
        public DbSet<Loan> Loans { get; set; }
        public DbSet<LoanDetail> LoansDetails { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<LoanPreview> LoanPreviews { get; set; }
        public DbSet<OTP> OTPs { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<LoanPreview>(lp =>
            {
                lp.ToTable("LoanPreview");
                lp.HasKey(lp => lp.Id);
                lp.HasOne(lp => lp.ApplicationAccount)
                    .WithMany(a => a.LoanPreviews)
                    .HasForeignKey(lp => lp.ApplicationAccountId);
                lp.HasOne(lp => lp.Book)
                    .WithMany(b => b.LoanPreviews)
                    .HasForeignKey(lp => lp.BookId);
            });

            modelBuilder.Entity<OTP>(o =>
            {
                o.ToTable("OTP");
                o.HasKey(o => o.Id);
                o.HasOne(o => o.ApplicationAccount)
                    .WithMany(a => a.OTPs)
                    .HasForeignKey(o => o.ApplicationAccountId);
            });
                

            modelBuilder.Entity<ApplicationAccount>(a =>
            {
                a.ToTable("ApplicationAccount");
                a.HasKey(a => a.Id);
                a.HasMany(a => a.Loans)
                    .WithOne(l => l.ApplicationAccount)
                    .HasForeignKey(l => l.ApplicationAccountId);
                a.HasMany(a => a.Books)
                    .WithOne(b => b.ApplicationAccount)
                    .HasForeignKey(b => b.ApplicationAccountId)
                    .OnDelete(DeleteBehavior.Restrict);
                a.HasMany(a => a.LoanPreviews)
                    .WithOne(lp => lp.ApplicationAccount)
                    .HasForeignKey(lp => lp.ApplicationAccountId)
                    .OnDelete(DeleteBehavior.Restrict);
                a.HasMany(a=> a.OTPs)
                    .WithOne(o => o.ApplicationAccount)
                    .HasForeignKey(o => o.ApplicationAccountId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Loan>(l =>
            {
                l.ToTable("Loan");
                l.HasKey(l => l.Id);
                l.HasMany(l=>l.LoanDetails)
                    .WithOne(ld => ld.Loan)
                    .HasForeignKey(ld => ld.LoanId)
                    .OnDelete(DeleteBehavior.Cascade);
                l.HasOne(l => l.ApplicationAccount)
                    .WithMany(a => a.Loans)
                    .HasForeignKey(l => l.ApplicationAccountId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<LoanDetail>(ld =>
            {
                ld.ToTable("LoanDetail");
                ld.HasKey(ld => ld.Id);
                ld.HasOne(ld => ld.Loan)
                    .WithMany(l => l.LoanDetails)
                    .HasForeignKey(ld => ld.LoanId);
                ld.HasOne(ld => ld.Book)
                    .WithMany(b => b.LoanDetails)
                    .HasForeignKey(ld => ld.BookId);
            });

            modelBuilder.Entity<Book>(b =>
            {
                b.ToTable("Book");
                b.HasKey(b => b.Id);
                b.HasMany(b => b.LoanDetails)
                    .WithOne(ld => ld.Book)
                    .HasForeignKey(lb => lb.BookId);
                b.HasMany(b => b.LoanPreviews)
                    .WithOne(lp => lp.Book)
                    .HasForeignKey(lp => lp.BookId);
                b.HasOne(b => b.ApplicationAccount)
                    .WithMany(a => a.Books)
                    .HasForeignKey(b => b.ApplicationAccountId);
            });

            modelBuilder.Entity<Branch>(b =>
            {
                b.ToTable("Branch");
                b.HasKey(b => b.Id);
            });

        }
    }
}
