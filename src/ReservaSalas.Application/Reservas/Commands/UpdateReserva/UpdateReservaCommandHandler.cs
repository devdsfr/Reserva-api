using MediatR;
using Microsoft.EntityFrameworkCore;
using ReservaSalas.Application.Interfaces;
using ReservaSalas.Application.Common.Interfaces;
using ReservaSalas.Application.Reservas;
using ReservaSalas.Domain.Entities;

namespace ReservaSalas.Application.Reservas.Commands.UpdateReserva
{
    public class UpdateReservaCommandHandler : IRequestHandler<UpdateReservaCommand, ReservaDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly IReservaValidator _validator;

        public UpdateReservaCommandHandler(IApplicationDbContext context, IReservaValidator validator)
        {
            _context = context;
            _validator = validator;
        }

        public async Task<ReservaDto> Handle(UpdateReservaCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Reservas
                .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

            if (entity == null)
            {
                // TODO Em um cenário real, lançaríamos uma exceção customizada (ex: NotFoundException)
                return null;
            }

            var updatedReserva = new Reserva
            {
                Room = request.Room,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                ReservedBy = request.ReservedBy
            };

            await _validator.ValidateConflict(updatedReserva, cancellationToken, request.Id);

            entity.Room = request.Room;
            entity.StartDate = request.StartDate;
            entity.EndDate = request.EndDate;
            entity.ReservedBy = request.ReservedBy;

            await _context.SaveChangesAsync(cancellationToken);

            return new ReservaDto
            {
                Id = entity.Id,
                Room = entity.Room,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                ReservedBy = entity.ReservedBy
            };
        }
    }
}
