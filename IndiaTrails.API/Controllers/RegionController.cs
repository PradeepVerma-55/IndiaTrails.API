using AutoMapper;
using IndiaTrails.API.Models.Domain;
using IndiaTrails.API.Models.DTOs.Request;
using IndiaTrails.API.Models.DTOs.Response;
using IndiaTrails.API.Repositories.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IndiaTrails.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionController : ControllerBase
    {
        private readonly IRegionRepository _repository;
        private readonly IMapper mapper;

        public RegionController(IRegionRepository repository,IMapper mapper)
        {
            this._repository = repository;
            this.mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllRegions()
        {
            var regions = await _repository.GetAllAsync();
            if(regions == null)
            {
                return NotFound("No region found!");
            }
            var regionsDto = mapper.Map<List<RegionResponseDto>>(regions);
            return Ok(regionsDto);
            
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetRegionById(Guid id)
        {
            var region = await _repository.GetByIdAsync(id);
            if(region == null)
                return NotFound("No region found with given id!");
            return Ok(mapper.Map<RegionResponseDto>(region));
        }

        [HttpPost]
        public async Task<IActionResult> CreateRegion([FromBody] AddRegionRequestDto region)
        {
            if(region == null)
                return BadRequest("Region data is null!");
            var regionModel = mapper.Map<Region>(region);
            var createdRegion = await _repository.CreateAsync(regionModel);
            return CreatedAtAction(nameof(GetRegionById), new { id = createdRegion.Id }, mapper.Map<RegionResponseDto>(createdRegion));
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateRegion([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto region)
        {
            if(region == null)
                return BadRequest("Region data is null!");

            var regionModel = mapper.Map<Region>(region);
            var updatedRegion = await _repository.UpdateAsync(id, regionModel);
            if (updatedRegion == null)
                return NotFound("No region found with given id to update!");
            return Ok(mapper.Map<RegionResponseDto>(updatedRegion));
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteRegion([FromRoute] Guid id)
        {
            var deletedRegion = await _repository.DeleteAsync(id);
            if (deletedRegion == null)
                return NotFound("No region found with given id to delete!");
            return Ok(mapper.Map<RegionResponseDto>(deletedRegion));
        }

    }
}
