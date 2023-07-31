using buddyUp.Data;
using buddyUp.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Eventing.Reader;

namespace buddyUp.Controllers
{
    [ApiController]
    [Route("api/match")]
    public class MatchController : ControllerBase
    {
        private readonly IMatchRepository _rep;
        private readonly ILogger<MatchController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IUserProfileRepository _repProfile;

        public MatchController(IMatchRepository rep, ILogger<MatchController> logger, 
            UserManager<IdentityUser> userManager, ApplicationDbContext context,
            IUserProfileRepository repProfile)
        {
            _rep = rep;
            _logger = logger;
            _userManager = userManager;
            _context = context;
            _repProfile = repProfile;
        }

        [HttpPost]
        [Route("like")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> Like(LikeDto dto)
        {
            if (_rep.LikeOrMutualLike(dto.User1Id, dto.User2Id) == 2)
            {
                return Ok(new Response
                {
                    Message = "The secret like was added",
                    Status = "OK"
                });
            }
            else if (_rep.LikeOrMutualLike(dto.User1Id, dto.User2Id) == 1)
            {
                return Ok(new Response
                {
                    Message = "It's a Match ❤️",
                    Status = "Nice"
                });
            }
            else
            {
                return BadRequest(new Response
                {
                    Message = "There was an error trying to get the user",
                    Status = "Not Ok"
                });
            }
        }
        [HttpPost]
        [Route("like-with")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> LikeWith(LikeWithDto dto)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
            if(userId is not null)
            {
                var pid = _repProfile.GetProfileById(userId)!.Id;

                if (_rep.LikeOrMutualLike(pid, dto.userToLike) == 2)
                {
                    return Ok(new Response
                    {
                        Message = "The secret like was added",
                        Status = "OK"
                    });
                }
                else if (_rep.LikeOrMutualLike(pid, dto.userToLike) == 1)
                {
                    return Ok(new Response
                    {
                        Message = "It's a Match ❤️",
                        Status = "Nice"
                    });
                }
                return BadRequest(new Response
                {
                    Message = "There was an error trying to get the user",
                    Status = "Not Ok"
                });
            }
            
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost]
        [Route("unmatch")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> UnMatch([FromQuery] int id)
        {
            var user = await _userManager.FindByIdAsync(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value);
            var currentProfile = _context.Profile.Where(p => p.UserId == user.Id).FirstOrDefault();
            if (currentProfile != null) 
            {
                var matchToRemove = _rep.GetByUsersId(currentProfile.Id, id);
                if (matchToRemove is not null)
                {
                    _rep.Delete(matchToRemove.id);
                    return Ok(new Response
                    {
                        Message = "The match was disgregated successfully",
                        Status = "OK"
                    });
                }
                else
                {
                    return NotFound(new Response
                    {
                        Message = "Catastrophic error, the memory can't be _read 0x0000433",
                        Status = "Not Found"
                    });
                }

            }
            else
            {
                return NotFound(new Response
                {
                    Message = "User hasn't even bearly updated his/her profile",
                    Status = "Not Found"
                });
            }
            
        }

        [HttpGet]
        [Route("my-matches")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetAllMyMatches()
        {
            var user = await _userManager.FindByIdAsync(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value);
            var profile = _context.Profile.Where(p => p.UserId == user.Id).FirstOrDefault();

            return Ok(_rep.GetAllMyMatches(profile.Id));
        }

    }
}
