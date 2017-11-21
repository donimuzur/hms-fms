using AutoMapper;
using FMS.BusinessObject;
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

namespace FMS.BLL.CAF
{
    public class CafBLL : ICafBLL
    {
        private ICAFService _CafService;
        private IUnitOfWork _uow;

        private IDocumentNumberService _docNumberService;
        private IWorkflowHistoryService _workflowService;
        private ISettingService _settingService;
        private IMessageService _messageService;
        private IEmployeeService _employeeService;
        private IEpafService _epafService;
        private IRemarkService _remarkService;
        private ITemporaryService _temporaryService;
        private IVendorService _vendorService;

        public CafBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _CafService = new CafService(_uow);

            _docNumberService = new DocumentNumberService(_uow);
            _workflowService = new WorkflowHistoryService(_uow);
            _settingService = new SettingService(_uow);
            _messageService = new MessageService(_uow);
            _employeeService = new EmployeeService(_uow);
            _epafService = new EpafService(_uow);
            _remarkService = new RemarkService(_uow);
            _temporaryService = new TemporaryService(_uow);
            _vendorService = new VendorService(_uow);
        }

        public void Save(BusinessObject.Dto.TraCafDto data, BusinessObject.Business.Login user)
        {
            throw new NotImplementedException();
        }

        public List<BusinessObject.Dto.TraCafDto> GetCaf()
        {
            var data = _CafService.GetList();

            return Mapper.Map<List<TraCafDto>>(data);
        }

        public BusinessObject.Dto.TraCafDto GetById(long id)
        {
            var data = _CafService.GetCafById(id);

            return Mapper.Map<TraCafDto>(data);
        }

        public void SaveList(List<TraCafDto> data, BusinessObject.Business.Login CurrentUser)
        {
            
            var datatoSave = Mapper.Map<List<TRA_CAF>>(data);
            foreach (var caf in datatoSave)
            {
                TRA_CAF dataCaf = _CafService.GetCafByNumber(caf.SIRS_NUMBER);
                if (!string.IsNullOrEmpty(caf.VENDOR_NAME))
                {
                    var vendorData = _vendorService.GetByShortName(caf.VENDOR_NAME);
                    caf.VENDOR_ID = vendorData.MST_VENDOR_ID;
                }
                else
                {
                    caf.VENDOR_ID = null;
                }
                if (dataCaf == null)
                {
                    caf.REMARK = null;
                    caf.IS_ACTIVE = true;
                    caf.DOCUMENT_NUMBER = _docNumberService.GenerateNumber(new GenerateDocNumberInput()
                    {
                        DocType = (int) Enums.DocumentType.CAF,
                        Month = DateTime.Now.Month,
                        Year = DateTime.Now.Year

                    });
                    caf.DOCUMENT_STATUS = (int) Enums.DocumentStatus.Draft;
                    _CafService.Save(caf, CurrentUser);
                }
                else
                {
                    dataCaf.IS_ACTIVE = true;
                    dataCaf.REMARK = null;
                }
                
                
            }
            _uow.SaveChanges();
        }


        public void ValidateCaf(TraCafDto dataTovalidate, out string message)
        {
            var dbData = _CafService.GetCafByNumber(dataTovalidate.SirsNumber);
            message = "";
            if (dbData != null)
            {
                message += "Sirs Number already registered in FMS.";
            }
        }


        public TraCafDto GetCafBySirs(string sirsNumber)
        {
            var data = _CafService.GetCafByNumber(sirsNumber);
            return Mapper.Map<TraCafDto>(data);
        }


        public int SaveProgress(TraCafProgressDto traCafProgressDto,string sirsNumber, BusinessObject.Business.Login CurrentUser)
        {
            var data = Mapper.Map<TRA_CAF_PROGRESS>(traCafProgressDto);

            data.CREATED_BY = CurrentUser.USER_ID;
            data.CREATED_DATE = DateTime.Now;
            _CafService.SaveProgress(data,sirsNumber,CurrentUser);
            var caf = _CafService.GetCafByNumber(sirsNumber);
            var lastStatus = caf.TRA_CAF_PROGRESS.OrderByDescending(x => x.STATUS_ID).Select(x => x.STATUS_ID).FirstOrDefault();
            if (lastStatus < caf.DOCUMENT_STATUS)
            {
                _workflowService.Save(new WorkflowHistoryDto()
                {
                    ACTION = Enums.ActionType.Modified,
                    ACTION_DATE = DateTime.Now,
                    ACTION_BY = CurrentUser.USER_ID,
                    FORM_ID = caf.TRA_CAF_ID,
                    MODUL_ID = Enums.MenuList.TraCaf
                    
                });
                _uow.SaveChanges();
            }
            return  lastStatus.HasValue ? lastStatus.Value : 0;
        }
    }
}
