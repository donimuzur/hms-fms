using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FMS.BusinessObject;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;
using FMS.Contract;
using FMS.Contract.BLL;
using FMS.Contract.Service;
using FMS.DAL.Services;

namespace FMS.BLL.AutoGR
{
    public class AutoGrBLL : IAutoGrBLL
    {
        private IUnitOfWork _uow;
        private IAutoGrService _grService;
        private IFleetService _fleetService;
        public AutoGrBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _grService = new AutoGrService(_uow);
            _fleetService = new FleetService(_uow);
        }

        public List<RptAutoGrDto> GetAutoGR(RptAutoGrInput rptAutoGrInput)
        {
            List<AUTO_GR> data = _grService.GetAutoGr(rptAutoGrInput);
            //_grService.get

            var autoGrIds = data.Select(x=> x.AUTO_GR_ID).ToList();
            List<AUTO_GR_DETAIL> details = _grService.GetAutoGrDetails(autoGrIds);
            var dataFleet = _fleetService.GetFleet();

            var autoGrDto = Mapper.Map<List<RptAutoGrDto>>(details);

            foreach(var dto in autoGrDto){
                var autoGr = data.FirstOrDefault(x=> x.AUTO_GR_ID == dto.AutoGrId);

                if(autoGr != null){
                    dto.PoNumber = autoGr.PO_NUMBER;
                    dto.GrDate = autoGr.PO_DATE.Value;
                    
                    var fleet = dataFleet.FirstOrDefault(x=> x.PO_NUMBER == dto.PoNumber && x.PO_LINE == dto.PoLine);

                    if(fleet != null){
                        dto.StartContract = fleet.START_CONTRACT;
                        dto.EndContract = fleet.END_CONTRACT;
                        dto.PoliceNumber = fleet.POLICE_NUMBER;
                        if (fleet.START_CONTRACT.HasValue && fleet.END_CONTRACT.HasValue)
                        {
                            dto.QtyRemaining = ((fleet.END_CONTRACT.Value.Year - fleet.START_CONTRACT.Value.Year) * 12) + fleet.END_CONTRACT.Value.Month - fleet.START_CONTRACT.Value.Month;
                        }
                    }
                }
                

                
                
            }

            return autoGrDto;
        }

    }
}
