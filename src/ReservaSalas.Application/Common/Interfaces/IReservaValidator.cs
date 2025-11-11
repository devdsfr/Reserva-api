using ReservaSalas.Domain.Entities;

namespace ReservaSalas.Application.Common.Interfaces
{
    public interface IReservaValidator
    {
        Task ValidateConflict(Reserva newReserva, CancellationToken cancellationToken, int? excludeId = null);
    }
}
