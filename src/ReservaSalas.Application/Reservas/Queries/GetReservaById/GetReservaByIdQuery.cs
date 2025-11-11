using MediatR;
using ReservaSalas.Application.Reservas;

namespace ReservaSalas.Application.Reservas.Queries.GetReservaById
{
    public record GetReservaByIdQuery(int Id) : IRequest<ReservaDto>;
}
