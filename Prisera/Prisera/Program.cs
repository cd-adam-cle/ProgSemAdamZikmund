using System;
using System.Collections.Generic;

namespace Prisera
{
    /// <summary>
    /// Výčtový typ pro směry, kterými může příšera směřovat v bludišti.
    /// </summary>
    public enum Smer
    {
        /// <summary>Značí směr nahoru (^).</summary>
        Nahoru = 0,
        /// <summary>Značí směr doprava (>).</summary>
        Doprava = 1,
        /// <summary>Značí směr dolů (v).</summary>
        Dolu = 2,
        /// <summary>Značí směr doleva (<).</summary>
        Doleva = 3
    }

    /// <summary>
    /// Hlavní třída programu, která načítá vstup a spouští simulaci příšery v bludišti.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Hlavní metoda programu. Načte rozměry bludiště, vytvoří bludiště a provede až 20 kroků příšery.
        /// </summary>
        /// <param name="args">Argumenty příkazové řádky (nepoužívají se).</param>
        static void Main(string[] args)
        {
            // Načtení šířky a výšky bludiště
            int sirka = int.Parse(Console.ReadLine());
            int vyska = int.Parse(Console.ReadLine());
            char[,] dataBludiste = NactiBludiste(sirka, vyska);

            // Vytvoření instance bludiště a spuštění simulace
            Bludiste bludiste = new Bludiste(dataBludiste);
            bludiste.VypisBludiste(); // Vypsání počátečního stavu bludiště
            for (int krok = 1; krok <= 20; krok++)
            {
                bool provedlPohyb = bludiste.ProvedKrok();
                bludiste.VypisBludiste();
                if (!provedlPohyb) // Zastavení, pokud příšera nemůže pokračovat
                    break;
            }
        }

