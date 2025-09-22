
namespace EncapsulationExample;

public static class Program
{
    public static void Main()
    {
        var juhani = new Driver("juhani", "kuru");
        var toyota = new Car("toyota", "corolla");
        
        juhani.Drive(toyota);
    }
}

public class Car(string make, string model)
{
    public string Make { get; } = make;
    public string Model { get; } = model;

    public void Ignite()
    {
        Console.WriteLine("Igniting {0} {1}", Make, Model);
    }

    public void Accelerate()
    {
        Console.WriteLine("Wroooom!");
    }
}


public class Driver(string firstname, string lastname)
{
    private string FirstName { get; } = firstname;
    private string LastName { get; } = lastname;
    
    // Koska Drive-metodi ottaa parametrinaan Car-luokan instanssin
    // on siinä paketoituna Car-luokan metoditkin
    // ja voimme kutsua niitä Drive-metodissa
    public void Drive(Car car)
    {
        
        Console.WriteLine("Hello my name is {0} {1} and I'm driving {2} {3}", FirstName, LastName, car.Make, car.Model);
        car.Ignite();
        car.Accelerate();
    }
}
