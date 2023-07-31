using buddyUp.Data;
using buddyUp.DTOs;
using buddyUp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace buddyUp.Controllers
{
    [Route("api/photo")]
    [ApiController]
    public class PhotoController : ControllerBase
    {
        private IPhotoRepository _repo;
        private ILogger<PhotoController> _logger;

        public PhotoController(IPhotoRepository repo, ILogger<PhotoController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        [Route("test")]
        [HttpPost]
        public void SavePhoto(PhotoInputDto dto)
        {
            _repo.SavePhoto(dto);
        }

        [HttpGet]        
        public ActionResult<IEnumerable<Profile>>? Get()
        {
            return Ok(_repo.GetAll());
        }
        [HttpGet("{id}")]
        public ActionResult<Photo> Get(int id)
        {
            var photo = _repo.GetById(id);
            if (photo is null)
            {
                return NotFound($"Tag with id = {id} not found");
            }
            return Ok(photo);
        }


        [HttpPost]
        [Route("add")]
        public async Task<ActionResult<Photo?>> Post([FromBody] PhotoDto photo)
        {
            var thePhoto = new Photo()
            {
                Image = photo.Image,
                UserProfileId = photo.UserProfileId
                
            };
            _repo.Create(thePhoto);

            return Ok("The photo was added successfully");
        }
        //[HttpPut]
        //[Route("add")]
        //public async Task<ActionResult<Photo?>> Post([FromBody] PhotoDto photo)
        //{
        //    var thePhoto = new Photo()
        //    {
        //        Image = photo.Image,
        //        UserProfileId = photo.UserProfileId

        //    };
        //    _repo.Create(thePhoto);

        //    return Ok("The photo was added successfully");
        //}
    }
}
