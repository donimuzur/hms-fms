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

namespace FMS.BLL.Crf
{
    public class CrfBLL : ICrfBLL
    {
        private ICRFService _CrfService;
        private IUnitOfWork _uow;
        public CrfBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _CrfService = new CrfService(_uow);
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

        public TraCrfDto SaveCrf(TraCrfDto data)
        {
            try {
                var datatosave = Mapper.Map<TRA_CRF>(data);
                data.TRA_CRF_ID = _CrfService.SaveCrf(datatosave, data.UserLogin);
                return data;
            }
            catch (Exception ex) {
                throw ex;
            }
            
        }
    }
}
