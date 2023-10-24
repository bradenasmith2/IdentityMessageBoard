using IdentityMessageBoard.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace IdentityMessageBoard.DataAccess
{
    public class MessageBoardContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Message> Messages { get; set; }

        public MessageBoardContext(DbContextOptions<MessageBoardContext> options) : base(options)
        {

        }
    }
}
