using System;
using System.Collections.Generic;

namespace volenka
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int[,] pole = Dostanipole();
            Volenka NulaJedna = new Volenka(pole);
            NulaJedna.SpoctiVysledky();
        }

        class Volenka
        {
            public int Pocet { get; private set; }
            public int[,] MuzPref { get; private set; }
            public int[,] ZenaPref { get; private set; }
            public int[] Parovani { get; private set; } // Index ženy -> muž

            public Volenka(int[,] vstupPole)
            {
                Pocet = (int)Math.Sqrt(vstupPole.GetLength(0)); // Předpokládáme čtvercovou matici
                MuzPref = new int[Pocet, Pocet];
                ZenaPref = new int[Pocet, Pocet];
                Parovani = new int[Pocet]; // Inicializace na -1 (nikdo nepárován)
                for (int i = 0; i < Pocet; i++) {Parovani[i] = -1;}

                // Načtení preferencí žen (první polovina)
                for (int i = 0; i < Pocet; i++)
                {
                    for (int j = 0; j < Pocet; j++)
                    { ZenaPref[i, j] = vstupPole[i, j]; }
                }
                // Načtení preferencí mužů (druhá polovina)
                for (int i = 0; i < Pocet; i++){
                    {for (int j = 0; j < Pocet; j++)
                        MuzPref[i, j] = vstupPole[Pocet + i, j];}}
            }

            public void SpoctiVysledky()
            {
                int[] muzUrovne = new int[Pocet]; // Aktuální preference muže
                int[] zenaPartner = new int[Pocet]; // Aktuální partner ženy
                for (int i = 0; i < Pocet; i++) zenaPartner[i] = -1;

                while (true)
                {
                    int volnyMuz = -1;
                    for (int m = 0; m < Pocet; m++){
                        if (Parovani[m] == -1)
                        {
                            volnyMuz = m;
                            break;
                        }}
                    if (volnyMuz == -1) break; // Všichni muži jsou spárováni

                    int zena = MuzPref[volnyMuz, muzUrovne[volnyMuz]];
                    muzUrovne[volnyMuz]++;

                    if (zenaPartner[zena] == -1) // Žena je volná
                    {
                        Parovani[volnyMuz] = zena;
                        zenaPartner[zena] = volnyMuz;
                    }
                    else // Žena má partnera, porovnáme preference
                    {
                        int staryMuz = zenaPartner[zena];
                        int zenaPrefStary = -1, zenaPrefNovy = -1;
                        for (int i = 0; i < Pocet; i++)
                        {
                            if (ZenaPref[zena, i] == staryMuz) zenaPrefStary = i;
                            if (ZenaPref[zena, i] == volnyMuz) zenaPrefNovy = i;
                        }
                        if (zenaPrefNovy < zenaPrefStary) // Nový muž je lepší
                        {
                            Parovani[staryMuz] = -1;
                            Parovani[volnyMuz] = zena;
                            zenaPartner[zena] = volnyMuz;
                        }
                    }
                }

                // Výpis výsledků
                Console.WriteLine("Výsledné párování:");
                for (int m = 0; m < Pocet; m++)
                    Console.WriteLine($"Muž {m} -> Žena {Parovani[m]}");
            }
        }

        static int[,] Dostanipole()
        {
            Console.WriteLine("Zadejte počet (např. 4):");
            int pocet = int.Parse(Console.ReadLine());
            int[,] pole = new int[pocet * 2, pocet]; // Pro muže i ženy

            Console.WriteLine("Zadejte preference žen (řádek po řádku):");
            for (int i = 0; i < pocet; i++)
            {
                string line = Console.ReadLine();
                string[] cisla = line.Split(' ');
                for (int j = 0; j < pocet; j++)
                    pole[i, j] = int.Parse(cisla[j]) - 1; // Převod na 0-based index
            }

            Console.WriteLine("Zadejte preference mužů (řádek po řádku):");
            for (int i = 0; i < pocet; i++)
            {
                string line = Console.ReadLine();
                string[] cisla = line.Split(' ');
                for (int j = 0; j < pocet; j++)
                    pole[pocet + i, j] = int.Parse(cisla[j]) - 1; // Převod na 0-based index
            }

            return pole;
        }
    }
}