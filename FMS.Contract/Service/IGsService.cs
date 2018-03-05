using FMS.BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Inputs;

namespace FMS.Contract.Service
{
    public interface IGsService
    {
        List<MST_GS> GetGs();
        MST_GS GetGsById(int MstGsId);
        void Save(MST_GS dbGs);
        void Save(MST_GS dbGs, Login userLogin);
        List<MST_GS> GetGsByParam(RptGsInput input);
        List<MST_GS> GetGsByParam(GSParamInput input);
    }
}
