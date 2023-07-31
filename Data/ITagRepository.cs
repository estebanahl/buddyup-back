
using buddyUp.Models;

namespace buddyUp.Data
{
    public interface ITagRepository
    {
        Tag Create(Tag tag);
        IEnumerable<Tag>? GetAll();
        Task<Tag?> GetById(int id);
        int Update(int id, Tag tag);
        int Delete(int? id);
        //IEnumerable<Tag> GetByNameLike(string namelike);
    }
}
