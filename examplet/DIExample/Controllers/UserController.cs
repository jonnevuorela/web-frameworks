
using DIExample.Interfaces;
using DIExample.Models;

namespace DIExample.Controllers;

// injektoidaan repository-instanssi UsersControllerille
// tätä käytetään sitten myöhemmin Program.cs-tiedostossa
public class UsersController(IUsersRepository repository)
{
    public IEnumerable<AppUser> GetAll()
    {
        var data = repository.GetAll();
        Console.WriteLine("################## UsersController::GetAll()");
        return data;
    }
}
