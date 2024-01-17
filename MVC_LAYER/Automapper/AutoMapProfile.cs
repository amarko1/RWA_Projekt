using AutoMapper;
using DATA_LAYER.BLModels;
using DATA_LAYER.DALModels;

namespace MVC_LAYER.Automapper
{
    public class AutoMapProfile : Profile
    {
        public AutoMapProfile()
        {
            CreateMap<BLTag, Tag>();
            CreateMap<Tag, BLTag>();
            CreateMap<BLTag, Tag>().ForMember(dest => dest.Id, opt => opt.Ignore()); // ignore the Id property when mapping
        }
    }
}
