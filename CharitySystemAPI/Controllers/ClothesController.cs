using CharitySystem.Application.DTOs.Clothes;
using CharitySystem.Application.Interfaces;
using CharitySystem.Domain.Entities;
using CharitySystem.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CharitySystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClothesController : ControllerBase
    {
        private readonly IClothService _clothService;

        public ClothesController(IClothService clothService)
        {
            _clothService = clothService;
        }

        [HttpGet("store-items")]
        public async Task<IActionResult> GetAllToStore([FromQuery] ClothCategory? category)
        {
            var result = await _clothService.GetAllClothesAsync(onlyAvailable: true, category);
            return Ok(result);
        }

        [HttpGet("inventory-items")]
        [Authorize(Roles = "WarehouseManager,Admin")]
        public async Task<IActionResult> GetAllToInventory([FromQuery] ClothCategory? category)
        {
            var result = await _clothService.GetAllClothesAsync(onlyAvailable: false, category);
            return Ok(result);
        }

        [HttpPost("add-cloth")]
        [Authorize(Roles = "WarehouseManager")]
        public async Task<IActionResult> AddCloth([FromForm] CreateClothDto dto)
        {
            Console.WriteLine(dto.Category);
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            try 
            {
                await _clothService.AddClothAsync(dto);
                return Ok(new { message = "Cloth added successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("update-cloth/{id}")]
        [Authorize(Roles = "WarehouseManager")]
        public async Task<IActionResult> UpdateCloth(int id, [FromForm] UpdateClothDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _clothService.UpdateClothAsync(id, dto);
                return Ok(new {message = "Cloth updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("delete-cloth")]
        [Authorize(Roles = "WarehouseManager")]
        public async Task<IActionResult> DeleteCloth(int id)
        {
            var result = await _clothService.DeleteClothAsync(id);
            return result ? Ok() : NotFound();
        }
    }
}
