using MediatR;
using ReservaSalas.Application.Reservas;

namespace ReservaSalas.Application.Reservas.Commands.UpdateReserva
{
    public record UpdateReservaCommand : IRequest<ReservaDto>
    {
        public int Id { get; init; }
        public string Room { get; init; } = string.Empty;
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }
        public string ReservedBy { get; init; } = string.Empty;
    }
}
