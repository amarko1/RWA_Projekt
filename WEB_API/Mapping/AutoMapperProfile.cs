using AutoMapper;
using DATA_LAYER.BLModels;
using DATA_LAYER.DALModels;

namespace WEB_API.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<BLVideo, Video>();
            CreateMap<Video, BLVideo>();
            CreateMap<BLVideo, Video>().ForMember(dest => dest.Id, opt => opt.Ignore()); // ignore the Id property when mapping

            CreateMap<BLTag,Tag>();
            CreateMap<Tag, BLTag>();
            CreateMap<BLTag, Tag>().ForMember(dest => dest.Id, opt => opt.Ignore()); // ignore the Id property when mapping

            CreateMap<BLGenre, Genre>();
            CreateMap<Genre, BLGenre>();
            CreateMap<BLGenre, Genre>().ForMember(dest => dest.Id, opt => opt.Ignore()); // ignore the Id property when mapping 

            CreateMap<BLUser, User>();
            CreateMap<User, BLUser>();
            CreateMap<BLUser, User>().ForMember(dest => dest.Id, opt => opt.Ignore()); // ignore the Id property when mapping

            CreateMap<BLNotification, Notification>();
            CreateMap<Notification, BLNotification>();
            CreateMap<BLNotification, Notification>().ForMember(dest => dest.Id, opt => opt.Ignore()); // ignore the Id property when mapping
        }
    }
}
