using buddy_up.Controllers;
using buddyUp.Data;
using buddyUp.DTOs;
using buddyUp.Models;
using Dapper;
using Geolocation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data;

namespace buddyUp.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<TagsController> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IUserProfileRepository _repository;
        private IConfiguration _configuration;

        public HomeController(UserManager<IdentityUser> userManager, ILogger<TagsController> logger,  ApplicationDbContext context, IUserProfileRepository repository, IConfiguration configuration)
        {
            _userManager = userManager;
            _logger = logger;
            _context = context;
            _repository = repository;
            _configuration = configuration;
        }

        [HttpGet]
        [Route("buddyup-curated")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> BuddyUpCurated()
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
            if (userId is not null)
            {
                var profile = _repository.GetProfileById(userId);
                if(profile is not null)
                {
                    var profiles = _repository.GetSelectionOfProfiles(profile.Id);
                    List<ProfileViewDto> profile_view = GetProfileView(profile, profiles);
                    return Ok(profile_view);
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return BadRequest();
            }

            
        }
        [HttpGet]
        [Route("buddyup-curated-one")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> OneRandomPosibleFriend()
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
            if (userId is not null)
            {
                var profile = _repository.GetProfileById(userId);
                if (profile is not null)
                {
                    var profiles = _repository.GetOnePosibleFriend(profile.Id);
                    List<ProfileViewDto> profile_view = GetProfileView(profile, profiles);
                    return Ok(profile_view);
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return BadRequest();
            }


        }
        private List<ProfileViewDto> GetProfileView(Profile? profile, IEnumerable<ProfileIntermediateDto> profiles)
        {
            List<ProfileViewDto> profile_view = new List<ProfileViewDto>();
            Coordinate thisUserCoordinate = GetCoordsProfile(profile.Id);
            foreach (var intermediateP in profiles)
            {
                var currentCoordinate = GetCoordsProfile(intermediateP.pid);
                
                var new_p_view = new ProfileViewDto
                {
                    id = intermediateP.pid,
                    user_id = intermediateP.user_id,
                    age = intermediateP.page,
                    quote = intermediateP.pquote,
                    bio = intermediateP.pbio,
                    gender = intermediateP.pgender,
                    name = intermediateP.pname,
                    tags = GetTagsOfProfile(intermediateP.pid),
                    photos = _repository.GetImagesOfUser(intermediateP.pid).ToList()
                    //distance_in_km = (int) (intermediateP.pdistance / 1000 == 0 ? 1 : intermediateP.pdistance / 1000)
                };
                profile_view.Add(new_p_view);
            }

            return profile_view;
        }

        private List<string> GetTagsOfProfile(int id)
        {
            var procedureName = "public.get_tags_of_user";
            IEnumerable<TagNameDto> tags_name = new List<TagNameDto>();
            using (var connection = new NpgsqlConnection(_configuration["PostgreSql:ConnectionString"]))
            {
                tags_name = connection.Query<TagNameDto>(
                    procedureName,
                    new { profile_id = id },
                    commandType: CommandType.StoredProcedure);

            }
            //var tags_of_user = tags_dynamic.Cast<TagNameDto>();
            List<string> s_tags_of_user = new List<string>();
            foreach(var tag in tags_name)
            {
                s_tags_of_user.Add(tag.name);
            }            
            return s_tags_of_user;
        }

        private Coordinate GetCoordsProfile(int id)
        {
            var procedureName = "public.get_coords_of_user";
            //IEnumerable<dynamic> coords_dynamic;          
            IEnumerable<Coordinate> coords = new List<Coordinate>();
            using (var connection = new NpgsqlConnection(_configuration["PostgreSql:ConnectionString"]))
            {
                coords = connection.Query<Coordinate>(
                    procedureName,
                    new { profile_id = id },
                    commandType: CommandType.StoredProcedure);
                    
            }
            //var coords = coords_dynamic.Cast<CoordsIntermediate>();
            //string[] sCoords = coords.First().paprox_location.Split(", ");
            //Coordinate coordinate = new()
            //{
            //    Latitude = Double.Parse(sCoords[0], System.Globalization.CultureInfo.InvariantCulture),
            //    Longitude = Double.Parse(sCoords[1], System.Globalization.CultureInfo.InvariantCulture),
            //};
            return coords.First();
        }
    }
}
