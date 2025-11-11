using MediatR;
using Microsoft.EntityFrameworkCore;
using ReservaSalas.Application.Interfaces;

namespace ReservaSalas.Application.Reservas.Queries.GetReservaById
{
    public class GetReservaByIdQueryHandler : IRequestHandler<GetReservaByIdQuery, ReservaDto>
    {
        private readonly IApplicationDbContext _context;

        public GetReservaByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ReservaDto> Handle(GetReservaByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _context.Reservas
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

            if (entity == null)
            {
                return null;
            }

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
