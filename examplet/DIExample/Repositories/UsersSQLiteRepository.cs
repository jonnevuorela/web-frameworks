
using DIExample.Interfaces;
using DIExample.Models;


namespace DIExample.Repositories;

public class UsersSQLiteRepository : IUsersRepository
{



    public void Dispose()
    {




    }
    // kuten huomaat repon metodi ei oikeasti tee kyselyä tietokantaan
    // mutta sillä ei ole merkitystä dependency injection-esimerkin kannalta
    public IEnumerable<AppUser> GetAll()
    {


        return new List<AppUser>
        {
            new()
            {
                Firstname = "Juhani",
                Lastname = "Kuru",
                Username = "juhani.kuru",
                Id = 1
            }
        };
    }
}
