using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.Contract;
using FMS.Contract.Service;
using FMS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.DAL.Services
{
    public class GsService : IGsService
    {
        private IGenericRepository<MST_GS> _gsRepository;
        private IUnitOfWork _uow;

        public GsService(IUnitOfWork uow)
        {
            _uow = uow;
            _gsRepository = _uow.GetGenericRepository<MST_GS>();
        }

        public List<MST_GS> GetGs()
        {
            return _gsRepository.Get().ToList();
        }

        public MST_GS GetGsById(int MstGsId)
        {
            return _gsRepository.GetByID(MstGsId);
        }
        public void Save(MST_GS dbGs)
        {
            _gsRepository.InsertOrUpdate(dbGs);
            _uow.SaveChanges();
        }
        public void Save(MST_GS dbGs, Login userLogin)
        {
            _gsRepository.InsertOrUpdate(dbGs, userLogin, Enums.MenuList.MasterGS);
            _uow.SaveChanges();
        }

    }
}
