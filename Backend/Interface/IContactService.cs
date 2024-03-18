using Backend.Model;

namespace Backend.Interface;

public interface IContactService
{
    Task<List<Contact>> GetAllContacts();
    Task<Contact> GetContact(int id);
    Task UpdateContact(int id, Contact contact);
    Task DeleteContact(int id);
    Task UploadContact(Contact contact);
    Task<Contact> RegisterUser(Contact user);
    Task<string> Authenticate(string email, string password);
}