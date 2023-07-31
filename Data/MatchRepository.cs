using buddyUp.DTOs;
using buddyUp.Models;

namespace buddyUp.Data
{
    public class MatchRepository : IMatchRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        public MatchRepository(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        public Match Add(Match match)
        {
            _context.Add(match);
            _context.SaveChanges();
            return match;
        }

        public int Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return 0;
                }
                Match match = _context.Match.Find(id)!;
                if (match is null)
                {
                    return 0;
                }
                _context.Match.Remove(match);
                _context.SaveChanges();
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public IEnumerable<Match> GetAll()
        {
            return _context.Match.ToList();
        }
        public IEnumerable<MatchOutDto> GetAllMyMatches(int id)
        {
            var rawMatches = _context.Match.Where(m => m.isMatch == true && (m.userp1_id == id || m.userp2_id == id));
            List<MatchOutDto> procesedMatches = new List<MatchOutDto>();
            foreach(var rawMatch in rawMatches)
            {
                procesedMatches.Add(new MatchOutDto
                {
                    id = rawMatch.id,
                    matchedUserPid = rawMatch.userp1_id == id ? rawMatch.userp2_id : rawMatch.userp1_id 
                });
            }
            return procesedMatches;
        }
        public Match? GetById(int id)
        {
            return _context.Match.Where(m => m.id == id).FirstOrDefault();
        }

        public Match? GetByUsersId(int user1_id, int user2_id)
        {           
            var match = _context.Match.Where(m => (m.userp1_id == user1_id || m.userp2_id == user1_id) && (m.userp1_id == user2_id || m.userp2_id == user2_id)).FirstOrDefault();
            return match;
        }

        public int Update(int id, Match match)
        {
            var theMatch = _context.Match.Where(m => m.id == id).FirstOrDefault();
            if (theMatch is not null)
            {
                theMatch.id = match.id;
                theMatch.userp1_id = match.userp1_id;
                theMatch.userp2_id = match.userp2_id;
                int cambios = _context.SaveChanges();
                return cambios;
            }
            else
            {
                return 0;
            }
        }

        public int LikeOrMutualLike(int likedBy, int liked)
        {
            var match = GetByUsersId(likedBy, liked);
            if(match is not null)
            {
                match.isMatch = true;
                _context.SaveChanges();
                return 1;
            }
            else
            {
                //if(_context.Match.Any(m => m.userp1_id == likedBy || m.userp1_id == liked))//es el mismo userp1_id, porque no se sabe el orden pero así aseguramos
                //{
                //    return 3;
                //}
                match = new()
                {
                    isMatch = false,
                    userp1_id = likedBy,
                    userp2_id = liked
                };
                Add(match);
                return 2;
            }            
        }
      
    }
}
