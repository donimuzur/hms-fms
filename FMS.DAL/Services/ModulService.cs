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
    public class ModulService : IModulService
    {
        private IGenericRepository<MST_MODUL> _modulRepository;
        private IUnitOfWork _uow;

        public ModulService(IUnitOfWork uow)
        {
            _uow = uow;
            _modulRepository = uow.GetGenericRepository<MST_MODUL>();
        }

        public List<MST_MODUL> GetModul()
        {
            return _modulRepository.Get().ToList();
        }

        public MST_MODUL GetModulById(int MstModulById)
        {
            return _modulRepository.GetByID(MstModulById);
        }
    }
}
