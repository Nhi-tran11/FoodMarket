using Microsoft.AspNetCore.Mvc;
using Backend.Data;
using Microsoft.Extensions.Configuration;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileUploadController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public FileUploadController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if(file == null || file.Length == 0)
            {
                return BadRequest(new { error = "No file uploaded." });
            }
            
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = System.IO.Path.GetExtension(file.FileName).ToLowerInvariant();
            if(!allowedExtensions.Contains(extension))
            {
                return BadRequest(new { error = "Invalid file type. Only image files are allowed." });
            }
            
            if(file.Length > 5 * 1024 * 1024)
            {
                return BadRequest(new { error = "File size exceeds the 5MB limit." });
            }
            
            try
            {
                // Fix for null reference warning
                var currentDirectory = Directory.GetCurrentDirectory();
                var parentDirectory = Directory.GetParent(currentDirectory);
                
                if (parentDirectory == null)
                {
                    return StatusCode(500, new { error = "Unable to determine parent directory." });
                }
                
                var uploadFolder = System.IO.Path.Combine(
                    parentDirectory.FullName, 
                    "frontend", 
                    "food-market-frontend", 
                    "public", 
                    "images" 
                );
                
                if(!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }
                
                var fileName = $"{Guid.NewGuid()}{extension}";
                var fullPath = System.IO.Path.Combine(uploadFolder, fileName);

                // Save the file
                using(var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                
                // Return relative path (this will be stored in database)
                var relativePath = $"/images/{fileName}";
                return Ok(new { path = relativePath, message = "File uploaded successfully" });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", details = ex.Message });
            }
        }
    }
}