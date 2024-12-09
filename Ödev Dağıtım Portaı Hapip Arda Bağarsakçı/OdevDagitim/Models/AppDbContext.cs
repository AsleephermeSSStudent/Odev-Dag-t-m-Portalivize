using AspNetCoreHero.ToastNotification.Notyf.Models;
using Microsoft.EntityFrameworkCore;
using NETCore.Encrypt.Extensions;

namespace OdevDagitim.Models
{
    public class AppDbContext : DbContext
    {
        private readonly IConfiguration _config;
        public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration config) : base(options)
        {
            _config = config;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<AssignmentSubmission> AssignmentSubmissions { get; set; }
        public string MD5Hash(string pass)
        {
            var salt = _config.GetValue<string>("AppSettings:MD5Salt");
            var password = pass + salt;
            var hashed = password.MD5();
            return hashed;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasData(
                new User { Id = 1, Username = "admin", Email = "admin", PasswordHash = MD5Hash("admin"), Role = "admin" },
                new User { Id=2 , Username="teacher",Email="teacher",PasswordHash=MD5Hash("teacher"),Role="teacher"}

                );

            // Assignment ilişkileri
            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.Class)
                .WithMany()
                .HasForeignKey(a => a.ClassId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.Teacher)
                .WithMany(u => u.TeacherAssignments)
                .HasForeignKey(a => a.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            // AssignmentSubmission ilişkileri
            modelBuilder.Entity<AssignmentSubmission>()
                .HasOne(s => s.Assignment)
                .WithMany(a => a.Submissions)
                .HasForeignKey(s => s.AssignmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AssignmentSubmission>()
                .HasOne(s => s.User)
                .WithMany(u => u.Submissions)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // User ilişkileri
            modelBuilder.Entity<User>()
                .HasOne(u => u.Class)
                .WithMany()
                .HasForeignKey(u => u.ClassId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}
