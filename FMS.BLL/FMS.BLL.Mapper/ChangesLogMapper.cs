using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.AutoMapperExtensions;
using FMS.BusinessObject;
using FMS.BusinessObject.Dto;

namespace FMS.BLL.Mapper
{
    public class ChangesLogMapper
    {
        public static void Initialize()
        {
            AutoMapper.Mapper.CreateMap<TRA_CHANGES_HISTORY, ChangesHistoryDto>().IgnoreAllNonExisting();
        }
    }
}
