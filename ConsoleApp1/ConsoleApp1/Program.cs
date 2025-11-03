using System;
using System.Collections.Generic;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int[,] pole = Dostanipole();
            if (pole.Length == 0)
            {
                Console.WriteLine("Program ukončen kvůli chybnému vstupu.");
                return;
            }
            Volenka nulaJedna = new Volenka(pole);
            nulaJedna.SpoctiVysledky();
        }

        class Volenka
        {
            public int Pocet { get; private set; }
            public int[,] MuzPref { get; private set; }
            public int[,] ZenaPref { get; private set; }
            public int[] Parovani { get; private set; } // Index ženy -> muž

            public Volenka(int[,] vstupPole)
            {
                Pocet = vstupPole.GetLength(1);
                MuzPref = new int[Pocet, Pocet];
                ZenaPref = new int[Pocet, Pocet];
                Parovani = new int[Pocet];
                for (int i = 0; i < Pocet; i++) { Parovani[i] = -1; }

                for (int i = 0; i < Pocet; i++)
                {
                    for (int j = 0; j < Pocet; j++)
                    { ZenaPref[i, j] = vstupPole[i, j]; }
                }

                for (int i = 0; i < Pocet; i++)
                {
                    for (int j = 0; j < Pocet; j++)
                    {
                        MuzPref[i, j] = vstupPole[Pocet + i, j];
                    }
                }
            }

            public void SpoctiVysledky()
            {
                int[] muzUrovne = new int[Pocet];

                List<int> volniMuzi = new List<int>();
                for (int i = 0; i < Pocet; i++)
                {
                    volniMuzi.Add(i);
                }

                while (volniMuzi.Count > 0)
                {
                    int muz = volniMuzi[0];
                    volniMuzi.RemoveAt(0);

                    while (muzUrovne[muz] < Pocet)
                    {
                        int zena = MuzPref[muz, muzUrovne[muz]];
                        muzUrovne[muz]++;

                        if (Parovani[zena] == -1)
                        {
                            Parovani[zena] = muz;
                            break;
                        }
                        else
                        {
                            int soucasnyPartner = Parovani[zena];
                            int poziceSoucasneho = -1, poziceNoveho = -1;
                            for (int i = 0; i < Pocet; i++)
                            {
                                if (ZenaPref[zena, i] == soucasnyPartner) poziceSoucasneho = i;
                                if (ZenaPref[zena, i] == muz) poziceNoveho = i;
                            }

                            if (poziceNoveho < poziceSoucasneho)
                            {
                                Parovani[zena] = muz;
                                volniMuzi.Add(soucasnyPartner);
                                break;
                            }
                        }
                    }
                }

                Console.WriteLine("Vysledne pary:");
                for (int z = 0; z < Pocet; z++)
                    Console.WriteLine($"{Parovani[z] + 1}");
            }
        }

        static int[,] Dostanipole()
        {
            Console.WriteLine("Zadejte počet (např. 4):");
            string pocetInput = Console.ReadLine();
            if (!int.TryParse(pocetInput, out int pocet) || pocet <= 0)
            {
                Console.WriteLine("Chyba: Neplatný vstup pro počet.");
                return new int[0, 0];
            }

            int[,] pole = new int[pocet * 2, pocet];

            Console.WriteLine("Zadejte preference žen (řádek po řádku):");
            for (int i = 0; i < pocet; i++)
            {
                string line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                {
                    Console.WriteLine("Chyba: Prázdný řádek, zadejte prosím znovu.");
                    i--;
                    continue;
                }
                string[] cisla = line.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (cisla.Length != pocet)
                {
                    Console.WriteLine($"Chyba: Očekáváno {pocet} čísel, dostáno {cisla.Length}");
                    i--;
                    continue;
                }

                for (int j = 0; j < pocet; j++)
                    pole[i, j] = int.Parse(cisla[j]) - 1;
            }

            Console.WriteLine("Zadejte preference mužů (řádek po řádku):");
            for (int i = 0; i < pocet; i++)
            {
                string line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                {
                    Console.WriteLine("Chyba: Prázdný řádek, zadejte prosím znovu.");
                    i--;
                    continue;
                }
                string[] cisla = line.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (cisla.Length != pocet)
                {
                    Console.WriteLine($"Chyba: Očekáváno {pocet} čísel, dostáno {cisla.Length}");
                    i--;
                    continue;
                }

                for (int j = 0; j < pocet; j++)
                    pole[pocet + i, j] = int.Parse(cisla[j]) - 1;
            }

            return pole;
        }
    }
}
