namespace ReservaSalas.Application.Reservas
{
    public class ReservaDto
    {
        public int Id { get; set; }
        public string Room { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ReservedBy { get; set; } = string.Empty;
    }
}
