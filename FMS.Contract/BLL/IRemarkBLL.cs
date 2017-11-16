using FMS.BusinessObject.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject.Business;

namespace FMS.Contract.BLL
{
    public interface IRemarkBLL
    {
        List<RemarkDto> GetRemark();
        void Save(RemarkDto RemarkDto);
        RemarkDto GetRemarkById(int MstRemarkId);
        void Save(RemarkDto dto, Login currentUser);
    }
}
