using AutoMapper;
using HomeChoreTracker.Api.Constants;
using HomeChoreTracker.Api.Contracts.HomeChoreBase;
using HomeChoreTracker.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HomeChoreTracker.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HomeChoreBaseController : Controller
    {
        private readonly IHomeChoreBaseRepository _homeChoreBaseRepository;
        private readonly IMapper _mapper;

        public HomeChoreBaseController(IHomeChoreBaseRepository homeChoreBaseRepository, IMapper mapper)
        {
            _homeChoreBaseRepository = homeChoreBaseRepository;
            _mapper = mapper;
        }

        [HttpGet("skip{skip}/take{take}")]
        [Authorize]
        public async Task<IActionResult> GetHomeChoresBase(int skip = 0, int take = 5)
        {
            try
            {
                var homeChoreBases = await _homeChoreBaseRepository.GetPaginated(skip, take);

                return Ok(homeChoreBases);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while fetching home chore bases: {ex.Message}");
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetHomeChoresBase()
        {
            try
            {
                var homeChoreBases = await _homeChoreBaseRepository.GetAll();

                if (homeChoreBases == null || !homeChoreBases.Any())
                {
                    return NotFound("No home chore bases found");
                }

                return Ok(homeChoreBases);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while fetching home chore bases: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> CreateHomeChoreBase(HomeChoreBaseRequest homeChoreBaseRequest)
        {
            await _homeChoreBaseRepository.AddHomeChoreBase(homeChoreBaseRequest);
            await _homeChoreBaseRepository.Save();

            return Ok("Home chore base created successfully");
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetHomeChoreBase(int id)
        {
            try
            {
                var homeChoreBase = await _homeChoreBaseRepository.GetChoreBase(id);

                if (homeChoreBase == null)
                {
                    return NotFound($"Home chore base with ID {id} not found");
                }

                return Ok(homeChoreBase);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while deleting the article: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> DeleteHomeChoreBase(int id)
        {
            try
            {
                await _homeChoreBaseRepository.Delete(id);
                await _homeChoreBaseRepository.Save();

                return Ok($"Home chore base with ID {id} deleted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while deleting the article: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> UpdateHomeChoreBase(int id, [FromBody] HomeChoreBaseRequest homeChoreBaseRequest)
        {
            try
            {
                var homeChoreBase = await _homeChoreBaseRepository.GetChoreBase(id);

                if (homeChoreBase == null)
                {
                    return NotFound($"Home chore base with ID {id} not found");
                }

                homeChoreBase.Id = id;
                homeChoreBase.Name = homeChoreBaseRequest.Name;
                homeChoreBase.ChoreType = homeChoreBaseRequest.ChoreType;
                homeChoreBase.Frequency = homeChoreBaseRequest.Frequency;
                homeChoreBase.Description = homeChoreBaseRequest.Description;

                await _homeChoreBaseRepository.Update(homeChoreBase);
                await _homeChoreBaseRepository.Save();

                return Ok($"Home chore base with ID {id} updated successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while updating the home chore base: {ex.Message}");
            }
        }
    }
}
