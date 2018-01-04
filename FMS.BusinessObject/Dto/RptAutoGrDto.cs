using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BusinessObject.Dto
{
    public class RptAutoGrDto
    {
        public int AutoGrId { get; set; }

        public string PoNumber { get; set; }

        public string PoLine { get; set; }

        public DateTime? GrDate { get; set; }

        public string PoliceNumber { get; set; }

        public DateTime? StartContract { get; set; }

        public DateTime? EndContract { get; set; }


        public DateTime? TerminationDate { get; set; }

        public decimal QtyAutoGr { get; set; }

        public decimal QtyCalculated { get; set; }

        public decimal QtyRemaining { get; set; }
    }
}
