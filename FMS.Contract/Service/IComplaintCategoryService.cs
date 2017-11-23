using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject;
using FMS.BusinessObject.Business;

namespace FMS.Contract.Service
{
    public interface IComplaintCategoryService
    {
        List<MST_COMPLAINT_CATEGORY> GetComplaintCategories();
        MST_COMPLAINT_CATEGORY GetComplaintById(int MstComplaintId);
        void save(MST_COMPLAINT_CATEGORY dbComplaint);
        void save(MST_COMPLAINT_CATEGORY dbComplaint, Login userLogin);
    }
}
