using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;
using WebApplication2.Properties.Models;

namespace WebApplication2.Properties.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Ad> Ads { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Subcategory> Subcategories { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Admedia> Admedias { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<AdminSession> AdminSessions { get; set; }
        public DbSet<UserSocialAuth> UserSocialAuths { get; set; }

        public DbSet<AdminAuditLog> AdminAuditLogs { get; set; }
        public DbSet<AdminPermission> AdminPermissions { get; set; }
        public DbSet<AdminRolePermission> AdminRolePermissions { get; set; }

        public DbSet<UserModeration> UserModerations { get; set; }
        public DbSet<AdModeration> AdModerations { get; set; }
        public DbSet<ReportedContent> ReportedContents { get; set; }
    }
}
