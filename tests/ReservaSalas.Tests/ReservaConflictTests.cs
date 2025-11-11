using Microsoft.EntityFrameworkCore;
using ReservaSalas.Application.Common.Exceptions;
using ReservaSalas.Application.Common.Services;
using ReservaSalas.Domain.Entities;
using ReservaSalas.Infrastructure.Persistence;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ReservaSalas.Tests
{
    public class ReservaConflictTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly ReservaValidator _validator;

        public ReservaConflictTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _validator = new ReservaValidator(_context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public async Task ValidateConflict_ShouldThrowException_WhenConflictExists()
        {
            // Arrange
            var existingReserva = new Reserva
            {
                Room = "Sala A",
                StartDate = new DateTime(2025, 11, 10, 10, 0, 0),
                EndDate = new DateTime(2025, 11, 10, 11, 0, 0),
                ReservedBy = "User 1"
            };
            _context.Reservas.Add(existingReserva);
            await _context.SaveChangesAsync(CancellationToken.None);

            var newReserva = new Reserva
            {
                Room = "Sala A",
                StartDate = new DateTime(2025, 11, 10, 10, 30, 0), // Conflito
                EndDate = new DateTime(2025, 11, 10, 11, 30, 0),
                ReservedBy = "User 2"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ReservaConflictException>(() =>
                _validator.ValidateConflict(newReserva, CancellationToken.None));
        }

        [Fact]
        public async Task ValidateConflict_ShouldNotThrowException_WhenNoConflictExists()
        {
            // Arrange
            var existingReserva = new Reserva
            {
                Room = "Sala A",
                StartDate = new DateTime(2025, 11, 10, 10, 0, 0),
                EndDate = new DateTime(2025, 11, 10, 11, 0, 0),
                ReservedBy = "User 1"
            };
            _context.Reservas.Add(existingReserva);
            await _context.SaveChangesAsync(CancellationToken.None);

            var newReserva = new Reserva
            {
                Room = "Sala A",
                StartDate = new DateTime(2025, 11, 10, 11, 0, 1), // Sem Conflito
                EndDate = new DateTime(2025, 11, 10, 12, 0, 0),
                ReservedBy = "User 2"
            };

            // Act & Assert
            await _validator.ValidateConflict(newReserva, CancellationToken.None);
        }

        [Fact]
        public async Task ValidateConflict_ShouldNotThrowException_WhenUpdatingSameReserva()
        {
            // Arrange
            var existingReserva = new Reserva
            {
                Id = 1,
                Room = "Sala A",
                StartDate = new DateTime(2025, 11, 10, 10, 0, 0),
                EndDate = new DateTime(2025, 11, 10, 11, 0, 0),
                ReservedBy = "User 1"
            };
            _context.Reservas.Add(existingReserva);
            await _context.SaveChangesAsync(CancellationToken.None);

            var updatedReserva = new Reserva
            {
                Room = "Sala A",
                StartDate = new DateTime(2025, 11, 10, 10, 30, 0), // Conflito com a reserva 1
                EndDate = new DateTime(2025, 11, 10, 11, 30, 0),
                ReservedBy = "User 1"
            };

            // Act & Assert
            // O excludeId é o Id da reserva que está sendo atualizada (1)
            await _validator.ValidateConflict(updatedReserva, CancellationToken.None, excludeId: existingReserva.Id);
        }
    }
}
