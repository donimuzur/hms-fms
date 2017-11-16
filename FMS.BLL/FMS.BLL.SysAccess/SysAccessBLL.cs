using FMS.BusinessObject.Dto;
using FMS.Contract;
using FMS.Contract.Service;
using FMS.DAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FMS.BusinessObject;
using FMS.Contract.BLL;
using FMS.BusinessObject.Business;

namespace FMS.BLL.SysAccess
{
    public class SysAccessBLL: ISysAccessBLL
    {
        private ISysAccessService _sysAccessService;
        private IUnitOfWork _uow;
        public SysAccessBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _sysAccessService = new SysAccessService(_uow);
        }

        public List<SysAccessDto> GetSysAccess()
        {
            var data = _sysAccessService.GetSysAccess();
            var redata = Mapper.Map<List<SysAccessDto>>(data);
            return redata;
        }
        public SysAccessDto GetSysAccessById(int MstSysAccessId)
        {
            var data = _sysAccessService.GetSysAccessById(MstSysAccessId);
            var redata = Mapper.Map<SysAccessDto>(data);
            return redata;
        }
        public void Save(SysAccessDto Dto)
        {
            var dbSysAccess = Mapper.Map<MST_SYSACCESS>(Dto);
            _sysAccessService.save(dbSysAccess);
        }
        public void Save(SysAccessDto Dto, Login userLogin)
        {
            var dbSysAccess = Mapper.Map<MST_SYSACCESS>(Dto);
            _sysAccessService.save(dbSysAccess, userLogin);
        }
    }
}
