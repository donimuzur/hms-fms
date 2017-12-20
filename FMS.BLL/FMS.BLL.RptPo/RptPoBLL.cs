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
using FMS.BusinessObject.Inputs;


namespace FMS.BLL.RptPo
{
    public class RptPoBLL : IRptPoBLL
    {
        private IRptPoService _RptPoService;
        private IUnitOfWork _uow;
        private ILocationMappingService _locationMappingService;

        public RptPoBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _RptPoService = new RptPoService(uow);
            _locationMappingService = new LocationMappingService(_uow);
        }

        public List<RptPODto> GetRptPo(RptPoByParamInput filter)
        {
            var data = _RptPoService.GetRptPo(filter);
            var retData = Mapper.Map<List<RptPODto>>(data);
            var dataListPo = new List<RptPODto>();

            var dataPo = _RptPoService.GetRptPoData().ToList();

            var groupList = retData
                .GroupBy(x => new {x.PoliceNumber, x.SupplyMethod, x.EmployeeName, x.CostCenter, x.Manufacturer, x.Models, x.Series,
                                    x.BodyType, x.Color, x.ChasisNumber , x.EngineNumber , x.VehicleType , x.VehicleUsage ,x.PoNumber ,
                                    x.PoLine , x.Vendor, x.StartContract, x.EndContract
                })
                .Select(p => new RptPODto()
                {
                    PoliceNumber = p.FirstOrDefault().PoliceNumber,
                    SupplyMethod = p.FirstOrDefault().SupplyMethod,
                    EmployeeName = p.FirstOrDefault().EmployeeName,
                    CostCenter = p.FirstOrDefault().CostCenter,
                    Manufacturer = p.FirstOrDefault().Manufacturer,
                    Models = p.FirstOrDefault().Models,
                    Series = p.FirstOrDefault().Series,
                    BodyType = p.FirstOrDefault().BodyType,
                    Color = p.FirstOrDefault().Color,
                    ChasisNumber = p.FirstOrDefault().ChasisNumber,
                    EngineNumber = p.LastOrDefault().EngineNumber,
                    VehicleType = p.FirstOrDefault().VehicleType,
                    VehicleUsage = p.FirstOrDefault().VehicleUsage,
                    PoNumber = p.FirstOrDefault().PoNumber,
                    PoLine = p.FirstOrDefault().PoLine,
                    Vendor = p.FirstOrDefault().Vendor,
                    StartContract = p.FirstOrDefault().StartContract,
                    EndContract = p.FirstOrDefault().EndContract
                });

            foreach (var item in groupList)
            {
                var retDataPo1 = dataPo.Where(x => x.POLICE_NUMBER == item.PoliceNumber && x.REPORT_MONTH == 1).FirstOrDefault();
                if (retDataPo1 != null)
                {
                    item.JanAmount = (Decimal)retDataPo1.MONTHLY_INSTALLMENT;
                    item.JanPPN = (Decimal)retDataPo1.GST;
                    item.JanTotal = (Decimal)retDataPo1.TOTAL_MONTHLY_INSTALLMENT;
                }

                var retDataPo2 = dataPo.Where(x => x.POLICE_NUMBER == item.PoliceNumber && x.REPORT_MONTH == 2).FirstOrDefault();
                if (retDataPo2 != null)
                {
                    item.PebAmount = (Decimal)retDataPo2.MONTHLY_INSTALLMENT;
                    item.PebPPN = (Decimal)retDataPo2.GST;
                    item.PebTotal = (Decimal)retDataPo2.TOTAL_MONTHLY_INSTALLMENT;
                }

                var retDataPo3 = dataPo.Where(x => x.POLICE_NUMBER == item.PoliceNumber && x.REPORT_MONTH == 3).FirstOrDefault();
                if (retDataPo3 != null)
                {
                    item.MarAmount = (Decimal)retDataPo3.MONTHLY_INSTALLMENT;
                    item.MarPPN = (Decimal)retDataPo3.GST;
                    item.MarTotal = (Decimal)retDataPo3.TOTAL_MONTHLY_INSTALLMENT;
                }

                var retDataPo4 = dataPo.Where(x => x.POLICE_NUMBER == item.PoliceNumber && x.REPORT_MONTH == 4).FirstOrDefault();
                if (retDataPo4 != null)
                {
                    item.AprAmount = (Decimal)retDataPo4.MONTHLY_INSTALLMENT;
                    item.AprPPN = (Decimal)retDataPo4.GST;
                    item.AprTotal = (Decimal)retDataPo4.TOTAL_MONTHLY_INSTALLMENT;
                }

                var retDataPo5 = dataPo.Where(x => x.POLICE_NUMBER == item.PoliceNumber && x.REPORT_MONTH == 5).FirstOrDefault();
                if (retDataPo5 != null)
                {
                    item.MeiAmount = (Decimal)retDataPo5.MONTHLY_INSTALLMENT;
                    item.MeiPPN = (Decimal)retDataPo5.GST;
                    item.MeiTotal = (Decimal)retDataPo5.TOTAL_MONTHLY_INSTALLMENT;
                }

                var retDataPo6 = dataPo.Where(x => x.POLICE_NUMBER == item.PoliceNumber && x.REPORT_MONTH == 6).FirstOrDefault();
                if (retDataPo6 != null)
                {
                    item.JunAmount = (Decimal)retDataPo6.MONTHLY_INSTALLMENT;
                    item.JunPPN = (Decimal)retDataPo6.GST;
                    item.JunTotal = (Decimal)retDataPo6.TOTAL_MONTHLY_INSTALLMENT;
                }

                var retDataPo7 = dataPo.Where(x => x.POLICE_NUMBER == item.PoliceNumber && x.REPORT_MONTH == 7).FirstOrDefault();
                if (retDataPo7 != null)
                {
                    item.JulAmount = (Decimal)retDataPo7.MONTHLY_INSTALLMENT;
                    item.JulPPN = (Decimal)retDataPo7.GST;
                    item.JulTotal = (Decimal)retDataPo7.TOTAL_MONTHLY_INSTALLMENT;
                }

                var retDataPo8 = dataPo.Where(x => x.POLICE_NUMBER == item.PoliceNumber && x.REPORT_MONTH == 8).FirstOrDefault();
                if (retDataPo8 != null)
                {
                    item.AgusAmount = (Decimal)retDataPo8.MONTHLY_INSTALLMENT;
                    item.AgusPPN = (Decimal)retDataPo8.GST;
                    item.AgusTotal = (Decimal)retDataPo8.TOTAL_MONTHLY_INSTALLMENT;
                }

                var retDataPo9 = dataPo.Where(x => x.POLICE_NUMBER == item.PoliceNumber && x.REPORT_MONTH == 9).FirstOrDefault();
                if (retDataPo9 != null)
                {
                    item.SepAmount = (Decimal)retDataPo9.MONTHLY_INSTALLMENT;
                    item.SepPPN = (Decimal)retDataPo9.GST;
                    item.SepTotal = (Decimal)retDataPo9.TOTAL_MONTHLY_INSTALLMENT;
                }

                var retDataPo10 = dataPo.Where(x => x.POLICE_NUMBER == item.PoliceNumber && x.REPORT_MONTH == 10).FirstOrDefault();
                if (retDataPo10 != null)
                {
                    item.OktAmount = (Decimal)retDataPo10.MONTHLY_INSTALLMENT;
                    item.OktPPN = (Decimal)retDataPo10.GST;
                    item.OktTotal = (Decimal)retDataPo10.TOTAL_MONTHLY_INSTALLMENT;
                }

                var retDataPo11 = dataPo.Where(x => x.POLICE_NUMBER == item.PoliceNumber && x.REPORT_MONTH == 11).FirstOrDefault();
                if (retDataPo11 != null)
                {
                    item.NopAmount = (Decimal)retDataPo11.MONTHLY_INSTALLMENT;
                    item.NopPPN = (Decimal)retDataPo11.GST;
                    item.NopTotal = (Decimal)retDataPo11.TOTAL_MONTHLY_INSTALLMENT;
                }

                var retDataPo12 = dataPo.Where(x => x.POLICE_NUMBER == item.PoliceNumber && x.REPORT_MONTH == 12).FirstOrDefault();
                if (retDataPo12 != null)
                {
                    item.DesAmount = (Decimal)retDataPo12.MONTHLY_INSTALLMENT;
                    item.DesPPN = (Decimal)retDataPo12.GST;
                    item.DesTotal = (Decimal)retDataPo12.TOTAL_MONTHLY_INSTALLMENT;
                }

                dataListPo.Add(item);
            }
            return dataListPo;
        }

        public List<RptPODto> GetRptPoData()
        {
            var data = _RptPoService.GetRptPoData();
            var redata = Mapper.Map<List<RptPODto>>(data);
            return redata;
        }
    }
}
