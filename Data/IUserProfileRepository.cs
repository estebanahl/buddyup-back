
using buddyUp.DTOs;
using buddyUp.Models;
using Geolocation;
using Microsoft.AspNetCore.Identity;

namespace buddyUp.Data
{
    public interface IUserProfileRepository
    {
        IdentityUser? GetByEmail(string email);
        IdentityUser? GetById(string id);
        ProfileSimple? GetById(int pid);
        ProfileSimple? GetByIdOrEmail(string identifier);
        IEnumerable<Tag> GetTagsOfUser(int pid);
        Profile? GetProfileById(string userId);
        int SetBirthdayAndAge(string id, DateTime birthday);
        int SetName(string id, string name);
        int SetBio(string id, string bio);
        int SetQuote(string id, string quote);
        int SetGender(string id, string quote);
        int SetTags(string id, List<TagDto> tags);
        int SetLocation(string id, Coordinate coordinate);
        int SetAgePreference(string id, int min, int max);
        int SetDistancePreference(string id, int min, int max);
        IEnumerable<ProfileIntermediateDto> GetSelectionOfProfiles(int id_perfil);
        IEnumerable<ProfileIntermediateDto> GetOnePosibleFriend(int id_perfil);
        IEnumerable<PhotoViewDto> GetImagesOfUser(int profileId);


    }
}
