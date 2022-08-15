using AutoMapper;
using Bot.Common.Mapper;
using Bot.Models;

namespace Bot.Common
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Section, SectionDTO>().ReverseMap();
            CreateMap<Teacher, TeacherDTO>().ReverseMap();
            CreateMap<RunningTimeSection, RunningTimeDTO>().ReverseMap();
        }
    }
}
