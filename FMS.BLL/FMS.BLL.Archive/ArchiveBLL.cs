using FMS.BusinessObject;
using FMS.BusinessObject.Inputs;
using FMS.Contract.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FMS.DAL.Services;
using FMS.Contract;
using FMS.BusinessObject.Business;
using FMS.Contract.BLL;
using FMS.Utils;
using FMS.Core;

namespace FMS.BLL.Archive
{
    public class ArchiveBLL: IArchiveBLL
    {
        private ICostObService _costObService;
        private ISettingService _settingService;
        private IEmployeeService _employeeService;
        private IFleetService _fleetService;
        private IEpafService _epafService;
        private IFuelOdometerService _fuelOdometerService;
        private IGroupCostCenterService _functionGroupService;
        private IGsService _gsService;
        private IHolidayCalenderService _holidayCalendarService;
        private ILocationMappingService _locationMappingService;
        private IPenaltyService _penaltyService;
        private IPriceListService _pricelistService;
        private ISalesVolumeService _salesVolumeService;
        private IVehicleSpectService _vehicleSpectService;
        private IMessageService _messageService;
        private ICcfService _ccfService;
        private ICAFService _cafService;
        private ICarComplaintFormService _carComplaintService;
        private ICRFService _crfService;
        private ICsfService _csfService;
        private ICtfService _ctfService;
        private ICtfExtendService _ctfExtendService;
        private ITemporaryService _temporaryService;

        private IArchCostObService _archCostObService;
        private IArchEmployeeService _archEmployeeService;
        private IArchFleetService _archFleetService;
        private IArchEpafService _archEpafService;
        private IArchFuelOdometerService _archFuelOdometerService;
        private IArchFunctionGroupService _archFunctionGroupService;
        private IArchGsService _archGsService;
        private IArchHolidayCalendarService _archHolidayCalendarService;
        private IArchLocationMappingService _archLocationMapingService;
        private IArchPenaltyService _archPenaltyService;
        private IArchPricelistService _archPricelistService;
        private IArchSalesVolumeService _archSalesVolumeService;
        private IArchVehicleSpectService _archVehicleSpectService;
        private IArchTraCafProgressService _archTraCafProgressService;
        private IArchTraCafService _archTraCafService;
        private IArchTraCcfDetailService _archTraCcfDetailService;
        private IArchTraCcfService _archTraCcfService;
        private IArchTraCrfService _archTraCrfService;
        private IArchTraCsfService _archTraCsfService;
        private IArchTraCtfExtendService _archCtfExtendService;
        private IArchTraCtfService _archTraCtfService;
        private IArchTraTemporaryService _archTraTemporaryService;

        private IUnitOfWork _uow;
        public ArchiveBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _settingService = new SettingService(_uow);
            _employeeService = new EmployeeService(_uow);
            _costObService = new CostObService(_uow);
            _fleetService = new FleetService(_uow);
            _epafService = new EpafService(_uow);
            _fuelOdometerService = new FuelOdometerService(_uow);
            _functionGroupService = new GroupCostCenterService(_uow);
            _gsService = new GsService(_uow);
            _holidayCalendarService = new HolidayCalenderService(_uow);
            _locationMappingService = new LocationMappingService(_uow);
            _pricelistService = new PriceListService(_uow);
            _salesVolumeService = new SalesVolumeService(_uow);
            _vehicleSpectService = new VehicleSpectService(_uow);
            _penaltyService = new PenaltyService(_uow);
            _messageService = new MessageService(_uow);
            _ccfService = new CcfService(_uow);
            
            _cafService = new CafService(_uow);
            _crfService = new CrfService(_uow);
            _csfService = new CsfService(_uow);
            _ctfService = new CtfService(_uow);
            _ctfExtendService = new CtfExtendService(_uow);
            _temporaryService = new TemporaryService(_uow);

