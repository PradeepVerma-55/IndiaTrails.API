namespace IndiaTrails.API.Models.DTOs.Response
{
    public class WalkResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double LengthInKm { get; set; }
        public string? WalkImageUrl { get; set; }

        public RegionResponseDto Region { get; set; }
        public DifficultyResponseDto Difficulty { get; set; }
    }

    public class WalkResponseDtoV1
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double LengthInKm { get; set; }
      
    }

    public class WalkResponseDtoV2
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Length { get; set; }

    }
}
