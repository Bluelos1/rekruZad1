using System.Net;
using Backend.Interface;
using Backend.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class ContactController : ControllerBase
{
    private IContactService _contactService;

    public ContactController(ILogger<ContactController> logger, IContactService contactService)
    {
        _contactService = contactService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Contact>>> GetAllContacts()
    {
        var item = await _contactService.GetAllContacts();
        if (!item.Any())
        {
            return NotFound();
        }

        return Ok(item);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Contact>> GetContact(int id)
    {
        var item = await _contactService.GetContact(id);
        return Ok(item);
    }
    
    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult> PutContact(int id, Contact contact)
    {
        if (id != contact.Id)
        {
            return BadRequest();
        }

        try
        {
            await _contactService.UpdateContact(id, contact);
        }
        catch (KeyNotFoundException e)
        {
            return NotFound($"Contact witch ID: {id} not found");
        }
        catch (Exception e)
        {
            return StatusCode(500, "An error occured while updating contact");
        }
        return NoContent();
    }
    
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteContact(int id)
    {
        var item = await _contactService.GetContact(id);
        await _contactService.DeleteContact(id);
        return NoContent();
    }
    
    [Authorize]
    [HttpPost("postContact")]
    public async Task<ActionResult> UploadContact(Contact contact)
    {
        await _contactService.UploadContact(contact);
        return CreatedAtAction("GetContact", new
        {
            id = contact.Id
        }, contact);
    }

    [HttpPost("register")]
    public async Task<ActionResult<Contact>> Register(Contact user)
    {
        try
        {
            var newUser = await _contactService.RegisterUser(user);
            return CreatedAtAction("GetContact", new { id = newUser.Id }, newUser);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(string email, string password)
    {
        try
        {
            var token = await _contactService.Authenticate(email, password);
            return Ok(token);
        }
        catch (Exception e)
        {
            return Unauthorized(e.Message);
        }
    }
    
}