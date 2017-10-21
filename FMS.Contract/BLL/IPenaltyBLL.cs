using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject.Dto;

namespace FMS.Contract.BLL
{
    public interface IPenaltyBLL
    {
        List<PenaltyDto> GetPenalty();
        PenaltyDto GetByID(int Id);
        void Save(PenaltyDto PenaltyDto);
    }
}
