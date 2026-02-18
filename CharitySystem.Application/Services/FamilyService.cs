using CharitySystem.Application.DTOs.Families;
using CharitySystem.Application.DTOs.Orders;
using CharitySystem.Application.Interfaces;
using CharitySystem.Domain.Entities;
using CharitySystem.Domain.Enums;
using CharitySystem.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace CharitySystem.Application.Services
{
    public class FamilyService : IFamilyService
    {
        private readonly IUnitOfWork _unitOfWork;
        public FamilyService(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

        public async Task<List<FamilyProfileDto>> GetAllFamiliesAsync()
        {
            var families = await _unitOfWork.Families.GetQueryable()
                .Select(f => new FamilyProfileDto
                {
                    FamilyId = f.FamilyID,
                    FullName = f.User.Fullname,
                    Username = f.User.Username,
                    FamilyCode = f.FamilyCode,
                    FamilyPoints = f.FamilyPoints,
                    FamilyAddress = f.FamilyAddress,
                    PhoneNumber = f.User.PhoneNumber,
                    AccountStatus = f.User.AccountStatus,
                    FamilyNotes = f.FamilyNotes,
                    Members = f.Members.Select(m => new FamilyMemberDto
                    {
                        MemberID = m.MemberID,
                        FullName = m.FullName,
                        Age = m.Age,
                        Gender = m.Gender
                    }).ToList() 

                }).ToListAsync();

            return families;
        }

        public async Task<FamilyProfileDto> GetFamilyProfileAsync(int userId) 
        {
            var family = await _unitOfWork.Families.GetQueryable()
                .Where(f => f.UserID == userId)
                .Select(f => new
                {
                    f.FamilyID,
                    f.User.Fullname,
                    f.User.Username,
                    f.User.PhoneNumber,
                    f.FamilyCode,
                    f.FamilyPoints,
                    f.FamilyAddress
                }).FirstOrDefaultAsync();

            if (family == null) throw new Exception("Family not found");

            return new FamilyProfileDto
            {
                FullName = family.Fullname,
                FamilyCode = family.FamilyCode,
                FamilyPoints = family.FamilyPoints,
                FamilyAddress = family.FamilyAddress,
                PhoneNumber = family.PhoneNumber
            };
        }

        public async Task<List<OrderResponseDto>> GetFamilyOrdersAsync(int userId)
        {
            var family = (await _unitOfWork.Families.FindAsync(f => f.UserID == userId)).FirstOrDefault();
            if (family == null) throw new Exception("Family not found");

            var orders = await _unitOfWork.Orders.GetQueryable()
                .Where(o => o.FamilyID == family.FamilyID)
                .Include(o => o.OrderDetails).ThenInclude(od => od.Cloth)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return [.. orders.Select(o => new OrderResponseDto 
            { 
                OrderId = o.OrderID,
                Status = o.OrderStatus,
                OrderDate = o.OrderDate,
                TotalPoints = o.TotalPoints,
                Items = [.. o.OrderDetails.Select(i => new OrderDetailResponseDto { ClothName = i.Cloth?.ClothName ?? "-", Quantity = i.Quantity })]
            })];
        }

        public async Task ResetAllFamilyPointsAsync(int pointsPerMember = 50, int basePoints = 50)
        {
            var families = await _unitOfWork.Families.GetQueryable()
                                           .Include(f => f.Members) 
                                           .ToListAsync();

            foreach (var family in families)
            {
                int count = family.Members != null ? family.Members.Count : 1;
                family.FamilyPoints = basePoints + (count * pointsPerMember);
            }

            _unitOfWork.Families.UpdateRange(families);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ArchiveAndDeleteFamilyAsync(int familyId)
        {
            var family = await _unitOfWork.Families.GetQueryable()
                .Include(f => f.Orders)
                    .ThenInclude(o => o.OrderDetails)
                        .ThenInclude(od => od.Cloth)
                .Include(f => f.User)
                .FirstOrDefaultAsync(f => f.FamilyID == familyId);

            if (family == null) throw new Exception($"Family not found");

            var csvContent = new StringBuilder();
            csvContent.AppendLine("FamilyCode,FamilyName/User,Address,FamilyPoints,OrderDate,ItemName,Quantity");

            if (family.Orders != null && family.Orders.Any())
            {
                foreach (var order in family.Orders)
                {
                    if (order.OrderDetails != null)
                    {
                        foreach (var item in order.OrderDetails)
                        {
                            var clothName = item.Cloth?.ClothName ?? "Deleted Item";
                            var username = family.User?.Username ?? "Unknown";
                            csvContent.AppendLine($"{family.FamilyCode},{username},{family.FamilyAddress},{family.FamilyPoints},{order.OrderDate},{clothName},{item.Quantity}");
                        }
                    }
                }
            }
            else
            {
                var username = family.User?.Username ?? "Unknown";
                csvContent.AppendLine($"{family.FamilyCode},{username},{family.FamilyAddress},{family.FamilyPoints},No Orders,-,-");
            }

            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "ArchivedFamilies");
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            string fileName = $"{family.User?.Username}_{family.FamilyCode}_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            string filePath = Path.Combine(folderPath, fileName);

            await File.WriteAllTextAsync(filePath, csvContent.ToString(), new UTF8Encoding(true));

            if (family.User != null)
            {
                _unitOfWork.Users.Delete(family.User);
            }
            else 
            {
                 _unitOfWork.Families.Delete(family);
            }
    
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task CreateFamilyAsync(CreateFamilyDto dto)
        {
            string generatedCode = await GenerateFamilyCode();
            var userCount = await _unitOfWork.Users.GetQueryable()
                                 .CountAsync(u => u.Username == dto.Username);
            if (userCount > 0)
                throw new Exception("Username already exist");

            var newUser = new User
            {
                Fullname = dto.FullName,
                Username = dto.Username.ToLower(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password), 
                Gender = dto.Gender,
                Role = UserRole.Family,
                PhoneNumber = dto.PhoneNumber,
                AccountStatus = AccountStatus.Active,
            };

            var newFamily = new Family
            {
                FamilyCode = generatedCode,
                FamilyAddress = dto.Address,
                FamilyPoints = dto.InitialPoints,
                FamilyNotes = dto.FamilyNote,
                User = newUser
            };
            await _unitOfWork.Families.AddAsync(newFamily);
            await _unitOfWork.SaveChangesAsync();
        }


        public async Task UpdateFamilyAsync(int familyId, UpdateFamilyDto dto)
        {
            var family = await _unitOfWork.Families.GetQueryable()
                                .Include(f => f.User)
                                .FirstOrDefaultAsync(f => f.FamilyID == familyId);

            if (family == null)
                throw new Exception($"Family not found: {familyId}");

            // Refresh Family table
            if (!string.IsNullOrWhiteSpace(dto.FamilyAddress))
                family.FamilyAddress = dto.FamilyAddress;

            if (dto.FamilyPoints >= 0)
                family.FamilyPoints = dto.FamilyPoints.Value;

            if (!string.IsNullOrWhiteSpace(dto.FamilyNotes))
                family.FamilyNotes = dto.FamilyNotes;

            // Rrefresh User table
            if (family.User != null)
            {
                if ((!string.IsNullOrWhiteSpace(dto.PhoneNumber)) && dto.PhoneNumber != family.User.PhoneNumber)
                {
                        var phoneExists = await _unitOfWork.Users.GetQueryable()
                            .CountAsync(u => u.PhoneNumber == dto.PhoneNumber && u.UserID != family.UserID);
            
                        if (phoneExists > 0) throw new Exception("Phone Number already used!");
            
                        family.User.PhoneNumber = dto.PhoneNumber;
                }

                if ((!string.IsNullOrWhiteSpace(dto.Username)) && dto.Username != family.User.Username)
                {
                    var userCount = await _unitOfWork.Users.GetQueryable()
                         .CountAsync(u => u.Username == dto.Username && u.UserID != family.UserID);

                    if (userCount > 0) throw new Exception("Username already used!");

                    family.User.Username = dto.Username;
                }

                if ((!string.IsNullOrWhiteSpace(dto.NewPassword)))
                {
                    family.User.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
                }

                if (dto.AccountStatus.HasValue)
                {
                    family.User.AccountStatus = dto.AccountStatus.Value;
                }
            }

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task AddFamilyMemberAsync(AddMemberDto dto)
        {
            var family = await _unitOfWork.Families.GetQueryable()
                .Include(f => f.User)
                .FirstOrDefaultAsync(f => 
                    f.FamilyCode == dto.FamilyCode &&
                    f.User.PhoneNumber == dto.PhoneNumber 
                );

            if (family == null) 
                throw new Exception($"Phone number or family code is incorrect!");

            var memberExists = await _unitOfWork.FamilyMembers.GetQueryable()
                .CountAsync(m => m.FullName == dto.FullName && m.FamilyID == family.FamilyID);
            
            if (memberExists > 0)
                throw new Exception($"Family member with the same name already exists!");

            var newMember = new FamilyMember
            {
                FullName = dto.FullName,
                Gender = dto.Gender,
                Age = dto.Age,
                FamilyID = family.FamilyID
                
            };

            await _unitOfWork.FamilyMembers.AddAsync(newMember);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteFamilyMemberAsync(int memberId)
        {
            var member = await _unitOfWork.FamilyMembers.GetByIdAsync(memberId);
            if (member == null) throw new Exception("Member not found");

            _unitOfWork.FamilyMembers.Delete(member);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<string> GenerateFamilyCode()
        {
            string generatedCode;
            bool isUnique = false;
            do 
            {
                int randomNum = new Random().Next(10000, 99999);
                generatedCode = $"FAM-{randomNum}";
        
                var codeCount = await _unitOfWork.Families.GetQueryable()
                    .CountAsync(f => f.FamilyCode == generatedCode);
        
                if (codeCount == 0) isUnique = true;
            } while (!isUnique);

            return generatedCode;
        }
    }
}
