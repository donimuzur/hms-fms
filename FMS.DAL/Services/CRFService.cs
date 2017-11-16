using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Dto;
using FMS.Contract;
using FMS.Contract.Service;
using FMS.Core;
using FMS.Utils;

namespace FMS.DAL.Services
{
    public class CrfService : ICRFService
    {
        private IUnitOfWork _uow;
        
        private IGenericRepository<TRA_CRF> _crfRepository;
        private string _includeTables = "MST_VENDOR,MST_EPAF,MST_EMPLOYEE,MST_REMARK";

        public CrfService(IUnitOfWork uow)
        {
            _uow = uow;

            _crfRepository = _uow.GetGenericRepository<TRA_CRF>();
        }


        public List<TRA_CRF> GetList(TraCrfEpafParamInput input = null)
        {
            Expression<Func<TRA_CRF, bool>> queryFilterCrf = c => c.IS_ACTIVE;
            
            if (input != null)
            {
                if (input.IsActive.HasValue)
                {
                    queryFilterCrf = c => c.IS_ACTIVE == input.IsActive.Value;

                }
                
                
                if (!string.IsNullOrEmpty(input.EmployeeId))
                {
                    queryFilterCrf = queryFilterCrf.And(c => c.EMPLOYEE_ID == input.EmployeeId);
                }

                if (!string.IsNullOrEmpty(input.CurrentLocation))
                {
                    queryFilterCrf = queryFilterCrf.And(c => c.LOCATION_CITY == input.CurrentLocation);
                }

                if (!string.IsNullOrEmpty(input.RelocateLocation))
                {
                    queryFilterCrf = queryFilterCrf.And(c => c.LOCATION_CITY_NEW == input.RelocateLocation);

                }

                if (input.EffectiveDate.HasValue )
                {
                    queryFilterCrf = queryFilterCrf.And(c => c.EFFECTIVE_DATE == input.EffectiveDate.Value);

                }

                if (!string.IsNullOrEmpty(input.DocumentNumber))
                {
                    queryFilterCrf = queryFilterCrf.And(c => c.DOCUMENT_NUMBER == input.DocumentNumber);

                }

                

            }
            return _crfRepository.Get(queryFilterCrf,null,"").ToList();
        }

        public TRA_CRF GetByNumber(string documentNumber)
        {
            return _crfRepository.Get(x => x.DOCUMENT_NUMBER == documentNumber, null, _includeTables).FirstOrDefault();
        }

        public TRA_CRF GetById(int id)
        {
            return _crfRepository.GetByID(id);
        }

        public long SaveCrf(TRA_CRF data,Login userData)
        {
            _crfRepository.InsertOrUpdate(data,userData, Enums.MenuList.TraCrf);//,userData, Enums.MenuList.TraCrf);
            _uow.SaveChanges();
            return data.TRA_CRF_ID;
        }


        public TRA_CRF GetByEpafId(long p)
        {
            return _crfRepository.Get(x => x.EPAF_ID == p).FirstOrDefault();
        }
    }
}
