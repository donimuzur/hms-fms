using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BusinessObject.Dto
{
    public class LdapDto
    {
        public string ADGroup { get; set; }
        public string EmployeeId { get; set; }
        public string Login { get; set; }
        public string DisplayName { get; set; }
        public string RoleName{ get; set; }
    }
}
