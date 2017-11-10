using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Dto;

namespace FMS.Contract.Service
{
    public interface ICRFService
    {
        List<TRA_CRF> GetList(TraCrfEpafParamInput input = null);
        TRA_CRF GetByNumber(string documentNumber);

        TRA_CRF GetById(int id);

        long SaveCrf(TRA_CRF data, Login userData);
    }
}
