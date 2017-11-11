using FMS.BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Contract.Service
{
    public interface ICtfService 
    {
        List<TRA_CTF> GetCtf();
        void Save(TRA_CTF dbCtf);
    }
}
