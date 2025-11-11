using Microsoft.EntityFrameworkCore;
using ReservaSalas.Application.Interfaces;
using ReservaSalas.Domain.Entities;

namespace ReservaSalas.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Reserva> Reservas { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            //TODO Aqui poderia ser o local ideal para implementar o logging de Insert/Update/Delete
            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Reserva>()
                .HasKey(r => r.Id);

            base.OnModelCreating(modelBuilder);
        }
    }
}
