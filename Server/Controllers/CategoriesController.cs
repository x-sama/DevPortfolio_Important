using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Shared.Models;

namespace Server.Controllers;
[ApiController]
[Route("api/[controller]")]
public class CategoriesController : Controller
{
    private readonly AppDataContext _appDataContext;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public CategoriesController(AppDataContext appDataContext , IWebHostEnvironment webHostEnvironment)
    {
        _appDataContext = appDataContext;
        _webHostEnvironment = webHostEnvironment;
    }

    #region Action Methods

    [HttpGet]
    public async Task<IActionResult> Get ()
    {
        List<Category> categories = await _appDataContext.Categories.ToListAsync();

        return Ok(categories);
      
    }

    [HttpGet("withposts")]
    public async Task<IActionResult> GetWithPosts()
    {
        List<Category> categories = await _appDataContext.Categories
            .Include(category => category.Posts)
            .ToListAsync();
        return Ok(categories);
    }  
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        Category category = await GetCategoriesByCategoryId(id,false);
        return Ok(category);
    } 
    [HttpGet("withposts/{id}")]
    public async Task<IActionResult> GetWithPosts(int id)
    {
        Category category = await GetCategoriesByCategoryId(id,true);
        return Ok(category);
    }
    
    // create category method
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Category? categoryToCreate)
    {
        try
        {
            if (categoryToCreate == null)
            {
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _appDataContext.Categories.AddAsync(categoryToCreate);
            bool changesPersistToDatabase = await PersistChangesToDatabase();
            if (!changesPersistToDatabase)
            {
                return StatusCode(500,
                    "Something went wrong from our side please contact administrator.");
            }
            else
            {
                return Created("Create", categoryToCreate);
            }
            
        }
        catch (Exception e)
        {
            return StatusCode(500,
                $"Something went wrong from our side please contact administrator. Error Message: {e.Message}.");
        }
    } 
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id,[FromBody] Category? categoryToUpdate)
    {
        try
        {
            if (id<1 || categoryToUpdate == null || id != categoryToUpdate.CategoryId)
            {
                return BadRequest(ModelState);
            }

            bool exists = await _appDataContext.Categories.AnyAsync(c => c.CategoryId == id);
            if (!exists)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _appDataContext.Categories.Update(categoryToUpdate);
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

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            if (id<1)
            {
                return BadRequest(ModelState);
            }

            bool exists = await _appDataContext.Categories.AnyAsync(c => c.CategoryId == id);
            if (!exists)
            {
                return BadRequest(ModelState);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Category categoryToDelete = await GetCategoriesByCategoryId(id, false);
            // delete the category image from the image folder 
            if (categoryToDelete.ThumbnailImagePath != "uploads/placeholder.jpg")
            {
                string fileName = categoryToDelete.ThumbnailImagePath.Split('/').Last();
                System.IO.File.Delete($"{_webHostEnvironment.ContentRootPath}\\wwwroot\\uploads\\{fileName}");
            }
            
            _appDataContext.Categories.Remove(categoryToDelete);
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
    private async Task<Category> GetCategoriesByCategoryId(int categoryId,bool withPosts)
    {
        Category categoryToGet = null;
        if (withPosts)
        {
            categoryToGet = await _appDataContext.Categories
                .Include(category => category.Posts)
                .FirstAsync(c => c.CategoryId == categoryId);
          
        }
        else
        {
            categoryToGet = await _appDataContext.Categories
                .FirstAsync(c => c.CategoryId == categoryId);
        }
        return categoryToGet;
    }

    #endregion
    
}