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
        private IFleetService _fleetService;
        private IVendorService _vendorService;
        private IWorkflowHistoryService _workflowService;

        private IUnitOfWork _uow;
        public CrfBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _CrfService = new CrfService(_uow);
            _epafService = new EpafService(_uow);
            _employeeService = new EmployeeService(_uow);
            _docNumberService = new DocumentNumberService(_uow);
            _workflowService = new WorkflowHistoryService(_uow);
            _vendorService = new VendorService(_uow);
            _fleetService = new FleetService(_uow);
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

        

        

        

        public TraCrfDto AssignCrfFromEpaf(long epafId, Login CurrentUser)
        {

            var epafData = _epafService.GetEpafById(epafId);
            
            if (epafData != null)
            {
                var existingData = _CrfService.GetByEpafId(epafId);
                if (existingData != null)
                {
                    throw new Exception("Epaf Already asigned.");
                }
                
                TraCrfDto item = new TraCrfDto();
                var employeeData = _employeeService.GetEmployeeById(epafData.EMPLOYEE_ID);
                //var employeeData = _employeeBLL.GetByID(epafData.EmployeeId);

                item = Mapper.Map<TraCrfDto>(epafData);
                if (CurrentUser.UserRole == Enums.UserRole.HR)
                {
                    item.VEHICLE_TYPE = "BENEFIT";
                    item.COST_CENTER_NEW = item.COST_CENTER;
                }
                else
                {
                    throw new Exception("You are not authorized for this action.");
                }

                if (employeeData == null)
                {
                    throw new Exception(string.Format("Employee Data {0} not found.", epafData.EMPLOYEE_ID));
                }

                item.CREATED_BY = CurrentUser.USER_ID;
                item.CREATED_DATE = DateTime.Now;
                item.DOCUMENT_STATUS = (int) Enums.DocumentStatus.Draft;
                item.IS_ACTIVE = true;

                var vehicleData = _fleetService.GetFleetByParam(new FleetParamInput()
                {
                    EmployeeId = epafData.EMPLOYEE_ID,
                    VehicleType = item.VEHICLE_TYPE,

                }).FirstOrDefault();

                if (vehicleData != null)
                {
                    var vendorData =
                        _vendorService.GetVendor().FirstOrDefault(x => x.SHORT_NAME == vehicleData.VENDOR_NAME);
                    item.POLICE_NUMBER = vehicleData.POLICE_NUMBER;
                    item.MANUFACTURER = vehicleData.MANUFACTURER;
                    item.MODEL = vehicleData.MODEL;
                    item.SERIES = vehicleData.SERIES;
                    item.BodyType = vehicleData.BODY_TYPE;
                    item.VENDOR_NAME = vendorData != null ? vendorData.SHORT_NAME : null;
                    item.VENDOR_ID = vendorData != null ? vendorData.MST_VENDOR_ID : (int?) null;
                    item.START_PERIOD = vehicleData.START_DATE;
                    item.END_PERIOD = vehicleData.END_DATE;
                }


                var returnData = this.SaveCrf(item, CurrentUser);

                //AddWorkflowHistory(returnData,CurrentUser,Enums.ActionType.Created, null);

                return returnData;
            }
            else
            {
                throw new Exception("Please select Epaf document first.");
            }
        }

        public TraCrfDto SaveCrf(TraCrfDto data,Login userLogin)
        {
            
            var datatosave = Mapper.Map<TRA_CRF>(data);
            datatosave.BODY_TYPE = data.BodyType;
            if (datatosave.TRA_CRF_ID > 0)
            {

            }
            else
            {
                //datatosave.role_type
                    

                datatosave.DOCUMENT_NUMBER = _docNumberService.GenerateNumber(new GenerateDocNumberInput() { 
                    Month = DateTime.Now.Month,
                    Year = DateTime.Now.Year,
                    DocType = (int) Enums.DocumentType.CRF
                });
                    
                    
            }
                
                
            datatosave.MST_REMARK = null;
            datatosave.REMARK = null;
            data.TRA_CRF_ID = _CrfService.SaveCrf(datatosave, userLogin);
                
            AddWorkflowHistory(data,userLogin,Enums.ActionType.Created, null);
            return data;
            
            
            
        }

        public bool IsAllowedEdit(Login currentUser, TraCrfDto data)
        {
            
            if (currentUser.EMPLOYEE_ID == data.EMPLOYEE_ID 
                && data.DOCUMENT_STATUS == (int) Enums.DocumentStatus.AssignedForUser) 
                return true;

            if (currentUser.UserRole == Enums.UserRole.HR 
                && data.DOCUMENT_STATUS == (int) Enums.DocumentStatus.Draft)
                return true;
            if (currentUser.UserRole == Enums.UserRole.Fleet 
                && data.DOCUMENT_STATUS == (int)Enums.DocumentStatus.AssignedForFleet)
                return true;
            
            return false;
        }

        public bool IsAllowedApprove(Login currentUser, TraCrfDto data)
        {
            bool isAllowed = false;

            if (currentUser.UserRole == Enums.UserRole.HR
                && data.DOCUMENT_STATUS == (int)Enums.DocumentStatus.WaitingHRApproval)
                return true;
            if (currentUser.UserRole == Enums.UserRole.Fleet
                && data.DOCUMENT_STATUS == (int)Enums.DocumentStatus.WaitingFleetApproval)
                return true;

            return isAllowed;
        }

        public void SubmitCrf(long crfId,Login currentUser)
        {
            var data = _CrfService.GetById((int)crfId);

            if (currentUser.UserRole == Enums.UserRole.HR && data.VEHICLE_TYPE.ToUpper() == "BENEFIT")
            {
                data.DOCUMENT_STATUS = (int) Enums.DocumentStatus.AssignedForUser;

            }
            
            if (currentUser.UserRole == Enums.UserRole.Fleet && data.VEHICLE_TYPE.ToUpper() == "WTC")
            {
                data.DOCUMENT_STATUS = (int) Enums.DocumentStatus.AssignedForUser;
            }

            if (currentUser.EMPLOYEE_ID == data.EMPLOYEE_ID
                && data.DOCUMENT_STATUS == (int)Enums.DocumentStatus.AssignedForUser)
            {
                data.DOCUMENT_STATUS = (int) (data.VEHICLE_TYPE.ToUpper() == "WTC"
                    ? Enums.DocumentStatus.AssignedForFleet : Enums.DocumentStatus.AssignedForHR);
            }
            data.IS_ACTIVE = true;
            _CrfService.SaveCrf(data, currentUser);
            var crfDto = Mapper.Map<TraCrfDto>(data);
            AddWorkflowHistory(crfDto,currentUser,Enums.ActionType.Submit,null);
            
        }

        public List<TraCrfDto> GetCrfByParam(TraCrfEpafParamInput input)
        {
            var data = _CrfService.GetList(input);

            return Mapper.Map<List<TraCrfDto>>(data);
        }

        

        private void AddWorkflowHistory(TraCrfDto input,Login currentUserLogin,Enums.ActionType action, int? RemarkId)
        {
            var dbData = new WorkflowHistoryDto();
            dbData.ACTION_BY = currentUserLogin.USER_ID;
            dbData.FORM_ID = input.TRA_CRF_ID;
            dbData.MODUL_ID = Enums.MenuList.TraCrf;
            dbData.REMARK_ID = RemarkId;
            dbData.ACTION_DATE = DateTime.Now;
            dbData.ACTION = action;
            dbData.REMARK_ID = null;
            
            _workflowService.Save(dbData);

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
