using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject;
using FMS.BusinessObject.Business;

namespace FMS.Contract.BLL
{
    public interface IPriceListBLL
    {
        List<PriceListDto> GetPriceList();
        PriceListDto GetExist(string Model);
        PriceListDto GetByID(int Id);
        void Save(PriceListDto PriceListDto);
        void Save(PriceListDto data, Login currentUser);
        void SaveChanges();
    }
}
