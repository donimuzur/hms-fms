using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject;
using FMS.Core;

namespace FMS.Contract.BLL
{
    public interface IEpafBLL
    {
        List<EpafDto> GetEpaf();
        List<EpafDto> GetEpafByDocType(Enums.DocumentType docType);
    }
}
