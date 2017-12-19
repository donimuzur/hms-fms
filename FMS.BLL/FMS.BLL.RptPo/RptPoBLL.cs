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

            var dataPo = _RptPoService.GetRptPoData().ToList();
            
            foreach (var item in retData)
            {
                var retDataPo = dataPo.Where(x => x.ID == item.ID).FirstOrDefault();
                var retDataFix = retData.Where(x => x.PoliceNumber == item.PoliceNumber).ToList().FirstOrDefault();
                
                if (item.ReportMonth == 1)
                {
                    retDataFix.JanAmount = (Decimal)retDataPo.MONTHLY_INSTALLMENT;
                    retDataFix.JanPPN = (Decimal)retDataPo.GST;
                    retDataFix.JanTotal = (Decimal)retDataPo.TOTAL_MONTHLY_INSTALLMENT;
                }
                if (item.ReportMonth == 2)
                {
                    retDataFix.PebAmount = (Decimal)retDataPo.MONTHLY_INSTALLMENT;
                    retDataFix.PebPPN = (Decimal)retDataPo.GST;
                    retDataFix.PebTotal = (Decimal)retDataPo.TOTAL_MONTHLY_INSTALLMENT;
                }
                if (item.ReportMonth == 3)
                {
                    retDataFix.MarAmount = (Decimal)retDataPo.MONTHLY_INSTALLMENT;
                    retDataFix.MarPPN = (Decimal)retDataPo.GST;
                    retDataFix.MarTotal = (Decimal)retDataPo.TOTAL_MONTHLY_INSTALLMENT;
                }
                if (item.ReportMonth == 4)
                {
                    retDataFix.AprAmount = (Decimal)retDataPo.MONTHLY_INSTALLMENT;
                    retDataFix.AprPPN = (Decimal)retDataPo.GST;
                    retDataFix.AprTotal = (Decimal)retDataPo.TOTAL_MONTHLY_INSTALLMENT;
                }
                if (item.ReportMonth == 5)
                {
                    retDataFix.MeiAmount = (Decimal)retDataPo.MONTHLY_INSTALLMENT;
                    retDataFix.MeiPPN = (Decimal)retDataPo.GST;
                    retDataFix.MeiTotal = (Decimal)retDataPo.TOTAL_MONTHLY_INSTALLMENT;
                }
                if (item.ReportMonth == 6)
                {
                    retDataFix.JunAmount = (Decimal)retDataPo.MONTHLY_INSTALLMENT;
                    retDataFix.JunPPN = (Decimal)retDataPo.GST;
                    retDataFix.JunTotal = (Decimal)retDataPo.TOTAL_MONTHLY_INSTALLMENT;
                }
                if (item.ReportMonth == 7)
                {
                    retDataFix.JulAmount = (Decimal)retDataPo.MONTHLY_INSTALLMENT;
                    retDataFix.JulPPN = (Decimal)retDataPo.GST;
                    retDataFix.JulTotal = (Decimal)retDataPo.TOTAL_MONTHLY_INSTALLMENT;
                }
                if (item.ReportMonth == 8)
                {
                    retDataFix.AgusAmount = (Decimal)retDataPo.MONTHLY_INSTALLMENT;
                    retDataFix.AgusPPN = (Decimal)retDataPo.GST;
                    retDataFix.AgusTotal = (Decimal)retDataPo.TOTAL_MONTHLY_INSTALLMENT;
                }
                if (item.ReportMonth == 9)
                {
                    retDataFix.SepAmount = (Decimal)retDataPo.MONTHLY_INSTALLMENT;
                    retDataFix.SepPPN = (Decimal)retDataPo.GST;
                    retDataFix.SepTotal = (Decimal)retDataPo.TOTAL_MONTHLY_INSTALLMENT;
                }
                if (item.ReportMonth == 10)
                {
                    retDataFix.OktAmount = (Decimal)retDataPo.MONTHLY_INSTALLMENT;
                    retDataFix.OktPPN = (Decimal)retDataPo.GST;
                    retDataFix.OktTotal = (Decimal)retDataPo.TOTAL_MONTHLY_INSTALLMENT;
                }
                if (item.ReportMonth == 11)
                {
                    retDataFix.NopAmount = (Decimal)retDataPo.MONTHLY_INSTALLMENT;
                    retDataFix.NopPPN = (Decimal)retDataPo.GST;
                    retDataFix.NopTotal = (Decimal)retDataPo.TOTAL_MONTHLY_INSTALLMENT;
                }
                if (item.ReportMonth == 12)
                {
                    retDataFix.DesAmount = (Decimal)retDataPo.MONTHLY_INSTALLMENT;
                    retDataFix.DesPPN = (Decimal)retDataPo.GST;
                    retDataFix.DesTotal = (Decimal)retDataPo.TOTAL_MONTHLY_INSTALLMENT;
                }

                item.OktAmount = retDataFix.OktAmount;
                item.OktPPN = retDataFix.OktPPN;
                item.OktTotal = retDataFix.OktTotal;

                item.NopAmount = retDataFix.NopAmount;
                item.NopPPN = retDataFix.NopPPN;
                item.NopTotal = retDataFix.NopTotal;

                item.DesAmount = retDataFix.DesAmount;
                item.DesPPN = retDataFix.DesPPN;
                item.DesTotal = retDataFix.DesTotal;
            }
            return retData;
        }

        public List<RptPODto> GetRptPoData()
        {
            var data = _RptPoService.GetRptPoData();
            var redata = Mapper.Map<List<RptPODto>>(data);
            return redata;
        }
    }
}
