using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImageUploadController : Controller
{
   private readonly IWebHostEnvironment _webHostEnvironment;
   // this controller will have one endpoint and listen to only post method 

   public ImageUploadController(IWebHostEnvironment webHostEnvironment)
   {
      _webHostEnvironment = webHostEnvironment;
   }
    // todo : fix where we can only call this post after we click create category button not when we upload the image . 
    // todo : display the image when we choose it form the system file
    
   // the post listen method
   [HttpPost]
   public async Task<IActionResult> Post([FromBody] UploadedImage uploadedImage)
   {
      try
      {
         if (!ModelState.IsValid)
         {
            // if the model was invalid
            return BadRequest(ModelState);
         }

         // if we already have an image then we delete it 
         if (uploadedImage.OldImagePath != string.Empty)
         {
            if (uploadedImage.OldImagePath != "uploads/placeholder.jpg")
            {
               // that means we have an old image not the default one and we have to remove it
               
               //get the file name ex : placeholder.jpg
               string oldImageUploadFileName = uploadedImage.OldImagePath.Split('/').Last(); // wwwroot/uploads/image.png => image.png
               // delete the image from the file 
               System.IO.File.Delete($"{_webHostEnvironment.WebRootPath}\\wwwroot\\uploads\\{oldImageUploadFileName}");
            }
         }

         string guid = Guid.NewGuid().ToString();
         string imageFileName = guid + uploadedImage.NewImageFileExtension;

         string fullImageFileSystemPath = $"{_webHostEnvironment.ContentRootPath}\\wwwroot\\uploads\\{imageFileName}";

         FileStream fileStream = System.IO.File.Create(fullImageFileSystemPath);
         byte[] imageContentAsByteArray = Convert.FromBase64String(uploadedImage.NewImageBase64Content);
         await fileStream.WriteAsync(imageContentAsByteArray, 0, imageContentAsByteArray.Length);
         fileStream.Close();

         string relativeFilePathWithoutTrailingSlashes = $"uploads/{imageFileName}";
         return Created("Create", relativeFilePathWithoutTrailingSlashes);
         
      }
      catch (Exception e)
      {
         return StatusCode(500,$"something wrong happen on our side please contact the administrator.{e.Message}");
      }
   }
}