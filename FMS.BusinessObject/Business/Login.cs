using System.Collections.Generic;
using FMS.Core;


namespace FMS.BusinessObject.Business
{
    public class Login
    {
        public string USER_ID { get; set; }
        public string USERNAME { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string USER_GROUP_ID { get; set; }
        public Enums.UserRole UserRole { get; set; }
        public List<int?> AuthorizePages { get; set; }

    }

}
