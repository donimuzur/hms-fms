namespace FMS.Website.Models
{
    public class LiterByFunctionProps
    {
        public string Function { get; set; }
        public VehicleTypeDetailLiterByFunction WTC { get; set; }
        public VehicleTypeDetailLiterByFunction Benefit { get; set; }
    }
    public class VehicleTypeDetailLiterByFunction
    {

        public string VehicleType { get; set; }
        public decimal? TotalLiter { get; set; }
    }
}