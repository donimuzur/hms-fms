using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;

namespace FMS.Contract.BLL
{
    public interface ITraTemporaryBLL
    {
        List<TemporaryDto> GetTemporary(Login userLogin, bool isCompleted);
        List<TemporaryDto> GetTempPersonal(Login userLogin);
        TemporaryDto Save(TemporaryDto item, Login userLogin);
        void TempWorkflow(TempWorkflowDocumentInput input);
        TemporaryDto GetTempById(long id);
        List<VehicleFromVendorUpload> ValidationUploadDocumentProcess(List<VehicleFromVendorUpload> inputs, int id);
        void CheckTempInProgress();
    }
}
