namespace FMS.Website.Models
{
    public class OdometerProps
    {
        public string Function { get; set; }
        public VehicleTypeDetail WTC { get; set; }
        public VehicleTypeDetail Benefit { get; set; }
    }
    public class VehicleTypeDetail
    {

        public string VehicleType { get; set; }
        public decimal? TotalKM { get; set; }
    }
}