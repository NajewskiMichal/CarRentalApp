namespace CarRental.Application.DTOs
{
    public class CarDto
    {
        public int Id { get; init; }
        public string Brand { get; init; } = string.Empty;
        public string Model { get; init; } = string.Empty;
        public int Year { get; init; }
        public string Vin { get; init; } = string.Empty;
    }
}
