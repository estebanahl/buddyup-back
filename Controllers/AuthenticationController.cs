using buddy_up.Controllers;
using buddyUp.Data;
using buddyUp.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace buddyUp.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<TagsController> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IUserProfileRepository _repository;
        private IConfiguration _configuration;
        //private readonly JwtConfig _jwtConfig;

        public AuthenticationController(UserManager<IdentityUser> userManager, ILogger<TagsController> logger, IConfiguration configuration, RoleManager<IdentityRole> roleManager, ApplicationDbContext context, IUserProfileRepository repository)
        {
            _userManager = userManager;
            _logger = logger;
            _configuration = configuration;
            _roleManager = roleManager;
            _context = context;
            _repository = repository;
            //_jwtConfig = jwtConfig;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDto requestDto)
        {
            // validate the incoming request
            try
            {
                if (ModelState.IsValid)
                {
                    // does the mf user exists?
                    var user_exist = await _userManager.FindByNameAsync(requestDto.Email);
                    if (user_exist is not null)
                    {
                        return BadRequest(new AuthResult()
                        {
                            Result = false,
                            Errors = new List<string>()
                            {
                                "Email already exist"
                            },
                        });
                    }
                    var user = new IdentityUser()
                    {                    
                        Email = requestDto.Email,
                        UserName = requestDto.Email
                    };                

                    var is_created = await _userManager.CreateAsync(user, requestDto.Password);
               
                    if (is_created.Succeeded)
                    {
                        if (await _roleManager.RoleExistsAsync(UserRoles.User))
                        {
                            await _userManager.AddToRoleAsync(user, UserRoles.User);
                        }
                        else
                        {
                            await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));
                            await _userManager.AddToRoleAsync(user, UserRoles.User);
                        }
                        _repository.SetName(_userManager.FindByEmailAsync(requestDto.Email).Result.Id, 
                            requestDto.FullName);
                        var token = GenerateJwtToken(user);
                        return Ok(new AuthResult()
                        {
                            Result = true,
                            Token = token
                        });
                    }
                    else
                    {
                        return BadRequest(new AuthResult()
                        {
                            Errors = new List<string>()
                            {
                                "Server Error",
                            },
                            Result = false
                        });
                    }
                }
                else
                {
                    return BadRequest();
                }
            }catch(Exception ex)
            {
                return new JsonResult(ex);
            }
        }

        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDto loginRequest)
        {
            if (ModelState.IsValid)
            {
                var existing_user = await _userManager.FindByEmailAsync(loginRequest.Email);

                if(existing_user is null)
                {
                    return BadRequest(new AuthResult()
                    {
                        Errors = new List<string>
                    {
                        "Invalid payload"
                    },
                        Result = false
                    });
                }

                var isCorrect = _userManager.CheckPasswordAsync(existing_user, loginRequest.Password);
                if (!isCorrect.Result)
                {
                    return BadRequest(new AuthResult()
                    {
                        Errors = new List<string>
                    {
                        "Invalid credentials"
                    },
                        Result = false
                    });
                }

                var jwtToken = GenerateJwtToken(existing_user);
                return Ok(new AuthResult()
                {
                    Token = jwtToken,
                    Result = true
                });
            }
            return BadRequest(new AuthResult()
            {
                Errors = new List<string>
                {
                    "Invalid payload"
                },
                Result = false
            });
        }
        
        [Route("google-login")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleResponse([FromBody] ExternalProviderRetrieve gData)
        {
            return await ExternProviderLogin(gData);
        }
        [Route("facebook-login")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> FacebookResponse([FromBody] ExternalProviderRetrieve fData)
        {
            return await ExternProviderLogin(fData);
        }

        private async Task<IActionResult> ExternProviderLogin(ExternalProviderRetrieve ePData)
        {
            try
            {   // validate the incoming request
                if (ModelState.IsValid)
                {
                    // does the mf user exists?
                    var user_exist = await _userManager.FindByEmailAsync(ePData.email);
                    IdentityUser thisUser;
                    if (user_exist is not null)
                    {                                            
                        _repository.SetName(user_exist.Id, ePData.name);
                        var token = GenerateJwtToken(user_exist);
                        return Ok(new AuthResult
                        {
                            Result = true,
                            Token = token
                        });
                    }
                    thisUser = new IdentityUser()
                    {
                        Email = ePData.email,                        
                        UserName = ePData.email
                    };

                    var is_created = await _userManager.CreateAsync(thisUser);

                    if (is_created.Succeeded)
                    {
                        if (await _roleManager.RoleExistsAsync(UserRoles.User))
                        {
                            await _userManager.AddToRoleAsync(thisUser, UserRoles.User);
                        }
                        else
                        {
                            await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));
                            await _userManager.AddToRoleAsync(thisUser, UserRoles.User);
                        }
                        var newUser = _userManager.FindByEmailAsync(thisUser.Email);
                        _repository.SetName(newUser.Result.Id, ePData.name);
                        var token = GenerateJwtToken(thisUser);
                        return Ok(new AuthResult()
                        {
                            Result = true,
                            Token = token
                        });
                    }
                    else
                    {
                        return BadRequest(new AuthResult()
                        {
                            Errors = new List<string>()
                            {
                                "Server Error"
                            },
                            Result = false
                        });
                    }
                }
                else
                {
                    return BadRequest();
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new AuthResult()
                {
                    Errors = new List<string>
                        {
                        "Invalid payload"
                        },
                    Result = false
                });
            }
        }

        //[AllowAnonymous]
        //[Route("google-login")]
        //[HttpGet]
        //public IActionResult GoogleLogin()
        //{
        //    var properties = new AuthenticationProperties
        //    {
        //        RedirectUri = Url.Action("GoogleResponse")
        //    };
        //    return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        //}

        //[Route("google-login")]
        //[HttpPost]
        //[AllowAnonymous]
        //public async Task<IActionResult> GoogleResponse([FromBody] GoogleRetrieve gData)
        //{
        //    try
        //    {     // validate the incoming request
        //        if (ModelState.IsValid)
        //        {
        //            // does the mf user exists?
        //            var user_exist = await _userManager.FindByEmailAsync(gData.email);
        //            IdentityUser thisUser;
        //            if (user_exist is not null)
        //            {
        //                var token = GenerateJwtToken(user_exist);
        //                return Ok(new AuthResult{                           
        //                    Result = true,
        //                    Token = token
        //                });
        //            }
        //            thisUser = new IdentityUser()
        //            {
        //                Email = gData.email,
        //                UserName = gData.email
        //            };

        //            var is_created = await _userManager.CreateAsync(thisUser);



        //            if (is_created.Succeeded)
        //            {
        //                if (await _roleManager.RoleExistsAsync(UserRoles.User))
        //                {
        //                    await _userManager.AddToRoleAsync(thisUser, UserRoles.User);
        //                }
        //                else
        //                {
        //                    await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));
        //                    await _userManager.AddToRoleAsync(thisUser, UserRoles.User);
        //                }
        //                _repository.SetName(user_exist.Id, gData.name);
        //                var token = GenerateJwtToken(thisUser);
        //                return Ok(new AuthResult()
        //                {
        //                    Result = true,
        //                    Token = token
        //                });
        //            }
        //            else
        //            {
        //                return BadRequest(new AuthResult()
        //                {
        //                    Errors = new List<string>()
        //                    {
        //                        "Server Error"
        //                    },
        //                    Result = false
        //                });
        //            }
        //        }
        //        else
        //        {
        //            return BadRequest();
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new AuthResult()
        //            {
        //            Errors = new List<string>
        //                {
        //                "Invalid payload"
        //                },
        //            Result = false
        //        });
        //    }
        //}
        //[Route("google-response")]
        //[HttpGet]
        //[AllowAnonymous]
        //public async Task<IActionResult> GoogleResponse()
        //{
        //    try
        //    {
        //        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        //        return new JsonResult(result);
        //        var claims = result.Principal!.Identities.FirstOrDefault()!
        //            .Claims.Select(claim => new
        //            {
        //                claim.Issuer,
        //                claim.OriginalIssuer,
        //                claim.Type,
        //                claim.Value
        //            });



        //        LoginGDto dto = new LoginGDto
        //        {
        //            Issuer = claims.First().Issuer,
        //            Email = claims.Where(c => c.Type.Split("/").Last() == "emailaddress").First().Value,
        //            Name = claims.Where(c => c.Type.Split("/").Last() == "name").First().Value
        //        };
        //        var user = _context.Users.Where(u => u.Email == dto.Email).FirstOrDefault();
        //        string jwt = string.Empty;
        //        if (user is null)
        //        {
        //            // si no existe y se logueó a google, registrarlo
        //            user = new IdentityUser();
        //            user.Email = dto.Email;
        //            user.UserName = dto.Email;
        //            var isCreated = await _userManager.CreateAsync(user);

        //            if (isCreated.Succeeded)
        //            {
        //                if (await _roleManager.RoleExistsAsync(UserRoles.User))
        //                {
        //                    await _userManager.AddToRoleAsync(user, UserRoles.User);
        //                }
        //                else
        //                {
        //                    await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));
        //                    await _userManager.AddToRoleAsync(user, UserRoles.User);
        //                }
        //                var createdUser = _context.Users.Where(u => u.Email == dto.Email).FirstOrDefault();
        //                var profile = new Profile();
        //                profile.name = dto.Name;
        //                profile.UserId = createdUser.Id;
        //                await _context.Profile.AddAsync(profile);
        //                jwt = GenerateJwtToken(user);
        //                return Ok(new AuthResult()
        //                {
        //                    Token = jwt,
        //                    Result = true
        //                });// hay que redirigir a una ruta del front end
        //            }

        //        }
        //        jwt = GenerateJwtToken(user);

        //        return Ok(new AuthResult()
        //        {
        //            Token = jwt,
        //            Result = true
        //        });// hay que redirigir a una ruta del front end
        //    }
        //    catch (Exception _)
        //    {
        //        return new JsonResult(new { error = _.Message });
        //    }

        //}

        private string GenerateJwtToken(IdentityUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]);

            //token descriptor
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString())
                }),
                Expires = DateTime.Now.AddDays(14),
                NotBefore = DateTime.Now,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);
            return jwtToken;
        }
    }
    
}
