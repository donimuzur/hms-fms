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
    
        public virtual DbSet<caf> cafs { get; set; }
        public virtual DbSet<caf_progress> caf_progress { get; set; }
        public virtual DbSet<ccf> ccfs { get; set; }
        public virtual DbSet<change_log> change_log { get; set; }
        public virtual DbSet<cost_center> cost_center { get; set; }
        public virtual DbSet<crf> crfs { get; set; }
        public virtual DbSet<csf> csfs { get; set; }
        public virtual DbSet<ctf> ctfs { get; set; }
        public virtual DbSet<epaf> epafs { get; set; }
        public virtual DbSet<penalty> penalties { get; set; }
        public virtual DbSet<penalty_logic> penalty_logic { get; set; }
        public virtual DbSet<role> roles { get; set; }
        public virtual DbSet<SysUser> SysUsers { get; set; }
        public virtual DbSet<SysUserRole> SysUserRoles { get; set; }
        public virtual DbSet<vehicle> vehicles { get; set; }
        public virtual DbSet<vehicle_specs> vehicle_specs { get; set; }
        public virtual DbSet<vehicle_type> vehicle_type { get; set; }
        public virtual DbSet<content_email> content_email { get; set; }
        public virtual DbSet<SysAccess> SysAccesses { get; set; }
        public virtual DbSet<SysMenu> SysMenus { get; set; }
        public virtual DbSet<SysMenuAccess> SysMenuAccesses { get; set; }
        public virtual DbSet<SysModule> SysModules { get; set; }
        public virtual DbSet<SysRole> SysRoles { get; set; }
        public virtual DbSet<complaint_category> complaint_category { get; set; }
    }
}
