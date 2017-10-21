using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject;

namespace FMS.Contract.BLL
{
    public interface IPriceListBLL
    {
        List<PriceListDto> GetPriceList();
        PriceListDto GetExist(string Model);
        PriceListDto GetByID(int Id);
        void Save(PriceListDto PriceListDto);
    }
}
