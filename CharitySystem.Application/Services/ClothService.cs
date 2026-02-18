using AutoMapper;
using CharitySystem.Application.DTOs.Clothes;
using CharitySystem.Application.Interfaces;
using CharitySystem.Domain.Entities;
using CharitySystem.Domain.Enums;
using CharitySystem.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CharitySystem.Application.Services
{
    public class ClothService : IClothService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ClothService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ClothDto>> GetAllClothesAsync(bool onlyAvailable, ClothCategory? category = null)
        {
            var query = _unitOfWork.Clothes.GetQueryable(); 

            if (onlyAvailable)
            {
                query = query.Where(c => c.IsAvailable);
            }

            if (category.HasValue)
            {
                query = query.Where(c => c.Category == category.Value);
            }

            var clothes = await query.ToListAsync();

            return _mapper.Map<IEnumerable<ClothDto>>(clothes);
        }

        public async Task<ClothDto> GetClothByIdAsync(int id)
        {
            var cloth = await _unitOfWork.Clothes.GetByIdAsync(id);
            if (cloth == null) throw new Exception("Cloth not found");
            return _mapper.Map<ClothDto>(cloth);
        }

        public async Task AddClothAsync(CreateClothDto dto)
        {
            string imagePath = "";

            if (dto.Image != null && dto.Image.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "clothes");
        
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.Image.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.Image.CopyToAsync(fileStream);
                }

                imagePath = $"/uploads/clothes/{uniqueFileName}";
            }
            string generatedCode = await GenerateClothCode();
            var newCloth = new Cloth
            {
                ClothName = dto.ClothName,
                ClothCode = generatedCode,
                ClothSize = dto.ClothSize,
                Description = dto.Description,
                Category = dto.Category, 
                ClothPoints = dto.ClothPoints,
                ImageUrl = imagePath, 
                IsAvailable = true
            };
            await _unitOfWork.Clothes.AddAsync(newCloth);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<ClothDto> UpdateClothAsync(int id, UpdateClothDto dto)
        {
            var existingCloth = await _unitOfWork.Clothes.GetByIdAsync(id);
            if (existingCloth == null) throw new Exception("Cloth not found");

            if (!string.IsNullOrWhiteSpace(dto.ClothName)) 
                existingCloth.ClothName = dto.ClothName;

            if (!string.IsNullOrWhiteSpace(dto.ClothSize)) 
                existingCloth.ClothSize = dto.ClothSize;

            if (!string.IsNullOrWhiteSpace(dto.Description)) 
                existingCloth.Description = dto.Description;

            if (dto.ClothPoints >= 0) 
                existingCloth.ClothPoints = dto.ClothPoints;

            existingCloth.Category = dto.Category; 

            if (dto.Image != null && dto.Image.Length > 0)
            {
                if (!string.IsNullOrEmpty(existingCloth.ImageUrl))
                {
                     var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingCloth.ImageUrl.TrimStart('/'));
                     if (File.Exists(oldPath)) File.Delete(oldPath);
                }

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "clothes");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.Image.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.Image.CopyToAsync(fileStream);
                }

                existingCloth.ImageUrl = $"/uploads/clothes/{uniqueFileName}";
            }

            _unitOfWork.Clothes.Update(existingCloth);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ClothDto>(existingCloth);
        }

        public async Task<bool> DeleteClothAsync(int id)
        {
            var cloth = await _unitOfWork.Clothes.GetQueryable()
                             .FirstOrDefaultAsync(c => c.ClothID == id);
            if (cloth == null) return false;
            cloth.IsAvailable = false;
            _unitOfWork.Clothes.Update(cloth);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        
        public async Task<string> GenerateClothCode()
        {
            string generatedCode;
            bool isUnique = false;
            do 
            {
                int randomNum = new Random().Next(1000, 9999);
                generatedCode = $"CL-{randomNum}";
        
                var codeCount = await _unitOfWork.Clothes.GetQueryable()
                    .CountAsync(f => f.ClothCode == generatedCode);
        
                if (codeCount == 0) isUnique = true;
            } while (!isUnique);

            return generatedCode;
        }
    }
}
