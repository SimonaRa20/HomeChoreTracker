using AutoMapper;
using HomeChoreTracker.Api.Constants;
using HomeChoreTracker.Api.Contracts.Forum;
using HomeChoreTracker.Api.Contracts.User;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
using HomeChoreTracker.Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HomeChoreTracker.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthRepository _authRepository;
        private readonly IMapper _mapper;

        public UserController(IUserRepository userRepository, IAuthRepository authRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _authRepository = authRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserData()
        {
            try
            {
                int id = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
                var user = await _userRepository.GetUserById(id);

                if (user == null)
                {
                    return NotFound();
                }

                UserGetResponse response = _mapper.Map<UserGetResponse>(user);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("BusyIntervals")]
        [Authorize]
        public async Task<IActionResult>GetUserBusyItervals()
        {
            int id = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            List<BusyInterval> busyIntervals = await _userRepository.GetUserBusyIntervals(id);

            List<BusyIntervalResponse> results = new List<BusyIntervalResponse>();

            foreach(BusyInterval busyInterval in busyIntervals)
            {
                BusyIntervalResponse response = new BusyIntervalResponse
                {
                    Id = busyInterval.Id,
                    Day = busyInterval.Day,
                    StartTime = busyInterval.StartTime,
                    EndTime = busyInterval.EndTime,
                };
                results.Add(response);
            }

            return Ok(results);
        }

        [HttpPost("BusyInterval")]
        [Authorize]
        public async Task<IActionResult> AddBusyInterval(BusyIntervalRequest interval)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            var user = await _userRepository.GetUserById(userId);

            if (user == null)
            {
                return NotFound();
            }

            var busyInterval = new BusyInterval
            {
                Day = interval.Day,
                StartTime = interval.StartTime,
                EndTime  = interval.EndTime,
                UserId = userId,
                User = user,
            };

            await _userRepository.AddBusyInterval(busyInterval);
            return Ok("Interval added successfully");
        }

        [HttpDelete("BusyInterval/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteBusyIntervalById(int id)
        {
            await _userRepository.DeleteInterval(id);
            return Ok($"Busy interval with ID {id} deleted successfully");
        }

        [HttpPut("BusyInterval/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateBusyInterval(int id, BusyIntervalRequest adviceRequest)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            var intervalToUpdate = await _userRepository.GetBusyIntervalById(id);

            if (intervalToUpdate == null)
            {
                return NotFound($"Advice with ID {id} not found");
            }

            if (intervalToUpdate.UserId != userId)
            {
                return Unauthorized("You do not have permission to update this advice");
            }

            intervalToUpdate.Day = adviceRequest.Day;
            intervalToUpdate.StartTime = adviceRequest.StartTime;
            intervalToUpdate.EndTime = adviceRequest.EndTime;

            await _userRepository.UpdateInterval(intervalToUpdate);

            return Ok("Advice updated successfully");
        }

        [HttpPut("Update")]
        [Authorize]
        public async Task<IActionResult> UpdateUserData(UserGetResponse updatedProfile)
        {
            try
            {
                int id = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
                var user = await _userRepository.GetUserById(id);

                if (user == null)
                {
                    return NotFound();
                }

                if(updatedProfile.UserName.IsNullOrEmpty())
                {
                    return new ObjectResult("Username cannot be empty")
                    {
                        StatusCode = (int)HttpStatusCode.UnprocessableEntity
                    };
                }

                if (updatedProfile.Email.IsNullOrEmpty())
                {
                    return new ObjectResult("Email cannot be empty")
                    {
                        StatusCode = (int)HttpStatusCode.UnprocessableEntity
                    };
                }

                var checkUser = await _authRepository.GetUserByEmail(updatedProfile.Email);

                if(checkUser != null && updatedProfile.Email != user.Email)
                {
                    return new ObjectResult("Email is used already, change to another.")
                    {
                        StatusCode = (int)HttpStatusCode.UnprocessableEntity
                    };
                }

                user.UserName = updatedProfile.UserName;
                user.Email = updatedProfile.Email;
                user.StartDayTime = updatedProfile.StartDayTime;
                user.EndDayTime = updatedProfile.EndDayTime;


                await _userRepository.UpdateUser(user);

                return Ok(updatedProfile);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
