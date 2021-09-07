using Forum.Common;
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
            mb.Entity<Channel>(entity =>
            {
                entity.HasKey(e => new { e.ChannelId });
                entity.Property(x => x.ChannelId).ValueGeneratedOnAdd();
            });
        }
        public DbSet<AspNetUser> AspNetUsers { get; set; }
        public DbSet<Channel> Channels { get; set; }
    }
}
