using MediatR;
using Microsoft.EntityFrameworkCore;
using ReservaSalas.Application.Interfaces;

namespace ReservaSalas.Application.Reservas.Commands.DeleteReserva
{
    public class DeleteReservaCommandHandler : IRequestHandler<DeleteReservaCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public DeleteReservaCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteReservaCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Reservas
                .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

            if (entity == null)
            {
                return false;
            }

            _context.Reservas.Remove(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
