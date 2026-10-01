
using IdentityMail.Web.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace IdentityMail.Web.Context
{
    public class AppDbContext : IdentityDbContext<AppUser,AppRole,int> // IdentityDbContext DbContext ten miras alıyor.
    {
        public AppDbContext(DbContextOptions options) : base(options)  //program.cs içinde yaptığımız configürasyonları yeniden yazmamıza gerek kalmıyor. Miras aldığımız için.
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<AppUser>().HasMany(message =>message.SentMessages)
                                     .WithOne(s =>s.Sender).HasForeignKey(x=>x.SenderId)
                                     .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AppUser>().HasMany(message => message.ReceivedMessages)
                                     .WithOne(s => s.Receiver).HasForeignKey(x => x.ReceiverId)
                                     .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(builder);
        }

        public DbSet<UserMessage> UserMessages { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}
