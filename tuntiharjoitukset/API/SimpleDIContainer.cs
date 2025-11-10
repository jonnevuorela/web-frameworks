// SimpleDIContainer.cs

namespace API
{
    public class SimpleDiContainer
    {
        // dictionary, joka sisältää kaikki tietotyypit
        private readonly Dictionary<Type, Type> _dependencies = new Dictionary<Type, Type>();

        // tällä metodilla voidaan rekisteröidä riippuvuuksia
        // jos käytämme tietotyyppinä rajapintaa

        // where TTo : class on rajoite,
        // joka pakottaa TTo:n tyypin olevan reference type (kuten class tai interface)
        // TTo:n pitää implementoida tyyppi TFrom
        public void Register<TFrom, TTo>()
            where TTo : class, TFrom
        {
            _dependencies.Add(typeof(TFrom), typeof(TTo));
        }

        // tämä on sama metodi mutta, jos riippuvuudella ei ole rajapintaa
        // voimme käyttää tätä
        public void Register<T>()
            where T : class
        {
            _dependencies.Add(typeof(T), typeof(T));
        }

        // Tällä metodilla palautetaan T:n tyyppinen instanssi _dependencies-listan riippuvuudesta
        public T Resolve<T>()
            where T : class
        {
            var resolvedClassInstance = Resolve(typeof(T));
            if (resolvedClassInstance == null)
            {
                throw new Exception($"Could not resolve type {typeof(T)}");
            }

            return (T)resolvedClassInstance;
        }

        // tämä on metodi jota ylemässä Resolve-metodissa käytetään
        private object? Resolve(Type type)
        {
            // koska käytämme Type-tyyppiä, resolvedType on tietotyyppi, ei instanssi siitä tietotyypistä
            Type resolvedType;
            try
            {
                // haetaan rekisteröity dependency tyypin mukaan
                resolvedType = _dependencies[type];
            }
            catch
            {
                // jos yrität tehdä instanssin tietotyypistä, jota ei ole rekisteröity
                // heitetään tämä virhe
                throw new Exception($"Could not resolve type {type}");
            }

            // jos Type on rekisteröity,
            // eli se on lisätty _dependenies-dictiin ennen Resolven kutsua
            // käyttäen Register-metodia
            //
            // haetaan luokan ensimmäinen constructor-metodi
            var ctor = resolvedType.GetConstructors().First();
            // katsotaan, onko luokan constructorilla parametrejä
            var arguments = ctor.GetParameters();
            // jos constructorilla ei ole parametrejä,
            // luodaan instanssi, joka on tyyppiä resolvedType
            if (arguments.Length == 0)
            {
                return Activator.CreateInstance(resolvedType);
            }

            // jos constructorilla on parametri / parametrejä
            // kutsutaan Resolve-metodia rekursiivisesti, jotta
            // kaikki riippuvuudet saadaan instantoitua
            var args = new List<object?>();
            foreach (var argument in arguments)
            {
                args.Add(Resolve(argument.ParameterType));
            }

            // kun kaikki constructorin parametrien tietotyypit on löydetty
            // luodaan luoksta  instanssi löydetyillä parametreillä käyttäen Invoce-metodia
            return ctor.Invoke(args.ToArray());
        }
    }
}
