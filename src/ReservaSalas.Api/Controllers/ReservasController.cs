using MediatR;
using Microsoft.AspNetCore.Mvc;
using ReservaSalas.Application.Reservas.Commands.CreateReserva;
using ReservaSalas.Application.Reservas.Commands.DeleteReserva;
using ReservaSalas.Application.Reservas.Commands.UpdateReserva;
using ReservaSalas.Application.Reservas.Queries.GetReservaById;
using ReservaSalas.Application.Reservas.Queries.GetReservasByRoom;
using ReservaSalas.Application.Reservas;
using ReservaSalas.Application.Common.Exceptions;

namespace ReservaSalas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservasController : ControllerBase
    {
        private readonly ISender _mediator;

        public ReservasController(ISender mediator)
        {
            _mediator = mediator;
        }

        // POST: api/Reservas
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ReservaDto>> Create(CreateReservaCommand command)
        {
            try
            {
                var reserva = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetById), new { id = reserva.Id }, reserva);
            }
            catch (ReservaConflictException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/Reservas/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ReservaDto>> GetById(int id)
        {
            var reserva = await _mediator.Send(new GetReservaByIdQuery(id));

            if (reserva == null)
            {
                return NotFound();
            }

            return Ok(reserva);
        }

        // GET: api/Reservas/Room/{room}
        [HttpGet("Room/{room}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ReservaDto>>> GetByRoom(string room, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var reservas = await _mediator.Send(new GetReservasByRoomQuery(room, startDate, endDate));
            return Ok(reservas);
        }

        // PUT: api/Reservas/{id}
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ReservaDto>> Update(int id, UpdateReservaCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }

            try
            {
                var reserva = await _mediator.Send(command);

                if (reserva == null)
                {
                    return NotFound();
                }

                return Ok(reserva);
            }
            catch (ReservaConflictException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/Reservas/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteReservaCommand(id));

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
