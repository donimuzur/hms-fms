using FMS.AutoMapperExtensions;
using FMS.BusinessObject;
using FMS.BusinessObject.Dto;

namespace FMS.BLL.Mapper
{
    public class WorkflowHistoryMapper
    {
        public static void Initialize()
        {
            AutoMapper.Mapper.CreateMap<WorkflowHistoryDto, TRA_WORKFLOW_HISTORY>().IgnoreAllNonExisting()
                .ForMember(dest => dest.REMARK, opt => opt.MapFrom(src => src.REMARK_ID));

            AutoMapper.Mapper.CreateMap<TRA_WORKFLOW_HISTORY, WorkflowHistoryDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.REMARK_ID, opt => opt.MapFrom(src => src.REMARK));
        }
    }
}
