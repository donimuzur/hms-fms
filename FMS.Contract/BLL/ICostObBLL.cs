using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Inputs;

namespace FMS.Contract.BLL
{
    public interface ICostObBLL
    {
        List<CostObDto> GetCostOb();
        CostObDto GetExist(string Model);
        CostObDto GetByID(int Id, bool? Archived = null);
        void Save(CostObDto CostObDto);
        void Save(CostObDto CostObDto, Login userLogin);
        void SaveChanges();
        List<CostObDto> GetByFilter(CostObParamInput filter);
    }
}
