﻿using FMS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FMS.Website.Models
{
    public class TraCafItemViewModel : BaseModel
    {
        public TraCafItemViewModel()
        {
           
        }

        public TraCafItemDetails Detail { get; set; }

        public System.Web.Mvc.SelectList RemarkList { get; set; }
    }

    public class TraCafUploadViewModel : BaseModel
    {
        public TraCafUploadViewModel() {
            Details = new List<TraCafItemDetails>();
        }

        public List<TraCafItemDetails> Details { get; set; }
    }


    public class TraCafIndexViewModel : BaseModel
    {
        public TraCafIndexViewModel()
        {
            Details = new List<TraCafItemDetails>();
        }

        public List<TraCafItemDetails> Details { get; set; }

        public System.Web.Mvc.SelectList RemarkList { get; set; }
    }

    public class TraCafItemDetails{

        public TraCafItemDetails()
        {
            ProgressDetails = new List<TraCafProgress>();
        }

        public long TraCafId {get;set;}
        public string DocumentNumber {get;set;}

        public int? DocumentStatus {get;set;}
        public string DocumentStatusString {get;set;}
        public string SirsNumber {get;set;}
        public string PoliceNumber {get;set;}
        public string EmployeeId{get;set;}
        public string EmployeeName {get;set;}
        public string Supervisor {get;set;}

        public string Area {get;set;}

        public int VendorId {get;set;}
        public string VendorName {get;set;}

        public string VehicleModel {get;set;}
        public DateTime IncidentDate {get;set;}
        public string IncidentLocation {get;set;}
        public int Remark {get;set;}
        public string CreatedBy {get;set;}
        public DateTime CreatedDate {get;set;}
        public string ModifiedBy {get;set;}
        public DateTime? ModifiedDate {get;set;}
        public bool IsActive {get;set;}

        public List<TraCafProgress> ProgressDetails { get; set; }

        public string IncidentDescription { get; set; }

        public string Message { get; set; }
    }

    public class TraCafProgress
    {
        public long TraCafId { get; set; }
        public int? StatusId { get; set; }

        public DateTime? ProgressDate { get; set; }

        public string Remark { get; set; }
        public int? Estimation { get; set; }

        public int? Actual { get; set; }
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

    }
}