using AutoMapper;
using IndiaTrails.API.Models.Domain;
using IndiaTrails.API.Models.DTOs.Request;
using IndiaTrails.API.Models.DTOs.Response;
using IndiaTrails.API.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Net;
namespace IndiaTrails.API.V1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MyWalksController : ControllerBase
    {
        private readonly IWalkRepository repository;
        private readonly IMapper mapper;

        public MyWalksController(IWalkRepository repository,IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        [HttpGet]
        // GET: /api/walks?filterOn=Name&filterQuery=Track&sortBy=Name&isAscending=true&pageNumber=1&pageSize=10
        public async Task<IActionResult> GetAllWalks([FromQuery] string? filterOn, [FromQuery] string? filterQuery,
            [FromQuery] string? sortBy, [FromQuery] bool? isAscending,
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000)
        {
            try
            {
                var walks = await repository.GetAllAsync(filterOn, filterQuery);
                if (walks == null || !walks.Any())
                {
                    return NotFound("No walks found!");
                }
                var walksDto = mapper.Map<List<WalkResponseDto>>(walks);
                return Ok(walksDto);
            }
            catch (Exception)
            {
                //Log this excpetion
                return Problem("An error occurred while processing your request.",null, (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetWalkById(Guid id)
        {
            var walk = await repository.GetByIdAsync(id);
            if (walk == null)
                return NotFound("No walk found with given id!");
            return Ok(mapper.Map<WalkResponseDto>(walk));
        }

        [HttpPost]
        public async Task<IActionResult> CreateWalk([FromBody] AddWalkRequestDto walk)
        {
            if (walk == null)
                return BadRequest("Walk data is null!");
            var walkModel = mapper.Map<Walk>(walk);
            var createdWalk = await repository.CreateAsync(walkModel);
            return CreatedAtAction(nameof(GetWalkById), new { id = createdWalk.Id }, mapper.Map<WalkResponseDto>(createdWalk));
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateWalk([FromRoute] Guid id, [FromBody] UpdateWalkRequestDto walk)
        {
            if (walk == null)
                return BadRequest("Walk data is null!");
            var walkModel = mapper.Map<Models.Domain.Walk>(walk);
            var updatedWalk = await repository.UpdateAsync(id, walkModel);
            if (updatedWalk == null)
                return NotFound("No walk found with given id to update!");
            return Ok(mapper.Map<WalkResponseDto>(updatedWalk));
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteWalk(Guid id)
        {
            var deletedWalk = await repository.DeleteAsync(id);
            if (deletedWalk == null)
                return NotFound("No walk found with given id to delete!");
            return Ok(mapper.Map<WalkResponseDto>(deletedWalk));
        }

    }

}
