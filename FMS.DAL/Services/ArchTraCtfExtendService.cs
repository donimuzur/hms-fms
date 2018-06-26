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
    public class ArchTraCtfExtendService : IArchTraCtfExtendService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<ARCH_TRA_CTF_EXTEND> _archTraCtfExtendRepository;
        
        public ArchTraCtfExtendService(IUnitOfWork uow)
        {
            _uow = uow;
            _archTraCtfExtendRepository= _uow.GetGenericRepository<ARCH_TRA_CTF_EXTEND>();
        }
        public void Save(ARCH_TRA_CTF_EXTEND db, Login userlogin)
        {
            _uow.GetGenericRepository<ARCH_TRA_CTF_EXTEND>().InsertOrUpdate(db, userlogin, Enums.MenuList.MasterData);
        }
        public ARCH_TRA_CTF_EXTEND GetCtfExtendByCtfId(long? TraCtfId)
        {
            return _archTraCtfExtendRepository.Get(x => x.TRA_CTF_ID == TraCtfId).FirstOrDefault();
        }
    }
}
