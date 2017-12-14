using System.Linq.Expressions;
using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Inputs;
using FMS.Contract;
using FMS.Contract.Service;
using FMS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.Utils;

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

        public List<MST_GS> GetGsByParam(RptGsInput input)
        {
            Expression<Func<MST_GS, bool>> queryFilter = c => c.IS_ACTIVE;

            if (input != null)
            {
                if (input.StartDateBegin.HasValue)
                {
                    queryFilter = queryFilter.And(x => x.START_DATE >= input.StartDateBegin.Value);
                }

                if (input.StartDateEnd.HasValue)
                {
                    queryFilter = queryFilter.And(x => x.START_DATE <= input.StartDateEnd.Value);
                }

                if (input.EndDateBegin.HasValue)
                {
                    queryFilter = queryFilter.And(x => x.END_DATE >= input.StartDateBegin.Value);
                }

                if (input.EndDateEnd.HasValue)
                {
                    queryFilter = queryFilter.And(x => x.END_DATE <= input.StartDateEnd.Value);
                }

                if (input.EndDateBegin.HasValue)
                {
                    queryFilter = queryFilter.And(x => x.END_DATE >= input.StartDateBegin.Value);
                }

                if (!string.IsNullOrEmpty(input.VehicleUsage))
                {
                    queryFilter = queryFilter.Or(x => x.VEHICLE_USAGE.ToUpper() == input.VehicleUsage.ToUpper());
                }

                if (!string.IsNullOrEmpty(input.Location))
                {
                    queryFilter = queryFilter.Or(x => input.Location.ToUpper().Contains(x.LOCATION.ToUpper()));
                }
                //queryFilter = queryFilter.And(x=> x.)
            }

            return _gsRepository.Get(queryFilter, null, "").ToList();
        }
    }
}
