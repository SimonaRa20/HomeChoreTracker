using AutoMapper;
using HomeChoreTracker.Api.Constants;
using HomeChoreTracker.Api.Contracts.HomeChore;
using HomeChoreTracker.Api.Contracts.HomeChoreBase;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
using HomeChoreTracker.Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace HomeChoreTracker.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HomeChoreController : Controller
    {
        private readonly IHomeChoreRepository _homeChoreRepository;
        private readonly IHomeChoreBaseRepository _homeChoreBaseRepository;
        private readonly IMapper _mapper;

        public HomeChoreController(IHomeChoreRepository homeChoreRepository,  IMapper mapper, IHomeChoreBaseRepository homeChoreBaseRepository)
        {
            _homeChoreRepository = homeChoreRepository;
            _mapper = mapper;
            _homeChoreBaseRepository = homeChoreBaseRepository;
        }

        [HttpPost("{homeId}")]
        [Authorize]
        public async Task<IActionResult> AddHomeChore(int homeId, int taskId)
        {
            try
            {
                var homeChoreBase = await _homeChoreBaseRepository.GetChoreBase(taskId);

                await _homeChoreRepository.AddHomeChoreBase(homeChoreBase, homeId);

                return Ok($"Home chore was added successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while adding the home chore: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateHomeChoreBase(HomeChoreRequest homeChoreBaseRequest)
        {
            await _homeChoreRepository.CreateHomeChore(homeChoreBaseRequest);
            await _homeChoreRepository.Save();
            return Ok("Home chore base created successfully");
        }
    }
}
