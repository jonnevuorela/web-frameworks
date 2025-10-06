
// Interfaces/IUsersRepository

using DIExample.Models;

namespace DIExample.Interfaces;

public interface IUsersRepository : IDisposable
{
    // Huomaa, että metodi ei ole tyyppia Task<T>
    // Tarkoitus on pitää esimerkki mahdollisimman yksinkertaisena
    // joten emme tee repositoriosta oikeaa tietokantakyselyä
    public IEnumerable<AppUser> GetAll();
}
