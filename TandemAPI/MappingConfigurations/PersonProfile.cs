using AutoMapper;
using TandemAPI.Models;
using TandemAPI.ViewModels;

namespace TandemAPI.MappingConfigurations
{
    public class PersonProfile : Profile
    {
        public PersonProfile()
        {
            CreateMap<Person, PersonViewModel>();
        }
    }
}
