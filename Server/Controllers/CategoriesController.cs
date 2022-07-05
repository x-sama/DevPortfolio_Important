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
    [HttpGet]
    public async Task<IActionResult> Get ()
    {
        List<Category> categories = await _appDataContext.Categories.ToListAsync();

        return Ok(categories);
      
    }
}