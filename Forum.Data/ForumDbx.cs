using Forum.Common;
using Forum.Models;
using Forum.Models.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;

namespace Forum.Data
{
    public class ForumDbx:DbContext
    {
        private readonly AppSettings appSettings;

        public ForumDbx(IOptions<AppSettings> appSettingsOptions)
        {
            appSettings = appSettingsOptions.Value;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(appSettings.DefaultConnectionString);
        }
        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<AspNetUser>(entity => {
                entity.HasKey(e => e.Id);
            });
            mb.Entity<MainTopic>(entity =>
            {
                entity.HasKey(e => new { e.MainTopicId });
                entity.Property(x => x.MainTopicId).ValueGeneratedOnAdd();

                entity.HasOne(x => x.Parent)
                .WithMany(x => x.ChildTopic)
                .HasForeignKey(x => x.ParentIdFK);
                
            });
            mb.Entity<MainTopicPost>(entity =>
            {
                entity.HasKey(e => new { e.MainTopicPostId });
                entity.Property(x => x.MainTopicPostId).ValueGeneratedOnAdd();
                entity.HasOne(p => p.MainTopic)
               .WithMany(p => p.MainTopicPost)
               .HasForeignKey(p => p.MainTopicsIdFK)
               //.IsRequired(true)
               .OnDelete(DeleteBehavior.Cascade);
            });
        }
        public DbSet<AspNetUser> AspNetUsers { get; set; }
        public DbSet<MainTopic> MainTopics { get; set; }
        public DbSet<MainTopicPost> MainTopicPosts { get; set; }
    }
}
