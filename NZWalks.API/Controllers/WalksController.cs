using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;
using System;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalksController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IWalkRepository walkRepository;

        //Create a new Walk
        //POST: api/walks

        public WalksController(IMapper mapper, IWalkRepository walkRepository)
        {
            this.mapper = mapper;
            this.walkRepository = walkRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddWalkRequestDto addWalkRequestDto)
        {
            //map DTO to Domain model to mapp correctly and this is changed by github
            var domainWalks = mapper.Map<Walk>(addWalkRequestDto);
            await walkRepository.CreateAsync(domainWalks);
            // Domain model to DtO

            var walkDto = mapper.Map<WalkDto>(domainWalks);
            return Ok(walkDto);
        }

        [HttpGet]

        public async Task<IActionResult> GetAll()
        {
            var domainWalks = await walkRepository.GetAllAsync();


            //map domainMOdel to DtoModel
            return Ok(mapper.Map<List<WalkDto>>(domainWalks));
        }

        [HttpGet]
        [Route("{id:Guid}")]

        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var domainwalk = await walkRepository.GetByIdAsync(id);

            if (domainwalk == null)
            {
                return NotFound();
            }

            //map domainMOdel to DtoModel
            return Ok(mapper.Map<WalkDto>(domainwalk));
        }

        //update Walk by id
        //PUT: /api/walks/{id}
        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateWalkRequestDto updateWalkRequestDto)
        {
            //map dto to domain model
            var walkDomainModel = mapper.Map<Walk>(updateWalkRequestDto);

            walkDomainModel = await walkRepository.UpdateAsync(walkDomainModel, id);

            if (walkDomainModel == null)
            {
                return NotFound();
            }

            //map domainmodel to dto
            return Ok(mapper.Map<WalkDto>(walkDomainModel));

        }

        //delete a walK by Id
        [HttpDelete]
        [Route("{id:Guid}")]

        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var deleteWalkDomainModel = await walkRepository.DeleteAsync(id);

            if(deleteWalkDomainModel == null)
            {
                return NotFound();
            }

            //map domain model to dto

            return Ok(mapper.Map<WalkDto>(deleteWalkDomainModel));
        }
    }
}
