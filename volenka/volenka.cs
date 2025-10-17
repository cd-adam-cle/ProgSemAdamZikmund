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
                Pocet = (int)Math.Sqrt(vstupPole.GetLength(0)); // dostanem pocet z te matice
                MuzPref = new int[Pocet, Pocet];//vytvorime muze
                ZenaPref = new int[Pocet, Pocet];//vytvorime zeny
                Parovani = new int[Pocet];//vytvorime parovani
                for (int i = 0; i < Pocet; i++) {Parovani[i] = -1;}//dame do parovani basic hodnotu

                //nactem co chteji zeny 
                for (int i = 0; i < Pocet; i++)
                {
                    for (int j = 0; j < Pocet; j++)
                    { ZenaPref[i, j] = vstupPole[i, j]; }
                }
                // nactem co chtej muzi 
                for (int i = 0; i < Pocet; i++){
                    {for (int j = 0; j < Pocet; j++)
                        MuzPref[i, j] = vstupPole[Pocet + i, j];}};
            }

            public void SpoctiVysledky()
            {
                int[] muzUrovne = new int[Pocet]; // rn preference muze
                int[] zenaPartner = new int[Pocet]; // rn partner zeny
                for (int i = 0; i < Pocet; i++) zenaPartner[i] = -1;

                while (true)
                {
                    int volnyMuz = -1; 
                    for (int m = 0; m < Pocet; m++){
                        if (Parovani[m] == -1)
                        {
                            volnyMuz = m;// preberem index volneho muzw
                            break;
                        }}
                    if (volnyMuz == -1) break;//vsichni nekoho maj 

                    int zena = MuzPref[volnyMuz, muzUrovne[volnyMuz]];//najdem si jak je na tom muz
                    muzUrovne[volnyMuz]++;

                    if (zenaPartner[zena] == -1) // pokud je zena je volna
                    {
                        Parovani[volnyMuz] = zena; //na index muze v parovani dame zenu 
                        zenaPartner[zena] = volnyMuz; //do seznamu partneru zeny dame muze 
                    }
                    else // Žena má partnera, porovnáme preference
                    {
                        int staryMuz = zenaPartner[zena]; //muz minuly
                        int zenaPrefStary = -1, zenaPrefNovy = -1; 
                        for (int i = 0; i < Pocet; i++)//projedem vsechny preference zeny 
                        {
                            if (ZenaPref[zena, i] == staryMuz) zenaPrefStary = i; // najdeme jak su na tom muzi
                            if (ZenaPref[zena, i] == volnyMuz) zenaPrefNovy = i; 
                        }
                        if (zenaPrefNovy < zenaPrefStary) // Nový muž je lepší
                        {
                            Parovani[staryMuz] = -1;
                            Parovani[volnyMuz] = zena;
                            zenaPartner[zena] = volnyMuz;
                        }
                        //pokud ne zustava to tak jak to bylp
                    }
                }

                // Vytisknem vysledek 
                Console.WriteLine("Vysledne pary:");
                for (int m = 0; m < Pocet; m++)
                    Console.WriteLine($"Muž {m} -> Žena {Parovani[m]}");
            }
        }

        static int[,] Dostanipole()
        {
            Console.WriteLine("Zadejte počet (např. 4):");
            int pocet = int.Parse(Console.ReadLine());
            int[,] pole = new int[pocet * 2, pocet]; // Pro muze i zeny 

            Console.WriteLine("Zadejte preference žen (řádek po řádku):");
            for (int i = 0; i < pocet; i++)
            {
                string line = Console.ReadLine();
                string[] cisla = line.Split(' ');
                for (int j = 0; j < pocet; j++)
                    pole[i, j] = int.Parse(cisla[j]) - 1;
            }

            Console.WriteLine("Zadejte preference mužů (řádek po řádku):");
            for (int i = 0; i < pocet; i++)
            {
                string line = Console.ReadLine();
                string[] cisla = line.Split(' ');
                for (int j = 0; j < pocet; j++)
                    pole[pocet + i, j] = int.Parse(cisla[j]) - 1; 
            }

            return pole;
        }
    }
}