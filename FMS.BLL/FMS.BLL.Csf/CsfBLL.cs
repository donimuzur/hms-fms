using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.Contract.BLL;
using FMS.Contract.Service;
using FMS.BusinessObject;
using FMS.BusinessObject.Dto;
using FMS.Contract;
using FMS.DAL.Services;
using AutoMapper;

namespace FMS.BLL.Csf
{
    public class CsfBLL : ITraCsfBLL
    {
        //private ILogger _logger;
        private ICsfService _CsfService;
        private IUnitOfWork _uow;
        public CsfBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _CsfService = new CsfService(_uow);
        }
    }
}
