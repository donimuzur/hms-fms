using FMS.BusinessObject;
using FMS.Contract;
using FMS.Contract.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.DAL.Services
{
    public class CafService : ICAFService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<TRA_CAF> _traCafRepository;

        public CafService(IUnitOfWork uow)
        {
            _uow = uow;
            _traCafRepository = _uow.GetGenericRepository<TRA_CAF>();
        }

        public void Save(TRA_CAF datatoSave, BusinessObject.Business.Login CurrentUser)
        {
            _traCafRepository.InsertOrUpdate(datatoSave,CurrentUser,Core.Enums.MenuList.TraCaf);
            _uow.SaveChanges();
        }


        public TRA_CAF GetCafByNumber(string p)
        {
            return _traCafRepository.Get(x => x.SIRS_NUMBER == p, null, "").FirstOrDefault();
        }


        public List<TRA_CAF> GetList()
        {
            return _traCafRepository.Get(x => x.IS_ACTIVE).ToList();
        }
    }
}
