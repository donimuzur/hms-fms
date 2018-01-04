using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FMS.Website.Models
{
    public class LoginModel
    {
        [Required]
        [Display(Name = "Username Login")]
        public string UserId { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

    }

    public class LoginFormModel : BaseModel
    {
        public IEnumerable<SelectListItem> Users { get; set; }
        public LoginModel Login { get; set; }
    }
}