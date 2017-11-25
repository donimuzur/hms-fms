using FMS.BusinessObject.Business;
using FMS.BusinessObject.Dto;
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

        List<TraCafDto> GetCaf();

        TraCafDto GetById(long id);

        void SaveList(List<TraCafDto> data, Login CurrentUser);

        

        void ValidateCaf(TraCafDto dataTovalidate, out string message);

        TraCafDto GetCafBySirs(string sirsNumber);

        int SaveProgress(TraCafProgressDto traCafProgressDto,string sirsNumber, Login CurrentUser);
    }
}
