using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.Contract;
using FMS.Contract.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.Core;
using System.Linq.Expressions;
using FMS.Utils;

namespace FMS.DAL.Services
{
    public class CafService : ICAFService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<TRA_CAF> _traCafRepository;
        private IGenericRepository<TRA_CAF_PROGRESS> _traCafProgressRepository;

        public CafService(IUnitOfWork uow)
        {
            _uow = uow;
            _traCafRepository = _uow.GetGenericRepository<TRA_CAF>();
            _traCafProgressRepository = _uow.GetGenericRepository<TRA_CAF_PROGRESS>();
        }

        public void Save(TRA_CAF datatoSave, BusinessObject.Business.Login CurrentUser)
        {
            _traCafRepository.InsertOrUpdate(datatoSave,CurrentUser,Core.Enums.MenuList.TraCaf);
            _uow.SaveChanges();
        }

        public int SaveProgress(TRA_CAF_PROGRESS dataToSave, string sirsNumber, Login CurrentUser)
        {
            var mainData = _traCafRepository.Get(x => x.SIRS_NUMBER == sirsNumber).FirstOrDefault();
            
            if (mainData != null)
            {
                //var dbData = _traCafProgressRepository.Get(
                //    x => x.TRA_CAF_ID == mainData.TRA_CAF_ID && x.STATUS_ID == dataToSave.STATUS_ID).FirstOrDefault();
                //if (dbData != null)
                //{
                //    dbData.ESTIMATION = dataToSave.ESTIMATION;
                //    _traCafProgressRepository.InsertOrUpdate(dbData);
                //    if (dbData.STATUS_ID.HasValue)
                //    {
                //        mainData.DOCUMENT_STATUS = dbData.STATUS_ID.Value;
                //        mainData.MODIFIED_BY = CurrentUser.USER_ID;
                //        mainData.MODIFIED_DATE = DateTime.Now;
                //    }
                //    _traCafRepository.InsertOrUpdate(mainData, CurrentUser, Enums.MenuList.TraCaf);
                //}
                //else
               //{
                    dataToSave.TRA_CAF_ID = mainData.TRA_CAF_ID;
                    var count = _traCafProgressRepository.Get(
                        x => x.TRA_CAF_ID == mainData.TRA_CAF_ID).Count();
                    var lastStatusData =
                        _traCafProgressRepository.Get(x => x.TRA_CAF_ID == mainData.TRA_CAF_ID, null, "")
                            .OrderByDescending(x => x.TRA_CAF_PROGRESS_ID)
                            .FirstOrDefault();

                    if (lastStatusData != null)
                    {
                        lastStatusData.ACTUAL = DateTime.Now;
                    }
                    _traCafProgressRepository.InsertOrUpdate(dataToSave);
                    if (dataToSave.STATUS_ID.HasValue)
                    {
                        mainData.DOCUMENT_STATUS = dataToSave.STATUS_ID.Value;
                        mainData.MODIFIED_BY = CurrentUser.USER_ID;
                        mainData.MODIFIED_DATE = DateTime.Now;
                    }
                    _traCafRepository.InsertOrUpdate(mainData,CurrentUser,Enums.MenuList.TraCaf);
                    _uow.SaveChanges();

                    return count;
                //}
            }
            return -1;
        }


        public TRA_CAF GetCafByNumber(string p)
        {
            return _traCafRepository.Get(x => x.SIRS_NUMBER == p, null, "TRA_CAF_PROGRESS").FirstOrDefault();
        }

        public bool IsCafExist(string policeNumber, DateTime incidentDate)
        {
            Expression<Func<TRA_CAF, bool>> queryFilterCrf = c => c.IS_ACTIVE;

            

            queryFilterCrf = queryFilterCrf.And(x => x.POLICE_NUMBER == policeNumber);

            queryFilterCrf = queryFilterCrf.And(x => x.INCIDENT_DATE.HasValue && x.INCIDENT_DATE.Value == incidentDate.Date);

            return _traCafRepository.Get(queryFilterCrf).Any();
        }

        public List<TRA_CAF> GetList()
        {
            return _traCafRepository.Get(x => x.IS_ACTIVE).ToList();
        }


        public TRA_CAF GetCafById(long id)
        {
            return _traCafRepository.Get(x=> x.TRA_CAF_ID == id,null,"TRA_CAF_PROGRESS").FirstOrDefault();
        }
    }
}
