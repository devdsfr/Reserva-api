using MediatR;
using ReservaSalas.Application.Reservas;

namespace ReservaSalas.Application.Reservas.Commands.CreateReserva
{
    public record CreateReservaCommand : IRequest<ReservaDto>
    {
        public string Room { get; init; } = string.Empty;
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }
        public string ReservedBy { get; init; } = string.Empty;
    }
}
