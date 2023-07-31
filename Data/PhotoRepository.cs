using buddyUp.DTOs;
using buddyUp.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace buddyUp.Data
{
    public class PhotoRepository : IPhotoRepository
    {
        private readonly ApplicationDbContext _context;

        public PhotoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Photo Create(Photo photo)
        {
            _context.Add(photo);
            _context.SaveChanges();
            return photo;
        }

        public int DeleteById(int id)
        {
            _context.Photo.Remove(GetById(id));
            return _context.SaveChanges();
        }

        public int DeleteByUser(int id)
        {
            _context.Photo.RemoveRange(_context.Photo.Where(p => p.UserProfileId == id));
            return _context.SaveChanges();
        }

        public IEnumerable<Photo>? GetAll()
        {
            return _context.Photo.ToList();
        }

        public IEnumerable<Photo>? GetAllOfUser(int profileId)
        {
            return _context.Photo.Where(p => p.UserProfileId == profileId);
        }

        public Photo GetById(int id)
        {
            return _context.Photo.Find(id)!;
        }

        public void SavePhoto(PhotoInputDto photo)
        {
            System.IO.File.WriteAllBytes("D:/laimagenpurete.jpg", Convert.FromBase64String(photo.Image));
        }

        public int Update(int id, Photo photo)
        {
            var thePhoto = _context.Photo.Where(t => t.Id == id).FirstOrDefault();
            if (thePhoto is not null)
            {
                thePhoto.UserProfileId = photo.UserProfileId;
                thePhoto.Image = photo.Image;
                int cambios = _context.SaveChanges();
                return cambios;
            }
            else
            {
                return 0;
            }
        }
    }
}
