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
    public class CarComplaintFormService : ICarComplaintFormService
    {
        private IUnitOfWork _uow;

        private IGenericRepository<TRA_CCF> _ccfRepository;
        private IGenericRepository<TRA_CCF_DETAIL> _ccfRepositoryd1;

        public CarComplaintFormService(IUnitOfWork uow)
        {
            _uow = uow;
            _ccfRepository = _uow.GetGenericRepository<TRA_CCF>();
            _ccfRepositoryd1 = _uow.GetGenericRepository<TRA_CCF_DETAIL>();
        }

        public List<TRA_CCF> GetCCF()
        {
            return _ccfRepository.Get().ToList();
        }

        public TRA_CCF GetCCFById(int Id)
        {
            return _ccfRepository.GetByID(Id);
        }

        public void save(TRA_CCF dbTraCCF, TRA_CCF_DETAIL dbTraCCFD1)
        {
            _uow.GetGenericRepository<TRA_CCF>().InsertOrUpdate(dbTraCCF);
            _uow.GetGenericRepository<TRA_CCF_DETAIL>().InsertOrUpdate(dbTraCCFD1);
            _uow.SaveChanges();
        }

        public List<TRA_CCF_DETAIL> GetCCFD1()
        {
            return _ccfRepositoryd1.Get().ToList();
        }
    }
}
