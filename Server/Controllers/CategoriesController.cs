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

    public CategoriesController(AppDataContext appDataContext)
    {
        _appDataContext = appDataContext;
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