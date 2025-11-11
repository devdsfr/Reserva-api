using MediatR;
using ReservaSalas.Application.Reservas;

namespace ReservaSalas.Application.Reservas.Queries.GetReservasByRoom
{
    public record GetReservasByRoomQuery(string Room, DateTime StartDate, DateTime EndDate) : IRequest<IEnumerable<ReservaDto>>;
}
