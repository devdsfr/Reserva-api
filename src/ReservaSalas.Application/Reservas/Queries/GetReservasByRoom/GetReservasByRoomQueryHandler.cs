using MediatR;
using Microsoft.EntityFrameworkCore;
using ReservaSalas.Application.Interfaces;
using ReservaSalas.Application.Reservas;

namespace ReservaSalas.Application.Reservas.Queries.GetReservasByRoom
{
    public class GetReservasByRoomQueryHandler : IRequestHandler<GetReservasByRoomQuery, IEnumerable<ReservaDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetReservasByRoomQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ReservaDto>> Handle(GetReservasByRoomQuery request, CancellationToken cancellationToken)
        {
            var reservas = await _context.Reservas
                .AsNoTracking()
                .Where(r => r.Room == request.Room &&
                            r.StartDate < request.EndDate &&
                            r.EndDate > request.StartDate)
                .Select(r => new ReservaDto
                {
                    Id = r.Id,
                    Room = r.Room,
                    StartDate = r.StartDate,
                    EndDate = r.EndDate,
                    ReservedBy = r.ReservedBy
                })
                .ToListAsync(cancellationToken);

            return reservas;
        }
    }
}
