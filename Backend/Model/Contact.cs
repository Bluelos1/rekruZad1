using System.ComponentModel.DataAnnotations;
using Backend.Model.Enums;

namespace Backend.Model;

public class Contact
{
    [Key]
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; } 
    public string Password { get; set; } 
    public Category Category { get; set; } 
    public SubCategory Subcategory { get; set; } 
    public string PhoneNumber { get; set; } 
    public DateTime BirthDate { get; set; }
}