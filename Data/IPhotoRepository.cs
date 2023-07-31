using buddyUp.DTOs;
using buddyUp.Models;

namespace buddyUp.Data
{
    public interface IPhotoRepository
    {
        void SavePhoto(PhotoInputDto photo);
        Photo Create(Photo photo);
        IEnumerable<Photo>? GetAll();
        IEnumerable<Photo>? GetAllOfUser(int profileId);
        Photo GetById(int id);
        int Update(int id, Photo photo);
        int DeleteByUser(int id);
        int DeleteById(int id);
    }
}
