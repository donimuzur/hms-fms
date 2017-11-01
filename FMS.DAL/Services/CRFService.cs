using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.Contract;
using FMS.Contract.Service;
using FMS.Core;

namespace FMS.DAL.Services
{
    public class CrfService : ICRFService
    {
        private IUnitOfWork _uow;
        
        private IGenericRepository<TRA_CRF> _crfRepository;
        private string _includeTables = "MST_VENDOR,MST_EPAF,MST_EMPLOYEE,MST_REMARK";

        public CrfService(IUnitOfWork uow)
        {
            _uow = uow;

            _crfRepository = _uow.GetGenericRepository<TRA_CRF>();
        }


        public List<TRA_CRF> GetList(bool isActive = false)
        {
            return _crfRepository.Get(x => x.IS_ACTIVE == isActive).ToList();
        }

        public TRA_CRF GetByNumber(string documentNumber)
        {
            return _crfRepository.Get(x => x.DOCUMENT_NUMBER == documentNumber, null, _includeTables).FirstOrDefault();
        }

        public TRA_CRF GetById(int id)
        {
            return _crfRepository.GetByID(id);
        }

        public long SaveCrf(TRA_CRF data,Login userData)
        {
            _crfRepository.InsertOrUpdate(data,userData, Enums.MenuList.TraCrf);
            _uow.SaveChanges();
            return data.TRA_CRF_ID;
        } 
    }
}
