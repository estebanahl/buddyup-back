using buddy_up.Controllers;
using buddyUp.Data;
using buddyUp.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace buddyUp.Controllers
{
    [Route("api/messages")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageRepository _mRepository;
        private readonly ILogger<MessagesController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public MessagesController(IMessageRepository mRepository, ILogger<MessagesController> logger, 
            UserManager<IdentityUser> userManager, IConfiguration configuration, 
            ApplicationDbContext context)
        {
            _mRepository = mRepository;
            _logger = logger;
            _userManager = userManager;
            _configuration = configuration;
            _context = context;
        }

        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> AddMessage(MessageDto dto)
        {
            if (_context.Match.Any(m => m.id == dto.chatId))
            {
                try
                {
                    var theMessage = new Models.Message
                    {
                        chatId = dto.chatId,
                        senderPId = dto.senderId,
                        text = dto.text,
                        timestamp = DateTime.UtcNow
                    };
                    _mRepository.addMessages(theMessage);
                    return Ok(new Response
                    {
                        Message = $"The message of {dto.senderId} was added",
                        Status = "OK"
                    });
                }
                catch(Exception ex)
                {
                    return BadRequest(ex.Message);
                }
               
            }
            else 
            {
                return NotFound(new Response
                { 
                    Status = "Not Found"
                });
            }           
        }
        [HttpGet]
        [Route("getmessages")]
        public async Task<IActionResult> GetMessages([FromQuery] int matchId)
        {
            if (_context.Match.Any(m => m.id == matchId))
            {
                try
                {
                    var messages = _mRepository.getMessagesByChat(matchId);
                    
                    return Ok(messages);
                } catch (Exception ex)
                {
                    return BadRequest($"{ex.Message}");
                }
             
            }
            else
            {
                return NotFound(new Response
                {
                    Status = "Not Found"
                });
            }
        }
    }
}
