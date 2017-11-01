using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject;

namespace FMS.Contract.BLL
{
    public interface ICostObBLL
    {
        List<CostObDto> GetCostOb();
        CostObDto GetExist(string Model);
        CostObDto GetByID(int Id);
        void Save(CostObDto CostObDto);
    }
}
