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
        PenaltyDto GetExist(int MstPenaltyId);
        PenaltyDto GetByID(string Id);
        void Save(PenaltyDto EmployeeDto);
    }
}
