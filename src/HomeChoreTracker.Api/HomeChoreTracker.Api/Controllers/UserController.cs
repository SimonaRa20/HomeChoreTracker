using AutoMapper;
using HomeChoreTracker.Api.Constants;
using HomeChoreTracker.Api.Contracts.User;
using HomeChoreTracker.Api.Interfaces;
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

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUserData(int id)
        {
            try
            {
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

        [HttpPut("Update/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUserData(int id, UserGetResponse updatedProfile)
        {
            try
            {
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
