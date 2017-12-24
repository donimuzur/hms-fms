using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Business;

namespace FMS.Contract.BLL
{
    public interface IPenaltyBLL
    {
        List<PenaltyDto> GetPenalty();
        PenaltyDto GetByID(int Id);
        void Save(PenaltyDto PenaltyDto);
        void Save(PenaltyDto PenaltyDto, Login userLogin);
        void SaveChanges();
    }
}
