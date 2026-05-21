using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public DbSet<GroupMessage> GroupMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasIndex(x => x.UserName).IsUnique();
                entity.HasIndex(x => x.Email).IsUnique();
            });

            modelBuilder.Entity<PersonalChat>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasOne(pc => pc.UserOne)
                    .WithMany(u => u.PersonalChats)
                    .HasForeignKey(pc => pc.UserOneId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(pc => pc.UserTwo)
                    .WithMany()
                    .HasForeignKey(pc => pc.UserTwoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(pc => new { pc.UserOneId, pc.UserTwoId })
                    .IsUnique();
            
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasOne(m => m.PersonalChat)
                    .WithMany(pc => pc.Messages)
                    .HasForeignKey(m => m.PersonalChatId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(m => m.Sender)
                    .WithMany(u => u.SentMessages)
                    .HasForeignKey(m => m.SenderId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(m => m.Receiver)
                    .WithMany(u => u.ReceivedMessages)
                    .HasForeignKey(m => m.ReceiverId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(m => m.PersonalChatId);
                entity.HasIndex(m => m.SentTime);
                entity.HasIndex(m => m.SenderId);
                entity.HasIndex(m => m.ReceiverId);
            });

            modelBuilder.Entity<GroupChat>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(gc => gc.Name)
                    .IsRequired()
                    .HasMaxLength(100);

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

                entity.HasOne(gm => gm.User)
                    .WithMany(u => u.GroupMemberships)
                    .HasForeignKey(gm => gm.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(gm => new { gm.GroupChatId, gm.UserId })
                    .IsUnique();
            });

            
            modelBuilder.Entity<GroupMessage>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasOne(gm => gm.GroupChat)
                    .WithMany(gc => gc.Messages)
                    .HasForeignKey(gm => gm.GroupChatId)
                    .OnDelete(DeleteBehavior.Cascade);

                
                entity.HasOne(gm => gm.Sender)
                    .WithMany(u => u.GroupMessages)
                    .HasForeignKey(gm => gm.SenderId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(gm => gm.ReplyTo)
                    .WithMany()
                    .HasForeignKey(gm => gm.ReplyToMessageId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(gm => gm.GroupChatId);
                entity.HasIndex(gm => gm.SentTime);
                entity.HasIndex(gm => gm.SenderId);
            });
        }
    }
}
