using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject.Inputs;

namespace FMS.Contract.Service
{
    public interface IDocumentNumberService
    {
        string GenerateNumber(GenerateDocNumberInput input);
    }
}
