using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.Core;
using FMS.Core.Exceptions;
using FMS.Contract.BLL;
using FMS.Contract.Service;
using FMS.BusinessObject;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;
using FMS.Contract;
using FMS.DAL.Services;
using AutoMapper;

namespace FMS.BLL.CarComplaintForm
{
    public class CarComplaintFormBLL : ICarComplaintFormBLL
    {
        private ICarComplaintFormService _ccf;
        private IUnitOfWork _uow;
        private IDocumentNumberService _docNumberService;

        public CarComplaintFormBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _ccf = new CarComplaintFormService(uow);
            _docNumberService = new DocumentNumberService(_uow);
        }

        public List<CarComplaintFormDto> GetCCF()
        {
            var data = _ccf.GetCCF();
            var retData = Mapper.Map<List<CarComplaintFormDto>>(data);
            return retData;
        }

        public CarComplaintFormDto GetCCFByID(int Id)
        {
            var data = _ccf.GetCCFById(Id);
            var retData = Mapper.Map<CarComplaintFormDto>(data);

            return retData;
        }

        public void Save(CarComplaintFormDto CCFDto, CarComplaintFormDtoDetil CCFDtoD1)
        {
            TRA_CCF model;
            
            if (CCFDto.TraCcfId > 0)
            {
                //update
                model = _ccf.GetCCFById(CCFDto.TraCcfId);

                if (model == null)
                    throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                Mapper.Map<CarComplaintFormDto, TRA_CCF>(CCFDto, model);
            }
            else
            {
                var inputDoc = new GenerateDocNumberInput();

                inputDoc.Month = DateTime.Now.Month;
                inputDoc.Year = DateTime.Now.Year;
                inputDoc.DocType = (int)Enums.DocumentType.CCF;

                CCFDto.DocumentNumber = _docNumberService.GenerateNumber(inputDoc);

                model = Mapper.Map<TRA_CCF>(CCFDto);
                

                model.TRA_CCF_ID = Convert.ToInt64(CCFDto.TraCcfId);
            }
            TRA_CCF_DETAIL model_d1;
            model_d1 = Mapper.Map<TRA_CCF_DETAIL>(CCFDtoD1);

            _ccf.save(model,model_d1);
            _uow.SaveChanges();
            
            //_ccf.save(dbCCF);
        }

        public List<CarComplaintFormDto> GetFleetByEmployee(string EmployeeId)
        {
            FMSEntities context = new FMSEntities();
            var query = (from a in context.MST_FLEET
                         where a.EMPLOYEE_ID == EmployeeId

                         select new CarComplaintFormDto()
                         {
                             Manufacturer = a.MANUFACTURER,
                             Models = a.MODEL,
                             Series = a.SERIES,
                             Vendor = a.VENDOR_NAME,
                             StartPeriod = a.START_CONTRACT,
                             EndPeriod = a.END_CONTRACT
                         });
            var dbResult = query.ToList();
            return dbResult;
        }

        public List<CarComplaintFormDtoDetil> GetCCFD1()
        {
            var datad1 = _ccf.GetCCFD1();
            var retData = Mapper.Map<List<CarComplaintFormDtoDetil>>(datad1);
            return retData;
        }
    }
}
