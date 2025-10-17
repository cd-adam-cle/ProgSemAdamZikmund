using System;
using System.Collections.Generic;

namespace Prisera01
{
    internal class Program
    {
        static void Main(string[] args)
        {
            char[,] pole = Dostanipole(); // základní pole a rozměry
            Pole Maze = new Pole(pole); //vytvoření Konstrukce Pole s nazvem Maze
            Maze.ZobrazPole();//zobrazeni pred kroky
            Maze.Kroky();//prisera chodi
        }

        static char[,] Dostanipole() 
        {
            //nacteme vse abysme to pak mohli využít k konstrukci pole
            Console.WriteLine("Zadejte šířku:");
            int sirka = int.Parse(Console.ReadLine());
            Console.WriteLine("Zadejte výšku:");
            int vyska = int.Parse(Console.ReadLine());
            Console.WriteLine(" bludiště:");
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

    class Pole //třída pole
    {
        public int Sirka { get; set; } 
        public int Vyska { get; set; }
        public char[,] Poles { get; private set; }

        
        //využijeme pak k lehkému přičítání/odčítání při změně pozic příšery
        private int[] dx = { -1, 0, 1, 0 }; //zmena x podle toho na jakou stranu je sipka nasmerovana
        private int[] dy = { 0, 1, 0, -1 };//zmena y -=-
        private char[] sipky = { '^', '>', 'v', '<' };//pro lehci zmenu smeru sipky
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

        public void Kroky() //chůze
        {
            List<int> pozice = NajdiSipku(); // list na pozici a smer sipky
                                             // Najdi sipku hodi do pozice tri hodnoty
                                             //[x pozice ,y pozice , smer]
            if (pozice.Count == 0)
            {
                Console.WriteLine("Šipka neni!"); //kdyby nahodou nebyla prisera
                return;
            }

            int x = pozice[0];
            int y = pozice[1]; 
            int smer = pozice[2];
            
            int startX = x; //pozice kde prisera zacinala abysme nechodili furt dokruhu
            int startY = y ;//duležitý je směr který se taé musí shodovat
                            //kdyby se neshodoval muže jen jit druhym smerem
            int startSmer = smer;
            
            int krok = 0; //pocet kroku
            int maxKroku = 20;  
            bool prvniKrok = true;

            while (krok < maxKroku)
            {
                krok++;
                
                var (pravyX, pravyY) = PoziceVpravo(x, y, smer); //skrz fce si dame do souradnice pozic u kterych se budem divat jestli tam je zed
                var (preduX, preduY) = PoziceVpredu(x, y, smer);

                Poles[x, y] = '.'; // hodime si tam kde sme byly volny misto 

                if (!JeZed(pravyX, pravyY)) //fce co kontroluje jestli na souradnicich je zed zkontroluje tamty souradky
                {
                    // Vpravo není zeď - otoč se doprava a jdi
                    smer = (smer + 1) % 4;
                    x = x + dx[smer];
                    y = y + dy[smer];
                }
                else if (!JeZed(preduX, preduY))
                {
                    // Vpravo je zeď, vpředu není - jdi vpřed
                    x = preduX;
                    y = preduY;
                }
                else
                {
                    // Vpravo i vpředu je zeď - otoč se doleva (bez pohybu))
                    smer = (smer + 3) % 4;
                }

                // hodime na tu novou nebo stejnou pozici novou nebo stejnou sipku 
                Poles[x, y] = sipky[smer];

                // vytiskneme krok
                Console.WriteLine($"{krok}. krok");
                ZobrazPole();
                Console.WriteLine();

                // zkontrolujem jestli teda neni na startu  ,v prvnim kroku preskakujem
                if (!prvniKrok && x == startX && y == startY && smer == startSmer)
                {
                    Console.WriteLine($"Šipka se vrátila na start po {krok} krocích!");
                    break;
                }
                
                prvniKrok = false;
            }

            if (krok >= maxKroku)//zastaví fci jakmile max pocet kroku 
            {
                Console.WriteLine("Dosažen maximální počet kroků!");
            }
        }

        public List<int> NajdiSipku()
        {
            List<int> pozice = new List<int>();//pozicni cisla

            for (int i = 0; i < Vyska; i++)//projdem cely pole
            {
                for (int j = 0; j < Sirka; j++)
                {
                    int smer = -1;
                    switch (Poles[i, j])
                    {
                        case '^': smer = 0; break; 
                        case '>': smer = 1; break;
                        case 'v': smer = 2; break;
                        case '<': smer = 3; break;
                    }

                    if (smer != -1) //jakmille se zmeni smer tak pridame pozici na ktery se zmenil a vratime pozici
                    {
                        pozice.Add(i);
                        pozice.Add(j);
                        pozice.Add(smer);
                        return pozice;
                    }
                }
            }
            return pozice;
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