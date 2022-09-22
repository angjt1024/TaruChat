using TaruChat.Models;
using Microsoft.EntityFrameworkCore;

namespace TaruChat.Data
{
    public class ChatContext : DbContext
    {
        public ChatContext(DbContextOptions<ChatContext> options) : base(options)
        {
        }

        public DbSet<Class> Classes { get; set; }
        public DbSet<Assign> Assigns { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }

        //Prevent Pluralized
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Class>().ToTable("Class");
            modelBuilder.Entity<Assign>().ToTable("Assign");
            modelBuilder.Entity<Subject>().ToTable("Subject");
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<Enrollment>().ToTable("Enrollment");
            modelBuilder.Entity<Chat>().ToTable("Chat");
            modelBuilder.Entity<Message>().ToTable("Message");

            modelBuilder.Entity<Assign>()
                .HasKey(a => new { a.ClassID, a.SubjectID  });

            modelBuilder.Entity<Enrollment>()
                .HasKey(a => new { a.ChatID, a.UserID });
        }
    }
}
