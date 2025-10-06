
// See https://aka.ms/new-console-template for more information

using DIExample.Controllers;
using DIExample.Interfaces;
using DIExample.Repositories;


namespace DIExample
{
    public static class Program
    {
        public static void Main()
        {
            while (true)
            {
                Console.WriteLine("Mitä haluat tehdä? (\n0: Lopeta\n" +
                                  "1: Listaa käyttäjät\n):");
                var choice = Console.ReadLine();

                if (choice == "0")
                {
                    break;
                }

                switch (choice)
                {
                    case "1":
                        // luodaan SimpleDiContainerista instanssi
                        var container = new SimpleDiContainer();

                        // rekisteröidään kaikki sovelluksen kerrokset
                        // UsersController-luokka ei käytä mitään rajapintoja
                        // joten voidaan käyttää Register-metodista versiota <T>

                        // tämä lisää _dependencies-dictionaryyn tietotyypin UsersController
                        container.Register<UsersController>();


                        // rekisteröidään UsersSQLiteRepository-tietotyyppi
                        // koska se käyttää rajapintaa IUsersRepository
                        // käytetään Registeristä versiota <TFrom, TTo>

                        // Register-metodin avulla meidän tarvitee vaihtaa vain UsersSQLiteRepository
                        // toiseen, jos datalähde vaihtuisi
                        // kunhan se toinenkin repository käyttää rajapintaa IUsersRepository
                        container.Register<IUsersRepository, UsersSQLiteRepository>();


                        // koska UsersController-tietotyyppi on ylempänä rekisteröity
                        // voimme luoda instanssin UsersController-luokasta käyttäen Resolve-metodia

                        // Huomaa, että emme enää käsin tee instanssia UsersRepositorysta,
                        // Koska UsersControllerin constructorin parametrina on IUsersRepository repository
                        // ao. Resolve löytää repositorion instanssin automaattisesti

                        // rajapinnan IUsersRepositoryn perusteella. Tämän ansiosta: container.Register<IUsersRepository, UsersSQLiteRepository>();

                        // Kaikkialla koodissa, missä constructorin parametrin tietotyyppinä käytetään interfacea IUsersRepository
                        // luodaan siitä konkreettinen instanssi tyyppiä UsersSQLiteRepository

                        var ctrl = container.Resolve<UsersController>();


                        // tässä voimme käyttää controllerin metodia GetAll
                        // koska Resolve luo instanssin siitä ja UsersSQLiteRepositorysta
                        var users = ctrl.GetAll();
                        foreach (var user in users)
                        {
                            Console.WriteLine(user.Firstname);
                        }


                        break;
                }

            }
        }
    }
}