            _archCostObService = new ArchCostObService(_uow);
            _archEmployeeService = new ArchEmployeeService(_uow);
            _archFleetService = new ArchFleetService(_uow);
            _archEpafService = new ArchEpafService(_uow);
            _archFuelOdometerService = new ArchFuelOdometerService(_uow);
            _archFunctionGroupService = new ArchFunctionGroupService(_uow);
            _archGsService = new ArchGsService(_uow);
            _archHolidayCalendarService = new ArchHolidayCalendarService(_uow);
            _archLocationMapingService = new ArchLocationMappingService(_uow);
            _archPricelistService = new ArchPricelistService(_uow);
            _archSalesVolumeService = new ArchSalesVolumeService(_uow);
            _archVehicleSpectService = new ArchVehicleSpectService(_uow);
            _archPenaltyService = new ArchPenaltyService(_uow);
            _archTraCafProgressService = new ArchTraCafProgressService(_uow);
            _archTraCafService = new ArchTraCafService(_uow);
            _archTraCcfDetailService = new ArchTraCcfDetailService(_uow);
            _archTraCcfService = new ArchTraCcfService(_uow);
            _archTraCrfService = new ArchTraCrfService(_uow);
            _archTraCsfService = new ArchTraCsfService(_uow);
            _archCtfExtendService = new ArchTraCtfExtendService(_uow);
            _archTraCtfService = new ArchTraCtfService(_uow);
            _archTraTemporaryService = new ArchTraTemporaryService(_uow);
            
        }
        public bool DoArchive (ArchiveParamInput input, Login Login)
        {
            var result = true;
            var TableList =new List<string>();
            var GetSetting = _settingService.GetSetting().Where(x => x.IS_ACTIVE).ToList();
            try
            {
                var ListTableId = input.TableId.Split(',').ToList();
                foreach(var item in ListTableId)
                {
                    long tableId = 0;
                    try
                    {
                        tableId = Convert.ToInt64(item);
                    }
                    catch (Exception)
                    {
                             
                    }
                    
                    ///---------- Master ----------///
                    #region --- Cost OB ---
                    if (tableId == 1)
                    {
                        var CostObList = new List<MST_COST_OB>();
                        if (input.ModifiedDate.HasValue && input.CreatedDate.HasValue)
                        {
                            if (input.Operator == "OR")
                                CostObList = _costObService.GetCostOb().Where(x => x.CREATED_DATE <= input.CreatedDate || x.MODIFIED_DATE <= input.ModifiedDate).ToList();
                            else if (input.Operator == "AND")
                                CostObList = _costObService.GetCostOb().Where(x => x.CREATED_DATE <= input.CreatedDate && x.MODIFIED_DATE <= input.ModifiedDate).ToList();
                        }
                        else
                        {
                            CostObList = _costObService.GetCostOb().Where(x => x.CREATED_DATE <= input.CreatedDate).ToList();
                        }

                        
                        foreach (var itemCostOb in CostObList)
                        {
                            var ArchCostOb = Mapper.Map<ARCH_MST_COST_OB>(itemCostOb);
                            ArchCostOb.ARCHIVED_BY = Login.USER_ID;
                            ArchCostOb.ARCHIVED_DATE = DateTime.Now;
                            _archCostObService.Save(ArchCostOb, Login);
                            _costObService.Delete(itemCostOb);
                        }
                        _uow.SaveChanges();
                        TableList.Add("Master Cost OB");
                    }
                    #endregion
                    #region --- Employee ---
                    else if(tableId == 2)
                    {
                        var EmployeeList = new List<MST_EMPLOYEE>();
                        if (input.ModifiedDate.HasValue && input.CreatedDate.HasValue)
                        {
                            if (input.Operator == "OR")
                                EmployeeList = _employeeService.GetEmployee().Where(x => (x.CREATED_DATE <= input.CreatedDate || x.MODIFIED_DATE<= input.ModifiedDate) && x.IS_ACTIVE == false).ToList();
                            else if (input.Operator == "AND")
                                EmployeeList = _employeeService.GetEmployee().Where(x => x.CREATED_DATE <= input.CreatedDate && x.MODIFIED_DATE <= input.ModifiedDate && x.IS_ACTIVE == false).ToList();
                        }
                        else
                        {
                            EmployeeList = _employeeService.GetEmployee().Where(x => x.CREATED_DATE <= input.CreatedDate && x.IS_ACTIVE == false).ToList();
                        }
                        foreach (var itemEmployee in EmployeeList)
                        {
                            var ArchEmployee = Mapper.Map<ARCH_MST_EMPLOYEE>(itemEmployee);
                            ArchEmployee.ARCHIVED_BY = Login.USER_ID;
                            ArchEmployee.ARCHIVED_DATE = DateTime.Now;
                            _archEmployeeService.Save(ArchEmployee, Login);
                            itemEmployee.MODIFIED_BY = "ARCHIVED";
                            _employeeService.save(itemEmployee);
                        }
                        _uow.SaveChanges();
                        TableList.Add("Master Employee");
                    }
                    #endregion
                    #region --- Epaf ---
                    else if (tableId == 3)
                    {
                        var EpafList = new List<MST_EPAF>();
                        if (input.ModifiedDate.HasValue && input.CreatedDate.HasValue)
                        {
                            if (input.Operator == "OR")
                                EpafList = _epafService.GetEpaf().Where(x => x.CREATED_DATE <= input.CreatedDate || x.MODIFIED_DATE <= input.ModifiedDate).ToList();
                            else if (input.Operator == "AND")
                                EpafList = _epafService.GetEpaf().Where(x => x.CREATED_DATE <= input.CreatedDate && x.MODIFIED_DATE <= input.ModifiedDate).ToList();
                        }
                        else
                        {
                            EpafList = _epafService.GetEpaf().Where(x => x.CREATED_DATE <= input.CreatedDate).ToList();
                        }
                        foreach (var itemEpaf in EpafList)
                        {
                            var ArchEpaf = Mapper.Map<ARCH_MST_EPAF>(itemEpaf);
                            ArchEpaf.ARCHIVED_BY = Login.USER_ID;
                            ArchEpaf.ARCHIVED_DATE = DateTime.Now;
                            _archEpafService.Save(ArchEpaf, Login);
                            itemEpaf.MODIFIED_BY = "ARCHIVED";
                            itemEpaf.IS_ACTIVE = false;
                            _uow.GetGenericRepository<MST_EPAF>().InsertOrUpdate(itemEpaf);
                        }
                        _uow.SaveChanges();
                        TableList.Add("Master Epaf");
                    }
                    #endregion
                    #region --- Fleet ---
                    else if (tableId == 4)
                    {
                        var FleetList = new List<MST_FLEET>();
                        if (input.ModifiedDate.HasValue && input.CreatedDate.HasValue)
                        {
                            if (input.Operator == "OR")
                                FleetList = _fleetService.GetFleet().Where(x => x.CREATED_DATE <= input.CreatedDate || x.MODIFIED_DATE <= input.ModifiedDate).ToList();
                            else if (input.Operator == "AND")
                                FleetList = _fleetService.GetFleet().Where(x => x.CREATED_DATE <= input.CreatedDate && x.MODIFIED_DATE <= input.ModifiedDate).ToList();
                        }
                        else
                        {
                            FleetList = _fleetService.GetFleet().Where(x => x.CREATED_DATE <= input.CreatedDate).ToList();
                        }


                        foreach (var itemFleet in FleetList)
                        {
                            var ArchFleet = Mapper.Map<ARCH_MST_FLEET>(itemFleet);
                            ArchFleet.ARCHIVED_BY = Login.USER_ID;
                            ArchFleet.ARCHIVED_DATE = DateTime.Now;
                            _archFleetService.Save(ArchFleet, Login);
                            _uow.GetGenericRepository<MST_FLEET>().Delete(itemFleet.MST_FLEET_ID);
                        }
                        _uow.SaveChanges();
                        TableList.Add("Master Fleet");
                    }
                    #endregion
                    #region --- Fuel Odometer ---
                    else if (tableId == 5)
                    {
                        var FuelOdometerList = new List<MST_FUEL_ODOMETER>();
                        if (input.ModifiedDate.HasValue && input.CreatedDate.HasValue)
                        {
                            if (input.Operator == "OR")
                                FuelOdometerList = _fuelOdometerService.GetFuelOdometer().Where(x => x.CREATED_DATE <= input.CreatedDate || x.MODIFIED_DATE <= input.ModifiedDate).ToList();
                            else if (input.Operator == "AND")
                                FuelOdometerList = _fuelOdometerService.GetFuelOdometer().Where(x => x.CREATED_DATE <= input.CreatedDate && x.MODIFIED_DATE <= input.ModifiedDate).ToList();
                        }
                        else
                        {
                            FuelOdometerList = _fuelOdometerService.GetFuelOdometer().Where(x => x.CREATED_DATE <= input.CreatedDate).ToList();
                        }


                        foreach (var itemFuelOdometer in FuelOdometerList)
                        {
                            var ArchFuelOdometer= Mapper.Map<ARCH_MST_FUEL_ODOMETER>(itemFuelOdometer);
                            ArchFuelOdometer.ARCHIVED_BY = Login.USER_ID;
                            ArchFuelOdometer.ARCHIVED_DATE = DateTime.Now;
                            _archFuelOdometerService.Save(ArchFuelOdometer, Login);
                            _uow.GetGenericRepository<MST_FUEL_ODOMETER>().Delete(itemFuelOdometer.MST_FUEL_ODOMETER_ID);
                        }
                        _uow.SaveChanges();
                        TableList.Add("Master Fuel Odometer");
                    }
                    #endregion
                    #region --- Function Group ---
                    else if (tableId == 6)
                    {
                        var FunctionGroupList = new List<MST_FUNCTION_GROUP>();
                        if (input.ModifiedDate.HasValue && input.CreatedDate.HasValue)
                        {
                            if (input.Operator == "OR")
                                FunctionGroupList = _functionGroupService.GetGroupCostCenter().Where(x => x.CREATED_DATE <= input.CreatedDate || x.MODIFIED_DATE <= input.ModifiedDate).ToList();
                            else if (input.Operator == "AND")
                                FunctionGroupList = _functionGroupService.GetGroupCostCenter().Where(x => x.CREATED_DATE <= input.CreatedDate && x.MODIFIED_DATE <= input.ModifiedDate).ToList();
                        }
                        else
                        {
                            FunctionGroupList = _functionGroupService.GetGroupCostCenter().Where(x => x.CREATED_DATE <= input.CreatedDate).ToList();
                        }


                        foreach (var itemFunctionGroup in FunctionGroupList)
                        {
                            var ArchFunctionGroup = Mapper.Map<ARCH_MST_FUNCTION_GROUP>(itemFunctionGroup);
                            ArchFunctionGroup.ARCHIVED_BY = Login.USER_ID;
                            ArchFunctionGroup.ARCHIVED_DATE = DateTime.Now;
                            _archFunctionGroupService.Save(ArchFunctionGroup, Login);
                            _uow.GetGenericRepository<MST_FUNCTION_GROUP>().Delete(itemFunctionGroup.MST_FUNCTION_GROUP_ID);
                        }
                        _uow.SaveChanges();
                        TableList.Add("Master Function Group");
                    }
                    #endregion
                    #region --- GS ---
                    else if (tableId == 7)
                    {
                        var GsList = new List<MST_GS>();
                        if (input.ModifiedDate.HasValue && input.CreatedDate.HasValue)
                        {
                            if (input.Operator == "OR")
                                GsList = _gsService.GetGs().Where(x => x.CREATED_DATE <= input.CreatedDate || x.MODIFIED_DATE <= input.ModifiedDate).ToList();
                            else if (input.Operator == "AND")
                                GsList = _gsService.GetGs().Where(x => x.CREATED_DATE <= input.CreatedDate && x.MODIFIED_DATE <= input.ModifiedDate).ToList();
                        }
                        else
                        {
                            GsList = _gsService.GetGs().Where(x => x.CREATED_DATE <= input.CreatedDate).ToList();
                        }


                        foreach (var itemGs in GsList)
                        {
                            var ArchGs = Mapper.Map<ARCH_MST_GS>(itemGs);
                            ArchGs.ARCHIVED_BY = Login.USER_ID;
                            ArchGs.ARCHIVED_DATE = DateTime.Now;
                            _archGsService.Save(ArchGs, Login);
                            _uow.GetGenericRepository<MST_GS>().Delete(itemGs.MST_GS_ID);
                        }
                        _uow.SaveChanges();
                        TableList.Add("Master GS");
                    }
                    #endregion
                    #region --- Holiday Calendar ---
                    else if (tableId == 8)
                    {
                        var HolidayCalendarList = new List<MST_HOLIDAY_CALENDAR>();
                        if (input.ModifiedDate.HasValue && input.CreatedDate.HasValue)
                        {
                            if (input.Operator == "OR")
                                HolidayCalendarList = _holidayCalendarService.GetHolidayCalender().Where(x => x.CREATED_DATE <= input.CreatedDate || x.MODIFIED_DATE <= input.ModifiedDate).ToList();
                            else if (input.Operator == "AND")
                                HolidayCalendarList = _holidayCalendarService.GetHolidayCalender().Where(x => x.CREATED_DATE <= input.CreatedDate && x.MODIFIED_DATE <= input.ModifiedDate).ToList();
                        }
                        else
                        {
                            HolidayCalendarList = _holidayCalendarService.GetHolidayCalender(new HolidayCalenderParamInput()).Where(x => x.CREATED_DATE <= input.CreatedDate).ToList();
                        }


                        foreach (var itemHolidayCalendar in HolidayCalendarList)
                        {
                            var ArchHolidayCalendar = Mapper.Map<ARCH_MST_HOLIDAY_CALENDAR>(itemHolidayCalendar);
                            ArchHolidayCalendar.ARCHIVED_BY = Login.USER_ID;
                            ArchHolidayCalendar.ARCHIVED_DATE = DateTime.Now;
                            _archHolidayCalendarService.Save(ArchHolidayCalendar, Login);
                            _uow.GetGenericRepository<MST_HOLIDAY_CALENDAR>().Delete(itemHolidayCalendar.MST_HOLIDAY_ID);
                        }
                        _uow.SaveChanges();
                        TableList.Add("Master Holiday Calendar");
                    }
                    #endregion
                    #region --- Location Mapping ---
                    else if (tableId == 9)
                    {
                        var LocationMappingList = new List<MST_LOCATION_MAPPING>();
                        if (input.ModifiedDate.HasValue && input.CreatedDate.HasValue)
                        {
                            if (input.Operator == "OR")
                                LocationMappingList = _locationMappingService.GetLocationMapping().Where(x => x.CREATED_DATE <= input.CreatedDate || x.MODIFIED_DATE <= input.ModifiedDate).ToList();
                            else if (input.Operator == "AND")
                                LocationMappingList = _locationMappingService.GetLocationMapping().Where(x => x.CREATED_DATE <= input.CreatedDate && x.MODIFIED_DATE <= input.ModifiedDate).ToList();
                        }
                        else
                        {
                            LocationMappingList = _locationMappingService.GetLocationMapping().Where(x => x.CREATED_DATE <= input.CreatedDate).ToList();
                        }


                        foreach (var itemLocationMapping in LocationMappingList)
                        {
                            var ArchLocationMapping = Mapper.Map<ARCH_MST_LOCATION_MAPPING>(itemLocationMapping);
                            ArchLocationMapping.ARCHIVED_BY = Login.USER_ID;
                            ArchLocationMapping.ARCHIVED_DATE = DateTime.Now;
                            _archLocationMapingService.Save(ArchLocationMapping, Login);
                            _uow.GetGenericRepository<MST_LOCATION_MAPPING>().Delete(itemLocationMapping.MST_LOCATION_MAPPING_ID);
                        }
                        _uow.SaveChanges();
                        TableList.Add("Master Location Mapping");
                    }
                    #endregion
                    #region --- Penalty ---
                    else if (tableId == 10)
                    {
                        var PenaltyList = new List<MST_PENALTY>();
                        if (input.ModifiedDate.HasValue && input.CreatedDate.HasValue)
                        {
                            if (input.Operator == "OR")
                                PenaltyList = _penaltyService.GetPenalty().Where(x => x.CREATED_DATE <= input.CreatedDate || x.MODIFIED_DATE <= input.ModifiedDate).ToList();
                            else if (input.Operator == "AND")
                                PenaltyList = _penaltyService.GetPenalty().Where(x => x.CREATED_DATE <= input.CreatedDate && x.MODIFIED_DATE <= input.ModifiedDate).ToList();
                        }
                        else
                        {
                            PenaltyList = _penaltyService.GetPenalty().Where(x => x.CREATED_DATE <= input.CreatedDate).ToList();
                        }


                        foreach (var itemPenalty in PenaltyList)
                        {
                            var ArchPenalty = Mapper.Map<ARCH_MST_PENALTY>(itemPenalty);
                            ArchPenalty.ARCHIVED_BY = Login.USER_ID;
                            ArchPenalty.ARCHIVED_DATE = DateTime.Now;
                            _archPenaltyService.Save(ArchPenalty, Login);
                            _uow.GetGenericRepository<MST_PENALTY>().Delete(itemPenalty.MST_PENALTY_ID);
                        }
                        _uow.SaveChanges();
                        TableList.Add("Master Penalty");
                    }
                    #endregion
                    #region --- Pricelist ---
                    else if (tableId == 11)
                    {
                        var PriceList = new List<MST_PRICELIST>();
                        if (input.ModifiedDate.HasValue && input.CreatedDate.HasValue)
                        {
                            if (input.Operator == "OR")
                                PriceList = _pricelistService.GetPriceList().Where(x => x.CREATED_DATE <= input.CreatedDate || x.MODIFIED_DATE <= input.ModifiedDate).ToList();
                            else if (input.Operator == "AND")
                                PriceList = _pricelistService.GetPriceList().Where(x => x.CREATED_DATE <= input.CreatedDate && x.MODIFIED_DATE <= input.ModifiedDate).ToList();
                        }
                        else
                        {
                            PriceList = _pricelistService.GetPriceList().Where(x => x.CREATED_DATE <= input.CreatedDate).ToList();
                        }


                        foreach (var itemPricelist in PriceList)
                        {
                            var ArchPriceList = Mapper.Map<ARCH_MST_PRICELIST>(itemPricelist);
                            ArchPriceList.ARCHIVED_BY = Login.USER_ID;
                            ArchPriceList.ARCHIVED_DATE = DateTime.Now;
                            _archPricelistService.Save(ArchPriceList, Login);
                            _uow.GetGenericRepository<MST_PRICELIST>().Delete(itemPricelist.MST_PRICELIST_ID);
                        }
                        _uow.SaveChanges();
                        TableList.Add("Master Pricelist");
                    }
                    #endregion
                    #region --- Sales Volume ---
                    else if (tableId == 12)
                    {
                        var SalesVolumeList = new List<MST_SALES_VOLUME>();
                        if (input.ModifiedDate.HasValue && input.CreatedDate.HasValue)
                        {
                            if (input.Operator == "OR")
                                SalesVolumeList = _salesVolumeService.GetSalesVolume(new SalesVolumeParamInput()).Where(x => x.CREATED_DATE <= input.CreatedDate || x.MODIFIED_DATE <= input.ModifiedDate).ToList();
                            else if (input.Operator == "AND")
                                SalesVolumeList = _salesVolumeService.GetSalesVolume(new SalesVolumeParamInput()).Where(x => x.CREATED_DATE <= input.CreatedDate && x.MODIFIED_DATE <= input.ModifiedDate).ToList();
                        }
                        else
                        {
                            SalesVolumeList = _salesVolumeService.GetSalesVolume(new SalesVolumeParamInput()).Where(x => x.CREATED_DATE <= input.CreatedDate).ToList();
                        }
                        foreach (var itemSalesVolume in SalesVolumeList)
                        {
                            var ArchSalesVolume = Mapper.Map<ARCH_MST_SALES_VOLUME>(itemSalesVolume);
                            ArchSalesVolume.ARCHIVED_BY = Login.USER_ID;
                            ArchSalesVolume.ARCHIVED_DATE = DateTime.Now;
                            _archSalesVolumeService.Save(ArchSalesVolume, Login);
                            _uow.GetGenericRepository<MST_SALES_VOLUME>().Delete(itemSalesVolume.MST_SALES_VOLUME_ID);
                        }
                        _uow.SaveChanges();
                        TableList.Add("Master Sales Volume");
                    }
                    #endregion
                    #region --- Vehicle Spect ---
                    else if (tableId == 13)
                    {
                        var VehicleSpectList = new List<MST_VEHICLE_SPECT>();
                        if (input.ModifiedDate.HasValue && input.CreatedDate.HasValue)
                        {
                            if (input.Operator == "OR")
                                VehicleSpectList = _vehicleSpectService.GetVehicleSpect().Where(x => x.CREATED_DATE <= input.CreatedDate || x.MODIFIED_DATE <= input.ModifiedDate).ToList();
                            else if (input.Operator == "AND")
                                VehicleSpectList = _vehicleSpectService.GetVehicleSpect().Where(x => x.CREATED_DATE <= input.CreatedDate && x.MODIFIED_DATE <= input.ModifiedDate).ToList();
                        }
                        else
                        {
                            VehicleSpectList = _vehicleSpectService.GetVehicleSpect().Where(x => x.CREATED_DATE <= input.CreatedDate).ToList();
                        }


                        foreach (var itemVehicle in VehicleSpectList)
                        {
                            var ArchVehicleSpect= Mapper.Map<ARCH_MST_VEHICLE_SPECT>(itemVehicle);
                            ArchVehicleSpect.ARCHIVED_BY = Login.USER_ID;
                            ArchVehicleSpect.ARCHIVED_DATE = DateTime.Now;
                            _archVehicleSpectService.Save(ArchVehicleSpect, Login);
                            _uow.GetGenericRepository<MST_VEHICLE_SPECT>().Delete(itemVehicle.MST_VEHICLE_SPECT_ID);
                        }
                        _uow.SaveChanges();
                        TableList.Add("Master Vehicle Spect");
                    }
                    #endregion
                    
                    ///----------- Transaction ----------///
                    #region --- Tra CSF ---
                    else if (tableId == 14)
                    {
                        var TraCsfList = new List<TRA_CSF>();
                        if (input.ModifiedDate.HasValue && input.CreatedDate.HasValue)
                        {
                            if (input.Operator == "OR")
                                TraCsfList = _csfService.GetAllCsf().Where(x => (x.CREATED_DATE <= input.CreatedDate || x.MODIFIED_DATE <= input.ModifiedDate) && x.DOCUMENT_STATUS == Enums.DocumentStatus.Completed).ToList();
                            else if (input.Operator == "AND")
                                TraCsfList = _csfService.GetAllCsf().Where(x => x.CREATED_DATE <= input.CreatedDate && x.MODIFIED_DATE <= input.ModifiedDate && x.DOCUMENT_STATUS == Enums.DocumentStatus.Completed).ToList();
                        }
                        else
                        {
                            TraCsfList = _csfService.GetAllCsf().Where(x => x.CREATED_DATE <= input.CreatedDate && x.DOCUMENT_STATUS == Enums.DocumentStatus.Completed).ToList();
                        }

                        if(!string.IsNullOrEmpty(input.VehicleType))
                        {
                            var VehType = GetSetting.Where(x => x.SETTING_GROUP == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType) && x.SETTING_NAME.ToUpper() == (input.VehicleType == null ? "" : input.VehicleType.ToUpper()) && x.IS_ACTIVE).FirstOrDefault();
                            TraCsfList = TraCsfList.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == (VehType == null ? "" : VehType.MST_SETTING_ID.ToString())).ToList();
                        }
                        foreach (var itemCsf in TraCsfList)
                        {
                            var ArchCsf = Mapper.Map<ARCH_TRA_CSF>(itemCsf);
                            ArchCsf.ARCHIVED_BY = Login.USER_ID;
                            ArchCsf.ARCHIVED_DATE = DateTime.Now;
                            _archTraCsfService.Save(ArchCsf, Login);
                            _uow.GetGenericRepository<TRA_CSF>().Delete(itemCsf.TRA_CSF_ID);
                        }
                        _uow.SaveChanges();
                        TableList.Add("Transaction CSF");
                    }
                    #endregion
                    #region --- Tra CRF ---
                    else if (tableId == 15)
                    {
                        var TraCrfList = new List<TRA_CRF>();
                        if (input.ModifiedDate.HasValue && input.CreatedDate.HasValue)
                        {
                            if (input.Operator == "OR")
                                TraCrfList = _crfService.GetList().Where(x => (x.CREATED_DATE <= input.CreatedDate || x.MODIFIED_DATE <= input.ModifiedDate) && x.DOCUMENT_STATUS == (int)Enums.DocumentStatus.Completed).ToList();
                            else if (input.Operator == "AND")
                                TraCrfList = _crfService.GetList().Where(x => x.CREATED_DATE <= input.CreatedDate && x.MODIFIED_DATE <= input.ModifiedDate && x.DOCUMENT_STATUS == (int)Enums.DocumentStatus.Completed).ToList();
                        }
                        else
                        {
                            TraCrfList = _crfService.GetList().Where(x => x.CREATED_DATE <= input.CreatedDate && x.DOCUMENT_STATUS == (int)Enums.DocumentStatus.Completed).ToList();
                        }

                        if (!string.IsNullOrEmpty(input.VehicleType))
                        {
                            TraCrfList = TraCrfList.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == (input.VehicleType == null ? "" : input.VehicleType.ToUpper())).ToList();
                        }
                        foreach (var itemCrf in TraCrfList)
                        {
                            var ArchCrf = Mapper.Map<ARCH_TRA_CRF>(itemCrf);
                            ArchCrf.ARCHIVED_BY = Login.USER_ID;
                            ArchCrf.ARCHIVED_DATE = DateTime.Now;
                            _archTraCrfService.Save(ArchCrf, Login);
                            _uow.GetGenericRepository<TRA_CRF>().Delete(itemCrf.TRA_CRF_ID);
                        }
                        _uow.SaveChanges();
                        TableList.Add("Transaction CRF");
                    }
                    #endregion
                    #region --- Tra Ctf ---
                    else if (tableId == 16)
                    {
                        var TraCtfList = new List<TRA_CTF>();
                        if (input.ModifiedDate.HasValue && input.CreatedDate.HasValue)
                        {
                            if (input.Operator == "OR")
                                TraCtfList = _ctfService.GetCtf().Where(x =>( x.CREATED_DATE <= input.CreatedDate || x.MODIFIED_DATE <= input.ModifiedDate) && x.DOCUMENT_STATUS == Enums.DocumentStatus.Completed).ToList();
                            else if (input.Operator == "AND")
                                TraCtfList = _ctfService.GetCtf().Where(x => x.CREATED_DATE <= input.CreatedDate && x.MODIFIED_DATE <= input.ModifiedDate && x.DOCUMENT_STATUS == Enums.DocumentStatus.Completed).ToList();
                        }
                        else
                        {
                            TraCtfList = _ctfService.GetCtf().Where(x => x.CREATED_DATE <= input.CreatedDate && x.DOCUMENT_STATUS == Enums.DocumentStatus.Completed).ToList();
                        }

                        if (!string.IsNullOrEmpty(input.VehicleType))
                        {
                            TraCtfList = TraCtfList.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == (input.VehicleType == null ? "" : input.VehicleType.ToUpper())).ToList();
                        }
                        foreach (var itemCtf in TraCtfList)
                        {
                            var ArchCtf = Mapper.Map<ARCH_TRA_CTF>(itemCtf);
                            ArchCtf.ARCHIVED_BY = Login.USER_ID;
                            ArchCtf.ARCHIVED_DATE = DateTime.Now;
                            var CtfExtendList = _ctfExtendService.GetCtfExtend().Where(x => x.TRA_CTF_ID == itemCtf.TRA_CTF_ID).ToList();
                            foreach(var itemCtfExtend in CtfExtendList)
                            {
                                var ArchCtfExtend = Mapper.Map<ARCH_TRA_CTF_EXTEND>(itemCtfExtend);
                                _archCtfExtendService.Save(ArchCtfExtend, Login);
                                _uow.GetGenericRepository<TRA_CTF_EXTEND>().Delete(itemCtfExtend.TRA_CTF_EXTEND_ID);
                            }
                            _archTraCtfService.Save(ArchCtf, Login);
                            _uow.GetGenericRepository<TRA_CTF>().Delete(itemCtf.TRA_CTF_ID);
                        }
                        _uow.SaveChanges();
                        TableList.Add("Transaction CTF");
                    }
                    #endregion
                    #region --- Tra CCF ---
                    else if (tableId == 17)
                    {
                        var TraCcfList = new List<TRA_CCF>();
                        if (input.ModifiedDate.HasValue && input.CreatedDate.HasValue)
                        {
                            if (input.Operator == "OR")
                                TraCcfList = _ccfService.GetCcf().Where(x =>( x.CREATED_DATE <= input.CreatedDate || x.MODIFIED_DATE <= input.ModifiedDate) && x.DOCUMENT_STATUS == (int)Enums.DocumentStatus.Completed).ToList();
                            else if (input.Operator == "AND")
                                TraCcfList = _ccfService.GetCcf().Where(x => x.CREATED_DATE <= input.CreatedDate && x.MODIFIED_DATE <= input.ModifiedDate && x.DOCUMENT_STATUS ==(int) Enums.DocumentStatus.Completed).ToList();
                        }
                        else
                        {
                            TraCcfList = _ccfService.GetCcf().Where(x => x.CREATED_DATE <= input.CreatedDate && x.DOCUMENT_STATUS == (int)Enums.DocumentStatus.Completed).ToList();
                        }

                        if (!string.IsNullOrEmpty(input.VehicleType))
                        {
                            TraCcfList = TraCcfList.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == (input.VehicleType == null ? "" : input.VehicleType.ToUpper())).ToList();
                        }
                        foreach (var itemCcf in TraCcfList)
                        {
                            var ArchCcf = Mapper.Map<ARCH_TRA_CCF>(itemCcf);
                            ArchCcf.ARCHIVED_BY = Login.USER_ID;
                            ArchCcf.ARCHIVED_DATE = DateTime.Now;
                            var CcfDetaillist = _ccfService.GetCcfDetil().Where(x => x.TRA_CCF_ID == itemCcf.TRA_CCF_ID).ToList();
                            foreach (var itemCcfDetail in CcfDetaillist)
                            {
                                var ArchCcfDetail= Mapper.Map<ARCH_TRA_CCF_DETAIL>(itemCcfDetail);
                                _archTraCcfDetailService.Save(ArchCcfDetail, Login);
                                _uow.GetGenericRepository<TRA_CCF_DETAIL>().Delete(itemCcfDetail.TRA_CCF_DETAIL_ID);
                            }
                            _archTraCcfService.Save(ArchCcf, Login);
                            _uow.GetGenericRepository<TRA_CCF>().Delete(itemCcf.TRA_CCF_ID);
                        }
                        _uow.SaveChanges();
                        TableList.Add("Transaction CCF");
                    }
                    #endregion
                    #region --- Tra CAF ---
                    else if (tableId == 18)
                    {
                        var TraCafList = new List<TRA_CAF>();
                        if (input.ModifiedDate.HasValue && input.CreatedDate.HasValue)
                        {
                            if (input.Operator == "OR")
                                TraCafList = _cafService.GetList().Where(x => (x.CREATED_DATE <= input.CreatedDate || x.MODIFIED_DATE <= input.ModifiedDate) && x.DOCUMENT_STATUS == (int)Enums.DocumentStatus.Completed).ToList();
                            else if (input.Operator == "AND")
                                TraCafList = _cafService.GetList().Where(x => x.CREATED_DATE <= input.CreatedDate && x.MODIFIED_DATE <= input.ModifiedDate && x.DOCUMENT_STATUS == (int)Enums.DocumentStatus.Completed).ToList();
                        }
                        else
                        {
                            TraCafList = _cafService.GetList().Where(x => x.CREATED_DATE <= input.CreatedDate && x.DOCUMENT_STATUS ==(int) Enums.DocumentStatus.Completed).ToList();
                        }
                        //if (!string.IsNullOrEmpty(input.VehicleType))
                        //{
                        //    TraCafList = TraCafList.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == (input.VehicleType == null ? "" : input.VehicleType.ToUpper())).ToList();
                        //}
                        foreach (var itemCaf in TraCafList)
                        {
                            var ArchCaf = Mapper.Map<ARCH_TRA_CAF>(itemCaf);
                            ArchCaf.ARCHIVED_BY = Login.USER_ID;
                            ArchCaf.ARCHIVED_DATE = DateTime.Now;
                            var CafProgresslist = _uow.GetGenericRepository<TRA_CAF_PROGRESS>().Get(x => x.TRA_CAF_ID == itemCaf.TRA_CAF_ID).ToList();
                            foreach (var itemCafProgress in CafProgresslist)
                            {
                                var ArchCafProgress = Mapper.Map<ARCH_TRA_CAF_PROGRESS>(itemCafProgress);
                                _archTraCafProgressService.Save(ArchCafProgress, Login);
                                _uow.GetGenericRepository<TRA_CAF_PROGRESS>().Delete(itemCafProgress.TRA_CAF_PROGRESS_ID);
                            }
                            _archTraCafService.Save(ArchCaf, Login);
                            _uow.GetGenericRepository<ARCH_TRA_CAF>().Delete(itemCaf.TRA_CAF_ID);
                        }
                        _uow.SaveChanges();
                        TableList.Add("Transaction CAF");
                    }
                    #endregion
                    #region --- Tra Temporary ---
                    else if (tableId == 19)
                    {
                        var TraTemporaryList = new List<TRA_TEMPORARY>();
                        if (input.ModifiedDate.HasValue && input.CreatedDate.HasValue)
                        {
                            if (input.Operator == "OR")
                                TraTemporaryList = _temporaryService.GetAllTemp().Where(x => (x.CREATED_DATE <= input.CreatedDate || x.MODIFIED_DATE <= input.ModifiedDate) && x.DOCUMENT_STATUS == Enums.DocumentStatus.Completed).ToList();
                            else if (input.Operator == "AND")
                                TraTemporaryList = _temporaryService.GetAllTemp().Where(x => x.CREATED_DATE <= input.CreatedDate && x.MODIFIED_DATE <= input.ModifiedDate && x.DOCUMENT_STATUS == Enums.DocumentStatus.Completed).ToList();
                        }
                        else
                        {
                            TraTemporaryList = _temporaryService.GetAllTemp().Where(x => x.CREATED_DATE <= input.CreatedDate && x.DOCUMENT_STATUS == Enums.DocumentStatus.Completed).ToList();
                        }
                        if (!string.IsNullOrEmpty(input.VehicleType))
                        {
                            var VehType = GetSetting.Where(x => x.SETTING_GROUP == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType) && x.SETTING_NAME.ToUpper() == (input.VehicleType == null ? "" : input.VehicleType.ToUpper()) && x.IS_ACTIVE).FirstOrDefault();
                            TraTemporaryList = TraTemporaryList.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == (VehType == null ? "" : VehType.MST_SETTING_ID.ToString())).ToList();
                        }
                        foreach (var itemTemporary in TraTemporaryList)
                        {
                            var ArchTemporary = Mapper.Map<ARCH_TRA_TEMPORARY>(itemTemporary);
                            ArchTemporary.ARCHIVED_BY = Login.USER_ID;
                            ArchTemporary.ARCHIVED_DATE = DateTime.Now;
                            _archTraTemporaryService.Save(ArchTemporary, Login);
                            _uow.GetGenericRepository<ARCH_TRA_TEMPORARY>().Delete(itemTemporary.TRA_TEMPORARY_ID);
                        }
                        _uow.SaveChanges();
                        TableList.Add("Transaction Temporary");
                    }
                    #endregion

                }
                input.EndDate= DateTime.Now;
                var mailProcess = ProsesMailNotificationBody(TableList, input, Login);

                //distinct double To email
                List<string> ListTo = mailProcess.To.Distinct().ToList();
                _messageService.SendEmailToList(ListTo, mailProcess.Subject, mailProcess.Body, true);


            }
            catch (Exception exp)
            {
                var msg = exp.Message;
                throw;
            }
            return result;
        }
        private ArchiveMailNotification ProsesMailNotificationBody(List<string>  TableList, ArchiveParamInput input, Login Login)
        {
            var bodyMail = new StringBuilder();
            var rc = new ArchiveMailNotification();

            rc.Subject = "";

            bodyMail.Append("Dear Team,<br /><br />");
            bodyMail.AppendLine();
            bodyMail.Append("Here Is the Summary of Archive Data<br />");
            bodyMail.AppendLine();
            bodyMail.Append("Start Date : " + (input.StartDate.HasValue? input.StartDate.Value.ToString("yyyy-MMM-dd HH:mm:ss"): "" ) + "<br />");
            bodyMail.AppendLine();
            bodyMail.Append("End Date : " + (input.EndDate.HasValue ? input.EndDate.Value.ToString("yyyy-MMM-dd HH:mm:ss") : "") + "<br />");
            bodyMail.AppendLine();
            bodyMail.Append("Created By: " + Login.USER_ID + "<br />");
            bodyMail.AppendLine();
            //bodyMail.Append("<tr><td style = 'border: 1px solid black; padding : 5px' >No</td><td style = 'border: 1px solid black; padding : 5px' >Table</td></tr>");
            //bodyMail.AppendLine();
            //foreach (var item in TableList)
            //{
            //    bodyMail.Append("<tr><td style = 'border: 1px solid black; padding : 5px' >" + (TableList.IndexOf(item)+1)  + "</td><td style = 'border: 1px solid black; padding : 5px' >" + item + "</td></tr>");
            //    bodyMail.AppendLine();
            //}
            //bodyMail.Append("</table>");
            bodyMail.Append("Thank you<br /><br />");
            bodyMail.AppendLine();
            rc.Body = bodyMail.ToString();
            rc.To.Add("HMS.FleetManagement@sampoerna.com");
            
            return rc;
        }
       
        private class ArchiveMailNotification
        {
            public ArchiveMailNotification()
            {
                To = new List<string>();
                CC = new List<string>();
                Attachments = new List<string>();
                IsCCExist = false;
            }
            public string Subject { get; set; }
            public string Body { get; set; }
            public List<string> To { get; set; }
            public List<string> CC { get; set; }
            public List<string> Attachments { get; set; }
            public bool IsCCExist { get; set; }
        }
    }
}
