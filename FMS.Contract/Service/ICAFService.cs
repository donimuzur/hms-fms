using FMS.BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Contract.Service
{
    public interface ICAFService
    {
        void Save(TRA_CAF datatoSave, BusinessObject.Business.Login CurrentUser);

        TRA_CAF GetCafByNumber(string p);

        List<TRA_CAF> GetList();
    }
}
