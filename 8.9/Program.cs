using System;
using System.Collections.Generic;

namespace _8._9
{
    internal class Program
    {
        static List<Film> Filmy = new List<Film>();//list pro ty vsechny filmy

        static void Main(string[] args)
        {
            //tvrime filmy do 
            Film Jungo = new Film("Jungo", "BB", "NN", 43);
            Film Auta = new Film("Auta", "Blesk", "MQ", 33);
            Film SpiderMan = new Film("SpiderMan", "IDK", "KDI", 55);


            Console.WriteLine("vsechny filmy:");
            foreach (Film film in Filmy)
            {
                Console.WriteLine(film);
            }


            Console.WriteLine("Pro vypocet nej filmu dejte enter,jinak cokoli namackejte a pak enter");
            string podsemkamdes = Console.ReadLine();
            if (string.IsNullOrEmpty(podsemkamdes))
            {
                Film.NejHodnoceni();
                Console.WriteLine();
                Jungo.VypisTo();
            }

            Console.ReadKey();//aby se terminal hned nezavrel
        }

        class Film
        {
            //definujeme vlastnosti
            public string Nazev { get; }
            public string JmenoRezisera { get; }
            public string PrijmeniRezisera { get; }
            public int RokVzniku { get; }
            public double Hodnoceni { get; private set; }
            public bool Odpad { get; private set; }

            private List<double> hodnoceni;

            public Film(string nazev, string jmeno, string prijmeni, int rok)//to co sse jakoby spusti kdyz vytvorime classu/inicializace
            {
                Nazev = nazev;
                JmenoRezisera = jmeno;
                PrijmeniRezisera = prijmeni;
                RokVzniku = rok;
                hodnoceni = new List<double>();
                Odpad = false;
                PridaniHodnoceni();
                UdelaniPrumeru();
                Program.Filmy.Add(this);//pridavame do Film listu s vsemi filmy
            }

            private void PridaniHodnoceni()//proste prida hodnoceni
            {
                Random random = new Random();
                for (int i = 0; i < 15; i++)
                {
                    hodnoceni.Add(random.Next(1, 6)); 
                }
            }

            private void UdelaniPrumeru()
            {
                if (hodnoceni == null || hodnoceni.Count == 0)
                {
                    Console.WriteLine("Žádná hodnocení k výpočtu průměru.");
                    return;
                }

                double soucet = 0;
                foreach (double hodn in hodnoceni)
                {
                    soucet += hodn;
                }
                Hodnoceni = soucet / hodnoceni.Count;
                Odpad = Hodnoceni < 3.0;//rovnou tu hazime true nebo false do odpadu at se stim pak nemazem
            }

            public override string ToString()//fce jak se to ma zachovat kdyz dame vypsat ten film
            {
                return $"Film: {Nazev}, Rok: {RokVzniku}, Režisér: {JmenoRezisera} {PrijmeniRezisera}, Průměr hodnocení {Hodnoceni:F3}";
            }

            
            public static void NejHodnoceni()
            {
                double NejHodnoceni = -1.0;
                List<string> Nejfilmy = new List<string>();//list pro ty nej at to pak lehce vypisu

                foreach (Film film in Program.Filmy)
                {
                    if (film.Hodnoceni > NejHodnoceni)//nove nej 
                    {
                        NejHodnoceni = film.Hodnoceni;
                        Nejfilmy.Clear();//aby tam nebyly ty stary
                        Nejfilmy.Add(film.Nazev);
                    }
                    else if (film.Hodnoceni == NejHodnoceni)//pokud maj stejne 
                    {
                        Nejfilmy.Add(film.Nazev);
                    }
                }

                
                foreach (Film film in Program.Filmy)//pro vypis odpadu
                {
                    if (film.Odpad)
                    {
                        Console.WriteLine($"Film {film.Nazev} je odpad! Má hodnocení jen {film.Hodnoceni:F3}.");
                    }
                }

                
                string vysledek = string.Join(", ", Nejfilmy);//spojim tu ty nej filmy a pak vypisu 
                Console.WriteLine($"Nejlepší film/filmy: {vysledek} (hodnocení {NejHodnoceni:F3})");
            }

            public void VypisTo() // jen vypsani 
            {
                foreach (Film film in Program.Filmy) 
                {
                    Console.WriteLine(film);
                }
            }
        }
    }
}