using FMS.BusinessObject.Business;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Contract.BLL
{
    public interface ICafBLL
    {
        void Save(TraCafDto data, Login user);

        List<TraCafDto> GetCafWithParam();
        List<TraCafDto> GetCafWithParam(CafParamInput Param);

        TraCafDto GetById(long id, bool? Archive = null);

        void SaveList(List<TraCafDto> data, Login CurrentUser);

        

        void ValidateCaf(TraCafDto dataTovalidate, out string message);

        TraCafDto GetCafBySirs(string sirsNumber);

        int SaveProgress(TraCafProgressDto traCafProgressDto,string sirsNumber, Login CurrentUser);

        List<TraCafDto> GetCafPersonal(Login CurrentUser);

        void CompleteCaf(int TraCafId, Login CurrentUser);

        void CloseCaf(long traCafId);
    }
}
