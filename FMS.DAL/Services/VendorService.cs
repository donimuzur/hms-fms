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

        public MST_VENDOR GetExist(string VendorName)
        {
            return _vendorRepository.Get(x => x.VENDOR_NAME == VendorName).FirstOrDefault(); ;

        }
    }
}
