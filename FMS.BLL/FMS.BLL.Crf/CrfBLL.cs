using AutoMapper;
using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;
using FMS.Contract;
using FMS.Contract.BLL;
using FMS.Contract.Service;
using FMS.Core;
using FMS.DAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BLL.Crf
{
    public class CrfBLL : ITraCrfBLL
    {
        private ICRFService _CrfService;
        private IEpafService _epafService;
        private IDocumentNumberService _docNumberService;
        private IEmployeeService _employeeService;
        private IUnitOfWork _uow;
        public CrfBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _CrfService = new CrfService(_uow);
            _epafService = new EpafService(_uow);
            _employeeService = new EmployeeService(_uow);
            _docNumberService = new DocumentNumberService(_uow);
        }


        public List<TraCrfDto> GetList()
        {
            var data = _CrfService.GetList();

            return Mapper.Map<List<TraCrfDto>>(data);
        }

        public TraCrfDto GetDataById(long id)
        {
            var data = _CrfService.GetById((int)id);

            return Mapper.Map<TraCrfDto>(data);
        }

        public TraCrfDto SaveCrf(TraCrfDto data,Login userLogin)
        {
            
                var datatosave = Mapper.Map<TRA_CRF>(data);
                if (datatosave.TRA_CRF_ID > 0)
                {

                }
                else
                {
                    //datatosave.role_type
                    if (datatosave.EPAF_ID.HasValue && datatosave.EPAF_ID > 0)
                    {
                        var existingData = _CrfService.GetByEpafId(datatosave.EPAF_ID.Value);
                        if (existingData != null)
                        {
                            throw new Exception("Epaf Already asigned.");
                        }

                        datatosave.MST_REMARK = null;
                        datatosave.REMARK = null;

                        if (userLogin.UserRole == Enums.UserRole.HR)
                        {
                            data.VEHICLE_TYPE = "BENEFIT";
                            
                        }
                    }
                    datatosave.DOCUMENT_NUMBER = _docNumberService.GenerateNumber(new GenerateDocNumberInput() { 
                        Month = DateTime.Now.Month,
                        Year = DateTime.Now.Year,
                        DocType = (int) Enums.DocumentType.CRF
                    });
                    
                    
                }
                data.TRA_CRF_ID = _CrfService.SaveCrf(datatosave, userLogin);
                return data;
            
            
            
        }

        public List<TraCrfDto> GetCrfByParam(TraCrfEpafParamInput input)
        {
            var data = _CrfService.GetList(input);

            return Mapper.Map<List<TraCrfDto>>(data);
        }


        public List<EpafDto> GetCrfEpaf(bool isActive = true)
        {
            var data = _epafService.GetEpafByDocumentType(Enums.DocumentType.CRF);
            var dataEpaf = Mapper.Map<List<EpafDto>>(data);

            

            var dataCrf = GetList();

            var dataJoin = (from ep in dataEpaf
                             join crf in dataCrf on ep.MstEpafId equals crf.EPAF_ID
                             select new EpafDto() {
                                 CrfNumber = crf.DOCUMENT_NUMBER,
                                MstEpafId = ep.MstEpafId,
                                 CrfStatus = Utils.EnumHelper.GetDescription((Enums.DocumentStatus)crf.DOCUMENT_STATUS),
                                EmployeeId = crf.EMPLOYEE_ID,
                                EmployeeName = crf.EMPLOYEE_NAME,
                                CityNew = ep.City,
                                BaseTownNew = ep.BaseTown,
                                BaseTown = crf.LOCATION_OFFICE,
                                City = crf.LOCATION_CITY,
                                CrfId = crf.TRA_CRF_ID
                             }).ToList();

            var dataEmployee = _employeeService.GetEmployee();
            

            foreach (var dtEp in dataEpaf)
            {
                var dataCrfJoin = dataJoin.Where(x=> x.MstEpafId == dtEp.MstEpafId).FirstOrDefault();
                if (dataCrfJoin != null)
                {
                    dtEp.CrfId = dataCrfJoin.CrfId;
                    dtEp.CrfNumber = dataCrfJoin.CrfNumber;
                    dtEp.CrfStatus = dataCrfJoin.CrfStatus;
                    dtEp.BaseTownNew = dataCrfJoin.BaseTownNew;
                    dtEp.CityNew = dataCrfJoin.CityNew;
                    dtEp.City = dataCrfJoin.City;
                    dtEp.BaseTown = dataCrfJoin.BaseTown;
                }
                else
                {
                    var employee = dataEmployee.Where(x=>x.EMPLOYEE_ID == dtEp.EmployeeId).FirstOrDefault();
                    var baseTownNew = dtEp.BaseTown;
                    var cityNew = dtEp.City;
                    dtEp.BaseTown = employee.BASETOWN;
                    dtEp.City = employee.CITY;
                    dtEp.BaseTownNew = baseTownNew;
                    dtEp.CityNew = cityNew;
                }
                
            }

            return dataEpaf;
        }
    }
}
