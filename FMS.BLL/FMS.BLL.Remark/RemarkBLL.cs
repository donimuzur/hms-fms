using AutoMapper;
using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Dto;
using FMS.Contract;
using FMS.Contract.BLL;
using FMS.Contract.Service;
using FMS.DAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BLL.Remark
{
    public class RemarkBLL :  IRemarkBLL
    {
        private IRemarkService _remarkService;
        private IUnitOfWork _uow;
        
        public RemarkBLL(IUnitOfWork uow)
        {
            _remarkService = new RemarkService(uow);
            _uow = uow;
        }

        public List<RemarkDto> GetRemark()
        {
            var data = _remarkService.GetRemark();
            var redata = Mapper.Map<List<RemarkDto>>(data);
            return redata;

        }
        public void Save(RemarkDto RemarkDto)
        {
            var dbVRemark = Mapper.Map<MST_REMARK>(RemarkDto);
            _remarkService.save(dbVRemark);
        }
        public RemarkDto GetRemarkById(int MstRemarkId)
        {
            var data = _remarkService.GetRemarkById(MstRemarkId);
            var redata = Mapper.Map<RemarkDto>(data);
            return redata;
        }
        public void Save(RemarkDto RemarkDto, Login userLogin)
        {
            var dbVRemark = Mapper.Map<MST_REMARK>(RemarkDto);
            _remarkService.save(dbVRemark, userLogin);
        }
        public void SaveChanges()
        {
            _uow.SaveChanges();
        }
    }
}
