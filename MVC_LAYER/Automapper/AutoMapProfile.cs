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
            //CreateMap<BLTag, Tag>().ForMember(dest => dest.Id, opt => opt.Ignore()); // ignore the Id property when mapping

            CreateMap<BLGenre, Genre>();
            CreateMap<Genre, BLGenre>();
            //CreateMap<BLGenre, Genre>().ForMember(dest => dest.Id, opt => opt.Ignore()); // ignore the Id property when mapping

            //CreateMap<BLVideo, Video>();
            //CreateMap<Video, BLVideo>();
            //CreateMap<BLVideo, Video>().ForMember(dest => dest.Id, opt => opt.Ignore()); // ignore the Id property when mapping
            //CreateMap<Video, BLVideo>().ForMember(dest => dest.Genre, opt => opt.MapFrom(src => src.Genre.Name)); 

            CreateMap<Video, BLVideo>()
                //.ForMember(dest => dest.GenreName, opt => opt.Ignore())
                //.ForMember(dest => dest.GenreName, opt => opt.MapFrom(src => src.Genre.Name))
                .ReverseMap();

            CreateMap<Country, BLCountry>()
                .ReverseMap();
        }
    }
}
