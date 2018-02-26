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
    public interface ITraCsfBLL
    {
        List<TraCsfDto> GetCsf(Login userLogin, bool isCompleted);
        List<TraCsfDto> GetCsfPersonal(Login userLogin);
        TraCsfDto Save(TraCsfDto item, Login userLogin);
        void CsfWorkflow(CsfWorkflowDocumentInput input);
        void CancelCsf(long id, int Remark, string user);
        TraCsfDto GetCsfById(long id);
        List<EpafDto> GetCsfEpaf(bool isActive = true);
        TemporaryDto SaveTemp(TemporaryDto item, Login userLogin);
        List<TemporaryDto> GetTempByCsf(string csfNumber);
        List<VehicleFromVendorUpload> ValidationUploadDocumentProcess(List<VehicleFromVendorUpload> inputs, int id);
        List<VehicleFromVendorUpload> ValidationUploadDocumentProcessMassUpload(List<VehicleFromVendorUpload> inputs);
        List<VehicleFromUserUpload> ValidationUploadVehicleProcess(List<VehicleFromUserUpload> inputs, int id);
        void CheckCsfInProgress();
        bool CheckCsfExists(TraCsfDto item);
        bool CheckCsfOpenExists(TraCsfDto item);
        List<TraCsfDto> GetList();
        bool BatchEmailCsf(List<TraCsfDto> ListCsf, string Vendor, string AttachmentWtc, string AttachmentBenefit);
        void SendEmailForErrorBatch(string messageError);
        void SendEmailNotificationCfmIdle(TraCsfDto csfData, TraCtfDto ctfData);
        void EpafCSFNotif();
    }
}
