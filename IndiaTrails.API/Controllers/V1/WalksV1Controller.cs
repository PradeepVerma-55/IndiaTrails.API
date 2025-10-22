using AutoMapper;
using IndiaTrails.API.Models.DTOs.Response;
using IndiaTrails.API.Repositories.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace IndiaTrails.API.V1.Controllers
{
    // Version 1.0
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class WalksController : ControllerBase
    {
        private readonly IWalkRepository repository;
        private readonly IMapper mapper;

        public WalksController(IWalkRepository repository, IMapper mapper)
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
                var walksDto = mapper.Map<List<WalkResponseDtoV1>>(walks);
                return Ok(walksDto);
            }
            catch (Exception)
            {
                //Log this excpetion
                return Problem("An error occurred while processing your request.", null, (int)HttpStatusCode.InternalServerError);
            }
        }

    }
}
