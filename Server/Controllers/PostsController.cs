using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Shared.Models;

namespace Server.Controllers;
[ApiController]
[Route("api/[controller]")]
public class PostsController : Controller
{
    private readonly AppDataContext _appDataContext;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IMapper _mapper;

    public PostsController(AppDataContext appDataContext , IWebHostEnvironment webHostEnvironment ,IMapper mapper)
    {
        _appDataContext = appDataContext;
        _webHostEnvironment = webHostEnvironment;
        _mapper = mapper;
    }

    #region Action Methods

    [HttpGet]
    public async Task<IActionResult> Get ()
    {
        List<Post> Posts = await _appDataContext.Posts.ToListAsync();

        return Ok(Posts);
      
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        Post Post = await GetPostsByPostId(id);
        return Ok(Post);
    }

    // create Post method
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PostDTO postDToToCreate)
    {
        try
        {
            if (postDToToCreate == null)
            {
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Post postToCreate = _mapper.Map<Post>(postDToToCreate);
            if (postToCreate.IsPublished)
            {
                postToCreate.PublishDate = DateTime.UtcNow.ToString("dd/MM/yyyy hh:mm");
            }

            await _appDataContext.Posts.AddAsync(postToCreate);
            bool changesPersistToDatabase = await PersistChangesToDatabase();
            if (!changesPersistToDatabase)
            {
                return StatusCode(500,
                    "Something went wrong from our side please contact administrator.");
            }
            else
            {
                return Created("Create", postToCreate);
            }
            
        }
        catch (Exception e)
        {
            return StatusCode(500,
                $"Something went wrong from our side please contact administrator. Error Message: {e.Message}.");
        }
    } 
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id,[FromBody] PostDTO? postToUpdateDTO)
    {
        try
        {
            if (id<1 || postToUpdateDTO == null || id != postToUpdateDTO.PostId)
            {
                return BadRequest(ModelState);
            }

            Post oldPost = await _appDataContext.Posts.FindAsync(id);
            if (oldPost == null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Post updatedPost = _mapper.Map<Post>(postToUpdateDTO);
            if (updatedPost.IsPublished) 
            {
                if (!oldPost.IsPublished)
                {
                    updatedPost.PublishDate = DateTime.UtcNow.ToString("dd/MM/yyyy hh:mm");
                }
                else
                {
                    updatedPost.PublishDate = oldPost.PublishDate;
                }
               
            }
            else
            {
                updatedPost.PublishDate = string.Empty;
            }
            
            //detached the oldPost from EF . else dont update
            _appDataContext.Entry(oldPost).State = EntityState.Detached;
            _appDataContext.Posts.Update(updatedPost);
            bool changesPersistToDatabase = await PersistChangesToDatabase();
            if (!changesPersistToDatabase)
            {
                return StatusCode(500,
                    "Something went wrong from our side please contact administrator.");
            }
            else
            {
                return Created("Create",updatedPost);
            }
            
        }
        catch (Exception e)
        {
            return StatusCode(500,
                $"Something went wrong from our side please contact administrator. Error Message: {e.Message}.");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            if (id<1)
            {
                return BadRequest(ModelState);
            }

            bool exists = await _appDataContext.Posts.AnyAsync(c => c.PostId == id);
            if (!exists)
            {
                return BadRequest(ModelState);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Post PostToDelete = await GetPostsByPostId(id);
            if (PostToDelete.ThumbnailImagePath != "uploads/placeholder.jpg")
            {
                string fileName = PostToDelete.ThumbnailImagePath.Split('/').Last();
                System.IO.File.Delete($"{_webHostEnvironment.ContentRootPath}\\wwwroot\\uploads\\{fileName}");
            }

            _appDataContext.Posts.Remove(PostToDelete);
            bool changesPersistToDatabase = await PersistChangesToDatabase();
            if (!changesPersistToDatabase)
            {
                return StatusCode(500,
                    "Something went wrong from our side please contact administrator.");
            }
            else
            {
                return NoContent();
            }
            
        }
        catch (Exception e)
        {
            return StatusCode(500,
                $"Something went wrong from our side please contact administrator. Error Message: {e.Message}.");
        }
    }
    #endregion

    #region Utilities

    [NonAction]
    [ApiExplorerSettings(IgnoreApi = true)]
    private async Task<bool> PersistChangesToDatabase()
    {
        int amountOfChanges = await _appDataContext.SaveChangesAsync();
        return amountOfChanges > 0;
    }
    [NonAction]
    [ApiExplorerSettings(IgnoreApi = true)]
    private async Task<Post> GetPostsByPostId(int PostId)
    {
        Post postToGet = await _appDataContext.Posts
            .Include(post => post.Category)
            .FirstAsync(c => c.PostId == PostId);
        return postToGet;
    }

    #endregion
    
}