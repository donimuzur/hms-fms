using FMS.BusinessObject;
using FMS.Contract;
using FMS.Contract.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.Core;

namespace FMS.DAL.Services
{
    public class EpafService : IEpafService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<MST_EPAF> _epafRepository;
        public EpafService(IUnitOfWork uow)
        {
            _uow = uow;
            _epafRepository = _uow.GetGenericRepository<MST_EPAF>();
        }

        public List<MST_EPAF> GetEpaf()
        {
            return _epafRepository.Get().ToList();
        }

        public List<MST_EPAF> GetEpafByDocumentType(Enums.DocumentType docType)
        {
            return _epafRepository.Get(x => x.DOCUMENT_TYPE == (int)docType && x.IS_ACTIVE && x.REMARK ==null).ToList();
        }

        public void DeactivateEpaf(long epafId, int Remark, string user)
        {
            var data = _epafRepository.GetByID(epafId);

            if (data != null)
            {
                data.MODIFIED_DATE = DateTime.Now;
                data.MODIFIED_BY = user;
                data.IS_ACTIVE = false;
                data.REMARK = Remark;
                _uow.SaveChanges();
            }
        }
    }
}
