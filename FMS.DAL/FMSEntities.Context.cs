﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FMS.BusinessObject
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class FMSEntities : DbContext
    {
        public FMSEntities()
            : base("name=FMSEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AC_VS_OB_REPORT_DATA> AC_VS_OB_REPORT_DATA { get; set; }
        public virtual DbSet<ACCIDENT_REPORT_DATA> ACCIDENT_REPORT_DATA { get; set; }
        public virtual DbSet<AUTO_GR> AUTO_GR { get; set; }
        public virtual DbSet<AUTO_GR_DETAIL> AUTO_GR_DETAIL { get; set; }
        public virtual DbSet<CFM_IDLE_REPORT_DATA> CFM_IDLE_REPORT_DATA { get; set; }
        public virtual DbSet<FUEL_COST_BY_FUNC_REPORT_DATA> FUEL_COST_BY_FUNC_REPORT_DATA { get; set; }
        public virtual DbSet<FUEL_REPORT_DATA> FUEL_REPORT_DATA { get; set; }
        public virtual DbSet<KPI_REPORT_DATA> KPI_REPORT_DATA { get; set; }
        public virtual DbSet<LEASE_COST_BY_FUNC_REPORT_DATA> LEASE_COST_BY_FUNC_REPORT_DATA { get; set; }
        public virtual DbSet<LITER_BY_FUNC_REPORT_DATA> LITER_BY_FUNC_REPORT_DATA { get; set; }
        public virtual DbSet<MST_COMPLAINT_CATEGORY> MST_COMPLAINT_CATEGORY { get; set; }
        public virtual DbSet<MST_COST_OB> MST_COST_OB { get; set; }
        public virtual DbSet<MST_DELEGATION> MST_DELEGATION { get; set; }
        public virtual DbSet<MST_DOCUMENT_TYPE> MST_DOCUMENT_TYPE { get; set; }
        public virtual DbSet<MST_EMPLOYEE> MST_EMPLOYEE { get; set; }
        public virtual DbSet<MST_EPAF> MST_EPAF { get; set; }
        public virtual DbSet<MST_FLEET> MST_FLEET { get; set; }
        public virtual DbSet<MST_FUEL_ODOMETER> MST_FUEL_ODOMETER { get; set; }
        public virtual DbSet<MST_FUNCTION_GROUP> MST_FUNCTION_GROUP { get; set; }
        public virtual DbSet<MST_GS> MST_GS { get; set; }
        public virtual DbSet<MST_HOLIDAY_CALENDAR> MST_HOLIDAY_CALENDAR { get; set; }
        public virtual DbSet<MST_LOCATION_MAPPING> MST_LOCATION_MAPPING { get; set; }
        public virtual DbSet<MST_MODUL> MST_MODUL { get; set; }
        public virtual DbSet<MST_PENALTY> MST_PENALTY { get; set; }
        public virtual DbSet<MST_PENALTY_LOGIC> MST_PENALTY_LOGIC { get; set; }
        public virtual DbSet<MST_PRICELIST> MST_PRICELIST { get; set; }
        public virtual DbSet<MST_REASON> MST_REASON { get; set; }
        public virtual DbSet<MST_REMARK> MST_REMARK { get; set; }
        public virtual DbSet<MST_SALES_VOLUME> MST_SALES_VOLUME { get; set; }
        public virtual DbSet<MST_SETTING> MST_SETTING { get; set; }
        public virtual DbSet<MST_SYSACCESS> MST_SYSACCESS { get; set; }
        public virtual DbSet<MST_VEHICLE_SPECT> MST_VEHICLE_SPECT { get; set; }
        public virtual DbSet<MST_VENDOR> MST_VENDOR { get; set; }
        public virtual DbSet<NO_OF_VEHICLE_MAKE_TYPE_REPORT_DATA> NO_OF_VEHICLE_MAKE_TYPE_REPORT_DATA { get; set; }
        public virtual DbSet<NO_OF_VEHICLE_REPORT_DATA> NO_OF_VEHICLE_REPORT_DATA { get; set; }
        public virtual DbSet<NO_OF_WTC_VEHICLE_REPORT_DATA> NO_OF_WTC_VEHICLE_REPORT_DATA { get; set; }
        public virtual DbSet<ODOMETER_REPORT_DATA> ODOMETER_REPORT_DATA { get; set; }
        public virtual DbSet<PO_REPORT_DATA> PO_REPORT_DATA { get; set; }
        public virtual DbSet<SALES_BY_REGION_REPORT_DATA> SALES_BY_REGION_REPORT_DATA { get; set; }
        public virtual DbSet<SUM_REPORT_DATA> SUM_REPORT_DATA { get; set; }
        public virtual DbSet<TRA_CAF> TRA_CAF { get; set; }
        public virtual DbSet<TRA_CAF_PROGRESS> TRA_CAF_PROGRESS { get; set; }
        public virtual DbSet<TRA_CCF> TRA_CCF { get; set; }
        public virtual DbSet<TRA_CCF_DETAIL> TRA_CCF_DETAIL { get; set; }
        public virtual DbSet<TRA_CHANGES_HISTORY> TRA_CHANGES_HISTORY { get; set; }
        public virtual DbSet<TRA_CRF> TRA_CRF { get; set; }
        public virtual DbSet<TRA_CSF> TRA_CSF { get; set; }
        public virtual DbSet<TRA_CTF> TRA_CTF { get; set; }
        public virtual DbSet<TRA_CTF_EXTEND> TRA_CTF_EXTEND { get; set; }
        public virtual DbSet<TRA_DOC_NUMBER> TRA_DOC_NUMBER { get; set; }
        public virtual DbSet<TRA_TEMPORARY> TRA_TEMPORARY { get; set; }
        public virtual DbSet<TRA_WORKFLOW_HISTORY> TRA_WORKFLOW_HISTORY { get; set; }
        public virtual DbSet<VEHICLE_HISTORY_REPORT_DATA> VEHICLE_HISTORY_REPORT_DATA { get; set; }
        public virtual DbSet<VEHICLE_REPORT_DATA> VEHICLE_REPORT_DATA { get; set; }
        public virtual DbSet<FLEET_CHANGE> FLEET_CHANGE { get; set; }
        public virtual DbSet<FUNCTION_CHANGE> FUNCTION_CHANGE { get; set; }
        public virtual DbSet<LOCATION_CHANGE> LOCATION_CHANGE { get; set; }
        public virtual DbSet<ARCH_MST_COST_OB> ARCH_MST_COST_OB { get; set; }
        public virtual DbSet<ARCH_MST_EMPLOYEE> ARCH_MST_EMPLOYEE { get; set; }
        public virtual DbSet<ARCH_MST_EPAF> ARCH_MST_EPAF { get; set; }
        public virtual DbSet<ARCH_MST_FLEET> ARCH_MST_FLEET { get; set; }
        public virtual DbSet<ARCH_MST_FUEL_ODOMETER> ARCH_MST_FUEL_ODOMETER { get; set; }
        public virtual DbSet<ARCH_MST_FUNCTION_GROUP> ARCH_MST_FUNCTION_GROUP { get; set; }
        public virtual DbSet<ARCH_MST_GS> ARCH_MST_GS { get; set; }
        public virtual DbSet<ARCH_MST_HOLIDAY_CALENDAR> ARCH_MST_HOLIDAY_CALENDAR { get; set; }
        public virtual DbSet<ARCH_MST_LOCATION_MAPPING> ARCH_MST_LOCATION_MAPPING { get; set; }
        public virtual DbSet<ARCH_MST_PENALTY> ARCH_MST_PENALTY { get; set; }
        public virtual DbSet<ARCH_MST_PRICELIST> ARCH_MST_PRICELIST { get; set; }
        public virtual DbSet<ARCH_MST_SALES_VOLUME> ARCH_MST_SALES_VOLUME { get; set; }
        public virtual DbSet<ARCH_MST_VEHICLE_SPECT> ARCH_MST_VEHICLE_SPECT { get; set; }
        public virtual DbSet<ARCH_TRA_CAF> ARCH_TRA_CAF { get; set; }
        public virtual DbSet<ARCH_TRA_CAF_PROGRESS> ARCH_TRA_CAF_PROGRESS { get; set; }
        public virtual DbSet<ARCH_TRA_CCF> ARCH_TRA_CCF { get; set; }
        public virtual DbSet<ARCH_TRA_CCF_DETAIL> ARCH_TRA_CCF_DETAIL { get; set; }
        public virtual DbSet<ARCH_TRA_CRF> ARCH_TRA_CRF { get; set; }
        public virtual DbSet<ARCH_TRA_CSF> ARCH_TRA_CSF { get; set; }
        public virtual DbSet<ARCH_TRA_CTF> ARCH_TRA_CTF { get; set; }
        public virtual DbSet<ARCH_TRA_CTF_EXTEND> ARCH_TRA_CTF_EXTEND { get; set; }
        public virtual DbSet<ARCH_TRA_TEMPORARY> ARCH_TRA_TEMPORARY { get; set; }
    }
}
