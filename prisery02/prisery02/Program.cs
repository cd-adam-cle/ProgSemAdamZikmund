using System;
using System.Collections.Generic;

namespace prisery02
{
    internal class Program
    {
        static void Main(string[] args)
        {
            char[,] pole = Dostanipole(); // základní pole a rozměry
            Pole Maze = new Pole(pole); //vytvoření Konstrukce Pole s nazvem Maze
            Maze.ZobrazPole();//zobrazeni pred kroky
            Maze.KrokyDvouPriser();//dve prisery chodi
        }

        static char[,] Dostanipole() 
        {
            //nacteme vse abysme to pak mohli využít k konstrukci pole
            Console.WriteLine("Zadejte šířku:");
            int sirka = int.Parse(Console.ReadLine());
            Console.WriteLine("Zadejte výšku:");
            int vyska = int.Parse(Console.ReadLine());
            Console.WriteLine(" bludiště:");
            Console.WriteLine("Pouzijte >, <, ^, v pro prvni priseru a 1, 2, 3, 4 pro druhou");
            Console.WriteLine("1=nahoru, 2=doprava, 3=dolu, 4=doleva");
            char[,] pole = new char[vyska, sirka];

            for (int i = 0; i < vyska; i++)//projdem kazdy radek
            {
                string line = Console.ReadLine();//nactem radek
                for (int j = 0; j < Math.Min(line.Length, sirka); j++)//pro kazdy char v line nacteme cahr a dame ho do zacatek pole
                {
                    pole[i, j] = line[j];
                }
            }
            return pole;
        }
    }

    // Trida pro jednu priseru - nova trida pro spravu dvou priser
    class Prisera
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Smer { get; set; }
        public char Symbol { get; set; }
        public int StartX { get; set; }
        public int StartY { get; set; }
        public int StartSmer { get; set; }
        public bool JeAktivni { get; set; }

        // Symboly pro prvni priseru
        private static char[] sipky1 = { '^', '>', 'v', '<' };
        // Symboly pro druhou priseru  
        private static char[] sipky2 = { '1', '2', '3', '4' };

        public Prisera(int x, int y, int smer, int cislo)
        {
            X = StartX = x;
            Y = StartY = y;
            Smer = StartSmer = smer;
            JeAktivni = true;
            AktualizujSymbol(cislo);
        }

        public void AktualizujSymbol(int cisloPrisery)
        {
            if (cisloPrisery == 1)
                Symbol = sipky1[Smer];
            else
                Symbol = sipky2[Smer];
        }

        public void OtocDoprava(int cisloPrisery)
        {
            Smer = (Smer + 1) % 4;
            AktualizujSymbol(cisloPrisery);
        }

        public void OtocDoleva(int cisloPrisery)
        {
            Smer = (Smer + 3) % 4;
            AktualizujSymbol(cisloPrisery);
        }

