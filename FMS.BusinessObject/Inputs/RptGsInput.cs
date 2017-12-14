using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BusinessObject.Inputs
{
    public class RptGsInput
    {
        public DateTime? StartDateBegin { get; set; }

        public DateTime? StartDateEnd { get; set; }

        public DateTime? EndDateBegin { get; set; }

        public DateTime? EndDateEnd { get; set; }

        public string VehicleUsage { get; set; }

        public string Location { get; set; }
    }
}
