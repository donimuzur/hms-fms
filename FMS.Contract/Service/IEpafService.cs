using FMS.BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.Core;
using FMS.BusinessObject.Inputs;

namespace FMS.Contract.Service
{
    public interface IEpafService
    {
        List<MST_EPAF> GetEpaf();
        List<MST_EPAF> GetEpaf(EpafParamInput input);

        List<MST_EPAF> GetEpafByDocumentType(Enums.DocumentType docType);

        void DeactivateEpaf(long epafId, int Remark, string user);

        MST_EPAF GetEpafById(long? epafId);
        
    }
}
