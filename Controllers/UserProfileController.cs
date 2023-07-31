using buddy_up.Controllers;
using buddyUp.Data;
using buddyUp.DTOs;
using buddyUp.Helpers;
using buddyUp.Models;
using Geolocation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;
using Dapper;
using System.Net;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace buddyUp.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileRepository _userRepository;
        //private readonly JwtService _jwtService;
        private readonly ILogger<TagsController> _logger;
        private readonly UserManager<IdentityUser> _userManager;       
        private readonly IConfiguration _configuration;

        public UserProfileController(IUserProfileRepository userRepository, ILogger<TagsController> logger, UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _userRepository = userRepository;
            //_jwtService = jwtService;
            _logger = logger;
            _userManager = userManager;
            _configuration = configuration;
        }


        [HttpPost]
        [Route("get-by-id-or-email")]
        //[AlloAnonymous]
        public ActionResult<ProfileSimple> GetByIdOrEmail([FromBody] OneStringDto dto)
        {
            try
            {
                if (dto.description is not null)
                {
                    return _userRepository.GetByIdOrEmail(dto.description)!;
                }
                else
                {
                    return BadRequest(new JsonResult(new
                    {
                        message = "The description was null"
                    }));
                }
            }        
            catch (Exception _)
            {
                return new JsonResult(_.Data);
            }
        }
        [HttpGet]
        [Route("get-by-pid")]
        public ActionResult<ProfileSimple> GetById([FromQuery] int? id)
        {
            try
            {
                if (id is null)
                {
                    var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
                    if (userId is not null)
                    {
                        return _userRepository.GetById(_userRepository.GetProfileById(userId)!.Id)!;
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
                else
                {
                    return _userRepository.GetById(id.Value)!;
                }
                
            }
            catch (Exception _)
            {
                return new JsonResult(_.Data);
            }
        }
        [HttpPut]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Route("bday")]
        public ActionResult<Response> UpdateBirthday([FromBody] BirthdayDto dto)
        {
            try
            {
                var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
                if (userId is not null)
                {
                    int cambios = _userRepository.SetBirthdayAndAge(userId, dto.birthday);
                    if (cambios == 1)
                    {
                        return Ok(new Response
                        {
                            Message = "The profile birthday was added/updated.",
                            Status = "OK"
                        });
                    }
                    else
                    {
                        return BadRequest(new Response
                        {
                            Message = "The birthay cannot be store.",
                            Status = "NOT OK"
                        });
                    }


                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception _)
            {
                return new JsonResult(new { error = _.Message });
            }
        }
        [HttpPut]
        [Route("name")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public ActionResult<Response> UpdateName([FromBody] OneStringDto dto)
        {
            try
            {
                var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
                if (userId is not null)
                {
                    int cambios = _userRepository.SetName(userId, dto.description);
                    if (cambios == 1)
                    {
                        return Ok(new Response
                        {
                            Message = "The name of the user was added/updated.",
                            Status = "OK"
                        });
                    }
                    else
                    {
                        return BadRequest(new Response
                        {
                            Message = "The name cannot be store.",
                            Status = "NOT OK"
                        });
                    }
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception _)
            {
                return new JsonResult(new { error = _.Message });
            }
        }
        [HttpPut]
        [Route("bio")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public ActionResult<Response> UpdateBio([FromBody] OneStringDto dto)
        {
            try
            {
                var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
                if (userId is not null)
                {
                    int cambios = _userRepository.SetBio(userId, dto.description);
                    if (cambios == 1)
                    {
                        return Ok(new Response
                        {
                            Message = "The bio of the user was added/updated.",
                            Status = "OK"
                        });
                    }
                    else
                    {
                        return BadRequest(new Response
                        {
                            Message = "The bio cannot be store.",
                            Status = "NOT OK"
                        });
                    }
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception _)
            {
                return new JsonResult(new { error = _.Message });
            }
        }
        [HttpPut]
        [Route("quote")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public ActionResult<Response> UpdateQuote([FromBody] OneStringDto dto)
        {
            try
            {
                var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
                if (userId is not null)
                {
                    int cambios = _userRepository.SetQuote(userId, dto.description);
                    if (cambios == 1)
                    {
                        return Ok(new Response
                        {
                            Message = "The quote of the user was added/updated.",
                            Status = "OK"
                        });
                    }
                    else
                    {
                        return BadRequest(new Response
                        {
                            Message = "The quote cannot be store.",
                            Status = "NOT OK"
                        });
                    }
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception _)
            {
                return new JsonResult(new { error = _.Message });
            }
        }
        [HttpPut]
        [Route("gender")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public ActionResult<Response> UpdateGender([FromBody] OneStringDto dto)
        {
            try
            {
                var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
                if (userId is not null)
                {
                    int cambios = _userRepository.SetGender(userId, dto.description);
                    if (cambios == 1)
                    {
                        return Ok(new Response
                        {
                            Message = "The gender of the user was ESTABLISHED in the db.",
                            Status = "OK"
                        });
                    }
                    else
                    {
                        return BadRequest(new Response
                        {
                            Message = "The gender cannot be store.",
                            Status = "NOT OK"
                        });
                    }
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception _)
            {
                return new JsonResult(new { error = _.Message });
            }
        }
        [HttpPut]
        [Route("tag")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public ActionResult<Response> UpdateTag([FromBody] List<TagDto> tags)
        {
            try
            {
                var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;

                if (userId is not null)
                {
                    int cambiosTablaAsociativa = _userRepository.SetTags(userId, tags);
                    if (cambiosTablaAsociativa == tags.Count() * -1) // los cambios los devuelve como opuesto aritmético no se por qué
                    {
                        return Ok(new Response
                        {
                            Message = $"All tags were added to the user",
                            Status = "OK"
                        }); 
                    }                    
                    else
                    {
                        return BadRequest(new Response
                        {
                            Message = $"Can't add a tag without other data in profile",
                            Status = "NOT OK"
                        });
                    }
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception _)
            {
                return new JsonResult(_.Message);
            }
        }

        [HttpPut]
        [Route("location")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public ActionResult<Response> UpdateLocation([FromBody] Coordinate coordinate)
        {
            try
            {
                var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;

                if (userId is not null)
                {
                    //string[] coordintatesParts = sCoordinates.Split(",");
                    //Coordinate coordinate = new()
                    //{
                    //    Latitude = Double.Parse(coordintatesParts[0]),
                    //    Longitude = Double.Parse(coordintatesParts[1])
                    //};
                    int cambiosPerifil = _userRepository.SetLocation(userId, coordinate);
                    if (cambiosPerifil ==  -1) 
                    {
                        return Ok(new Response
                        {
                            Message = $"The aproximated location were added to the user",
                            Status = "OK"
                        });
                    }
                    else
                    {
                        return BadRequest(new Response
                        {
                            Message = $"Can't add the geoloaction without other data in profile",
                            Status = "NOT OK"
                        });
                    }
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception _)
            {
                return new JsonResult(_.Message);
            }
        }
        [HttpPut]
        [Route("pref-distance")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public ActionResult<Response> UpdatePrefDistance([FromBody] PrefsDto dto)
        {
            try
            {
                var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;

                if (userId is not null)
                {         
                    int cambiosPerifil = _userRepository.SetDistancePreference(userId, dto.minimum, dto.maximum);
                    if (cambiosPerifil == 1)
                    {
                        return Ok(new Response
                        {
                            Message = $"The preferences for distance were added to the user",
                            Status = "OK"
                        });
                    }
                    else
                    {
                        return BadRequest(new Response
                        {
                            Message = $"Can't add the preferences",
                            Status = "NOT OK"
                        });
                    }
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception _)
            {
                return new JsonResult(_.Message);
            }
        }
        [HttpPut]
        [Route("pref-age")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public ActionResult<Response> UpdatePrefAge([FromBody] PrefsDto dto)
        {
            try
            {
                var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;

                if (userId is not null)
                {
                    int cambiosPerifil = _userRepository.SetAgePreference(userId, dto.minimum, dto.maximum);
                    if (cambiosPerifil == 1)
                    {
                        return Ok(new Response
                        {
                            Message = $"The preferences for age were added to the user",
                            Status = "OK"
                        });
                    }
                    else
                    {
                        return BadRequest(new Response
                        {
                            Message = $"Can't add the preferences",
                            Status = "NOT OK"
                        });
                    }
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception _)
            {
                return new JsonResult(_.Message);
            }
        }
    }
}

    