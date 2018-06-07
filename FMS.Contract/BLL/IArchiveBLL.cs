using FMS.BusinessObject.Business;
using FMS.BusinessObject.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Contract.BLL
{
    public interface IArchiveBLL
    {
        bool DoArchive(ArchiveParamInput input, Login Login);
    }
}
