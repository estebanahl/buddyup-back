
using Microsoft.EntityFrameworkCore;
using buddyUp.Models;
using Microsoft.AspNetCore.Identity;
using buddyUp.DTOs;
using System.Xml.Linq;
using System.Reflection;
using System.Data.SqlClient;
using Dapper;
using System.Data;
using Npgsql;
using Geolocation;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace buddyUp.Data
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        public UserProfileRepository(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        
        public IdentityUser? GetByEmail(string email)
        {
            return _context.ApplicationUsers.FirstOrDefault(u => u.Email == email);
        }

        public IdentityUser? GetById(string id)
        {
            return _context.ApplicationUsers.FirstOrDefault(u => u.Id == id);
        }
        public Profile? GetProfileById(string id)
        {
            return _context.Profile.FirstOrDefault(p => p.UserId == id);
        }

        public int SetBirthdayAndAge(string id, DateTime birthday)
        {
            
            if (_context.Users.Any(u => u.Id == id))
            {
                Profile? profile_exists = GetProfileById(id);
                if (profile_exists is null)
                {
                    profile_exists = new();
                    profile_exists.date_of_birth = birthday;
                    profile_exists.age = CalculateAge(birthday);
                    profile_exists.UserId = id;
                    _context.Profile.Add(profile_exists);
                    return _context.SaveChanges();                    
                }
                else
                {
                    profile_exists.date_of_birth = birthday;
                    profile_exists.age = CalculateAge(birthday.Date);
                    return _context.SaveChanges();
                }
               
            }else
            {
                return -1;
            }

        }
        public ProfileSimple? GetByIdOrEmail(string identifier)
        {
            var user = new ProfileSimple();
            using (var connection = new NpgsqlConnection(_config["PostgreSql:ConnectionString"]))
            {
                user = connection.Query<ProfileSimple>(
                    "public.get_u_by_id_or_email",
                    new { the_identifier = identifier },
                    commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
            if (user is not null)
            {
                user.tags = GetTagsOfUser(user.pid).ToList();
                user.images = GetImagesOfUser(user.pid).ToList();
                return user;
            }
            return null;
        }
        public int SetName(string id, string name)
        {
            if (_context.Users.Any(u => u.Id == id))
            {
                Profile? profile = GetProfileById(id);
                if (profile is null)
                {
                    profile = new Profile();
                    profile.name = name;
                    profile.UserId = id;
                    _context.Profile.Add(profile); 
                    return _context.SaveChanges();
                }
                else
                {
                    profile.name = name;
                    return _context.SaveChanges();
                }                
            }
            return -1;
        }
        public int SetBio(string id, string bio)
        {
            if (_context.Users.Any(u => u.Id == id))
            {
                Profile? profile = GetProfileById(id);
                if (profile is null)
                {
                    profile = new Profile();
                    profile.bio = bio;
                    profile.UserId = id;
                    _context.Profile.Add(profile);
                    return _context.SaveChanges();
                }
                else
                {
                    profile.bio = bio;
                    return _context.SaveChanges();
                }                
            }
            return -1;
        }

        public int SetTags(string id, List<TagDto> tags)
        {          
            var procedureName = "public.setusertags";
            int cambiosEnProfileTag = 0;
            Profile? profile = GetProfileById(id);
            if (profile is not null)
            { 
                using (var connection = new NpgsqlConnection(_config["PostgreSql:ConnectionString"]))
                {
                    foreach(var theTag in tags)
                    {
                        cambiosEnProfileTag += connection.Execute(
                            procedureName, 
                            new { tagid = theTag.id, profileid = profile.Id }, 
                            commandType: CommandType.StoredProcedure);
                    }                        
                }
                return cambiosEnProfileTag;
            }
            else
            {
                return -1;
            }

        }
        public int SetGender(string id, string genderStringSpanish)
        {           
            if (_context.Users.Any(u => u.Id == id))
            {
                Profile? profile = GetProfileById(id);
                if (profile is null)
                {
                    profile = new Profile();
                    profile.gender = CalculateGender(genderStringSpanish);
                    profile.UserId = id;
                    _context.Profile.Add(profile);
                    return _context.SaveChanges();
                }
                else
                {
                    profile.gender = CalculateGender(genderStringSpanish);
                    return _context.SaveChanges();
                }
            }
            return -1;                             
        }
        public int SetQuote(string id, string quote)
        {
            if (_context.Users.Any(u => u.Id == id))
            {
                Profile? profile = GetProfileById(id);
                if (profile is null)
                {
                   profile = new Profile();
                    profile.quote = quote;
                    profile.UserId = id;
                    _context.Profile.Add(profile);
                    return _context.SaveChanges();
                }
                else
                {
                    profile.quote = quote;
                    return _context.SaveChanges();
                }
            }
            return -1;
        }

        private int CalculateAge(DateTime anniversaire)
        {
            DateTime now = DateTime.Today;
            int age = now.Year - anniversaire.Year;
            if (anniversaire > now.AddYears(-age))
                age--;
            return age;
        }
        private string CalculateGender(string genderStringSpanish)
        {
            switch (genderStringSpanish)
            {
                case "Hombre":
                    return Gender.Male.ToString();
                case "Mujer":
                    return Gender.Female.ToString();
                case "No binario":
                    return Gender.NonBinary.ToString();
                case "Prefiero no decirlo":
                    return Gender.IRatherNotSay.ToString();
                default:
                    throw new Exception("Gender has not the proper format");
            }
        }

        public int SetLocation(string id, Coordinate coordinate)
        {
            if (_context.Users.Any(u => u.Id == id))
            {
                Profile? profile = GetProfileById(id);
                if (profile is not null)
                {                    
                    using (var connection = new NpgsqlConnection(_config["PostgreSql:ConnectionString"]))
                    {
                        
                        return connection.Execute(
                            "public.insert_location",
                            new { longitude_new = coordinate.Longitude, latitude_new = coordinate.Latitude, profile_id = profile.Id },
                            commandType: CommandType.StoredProcedure);
                    }                   
                }
                else
                {
                    return -1;
                }
            }
            return -1;
        }

        public int SetAgePreference(string id, int min, int max)
        {
            if (_context.Users.Any(u => u.Id == id))
            {
                Profile? profile = GetProfileById(id);
                if (profile is null)
                {
                    profile = new Profile();
                    profile.minimun_age = min;
                    profile.maximun_age = max;
                    _context.Profile.Add(profile);
                    return _context.SaveChanges();
                }
                else
                {
                    profile.minimun_age = min;
                    profile.maximun_age = max;
                    return _context.SaveChanges();
                }
            }
            return -1;
        }

        public int SetDistancePreference(string id, int min, int max)
        {
            if (_context.Users.Any(u => u.Id == id))
            {
                Profile? profile = GetProfileById(id);
                if (profile is null)
                {
                    profile = new Profile();
                    profile.minimun_distance = min;
                    profile.maximun_distance = max;
                    _context.Profile.Add(profile);
                    return _context.SaveChanges();
                }
                else
                {
                    profile.minimun_distance = min;
                    profile.maximun_distance = max;
                    return _context.SaveChanges();
                }
            }
            return -1;
        }

        public IEnumerable<ProfileIntermediateDto> GetSelectionOfProfiles(int id_perfil)
        {
            var procedureName = "public.get_view_for_users_final";
            IEnumerable<ProfileIntermediateDto> table = new List<ProfileIntermediateDto>(); 
            Profile? profile = _context.Profile.Where(p => p.Id == id_perfil).FirstOrDefault();
            using (var connection = new NpgsqlConnection(_config["PostgreSql:ConnectionString"]))
            {

                table = connection.Query<ProfileIntermediateDto>(
                    procedureName, new
                    {
                        age_minimum = profile.minimun_age == 0 ? 18 : profile.minimun_age,
                        age_maximum = profile.maximun_age == 0 ? 99 : profile.maximun_age,
                        //distance_minimum = profile.minimun_distance == 0 ? 1 : profile.minimun_distance * 1000,
                        //distance_maximum = profile.maximun_distance == 0 ? 50000000 : profile.maximun_distance * 1000,
                        current_profile = profile.Id
                    },
                    commandType: CommandType.StoredProcedure); ;
                    
            }
            return table;            
        }
        public IEnumerable<ProfileIntermediateDto> GetOnePosibleFriend(int id_perfil)
        {
            var procedureName = "public.get_one_posible_friend_final";
            IEnumerable<ProfileIntermediateDto> table = new List<ProfileIntermediateDto>();
            Profile? profile = _context.Profile.Where(p => p.Id == id_perfil).FirstOrDefault();
            using (var connection = new NpgsqlConnection(_config["PostgreSql:ConnectionString"]))
            {

                table = connection.Query<ProfileIntermediateDto>(
                    procedureName, new
                    {
                        age_minimum = profile.minimun_age == 0 ? 18 : profile.minimun_age,
                        age_maximum = profile.maximun_age == 0 ? 99 : profile.maximun_age,
                        //distance_minimum = profile.minimun_distance == 0 ? 1 : profile.minimun_distance * 1000,
                        //distance_maximum = profile.maximun_distance == 0 ? 50000000 : profile.maximun_distance * 1000,
                        current_profile = profile.Id
                    },
                    commandType: CommandType.StoredProcedure); ;

            }
            return table;
        }

        public ProfileSimple? GetById(int id)
        {
            ProfileSimple? profile = new();
            using (var connection = new NpgsqlConnection(_config["PostgreSql:ConnectionString"]))
            {
                profile = connection.Query<ProfileSimple>(
                       "public.get_profile_simple_by_identifier",
                       new { identifier = id },
                       commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
            if (profile is not null){
                profile.tags = GetTagsOfUser(id).ToList();
                profile.images = GetImagesOfUser(id).ToList();
                return profile;
            }
            return null;
        }
        public IEnumerable<Tag> GetTagsOfUser(int profileId)
        {
            IEnumerable<Tag> tags = new List<Tag>();
            using (var connection = new NpgsqlConnection(_config["PostgreSql:ConnectionString"]))
            {

                 tags = connection.Query<Tag>(
                        "public.get_tags_of_user",
                        new { profile_id = profileId},
                        commandType: CommandType.StoredProcedure);                
            }
            return tags;
        }
        public IEnumerable<PhotoViewDto> GetImagesOfUser(int profileId)
        {
            IEnumerable<PhotoViewDto> photos = new List<PhotoViewDto>();
            using (var connection = new NpgsqlConnection(_config["PostgreSql:ConnectionString"]))
            {

                photos = connection.Query<PhotoViewDto>(
                       "public.get_images_of_user",
                       new { identifier = profileId },
                       commandType: CommandType.StoredProcedure);
            }
            return photos;
        }


        //public User SetProfile(User user)
        //{
        //    var user = _users.Find(u => u.Id == ObjectId.Parse(id).ToString()).FirstOrDefault();
        //    if (user != null)
        //    {
        //        if (user.profile is null) new Profile();
        //        user.profile.tags = tags;

        //        //        user.profile.name = name;
        //        //        user.profile.bio = bio;
        //        //        user.profile.quote = quote;
        //        return _users.ReplaceOne(t => t.Id == ObjectId.Parse(id).ToString(), user);
        //    }
        //    return null;
        //}
    }

}
