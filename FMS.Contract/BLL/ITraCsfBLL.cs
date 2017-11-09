using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject.Dto;

namespace FMS.Contract.BLL
{
    public interface ITraCsfBLL
    {
        List<TraCsfDto> GetCsf();
        TraCsfDto Save(TraCsfDto item, string userId);
    }
}
