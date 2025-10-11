using AutoMapper;
using IndiaTrails.API.Models.Domain;
using IndiaTrails.API.Models.DTOs.Response;

namespace IndiaTrails.API.Mappings
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Region,RegionResponseDto>().ReverseMap();

        }
    }
}