        /// <summary>
        /// Načte bludiště ze standardního vstupu.
        /// </summary>
        /// <param name="sirka">Šířka bludiště (počet sloupců).</param>
        /// <param name="vyska">Výška bludiště (počet řádků).</param>
        /// <returns>Dvourozměrné pole znaků představující bludiště.</returns>
        static char[,] NactiBludiste(int sirka, int vyska)
        {
            char[,] bludiste = new char[vyska, sirka];
            for (int i = 0; i < vyska; i++)
            {
                string radek = Console.ReadLine();
                // Načtení znaků z řádku
                for (int j = 0; j < Math.Min(radek.Length, sirka); j++)
                {
                    bludiste[i, j] = radek[j];
                }
                // Vyplnění zbývajících sloupců volným polem (.)
                for (int j = radek.Length; j < sirka; j++)
                {
                    bludiste[i, j] = '.';
                }
            }
            return bludiste;
        }
    }

    /// <summary>
    /// Třída představující bludiště s příšerou, která se pohybuje podle pravidla pravé ruky.
    /// </summary>
    class Bludiste
    {
        /// <summary>
        /// Šířka bludiště (počet sloupců).
        /// </summary>
        public int Sirka { get; private set; }

        /// <summary>
        /// Výška bludiště (počet řádků).
        /// </summary>
        public int Vyska { get; private set; }

        /// <summary>
        /// Dvourozměrné pole znaků představující políčka bludiště ('X' = zeď, '.' = volné, '^>v<' = příšera).
        /// </summary>
        public char[,] Policka { get; private set; }

        /// <summary>
        /// Řádek, na kterém se příšera nachází.
        /// </summary>
        private int radekPrisery;

        /// <summary>
        /// Sloupec, na kterém se příšera nachází.
        /// </summary>
        private int sloupecPrisery;

        /// <summary>
        /// Směr, kterým příšera směřuje.
        /// </summary>
        private Smer smerPrisery;

        /// <summary>
        /// Vytvoří nové bludiště a inicializuje pozici příšery.
        /// </summary>
        /// <param name="vstupniBludiste">Vstupní dvourozměrné pole znaků představující bludiště.</param>
        public Bludiste(char[,] vstupniBludiste)
        {
            Vyska = vstupniBludiste.GetLength(0);
            Sirka = vstupniBludiste.GetLength(1);
            Policka = new char[Vyska, Sirka];

            // Zkopírování vstupního bludiště a nalezení příšery
            for (int i = 0; i < Vyska; i++)
            {
                for (int j = 0; j < Sirka; j++)
                {
                    Policka[i, j] = vstupniBludiste[i, j];
                    if ("^>v<".Contains(Policka[i, j]))
                    {
                        radekPrisery = i;
                        sloupecPrisery = j;
                        smerPrisery = ZnakNaSmer(Policka[i, j]);
                    }
                }
            }
        }

        /// <summary>
        /// Převede znak směru na hodnotu enumu Smer.
        /// </summary>
        /// <param name="znak">Znak směru (^, >, v, <).</param>
        /// <returns>Hodnota enumu Smer.</returns>
        private Smer ZnakNaSmer(char znak)
        {
            return znak switch
            {
                '^' => Smer.Nahoru,
                '>' => Smer.Doprava,
                'v' => Smer.Dolu,
                '<' => Smer.Doleva,
                _ => throw new ArgumentException("Neplatný znak směru")
            };
        }

        /// <summary>
        /// Převede hodnotu enumu Smer na odpovídající znak.
        /// </summary>
        /// <param name="smer">Směr příšery.</param>
        /// <returns>Znak směru (^, >, v, <).</returns>
        private char SmerNaZnak(Smer smer)
        {
            return smer switch
            {
                Smer.Nahoru => '^',
                Smer.Doprava => '>',
                Smer.Dolu => 'v',
                Smer.Doleva => '<',
                _ => '?'
            };
        }

        /// <summary>
        /// Otočí směr příšery doprava (např. Nahoru -> Doprava).
        /// </summary>
        /// <param name="smer">Aktuální směr příšery.</param>
        /// <returns>Nový směr po otočení doprava.</returns>
        private Smer OtocDoprava(Smer smer)
        {
            return (Smer)(((int)smer + 1) % 4);
        }

        /// <summary>
        /// Otočí směr příšery doleva (např. Nahoru -> Doleva).
        /// </summary>
        /// <param name="smer">Aktuální směr příšery.</param>
        /// <returns>Nový směr po otočení doleva.</returns>
        private Smer OtocDoleva(Smer smer)
        {
            return (Smer)(((int)smer + 3) % 4);
        }

        /// <summary>
        /// Zkontroluje, zda je dané pole volné (není zeď a je v rámci bludiště).
        /// </summary>
        /// <param name="radek">Řádek pole.</param>
        /// <param name="sloupec">Sloupec pole.</param>
        /// <returns>True, pokud je pole volné; jinak false.</returns>
        private bool JePoleVolne(int radek, int sloupec)
        {
            return radek >= 0 && radek < Vyska && sloupec >= 0 && sloupec < Sirka && Policka[radek, sloupec] != 'X';
        }

        /// <summary>
        /// Vrátí souřadnice pole ve směru od aktuální pozice příšery.
        /// </summary>
        /// <param name="smer">Směr, ve kterém se má kontrolovat pole.</param>
        /// <param name="aktualniRadek">Aktuální řádek příšery.</param>
        /// <param name="aktualniSloupec">Aktuální sloupec příšery.</param>
        /// <returns>Tuple s řádkem a sloupcem pole ve směru.</returns>
        private (int radek, int sloupec) ZiskejPoleVeSmeru(Smer smer, int aktualniRadek, int aktualniSloupec)
        {
            return smer switch
            {
                Smer.Nahoru => (aktualniRadek - 1, aktualniSloupec),
                Smer.Doprava => (aktualniRadek, aktualniSloupec + 1),
                Smer.Dolu => (aktualniRadek + 1, aktualniSloupec),
                Smer.Doleva => (aktualniRadek, aktualniSloupec - 1),
                _ => (aktualniRadek, aktualniSloupec)
            };
        }

        /// <summary>
        /// Provede jeden krok příšery podle pravidla pravé ruky. Pokusí se otočit doprava a jít,
        /// pokud je to možné; jinak zkouší jít dopředu; pokud je to blokováno, zkouší otočení
        /// doleva a pohyb; pokud nic nejde, otočí se doleva na místě.
        /// </summary>
        /// <returns>True, pokud příšera provedla pohyb; false, pokud se pouze otočila na místě.</returns>
        public bool ProvedKrok()
        {
            // Kontrola pole vpravo od příšery
            Smer smerDoprava = OtocDoprava(smerPrisery);
            (int radekVpravo, int sloupecVpravo) = ZiskejPoleVeSmeru(smerDoprava, radekPrisery, sloupecPrisery);

            if (JePoleVolne(radekVpravo, sloupecVpravo))
            {
                // Otočení doprava a pohyb na nové pole
                smerPrisery = smerDoprava;
                Policka[radekPrisery, sloupecPrisery] = '.'; // Vymazání aktuální pozice
                radekPrisery = radekVpravo;
                sloupecPrisery = sloupecVpravo;
                Policka[radekPrisery, sloupecPrisery] = SmerNaZnak(smerPrisery);
                return true;
            }

            // Pokus o pohyb dopředu
            (int radekDopredu, int sloupecDopredu) = ZiskejPoleVeSmeru(smerPrisery, radekPrisery, sloupecPrisery);
            if (JePoleVolne(radekDopredu, sloupecDopredu))
            {
                // Pohyb dopředu na nové pole
                Policka[radekPrisery, sloupecPrisery] = '.'; // Vymazání aktuální pozice
                radekPrisery = radekDopredu;
                sloupecPrisery = sloupecDopredu;
                Policka[radekPrisery, sloupecPrisery] = SmerNaZnak(smerPrisery);
                return true;
            }

            // Pokus o otočení doleva a pohyb
            Smer smerDoleva = OtocDoleva(smerPrisery);
            (int radekDoleva, int sloupecDoleva) = ZiskejPoleVeSmeru(smerDoleva, radekPrisery, sloupecPrisery);
            if (JePoleVolne(radekDoleva, sloupecDoleva))
            {
                // Otočení doleva a pohyb na nové pole
                smerPrisery = smerDoleva;
                Policka[radekPrisery, sloupecPrisery] = '.'; // Vymazání aktuální pozice
                radekPrisery = radekDoleva;
                sloupecPrisery = sloupecDoleva;
                Policka[radekPrisery, sloupecPrisery] = SmerNaZnak(smerPrisery);
                return true;
            }

            // Pokud není možný žádný pohyb, otočí se doleva na místě
            smerPrisery = smerDoleva;
            Policka[radekPrisery, sloupecPrisery] = SmerNaZnak(smerPrisery);
            return false;
        }

        /// <summary>
        /// Vypíše aktuální stav bludiště na konzoli s prázdným řádkem na konci.
        /// </summary>
        public void VypisBludiste()
        {
            for (int i = 0; i < Vyska; i++)
            {
                for (int j = 0; j < Sirka; j++)
                {
                    Console.Write(Policka[i, j]);
                }
                Console.WriteLine();
            }
            Console.WriteLine(); // Prázdný řádek po vypsání bludiště
        }
    }
}