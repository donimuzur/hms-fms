using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.Contract.BLL;
using FMS.Contract.Service;
using FMS.BusinessObject;
using FMS.BusinessObject.Dto;
using FMS.Contract;
using FMS.DAL.Services;
using AutoMapper;

namespace FMS.BLL.CarComplaintForm
{
    public class CarComplaintFormBLL : ICarComplaintFormBLL
    {
        private ICarComplaintFormService _ccf;
        private IUnitOfWork _uow;

        public CarComplaintFormBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _ccf = new CarComplaintFormService(uow);
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

        public void Save(CarComplaintFormDto CCFDto)
        {
            var dbCCF = Mapper.Map<TRA_CCF>(CCFDto);
            _ccf.save(dbCCF);
        }
    }
}
