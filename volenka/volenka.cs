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
                int[] zenaUrovne = new int[Pocet]; // kolikatyho muze zkusi zena
                int[] muzPartnerka = new int[Pocet]; // rn partnerka muze
                
                // inicializace - vsichni jsou volni
                for (int i = 0; i < Pocet; i++)
                {
                    muzPartnerka[i] = -1;
                    Parovani[i] = -1; // zena -> muz
                }

                while (true)
                {
                    int volnaZena = -1; 
                    // najdem volnou zenu
                    for (int z = 0; z < Pocet; z++){
                        if (Parovani[z] == -1 && zenaUrovne[z] < Pocet)
                        {
                            volnaZena = z;// preberem index volne zeny
                            break;
                        }}
                    if (volnaZena == -1) break;//vsechny zeny maji nekoho 

                    int muz = ZenaPref[volnaZena, zenaUrovne[volnaZena]];//najdem si komu zena nabizi
                    zenaUrovne[volnaZena]++;

                    if (muzPartnerka[muz] == -1) // pokud muz je volny
                    {
                        Parovani[volnaZena] = muz; //na index zeny v parovani dame muze 
                        muzPartnerka[muz] = volnaZena; //do seznamu partnerek muze dame zenu 
                    }
                    else // Muž má partnerku, porovnáme preference
                    {
                        int staraZena = muzPartnerka[muz]; //zena minula
                        int muzPrefStara = -1, muzPrefNova = -1; 
                        for (int i = 0; i < Pocet; i++)//projedem vsechny preference muze 
                        {
                            if (MuzPref[muz, i] == staraZena) muzPrefStara = i; // najdeme jak su na tom zeny
                            if (MuzPref[muz, i] == volnaZena) muzPrefNova = i; 
                        }
                        if (muzPrefNova < muzPrefStara) // Nová žena je lepší
                        {
                            Parovani[staraZena] = -1; //stara zena je volna
                            Parovani[volnaZena] = muz; //nova zena ma muze
                            muzPartnerka[muz] = volnaZena; //muz ma novou zenu
                        }
                        //pokud ne zustava to tak jak to bylo
                    }
                }

                // Vytisknem vysledek - pro kazdou zenu jejiho muze
                Console.WriteLine("Vysledne pary:");
                for (int z = 0; z < Pocet; z++)
                    Console.WriteLine($"{Parovani[z] + 1}"); // +1 protoze cislujem od 1
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