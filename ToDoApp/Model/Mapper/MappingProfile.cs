using AutoMapper;
using ToDoApp.Model.DTO;
using ToDoAppWebService.DTO;

namespace ToDoApp.Model.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Task, TaskDto>().ReverseMap();
            CreateMap<ToDoList, ToDoListDto>().ReverseMap();
        }
    }
}