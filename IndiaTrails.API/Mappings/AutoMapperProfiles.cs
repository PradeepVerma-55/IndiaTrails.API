using AutoMapper;
using IndiaTrails.API.Models.Domain;
using IndiaTrails.API.Models.DTOs.Request;
using IndiaTrails.API.Models.DTOs.Response;

namespace IndiaTrails.API.Mappings
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Region,RegionResponseDto>().ReverseMap();
            CreateMap<Difficulty,DifficultyResponseDto>().ReverseMap();
            CreateMap<AddWalkRequestDto, Walk>().ReverseMap();
            CreateMap<UpdateWalkRequestDto, Walk>().ReverseMap();
            CreateMap<Walk, WalkResponseDto>();

        }
    }
}
