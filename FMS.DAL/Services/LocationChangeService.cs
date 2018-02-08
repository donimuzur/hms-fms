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
    public class LocationChangeService : ILocationChangeService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<LOCATION_CHANGE> _locationChangeRepo;

        public LocationChangeService(IUnitOfWork uow)
        {
            _uow = uow;
            _locationChangeRepo = uow.GetGenericRepository<LOCATION_CHANGE>();
        }

        public List<LOCATION_CHANGE>GetListLocationChange()
        {
            return _locationChangeRepo.Get().Where(x => x.DATE_SEND == null).ToList();
        }

        public void Save(LOCATION_CHANGE DbLocationChange)
        {
            _locationChangeRepo.InsertOrUpdate(DbLocationChange);
            _uow.SaveChanges();
        }
    }
}
