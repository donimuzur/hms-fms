using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject;
using FMS.Core;
using FMS.BusinessObject.Inputs;

namespace FMS.Contract.BLL
{
    public interface IEpafBLL
    {
        List<EpafDto> GetEpaf();
        List<EpafDto> GetEpaf(EpafParamInput filter);
        List<EpafDto> GetEpafByDocType(Enums.DocumentType docType);
        void DeactivateEpaf(long epafId, int Remark, string user);

        EpafDto GetEpafById(long? epafId);
    }
}
