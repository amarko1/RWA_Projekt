using AutoMapper;
using DATA_LAYER.BLModels;
using DATA_LAYER.DALModels;
using MVC_LAYER.Models;

namespace MVC_LAYER.Automapper
{
    public class AutoMapProfile : Profile
    {
        public AutoMapProfile()
        {
            CreateMap<BLTag, Tag>();
            CreateMap<Tag, BLTag>();

            CreateMap<BLGenre, Genre>();
            CreateMap<Genre, BLGenre>();

            CreateMap<Video, BLVideo>()
                .ForMember(dest => dest.GenreName, opt => opt.MapFrom(src => src.Genre.Name));
            CreateMap<BLVideo, Video>();

            CreateMap<Country, BLCountry>()
                .ReverseMap();

            CreateMap<User, BLUser>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName.Trim()))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName.Trim()));

            CreateMap<BLUser, VMUser>();
        }
    }
}
