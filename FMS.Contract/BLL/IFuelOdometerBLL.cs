using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Inputs;

namespace FMS.Contract.BLL
{
    public interface IFuelOdometerBLL
    {
        List<FuelOdometerDto> GetFuelOdometer();
        List<FuelOdometerDto> GetFuelOdometerByParam(FuelOdometerParamInput param);
        FuelOdometerDto GetByID(long mstFuelOdometerId);
        void Save(FuelOdometerDto SettingDto, Login userLogin);
        void SaveChanges();
    }
}
