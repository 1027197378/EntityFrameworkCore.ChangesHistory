using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.ChangesHistory
{
    public class TemplateDbContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<ChangesHistory> ChangesHistorys { get; set; }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            this.AutoHistory<ChangesHistory>(x =>
            {
                x.ModifyUser = "001";
                x.ModifyName = "xxx";
            });

            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.UseChangesHistory<ChangesHistory>("changes");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(@"Server=localhost;Port=5432;UserId=postgres;Password=123;Database=test;");
        }
    }
}