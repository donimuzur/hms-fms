using FMS.BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject.Business;

namespace FMS.Contract.Service
{
    public interface ICAFService
    {
        void Save(TRA_CAF datatoSave, BusinessObject.Business.Login CurrentUser);

        int SaveProgress(TRA_CAF_PROGRESS dataToSave,string sirsNumber, Login CurrentUser);

        TRA_CAF GetCafByNumber(string p);

        List<TRA_CAF> GetList();

        TRA_CAF GetCafById(long id);

        bool IsCafExist(string policeNumber, DateTime incidentDate);
    }
}
