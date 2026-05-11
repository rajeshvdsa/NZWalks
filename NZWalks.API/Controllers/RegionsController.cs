using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    // https://localhost:1212/api/Regions
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RegionsController : ControllerBase
    {
        private readonly NZWalksDbContext dbcontext;
        private readonly IRegionRepository regionRepository;
        private readonly IMapper mapper;

        public RegionsController(NZWalksDbContext dbContext, IRegionRepository regionRepository,
            IMapper mapper)
        {
            this.dbcontext = dbContext;
            this.regionRepository = regionRepository;
            this.mapper = mapper;
        }
        //get all regions
        //GET: https://Localhost:portnumber/api/regions
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // Get data from Database - domain models
            var regionsDomains = await regionRepository.GetAllAsync();

            //Map domain models to DTOs
           var regions = mapper.Map<List<RegionDto>>(regionsDomains);

            //return DTOs
            return Ok(regions);
        }

        //Get single region by Id
        //GET: https://localhost:portnumber/api/regions/{id}

        [HttpGet]
        [Route("{id:Guid}")]

        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var Region = await regionRepository.GetByIdAsync(id);

            if (Region == null)
            {
                return NotFound();
            }

            //map Region Domain Model to Region DTO
            var regions = mapper.Map<RegionDto>(Region);

            return Ok(regions);
        }


        [HttpPost]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Create([FromBody] AddRegionRequestDto addRegionRequestDto)
        {
            //Map DTO to Domain Model
            var regionDomainModel = mapper.Map<Region>(addRegionRequestDto);

            //Use Domain Model to create Region
            regionDomainModel = await regionRepository.CreateAsync(regionDomainModel);


            //Map Domain Model to Dto
            var regionDto = mapper.Map<RegionDto>(regionDomainModel);
           

            return CreatedAtAction(nameof(GetById), new { id = regionDto.Id }, regionDto);

        }

        [HttpPut]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Writer")]
        public async  Task<IActionResult> update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto updateRegionRequestDto)
        {

            //map dto to Domain Model
            var regionDomainModel = mapper.Map<Region>(updateRegionRequestDto);
          
            regionDomainModel = await regionRepository.UpdateAsync(regionDomainModel, id);

            if(regionDomainModel == null)
            {
                return NotFound();
            }

            //convert Domain Model to Dto
            var regionDto = mapper.Map<RegionDto>(regionDomainModel);

            return Ok(regionDto);

        }

        [HttpDelete]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> delete([FromRoute] Guid id)
        {
            var regionDomainModel = await regionRepository.DeleteAsync(id);

            if(regionDomainModel == null) { return NotFound();}

            var regionDto = mapper.Map<RegionDto>(regionDomainModel);

            return Ok(regionDto);
        }

    }


}
