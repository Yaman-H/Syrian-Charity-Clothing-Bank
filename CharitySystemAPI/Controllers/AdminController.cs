using CharitySystem.Application.DTOs.Families;
using CharitySystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CharitySystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IFamilyService _familyService;
        private readonly IOrderService _orderService;

        public AdminController(IFamilyService familyService, IOrderService orderService)
        {
            _familyService = familyService;
            _orderService = orderService;
        }

        [HttpGet("all-families")]
        public async Task<IActionResult> GetAllFamilies()
        {
            try
            {
                var families = await _familyService.GetAllFamiliesAsync();
                return Ok(families);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });            
            }
        }

        [HttpPost("create-family")]
        public async Task<IActionResult> CreateFamily([FromBody] CreateFamilyDto dto)
        {
            try
            {
                await _familyService.CreateFamilyAsync(dto);
                return Ok(new { message = "Family Created successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("edit-family")]
        public async Task<IActionResult> EditFamily(int familyId, [FromBody] UpdateFamilyDto dto)
        {
            try
            {
                await _familyService.UpdateFamilyAsync(familyId, dto);
                return Ok(new { message = "Family updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("delete-family/{familyId}")]
        public async Task<IActionResult> DeleteFamily(int familyId)
        {
            try
            {
                await _familyService.ArchiveAndDeleteFamilyAsync(familyId);
        
                return Ok(new { message = $"Family {familyId} deleted and archived successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("add-member")]
        public async Task<IActionResult> AddMember([FromBody] AddMemberDto dto)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null) return Unauthorized();

            try 
            {
                await _familyService.AddFamilyMemberAsync(dto);
                return Ok(new { message = "Member added successfully"});
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("delete-member/{id}")]
        public async Task<IActionResult> DeleteMember(int id)
        {
            try
            {
                await _familyService.DeleteFamilyMemberAsync(id);
                return Ok(new { message = "Member deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("reset-points")]
        public async Task<IActionResult> ManualResetPoints()
        {
            try
            {
                await _familyService.ResetAllFamilyPointsAsync(); 
                return Ok(new { message = "Points have Reset." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("archived-files")]
        public IActionResult GetArchivedFiles()
        {
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "ArchivedFamilies");

            if (!Directory.Exists(folderPath))
            {
                return Ok(new List<string>());
            }

            var dirInfo = new DirectoryInfo(folderPath);
            var files = dirInfo.GetFiles("*.csv")
                .OrderByDescending(f => f.CreationTime) 
                .Select(f => new 
                { 
                    FileName = f.Name, 
                    CreatedDate = f.CreationTime.ToString("yyyy-MM-dd HH:mm"),
                    Size = $"{f.Length / 1024} KB"
                })
                .ToList();

            return Ok(files);
        }

        [HttpGet("download-archive/{fileName}")]
        public IActionResult DownloadArchivedFile(string fileName)
        {
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "ArchivedFamilies");
            string filePath = Path.Combine(folderPath, fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(new { message = "File not found" });
            }

            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            // الـ File Result رح تتكفل بإغلاق الـ Stream تلقائياً بعد ما التحميل يخلص
            return File(stream, "text/csv", fileName);
        }
    }
}
