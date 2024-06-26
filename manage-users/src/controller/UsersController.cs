using System.Net;
using manage_users.src.models;
using manage_users.src.models.errors;
using manage_users.src.models.requests;
using manage_users.src.repository;
using manage_users.src.util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace manage_users.src.controller
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        IRequestValidator _validator;
        IUsersRepository _usersRepository;

        public UsersController(IUsersRepository usersRepository, IRequestValidator requestValidator)
        {
            _validator = requestValidator;
            _usersRepository = usersRepository;
        }

        [HttpGet("id/{id}")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            if (_validator.ValidateGetUser(id))
            {
                try
                {
                    User user = await _usersRepository.GetUser(id);
                    return Ok(user);
                }
                catch (Exception ex)
                {
                    return InternalError(ex.Message);
                }
            }
            else
            {
                return BadRequest("User ID is required.");
            }
        }
        
        [HttpGet("email/{email}")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public async Task<ActionResult<User>> GetUser(string email)
        {
            if (_validator.ValidateGetUser(email))
            {
                try
                {
                    User user = await _usersRepository.GetUser(email);
                    return Ok(user);
                }
                catch (Exception ex)
                {
                    return InternalError(ex.Message);
                }
            }
            else
            {
                return BadRequest("Email is required.");
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(UserList), StatusCodes.Status200OK)]
        public async Task<ActionResult<UserList>> GetUsers()
        {
            if (_validator.ValidateGetUsers())
            {
                try
                {
                    UserList userList = await _usersRepository.GetUsers();
                    return Ok(userList);
                }
                catch (Exception ex)
                {
                    return InternalError(ex.Message);
                }
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult CreateUser(CreateUser createUserRequest)
        {
            if (_validator.ValidateCreateUser(createUserRequest))
            {
                try
                {
                    _usersRepository.CreateUser(createUserRequest);
                    return Ok("User Created");
                }
                catch (Exception ex)
                {
                    return InternalError(ex.Message);
                }
            }
            else
            {
                return BadRequest("CreateUser is required.");
            }
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult UpdateUser(UpdateUser updateUserRequest)
        {
            if (_validator.ValidateUpdateUser(updateUserRequest))
            {
                try
                {
                    _usersRepository.UpdateUser(updateUserRequest);
                    return Ok("User Updated");
                }
                catch (Exception ex)
                {
                    return InternalError(ex.Message);
                }
            }
            else
            {
                return BadRequest("UpdateUser is required.");
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult DeleteUser(int id, int updateUserId)
        {
            if (_validator.ValidateDeleteUser(id, updateUserId))
            {
                try
                {
                    _usersRepository.DeleteUser(id, updateUserId);
                    return Ok("User Deleted");
                }
                catch (Exception ex)
                {
                    return InternalError(ex.Message);
                }
            }
            else
            {
                return BadRequest("id and updateUserId params are required.");
            }
        }

        #region HELPERS

        private ObjectResult InternalError(string message)
        {
            var errorResponse = new ErrorResponse
            {
                Status = HttpStatusCode.InternalServerError,
                Type = "InternalServerError",
                Detail = message
            };
            return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
        }

        #endregion 
    }
}