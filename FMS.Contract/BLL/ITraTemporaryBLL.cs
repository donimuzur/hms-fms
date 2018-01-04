﻿using System;
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
        List<VehicleFromVendorUpload> ValidationUploadDocumentProcessMassUpload(List<VehicleFromVendorUpload> inputs);
        void CheckTempInProgress();
        bool CheckTempExistsInFleet(TemporaryDto item);
        bool CheckTempOpenExists(TemporaryDto item);
        List<TemporaryDto> GetList();
        void CancelTemp(long id, int Remark, string user);
    }
}
