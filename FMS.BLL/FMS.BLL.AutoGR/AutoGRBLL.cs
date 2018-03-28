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
            var Dto = Mapper.Map<List<RptAutoGrDto>>(data);
            if (data.Count == 0) return Dto;

            var GetListAutoGr = _grService.GetAutoGr();
            var polineList = data.GroupBy(x=> new { x.PO_NUMBER, x.LINE_ITEM }).Select(x => x.Key.PO_NUMBER + "_" + x.Key.LINE_ITEM).ToList();
            var dataFleet = _fleetService.GetFleet().Where(x=> polineList.Contains(x.PO_NUMBER + "_"+x.PO_LINE)).ToList();
            //var dataFleetTerminated = dataFleet.Where(x => x.END_DATE.HasValue && x.END_DATE.Value < x.END_CONTRACT).ToList();
            
            

            var autoGrDto = (from autoGr in data
                            from fleet in dataFleet.Where(x=> autoGr.PO_NUMBER == x.PO_NUMBER 
                                && autoGr.LINE_ITEM.ToString() == x.PO_LINE )
                            select new RptAutoGrDto()
                            {
                                
                                AutoGrId = autoGr.AUTO_GR_ID,
                                EndContract = fleet.END_CONTRACT,
                                StartContract = fleet.START_CONTRACT,
                                GrDate = autoGr.PO_DATE ,
                                PoLine = autoGr.LINE_ITEM.HasValue ? autoGr.LINE_ITEM.Value.ToString() : "",
                                PoNumber = autoGr.PO_NUMBER,
                                PoliceNumber = fleet.POLICE_NUMBER,
                                QtyAutoGr = autoGr.QTY_ITEM.HasValue? autoGr.QTY_ITEM.Value : 0,
                                //QtyRemaining = ((fleet.END_CONTRACT.Value.Year - fleet.START_CONTRACT.Value.Year) * 12) + fleet.END_CONTRACT.Value.Month - fleet.START_CONTRACT.Value.Month,
                                //TerminationDate = fleet.END_DATE,
                                //QtyCalculated = calculatedGr.Where(x=> x.PoLine == autoGr.LINE_ITEM)
                                
                    
                            }).ToList();

            var groupData = autoGrDto.GroupBy(x => new { x.AutoGrId, x.EndContract, x.StartContract, x.GrDate, x.PoLine, x.PoNumber, x.PoliceNumber, x.QtyAutoGr })
                .Select(p => new RptAutoGrDto()
                {
                    AutoGrId = p.FirstOrDefault().AutoGrId,
                    EndContract = p.FirstOrDefault().EndContract,
                    StartContract = p.FirstOrDefault().StartContract,
                    GrDate = p.FirstOrDefault().GrDate,
                    PoLine = p.FirstOrDefault().PoLine,
                    PoNumber = p.FirstOrDefault().PoNumber,
                    PoliceNumber = p.FirstOrDefault().PoliceNumber,
                    QtyAutoGr = p.FirstOrDefault().QtyAutoGr
                }).ToList();


            foreach (var dto in groupData)
            {
                var calculatedGr = GetListAutoGr.Where(x => x.PO_DATE < dto.GrDate)
                    .GroupBy(x => new { x.LINE_ITEM, x.PO_NUMBER })
                    .Select(x => new RptAutoGrDto()
                    {
                        PoLine = x.Key.LINE_ITEM.HasValue ? x.Key.LINE_ITEM.Value.ToString() : "",
                        PoNumber = x.Key.PO_NUMBER,
                        QtyCalculated = x.Sum(y => y.QTY_ITEM.HasValue ? y.QTY_ITEM.Value : 0)

                    }).ToList();

                dto.QtyCalculated =
                    calculatedGr.Where(x => x.PoLine == dto.PoLine && x.PoNumber == dto.PoNumber)
                        .Sum(x => x.QtyCalculated);

                if (dto.StartContract != null && dto.EndContract != null)
                {
                    var contractQty = ((decimal)(dto.EndContract.Value - dto.StartContract.Value).TotalDays + 1 + (decimal)GetCalday(dto.StartContract.Value, dto.EndContract.Value))/(decimal)30.00;
                    if (dto.EndContract.Value.Year == dto.StartContract.Value.Year && dto.EndContract.Value.Month == dto.StartContract.Value.Month) contractQty = 1;

                    dto.QtyRemaining = contractQty - dto.QtyCalculated - dto.QtyAutoGr;
                }

                //var terminatedData = dataFleetTerminated.Where(x => x.PO_LINE == dto.PoLine && x.PO_NUMBER == dto.PoNumber
                //                               && x.VEHICLE_USAGE != "CFM IDLE").FirstOrDefault();

                //if (terminatedData != null)
                //{
                //    dto.TerminationDate = terminatedData.END_DATE;
                //}
            }
            groupData.OrderByDescending(x => x.GrDate);
            return groupData;
        }
        public int GetCalday(DateTime StartContract, DateTime EndContract)
        {
            var End_Contract = new DateTime(EndContract.Year, EndContract.Month,15);
            DateTime Start_Contract = new DateTime(StartContract.Year, StartContract.Month, 15);
            int Result =0;
            while (Start_Contract < End_Contract)
            {
                if (Start_Contract.Month == 1 || Start_Contract.Month == 3 || Start_Contract.Month == 5 || Start_Contract.Month == 7 || Start_Contract.Month == 8 || Start_Contract.Month == 10 || Start_Contract.Month == 12)
                {
                    Result = Result - 1;
                }
		        else if (Start_Contract.Month == 2)
                {
                    if(Start_Contract.Year % 4 == 0)
                    {
                        Result = Result + 1;
                    }
                    else
                    {
                        Result = Result + 2;
                    }
                }
                Start_Contract = Start_Contract.AddMonths(1);
            }
            return Result;
        }

    }
}
