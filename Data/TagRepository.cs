
using buddyUp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace buddyUp.Data
{
    public class TagRepository : ITagRepository
    {
        private readonly ApplicationDbContext _context;
        public TagRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Tag Create(Tag tag)
        {
            _context.Tag.Add(tag); // tiene que ser async?
            _context.SaveChanges();
            return tag;
        }
        public IEnumerable<Tag>? GetAll()
        {
            return _context.Tag.ToList();
        }
        public async Task<Tag?> GetById(int id)
        {
            return await _context.Tag.FirstOrDefaultAsync(t => t.Id == id);
        }
        public int Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return 0;
                }
                Tag tag = _context.Tag.Find(id)!;
                if (tag is null)
                {
                    return 0;
                }
                _context.Tag.Remove(tag);
                _context.SaveChanges();
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
            
        }

        public int Update(int id, Tag tag)
        {
            
            var theTag = _context.Tag.Where(t => t.Id == id).FirstOrDefault();
            if (theTag is not null)
            {
                theTag.name = tag.name;
                theTag.UsersWithInterest = tag.UsersWithInterest;
                int cambios = _context.SaveChanges();
                return cambios;
            }
            else
            {
                return 0;
            }
            
        }

        //public IEnumerable<Tag> GetByNameLike(string namelike)
        //{
        //    var entity = _tags.Find(new BsonDocument("$text", new BsonDocument
        //        {
        //            { "$search", "aprender" },
        //            { "$caseSensitive", false }
        //        }));
        //    return entity.ToEnumerable();
        //}
    }
}
