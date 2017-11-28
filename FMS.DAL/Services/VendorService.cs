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
    public class VendorService : IVendorService 
    {
        private IUnitOfWork _uow;

        private IGenericRepository<MST_VENDOR> _vendorRepository;

        public VendorService (IUnitOfWork uow)
        {
            _uow = uow;
            _vendorRepository = _uow.GetGenericRepository<MST_VENDOR>();
        }

        public List <MST_VENDOR > GetVendor()
        {
            return _vendorRepository.Get().ToList();
        }

        public MST_VENDOR GetVendorById(int MstVendorId)
        {
            return _vendorRepository.GetByID(MstVendorId);
        }

        public MST_VENDOR GetByShortName(string shortName)
        {
            return _vendorRepository.Get(x => x.SHORT_NAME == shortName).FirstOrDefault(); 
        }

        public MST_VENDOR GetExist(string VendorName)
        {
            return _vendorRepository.Get(x => x.VENDOR_NAME == VendorName).FirstOrDefault(); ;
        }

        public void save(MST_VENDOR dbVendor)
        {
            _uow.GetGenericRepository<MST_VENDOR>().InsertOrUpdate(dbVendor);
            _uow.SaveChanges();
        }

        public void save(MST_VENDOR dbVendor, Login userLogin)
        {
            _uow.GetGenericRepository<MST_VENDOR>().InsertOrUpdate(dbVendor, userLogin, Enums.MenuList.MasterVendor);
            _uow.SaveChanges();
        }
    }
}
