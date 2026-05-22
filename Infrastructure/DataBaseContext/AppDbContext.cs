using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataBaseContext
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<PersonalChat> PersonalChats { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<GroupChat> GroupChats { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasIndex(x => x.UserName).IsUnique();
                entity.HasIndex(x => x.Email).IsUnique();

                entity.HasMany(u => u.SentMessages)
                    .WithOne(m => m.Sender)
                    .HasForeignKey(m => m.SenderId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(u => u.ReceivedMessages)
                    .WithOne(m => m.Receiver)
                    .HasForeignKey(m => m.ReceiverId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(u => u.PersonalChats)
                    .WithOne(pc => pc.UserOne)
                    .HasForeignKey(pc => pc.UserOneId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(u => u.GroupMemberships)
                    .WithOne(gm => gm.User)
                    .HasForeignKey(gm => gm.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<PersonalChat>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasOne(pc => pc.UserOne)
                    .WithMany()
                    .HasForeignKey(pc => pc.UserOneId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(pc => pc.UserTwo)
                    .WithMany()
                    .HasForeignKey(pc => pc.UserTwoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(pc => new { pc.UserOneId, pc.UserTwoId }).IsUnique();
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(m => m.Text).IsRequired().HasMaxLength(500);

                entity.HasOne(m => m.PersonalChat)
                    .WithMany(pc => pc.Messages)
                    .HasForeignKey(m => m.PersonalChatId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(m => m.GroupChat)
                    .WithMany(gc => gc.Messages)
                    .HasForeignKey(m => m.GroupChatId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(m => m.ReplyTo)
                    .WithMany()
                    .HasForeignKey(m => m.ReplyToMessageId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(m => m.PersonalChatId);
                entity.HasIndex(m => m.GroupChatId);
                entity.HasIndex(m => m.SentTime);
                entity.HasIndex(m => m.SenderId);
                entity.HasIndex(m => m.ReceiverId);
            });

            modelBuilder.Entity<GroupChat>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(gc => gc.Name).IsRequired().HasMaxLength(100);

                entity.HasOne(gc => gc.Owner)
                    .WithMany()
                    .HasForeignKey(gc => gc.OwnerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<GroupMember>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasOne(gm => gm.GroupChat)
                    .WithMany(gc => gc.Members)
                    .HasForeignKey(gm => gm.GroupChatId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(gm => new { gm.GroupChatId, gm.UserId }).IsUnique();
            });
        }
    }
}