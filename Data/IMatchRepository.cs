using buddyUp.DTOs;
using buddyUp.Models;

namespace buddyUp.Data
{
    public interface IMatchRepository
    {
        Match Add(Match match);
        IEnumerable<Match> GetAll();
        Match? GetByUsersId(int id1, int id2);
        Match? GetById(int id);
        int Update(int id, Match match);
        int Delete(int? id);
        int LikeOrMutualLike(int likedBy, int liked);
        IEnumerable<MatchOutDto> GetAllMyMatches(int id);
    }
}
