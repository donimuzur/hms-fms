using FMS.BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Contract.Service
{
    public interface IModulService
    {
        List<MST_MODUL> GetModul();
        MST_MODUL GetModulById(int MstModulById);
    }
}
