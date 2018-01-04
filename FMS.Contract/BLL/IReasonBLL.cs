﻿using FMS.BusinessObject.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject.Business;

namespace FMS.Contract.BLL
{
    public interface IReasonBLL
    {
        List<ReasonDto> GetReason();
        void save(ReasonDto ReasonDto);
        ReasonDto GetReasonById(int MstReasonId);
        void save(ReasonDto dto, Login currentUser);
        void SaveCanges();
    }
}
