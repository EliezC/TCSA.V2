using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TCSA.V2.Models;

namespace TCSA.V2.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public virtual DbSet<DashboardProject> DashboardProjects { get; set; }
        public virtual DbSet<AppUserActivity> UserActivity { get; set; }
        public virtual DbSet<UserReview> UserReviews { get; set; }
        public virtual DbSet<ApplicationUser> AspNetUsers { get; set; }
    }
}
