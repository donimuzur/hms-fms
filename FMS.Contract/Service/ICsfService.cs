using FMS.BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Contract.Service
{
    public interface ICsfService
    {
        List<TRA_CSF> GetCsf();
        void save(TRA_CSF dbTraCsf);
    }
}
