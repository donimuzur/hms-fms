using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BusinessObject.Dto
{
    public class LocationChangeDto
    {
        public string EmployeeId { get; set; }
        public string FormalName{ get; set; }
        public string BasetownOld { get; set; }
        public string BasetownNew { get; set; }
        public string AddressOld { get; set; }
        public string AddressNew { get; set; }
        public string CityOld { get; set; }
        public string CityNew { get; set; }
        public DateTime ChangeDate { get; set; }
        public DateTime DateSend { get; set; }
        public long LocationChangeId { get; set; }

    }
}
