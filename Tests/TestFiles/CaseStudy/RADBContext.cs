namespace RentApp.Persistance
{
    public class RADBContext : IdentityDbContext<RAIdentityUser>
    {
        public virtual DbSet<AppUser> AppUsers { get; set; }

        public DbSet<Service> Services { get; set; }

        public DbSet<Vehicle> Vehicles { get; set; }

        public DbSet<Branch> Branches { get; set; }

        public DbSet<Rent> Rents { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<Grade> Grades { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        public RADBContext() : base("name=RADB")
        {
            Configuration.LazyLoadingEnabled = false;
        }

        public static RADBContext Create()
        {
            return new RADBContext();
        }
    }
}