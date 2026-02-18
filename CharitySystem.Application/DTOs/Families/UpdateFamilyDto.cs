using System.ComponentModel.DataAnnotations;
using CharitySystem.Domain.Enums; 

public class UpdateFamilyDto
{
    public string? FamilyAddress { get; set; }
    public int? FamilyPoints { get; set; }
    public string? FamilyNotes { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Username { get; set; } 
    public string? NewPassword { get; set; } 
    public AccountStatus? AccountStatus { get; set; }
}