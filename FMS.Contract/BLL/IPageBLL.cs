﻿using System.Collections.Generic;
using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Dto;

namespace FMS.Contract.BLL
{
    public interface IPageBLL
    {
        MST_MODUL GetPageByID(int id);
        MST_MODUL GetPageByModulName(string ModulName);
        List<MST_MODUL> GetPages();

        List<MST_MODUL> GetModulePages();

        void Save(MST_MODUL pageMap);

        void DeletePageMap(int id);

        List<MST_MODUL> GetParentPages();

        List<RoleDto> GetAuthPages(Login user);
        List<int?> GetAuthPages(System.String userId);
        List<ChangesHistoryDto> GetChangesHistory(int modulId, long formId);
        List<WorkflowHistoryDto> GetWorkflowHistory(int modulId, long formId);

        List<RemarkDto> GetAllRemark();
    }
}
