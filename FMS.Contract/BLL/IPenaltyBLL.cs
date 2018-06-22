using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Inputs;

namespace FMS.Contract.BLL
{
    public interface IPenaltyBLL
    {
        List<PenaltyDto> GetPenalty();
        PenaltyDto GetByID(int Id, bool? Archive = null);
        void Save(PenaltyDto PenaltyDto);
        void Save(PenaltyDto PenaltyDto, Login userLogin);
        void SaveChanges();
        List<PenaltyDto> GetPenalty(PenaltyParamInput input);
    }
}
