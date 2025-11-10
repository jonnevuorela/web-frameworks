// See https://aka.ms/new-console-template for more information

using API.Controllers;
using API.Interfaces;
using API.Repositories;
using Microsoft.Data.Sqlite;

namespace API
{
    public static class Program
    {
        public static void Main()
        {
            while (true)
            {
                Console.WriteLine("Mitä haluat tehdä? (\n0: Lopeta\n" + "1: Listaa käyttäjät\n):");
                var choice = Console.ReadLine();
                Console.WriteLine(choice);
                if (choice == "0")
                {
                    break;
                }

                switch (choice)
                {
                    case "1":

                        var container = new SimpleDiContainer();
                        container.Register<IUsersRepository, UsersSQLiteRepository>();
                        container.Register<UsersController>();

                        var ctrl = container.Resolve<UsersController>();
                        var users = ctrl.GetAllUsers();
                        Console.WriteLine(users);
                        break;
                }
            }
        }
    }
}
