using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MinitwitReact.Entities
{
    public partial class MinitwitContext : DbContext
    {
        public MinitwitContext()
        {
        }

        public MinitwitContext(DbContextOptions<MinitwitContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Follower> Followers { get; set; } = null!;
        public virtual DbSet<Message> Messages { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlite("Data Source=./../../minitwit.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Follower>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("follower");

                entity.Property(e => e.WhoId)
                    .HasColumnType("integer")
                    .HasColumnName("who_id");

                entity.Property(e => e.WhomId)
                    .HasColumnType("integer")
                    .HasColumnName("whom_id");
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.ToTable("message");

                entity.Property(e => e.MessageId)
                    .HasColumnType("integer")
                    .HasColumnName("message_id");

                entity.Property(e => e.AuthorId)
                    .HasColumnType("integer")
                    .HasColumnName("author_id");

                entity.Property(e => e.Flagged)
                    .HasColumnType("integer")
                    .HasColumnName("flagged");

                entity.Property(e => e.PubDate)
                    .HasColumnType("integer")
                    .HasColumnName("pub_date");

                entity.Property(e => e.Text)
                    .HasColumnType("string")
                    .HasColumnName("text");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.Property(e => e.UserId)
                    .HasColumnType("integer")
                    .HasColumnName("user_id");

                entity.Property(e => e.Email)
                    .HasColumnType("string")
                    .HasColumnName("email");

                entity.Property(e => e.PwHash)
                    .HasColumnType("string")
                    .HasColumnName("pw_hash");

                entity.Property(e => e.Username)
                    .HasColumnType("string")
                    .HasColumnName("username");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
