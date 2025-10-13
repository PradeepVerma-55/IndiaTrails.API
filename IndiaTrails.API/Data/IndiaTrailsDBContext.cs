using IndiaTrails.API.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace IndiaTrails.API.Data
{
    public class IndiaTrailsDBContext : DbContext
    {
        public IndiaTrailsDBContext(DbContextOptions<IndiaTrailsDBContext> options) : base(options)
        {
        }

        public DbSet<Region> Regions { get; set; }
        public DbSet<Walk> Walks { get; set; }
        public DbSet<Difficulty> Difficulties { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ✅ Seed data for Difficulties (use static GUIDs)
            var difficulties = new List<Difficulty>()
            {
                new Difficulty()
                {
                    Id = new Guid("11111111-1111-1111-1111-111111111111"),
                    Name = "Easy"
                },
                new Difficulty()
                {
                    Id = new Guid("22222222-2222-2222-2222-222222222222"),
                    Name = "Medium"
                },
                new Difficulty()
                {
                    Id = new Guid("33333333-3333-3333-3333-333333333333"),
                    Name = "Hard"
                }
            };
            modelBuilder.Entity<Difficulty>().HasData(difficulties);

            // ✅ Seed data for Regions (use static GUIDs)
            var regions = new List<Region>
            {
                new Region
                {
                    Id = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1"),
                    Name = "Himachal Pradesh",
                    Code = "HP",
                    RegionImageUrl = "https://images.pexels.com/photos/674010/pexels-photo-674010.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1"
                },
                new Region
                {
                    Id = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2"),
                    Name = "Uttarakhand",
                    Code = "UK",
                    RegionImageUrl = "https://images.pexels.com/photos/5334653/pexels-photo-5334653.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1"
                },
                new Region
                {
                    Id = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3"),
                    Name = "Ladakh",
                    Code = "LD",
                    RegionImageUrl = "https://images.pexels.com/photos/1566435/pexels-photo-1566435.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1"
                },
                new Region
                {
                    Id = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa4"),
                    Name = "Sikkim",
                    Code = "SK",
                    RegionImageUrl = "https://images.pexels.com/photos/1547613/pexels-photo-1547613.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1"
                },
                new Region
                {
                    Id = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa5"),
                    Name = "Arunachal Pradesh",
                    Code = "AR",
                    RegionImageUrl = "https://images.pexels.com/photos/1868778/pexels-photo-1868778.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1"
                },
                new Region
                {
                    Id = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa6"),
                    Name = "Kerala",
                    Code = "KL",
                    RegionImageUrl = "https://images.pexels.com/photos/248062/pexels-photo-248062.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1"
                },
                new Region
                {
                    Id = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa7"),
                    Name = "Goa",
                    Code = "GA",
                    RegionImageUrl = "https://images.pexels.com/photos/372281/pexels-photo-372281.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1"
                },
            };

            modelBuilder.Entity<Region>().HasData(regions);
        }
    }
}
