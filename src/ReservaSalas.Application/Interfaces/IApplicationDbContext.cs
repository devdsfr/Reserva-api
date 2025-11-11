using Microsoft.EntityFrameworkCore;
using ReservaSalas.Domain.Entities;

namespace ReservaSalas.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Reserva> Reservas { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
