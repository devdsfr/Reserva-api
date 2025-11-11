using MediatR;

namespace ReservaSalas.Application.Reservas.Commands.DeleteReserva
{
    public record DeleteReservaCommand(int Id) : IRequest<bool>;
}
