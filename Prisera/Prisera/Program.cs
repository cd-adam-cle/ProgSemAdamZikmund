using System;

namespace prisera
{
    internal class Program
    {
        static void Main(string[] args)
        {
            char[,] pole = Dostanipole(); // Přidána středník
            Pole Maze = new Pole(pole); // Opravena syntaxe konstruktoru
            Maze.ZobrazPole(); // Zobrazíme pole pro kontrolu
        }

        static char[,] Dostanipole()
        {
            Console.WriteLine("zadejte vysku");
            int vyska = int.Parse(Console.ReadLine()); // Opravena konverze
            Console.WriteLine("zadejte sirku");
            int sirka = int.Parse(Console.ReadLine()); // Opravena konverze a překlep
            Console.WriteLine("zadejte pole"); // Opravena metoda
            char[,] pole = new char[vyska, sirka];

            for (int i = 0; i < vyska; i++)
            {
                string line = Console.ReadLine(); // Přidán středník
                for (int j = 0; j < Math.Min(line.Length, sirka); j++) // Opravena vlastnost a přidána ochrana
                {
                        pole[i, j] = line[j];
                }
            }
            return pole;
        }
    }

    class Pole
    {
        public int Sirka { get; set; }
        public int Vyska { get; set; }
        public char[,] Poles { get; private set; }

        public Pole(char[,] vstupPole) // Zjednodušený konstruktor
        {
            Vyska = vstupPole.GetLength(0);
            Sirka = vstupPole.GetLength(1);
            Poles = new char[Vyska, Sirka];

            // Zkopírujeme pole
            for (int i = 0; i < Vyska; i++)
            {
                for (int j = 0; j < Sirka; j++)
                {
                    Poles[i, j] = vstupPole[i, j]; // Opravený název proměnné
                }
            }
            Kroky();
            
        }
        public void Kroky()
        {
            List<int> KdeJe = new List<int>();

            // OPRAVENO - odstranění nekonečné smyčky
            for (int i = 0; i < Vyska; i++)
            {
                for (int j = 0; j < Sirka; j++)
                {
                    if (Poles[i, j] == '>' || Poles[i, j] == '<' || Poles[i, j] == '^' || Poles[i, j] == 'ˇ')
                    {
                        KdeJe.Add(i);
                        KdeJe.Add(j);

                        // Ukončíme hledání po první nalezenej příšeře
                        //Console.WriteLine($"Prisera je na souradnicich {KdeJe[0] + 1} , {KdeJe[1] + 1} ");
                        return; // Ukončí metodu
                    }
                }
            }
        }

        public void ZobrazPole()
        {
            Console.WriteLine("\nNačtené pole:");
            for (int i = 0; i < Vyska; i++)
            {
                for (int j = 0; j < Sirka; j++)
                {
                    Console.Write(Poles[i, j]);
                }
                Console.WriteLine();
            }
        }
    }
}