using Microsoft.EntityFrameworkCore;
using ReservaSalas.Application.Common.Exceptions;
using ReservaSalas.Application.Common.Interfaces;
using ReservaSalas.Application.Interfaces;
using ReservaSalas.Domain.Entities;

namespace ReservaSalas.Application.Common.Services
{
    public class ReservaValidator : IReservaValidator
    {
        private readonly IApplicationDbContext _context;

        public ReservaValidator(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ValidateConflict(Reserva newReserva, CancellationToken cancellationToken, int? excludeId = null)
        {
            var conflictingReserva = await _context.Reservas
                .AsNoTracking()
                .Where(r => r.Room == newReserva.Room &&
                            r.Id != excludeId &&
                            r.StartDate < newReserva.EndDate &&
                            r.EndDate > newReserva.StartDate)
                .FirstOrDefaultAsync(cancellationToken);

            if (conflictingReserva != null)
            {
                throw new ReservaConflictException($"Conflito de horário detectado na sala {newReserva.Room} com a reserva {conflictingReserva.Id}.");
            }
        }
    }
}
