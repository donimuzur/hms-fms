using FMS.BusinessObject.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Inputs;

namespace FMS.Contract.BLL
{
    public interface IGsBLL
    {
        List<GsDto> GetGs();
        GsDto GetGsById(int MstGsId);
        void Save(GsDto Dto);
        void Save(GsDto data, Login currentUser);

        List<GsDto> GetGsReport(RptGsInput rptGsInput);

        void SaveChanges();
    }
}
