namespace RentApp.Models.Entities
{
    public class Vehicle
    {
        public int Id { get; set; }

        [Required]
        public string Model { get; set; }

        [Required]
        public string Manufacturer { get; set; }

        [Required]
        public int YearOfProduction { get; set; }

        [Required]
        public string Description { get; set; }

        public bool IsAvailable { get; set; }

        public List<Item> Items { get; set; }

        public List<VehicleImage> Images { get; set; }

        public List<Reservation> Reservations { get; set; }

        [ForeignKey("Type")]
        public int TypeId { get; set; }

        [ForeignKey("VehicleService")]
        public int VehicleServiceId { get; set; }

        public Service VehicleService { get; set; }
    }
}