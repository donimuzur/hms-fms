using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject;
using FMS.Contract;
using FMS.Contract.Service;
using NLog;
using FMS.BusinessObject.Inputs;
using System.Linq.Expressions;
using FMS.Utils;


namespace FMS.DAL.Services
{
    public class RptPoService : IRptPoService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<PO_REPORT_DATA> _rptPoRepository;

        public RptPoService(IUnitOfWork uow)
        {
            _uow = uow;

            _rptPoRepository = _uow.GetGenericRepository<PO_REPORT_DATA>();
        }

        public List<PO_REPORT_DATA> GetRptPo(RptPoByParamInput filter)
        {
            Expression<Func<PO_REPORT_DATA, bool>> queryFilter = PredicateHelper.True<PO_REPORT_DATA>();

            if (filter != null)
            {
                //if (filter.PeriodFrom != null)
                //{
                //    //queryFilter = queryFilter.And(c => (c.CREATED_DATE.Day >= filter.PeriodFrom.Day) &&
                //    //                                    (c.CREATED_DATE.Month >= filter.PeriodFrom.Month) &&
                //    //                                    (c.CREATED_DATE.Year >= filter.PeriodFrom.Year));
                //    queryFilter = queryFilter.And(c => (c.START_CONTRACT.Value.Day >= filter.PeriodFrom.Day) &&
                //                                       (c.START_CONTRACT.Value.Month >= filter.PeriodFrom.Month) &&
                //                                       (c.START_CONTRACT.Value.Year >= filter.PeriodFrom.Year));
                //}
                //if (filter.PeriodTo != null)
                //{
                //    //queryFilter = queryFilter.And(c => (c.CREATED_DATE.Day <= filter.PeriodTo.Day) &&
                //    //                                    (c.CREATED_DATE.Month <= filter.PeriodTo.Month) &&
                //    //                                    (c.CREATED_DATE.Year <= filter.PeriodTo.Year));
                //    queryFilter = queryFilter.And(c => (c.END_CONTRACT.Value.Day <= filter.PeriodTo.Day) &&
                //                                        (c.END_CONTRACT.Value.Month <= filter.PeriodTo.Month) &&
                //                                        (c.END_CONTRACT.Value.Year <= filter.PeriodTo.Year));
                //}
                if (filter.MonthFrom > 0)
                {
                    queryFilter = queryFilter.And(c => c.REPORT_MONTH >= filter.MonthFrom);
                }
                if (filter.YearFrom > 0)
                {
                    queryFilter = queryFilter.And(c => c.REPORT_YEAR >= filter.YearFrom);
                }
                if (filter.MonthTo > 0)
                {
                    queryFilter = queryFilter.And(c => c.REPORT_MONTH <= filter.MonthTo);
                }
                if (filter.YearTo > 0)
                {
                    queryFilter = queryFilter.And(c => c.REPORT_YEAR <= filter.YearTo);
                }
                if (!string.IsNullOrEmpty(filter.EmployeeName))
                {
                    queryFilter = queryFilter.And(c => c.EMPLOYEE_NAME.ToUpper() == filter.EmployeeName.ToUpper());
                }
                if (!string.IsNullOrEmpty(filter.CostCenter))
                {
                    queryFilter = queryFilter.And(c => c.COST_CENTER.ToUpper() == filter.CostCenter.ToUpper());
                }
                if (!string.IsNullOrEmpty(filter.SupplyMethod))
                {
                    queryFilter = queryFilter.And(c => c.SUPPLY_METHOD.ToUpper() == filter.SupplyMethod.ToUpper());
                }
                if (!string.IsNullOrEmpty(filter.PoliceNumber))
                {
                    queryFilter = queryFilter.And(c => c.POLICE_NUMBER.ToUpper() == filter.PoliceNumber.ToUpper());
                }
                if (filter.GroupLevel != null)
                {
                    if (filter.GroupLevel.Value > 0)
                    {
                        queryFilter = queryFilter.And(c => c.MST_EMPLOYEE.GROUP_LEVEL == filter.GroupLevel);
                    }
                }
            }

            return _rptPoRepository.Get(queryFilter, null, "").ToList();
        }

        public List<PO_REPORT_DATA> GetRptPoData()
        {
            return _rptPoRepository.Get().ToList();
        }
    }
}
