using AutoMapper;
using FMS.BusinessObject;
using FMS.BusinessObject.Dto;
using FMS.Contract;
using FMS.Contract.BLL;
using FMS.Contract.Service;
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
            throw new NotImplementedException();
        }

        public void SaveList(List<TraCafDto> data, BusinessObject.Business.Login CurrentUser)
        {
            
            var datatoSave = Mapper.Map<List<TRA_CAF>>(data);
            foreach (var caf in datatoSave)
            {
                TRA_CAF dataCaf = _CafService.GetCafByNumber(caf.SIRS_NUMBER);
                if (dataCaf != null)
                {
                    _CafService.Save(caf, CurrentUser);
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
    }
}
