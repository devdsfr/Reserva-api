using MediatR;
using ReservaSalas.Application.Interfaces;
using ReservaSalas.Domain.Entities;
using ReservaSalas.Application.Common.Interfaces;

namespace ReservaSalas.Application.Reservas.Commands.CreateReserva
{
    public class CreateReservaCommandHandler : IRequestHandler<CreateReservaCommand, ReservaDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly IReservaValidator _validator;

        public CreateReservaCommandHandler(IApplicationDbContext context, IReservaValidator validator)
        {
            _context = context;
            _validator = validator;
        }

        public async Task<ReservaDto> Handle(CreateReservaCommand request, CancellationToken cancellationToken)
        {
            var entity = new Reserva
            {
                Room = request.Room,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                ReservedBy = request.ReservedBy
            };

            await _validator.ValidateConflict(entity, cancellationToken);

            _context.Reservas.Add(entity);

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
