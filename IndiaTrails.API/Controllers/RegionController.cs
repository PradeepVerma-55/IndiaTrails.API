using AutoMapper;
using IndiaTrails.API.Models.Domain;
using IndiaTrails.API.Models.DTOs.Request;
using IndiaTrails.API.Models.DTOs.Response;
using IndiaTrails.API.Repositories.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace IndiaTrails.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Protect all endpoints in this controller
    public class RegionController : ControllerBase
    {
        private readonly IRegionRepository _repository;
        private readonly IMapper mapper;
        private readonly ILogger<RegionController> logger;

        public RegionController(IRegionRepository repository,IMapper mapper, ILogger<RegionController> logger)
        {
            this._repository = repository;
            this.mapper = mapper;
            this.logger = logger;
        }
        [HttpGet]
        [AllowAnonymous] // Allow public access to GET all regions
        public async Task<IActionResult> GetAllRegions()
        {
            logger.LogInformation("Getting all regions");
            var regions = await _repository.GetAllAsync();
            if(regions == null)
            {
                return NotFound("No region found!");
            }
            logger.LogInformation($"Found {regions.Count} regions Data - {JsonSerializer.Serialize(regions)}");

            var regionsDto = mapper.Map<List<RegionResponseDto>>(regions);
            return Ok(regionsDto);
            
        }

        [HttpGet]
        [Route("{id:guid}")]
        [AllowAnonymous] // Allow public access to GET region by ID
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
