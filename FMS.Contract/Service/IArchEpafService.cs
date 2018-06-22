using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Contract.Service
{
    public interface IArchEpafService
    {
        void Save(ARCH_MST_EPAF db, Login userlogin);
        List<ARCH_MST_EPAF> GetEpaf();
        List<ARCH_MST_EPAF> GetEpaf(EpafParamInput input);
        ARCH_MST_EPAF GetEpafById(long? epafId);
    }
}
