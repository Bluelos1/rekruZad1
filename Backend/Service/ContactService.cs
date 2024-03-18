using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.DbContext;
using Backend.Interface;
using Backend.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Service;

public class ContactService : IContactService
{
    private readonly ContactDbContext _context;
    private readonly ILogger<ContactService> _logger;
    private IConfiguration _configuration;

    public ContactService(ContactDbContext context, ILogger<ContactService> logger, IConfiguration configuration)
    {
        _context = context;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<List<Contact>> GetAllContacts()
    {
        try
        {
            
            return await _context.ContactDb.ToListAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e,"No contacts");
            throw;
        }
    }

    public async Task<Contact> GetContact(int id)
    {
        try
        {
            return await _context.ContactDb.FirstOrDefaultAsync(x => x.Id == id);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "error");
            throw;
        }
        
    }

    public async Task UpdateContact(int id, Contact contact)
    {
        var item = await _context.ContactDb.FirstOrDefaultAsync(x => x.Id == id);
        if (item != null)
        {
            item.FirstName = contact.FirstName;
            item.LastName = contact.LastName;
            item.Email = contact.Email;
            item.Password = contact.Password; 
            item.Category = contact.Category;
            item.Subcategory = contact.Subcategory;
            item.PhoneNumber = contact.PhoneNumber;
            item.BirthDate = contact.BirthDate;
            
            _context.ContactDb.Update(item);
            await _context.SaveChangesAsync();
        }

    }

    public async Task DeleteContact(int id)
    {
        var item = await _context.ContactDb.FirstOrDefaultAsync(x => x.Id == id);
        if (item != null)
        {
            _context.ContactDb.Remove(item);
            await _context.SaveChangesAsync();
        }
    }

    public async Task UploadContact(Contact contact)
    {
        var hasher = new PasswordHasher<Contact>();
        contact.Password = hasher.HashPassword(contact, contact.Password);
        
        await _context.ContactDb.AddAsync(contact);
        await _context.SaveChangesAsync();
    }
    
    
    public async Task<Contact> RegisterUser(Contact user)
    {
        var existingUser = await _context.ContactDb.FirstOrDefaultAsync(x => x.Email == user.Email);
        if (existingUser != null)
        {
            throw new Exception("User already exists with the provided email.");
        }

        var hasher = new PasswordHasher<Contact>();
        user.Password = hasher.HashPassword(user, user.Password);

        await _context.ContactDb.AddAsync(user);
        await _context.SaveChangesAsync();

        return user;
    }
    
    public async Task<string> Authenticate(string email, string password)
    {
        var user = await _context.ContactDb.FirstOrDefaultAsync(x => x.Email == email);
        if (user == null || !(new PasswordHasher<Contact>().VerifyHashedPassword(user, user.Password, password) == PasswordVerificationResult.Success))
        {
            throw new Exception("Invalid credentials");
        }
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenFromAppsettings = _configuration.GetSection("SecretKey").Value;
        var key = Encoding.ASCII.GetBytes(tokenFromAppsettings); 
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[] 
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            }),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    
    
}