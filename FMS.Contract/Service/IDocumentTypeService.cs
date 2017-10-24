using FMS.BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Contract.Service
{
    public interface IDocumentTypeService
    {
        MST_DOCUMENT_TYPE GetDocTypeById(int MstDocType);

        List<MST_DOCUMENT_TYPE> GetDocumentType();
    }
}