        public bool JeNaStartu()
        {
            return X == StartX && Y == StartY && Smer == StartSmer;
        }
    }


    class Pole //třída pole
    {
        public int Sirka { get; set; } 
        public int Vyska { get; set; }
        public char[,] Poles { get; private set; }

        
        //využijeme pak k lehkému přičítání/odčítání při změně pozic příšery
        private int[] dx = { -1, 0, 1, 0 }; //zmena x podle toho na jakou stranu je sipka nasmerovana
        private int[] dy = { 0, 1, 0, -1 };//zmena y -=-
        // Směry: 0=nahoru, 1=doprava, 2=dolů, 3=doleva

        public Pole(char[,] vstupPole) // konstruktor , potřebujem jen to pole co jsme nacetli
        {
            Vyska = vstupPole.GetLength(0);//ziskame z toho pocet radku
            Sirka = vstupPole.GetLength(1);//a pocet sloupcu 
            Poles = new char[Vyska, Sirka]; //vytvorime nas prazdny maze jakoby

            for (int i = 0; i < Vyska; i++) // prakticky to same jako pri nacteni pocatecniho pole
                                            // ale jednodusi protoze uz to je z pole do pole
            {
                for (int j = 0; j < Sirka; j++)
                {
                    Poles[i, j] = vstupPole[i, j];
                }
            }
        }

        public void KrokyDvouPriser() //chuze dvou priser
        {
            // Najdi obe prisery
            Prisera prisera1 = NajdiPriseru(1);
            Prisera prisera2 = NajdiPriseru(2);

            if (prisera1 == null && prisera2 == null)
            {
                Console.WriteLine("Zadna prisera nebyla nalezena!");
                return;
            }

            int krok = 0; //pocet kroku
            int maxKroku = 20; // Zvysime limit pro dve prisery
            bool prvniKrok = true;

            while (krok < maxKroku)
            {
                krok++;

                // hodime si tam kde sme byly volny misto pro obe prisery
                if (prisera1 != null && prisera1.JeAktivni)
                    Poles[prisera1.X, prisera1.Y] = '.';
                if (prisera2 != null && prisera2.JeAktivni)
                    Poles[prisera2.X, prisera2.Y] = '.';

                // Pohyb prvni prisery
                if (prisera1 != null && prisera1.JeAktivni)
                {
                    PohniPriserou(prisera1, 1);
                    
                    // zkontrolujem jestli teda neni na startu  ,v prvnim kroku preskakujem
                    if (!prvniKrok && prisera1.JeNaStartu())
                    {
                        Console.WriteLine($"Prisera 1 se vratila na start po {krok} krocich!");
                        prisera1.JeAktivni = false;
                    }
                }

                // Pohyb druhe prisery
                if (prisera2 != null && prisera2.JeAktivni)
                {
                    PohniPriserou(prisera2, 2);
                    
                    // zkontrolujem jestli teda neni na startu  ,v prvnim kroku preskakujem
                    if (!prvniKrok && prisera2.JeNaStartu())
                    {
                        Console.WriteLine($"Prisera 2 se vratila na start po {krok} krocich!");
                        prisera2.JeAktivni = false;
                    }
                }

                // hodime na tu novou nebo stejnou pozici novou nebo stejnou sipku
                if (prisera1 != null && prisera1.JeAktivni)
                {
                    if (prisera2 != null && prisera2.JeAktivni && 
                        prisera1.X == prisera2.X && prisera1.Y == prisera2.Y)
                    {
                        // Pokud jsou na stejne pozici, zobraz kombinovany symbol
                        Poles[prisera1.X, prisera1.Y] = '@';
                    }
                    else
                    {
                        Poles[prisera1.X, prisera1.Y] = prisera1.Symbol;
                    }
                }
                
                if (prisera2 != null && prisera2.JeAktivni && 
                    !(prisera1 != null && prisera1.JeAktivni && 
                      prisera1.X == prisera2.X && prisera1.Y == prisera2.Y))
                {
                    Poles[prisera2.X, prisera2.Y] = prisera2.Symbol;
                }

                // vytiskneme krok
                Console.WriteLine($"{krok}. krok");
                ZobrazPole();
                Console.WriteLine();

                // Kontrola ukonceni - obe prisery se vratily nebo nejsou aktivni
                if ((prisera1 == null || !prisera1.JeAktivni) && 
                    (prisera2 == null || !prisera2.JeAktivni))
                {
                    Console.WriteLine("Obe prisery dokoncily svou cestu!");
                    break;
                }

                prvniKrok = false;
            }

            if (krok >= maxKroku)//zastaví fci jakmile max pocet kroku
            if (krok >= maxKroku)//zastaví fci jakmile max pocet kroku
            {
                Console.WriteLine("Dosazen maximalni pocet kroku!");
            }
        }

        private void PohniPriserou(Prisera prisera, int cisloPrisery)
        {
            var (pravyX, pravyY) = PoziceVpravo(prisera.X, prisera.Y, prisera.Smer); //skrz fce si dame do souradnice pozic u kterych se budem divat jestli tam je zed
            var (preduX, preduY) = PoziceVpredu(prisera.X, prisera.Y, prisera.Smer);

            if (!JeZed(pravyX, pravyY)) //fce co kontroluje jestli na souradnicich je zed zkontroluje tamty souradky
            {
                // Vpravo není zeď - otoč se doprava a jdi
                prisera.OtocDoprava(cisloPrisery);
                prisera.X = prisera.X + dx[prisera.Smer];
                prisera.Y = prisera.Y + dy[prisera.Smer];
            }
            else if (!JeZed(preduX, preduY))
            {
                // Vpravo je zeď, vpředu není - jdi vpřed
                prisera.X = preduX;
                prisera.Y = preduY;
            }
            else
            {
                // Vpravo i vpředu je zeď - otoč se doleva (bez pohybu)
                prisera.OtocDoleva(cisloPrisery);
            }
        }

        private Prisera NajdiPriseru(int cisloPrisery)
        {
            for (int i = 0; i < Vyska; i++)//projdem cely pole
            {
                for (int j = 0; j < Sirka; j++)
                {
                    int smer = -1;
                    
                    if (cisloPrisery == 1)
                    {
                        // Hledame prvni priseru (sipky)
                        switch (Poles[i, j])
                        {
                            case '^': smer = 0; break; 
                            case '>': smer = 1; break;
                            case 'v': smer = 2; break;
                            case '<': smer = 3; break;
                        }
                    }
                    else if (cisloPrisery == 2)
                    {
                        // Hledame druhou priseru (cisla)
                        switch (Poles[i, j])
                        {
                            case '1': smer = 0; break;
                            case '2': smer = 1; break;
                            case '3': smer = 2; break;
                            case '4': smer = 3; break;
                        }
                    }

                    if (smer != -1) //jakmille se zmeni smer tak pridame pozici na ktery se zmenil a vratime pozici
                    {
                        return new Prisera(i, j, smer, cisloPrisery);
                    }
                }
            }
            return null;
        }

        public bool JeZed(int x, int y)
        {
            if (x < 0 || x >= Vyska || y < 0 || y >= Sirka) //pokud je prisera v rozhranich pole 
                return true;

            char znak = Poles[x, y]; 
            return znak == 'X' || znak == '#' || znak == '*' || znak == '█'; // vraci to false pokud tam neni nektery z tech znaku
        }

        public (int x, int y) PoziceVpravo(int x, int y, int smer)
        {
            int pravySmer = (smer + 1) % 4; //pri divani do prava se otocime  doprava aby nam fungovalo to co sme udelli drive 
            int novyX = x + dx[pravySmer]; // tady vyuzijeme toho co jsme delali v radku 46-48 
            int novyY = y + dy[pravySmer]; //podle smeru vybereme co odecist pricist a hodi nam to souradky
            return (novyX, novyY); //returne tu pozici
        }

        public (int x, int y) PoziceVpredu(int x, int y, int smer)
        {
            int novyX = x + dx[smer]; // to same jako v tom minulem jen nemusime menit smer protoze sme rovne
            int novyY = y + dy[smer];
            return (novyX, novyY);
        }

        public void ZobrazPole() 
        {
            for (int i = 0; i < Vyska; i++)//vytiskne cely pole 
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