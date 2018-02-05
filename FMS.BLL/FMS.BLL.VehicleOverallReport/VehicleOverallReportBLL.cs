using AutoMapper;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;
using FMS.Contract;
using FMS.Contract.BLL;
using FMS.Contract.Service;
using FMS.DAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BLL.VehicleOverallReport
{
    public class VehicleOverallReportBLL : IVehicleOverallReportBLL
    {
        private IVehicleOverallReportService _vehicleOverallReportService;
        private IUnitOfWork _uow;

        public VehicleOverallReportBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _vehicleOverallReportService = new VehicleOverallReportService(_uow);
        }

        public List<VehicleOverallReportDto> GetVehicle(VehicleOverallReportGetByParamInput filter)
        {
            var data = _vehicleOverallReportService.GetVehicle(filter);
            var redata = Mapper.Map<List<VehicleOverallReportDto>>(data);
            var grouredata = redata.GroupBy(x => new {
                x.Abs,
                x.Address,
                x.Airbag,
                x.AssetsNumber,
                x.AssignedTo,
                x.BodyType,
                x.Branding,
                x.carGrouplevel,
                x.CertificateOfOwnership,
                x.ChasisNumber,
                x.City,
                x.Colour,
                x.Comments,
                x.CostCenter,
                x.CreatedDate,
                x.EmployeeGroupLevel,
                x.EmployeeId,
                x.EmployeeName,
                x.EndContract,
                x.EndDate,
                x.EngineNumber,
                x.FuelType,
                x.Function,
                x.Manufacture,
                x.Models,
                x.MonthlyInstallment,
                x.MstFleetId,
                x.PoliceNumber,
                x.PoLine,
                x.PoNumber,
                x.Project,
                x.ProjectName,
                x.Regional,
                x.ReportMonth,
                x.ReportYear,
                x.Restitution,
                x.Series,
                x.StartContract,
                x.StartDate,
                x.SupplyMethod,
                x.TerminationDate,
                x.TotalMonthlyInstallment,
                x.Transmission,
                x.Vat,
                x.VehicleStatus,
                x.VehicleType,
                x.VehicleUsage,
                x.VehicleYear,
                x.Vendor
            }).ToList().Select(grouping =>
            new VehicleOverallReportDto
            {
                Abs = grouping.FirstOrDefault().Abs,
                Address = grouping.FirstOrDefault().Address,
                Airbag = grouping.FirstOrDefault().Airbag,
                AssetsNumber = grouping.FirstOrDefault().AssetsNumber,
                AssignedTo = grouping.FirstOrDefault().AssignedTo,
                BodyType = grouping.FirstOrDefault().BodyType,
                Branding = grouping.FirstOrDefault().Branding,
                carGrouplevel = grouping.FirstOrDefault().carGrouplevel,
                CertificateOfOwnership = grouping.FirstOrDefault().CertificateOfOwnership,
                ChasisNumber = grouping.FirstOrDefault().ChasisNumber,
                City = grouping.FirstOrDefault().City,
                Colour = grouping.FirstOrDefault().Colour,
                Comments = grouping.FirstOrDefault().Comments,
                CostCenter = grouping.FirstOrDefault().CostCenter,
                CreatedDate = grouping.FirstOrDefault().CreatedDate,
                EmployeeGroupLevel = grouping.FirstOrDefault().EmployeeGroupLevel,
                EmployeeId = grouping.FirstOrDefault().EmployeeId,
                EmployeeName = grouping.FirstOrDefault().EmployeeName,
                EndContract = grouping.FirstOrDefault().EndContract,
                EndDate = grouping.FirstOrDefault().EndDate,
                EngineNumber = grouping.FirstOrDefault().EngineNumber,
                FuelType = grouping.FirstOrDefault().FuelType,
                Function = grouping.FirstOrDefault().Function,
                Manufacture = grouping.FirstOrDefault().Manufacture,
                Models = grouping.FirstOrDefault().Models,
                MonthlyInstallment = grouping.FirstOrDefault().MonthlyInstallment,
                MstFleetId = grouping.FirstOrDefault().MstFleetId,
                Regional = grouping.FirstOrDefault().Regional,
                PoliceNumber = grouping.FirstOrDefault().PoliceNumber,
                PoLine = grouping.FirstOrDefault().PoLine,
                PoNumber = grouping.FirstOrDefault().PoNumber,
                Project = grouping.FirstOrDefault().Project,
                ReportMonth = grouping.FirstOrDefault().ReportMonth,
                ProjectName = grouping.FirstOrDefault().ProjectName,
                ReportYear = grouping.FirstOrDefault().ReportYear,
                Restitution = grouping.FirstOrDefault().Restitution,
                Series = grouping.FirstOrDefault().Series,
                StartContract = grouping.FirstOrDefault().StartContract,
                StartDate = grouping.FirstOrDefault().StartDate,
                SupplyMethod = grouping.FirstOrDefault().SupplyMethod,
                TerminationDate = grouping.FirstOrDefault().EndDate,
                 Transmission = grouping.FirstOrDefault().Transmission,
                 Vendor = grouping.FirstOrDefault().Vendor,
                 VehicleYear = grouping.FirstOrDefault().VehicleYear,
                 VehicleUsage = grouping.FirstOrDefault().VehicleUsage,
                 VehicleType = grouping.FirstOrDefault().VehicleType,
                 VehicleStatus = grouping.FirstOrDefault().VehicleStatus,
                 Vat = grouping.FirstOrDefault().Vat,
                 TotalMonthlyInstallment = grouping.FirstOrDefault().TotalMonthlyInstallment
             }).ToList(); 

            return grouredata;
        }
    }
}
