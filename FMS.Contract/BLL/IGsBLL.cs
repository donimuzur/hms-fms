using FMS.BusinessObject.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Contract.BLL
{
    public interface IGsBLL
    {
        List<GsDto> GetGs();
        GsDto GetGsById(int MstGsId);
        void Save(GsDto Dto);
    }
}
