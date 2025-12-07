using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Priprava_na_maturitni_priklad
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] radky = File.ReadAllLines("files.txt");

            try
            {
                Graficek mujGraf = new Graficek(radky);
                Console.WriteLine($"Počet měst: {mujGraf.PocetMest}");
                Console.WriteLine($"Počet silnic: {mujGraf.PocetSilnic}");
                Console.WriteLine($"Hledáme cestu z {mujGraf.Start} do {mujGraf.Cil}");
                mujGraf.NajdiNejkratsiCestu();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Chyba : {ex.Message}");
            }
            Console.ReadKey(); 
        }
    }

    class Graficek
    {
        public int PocetMest { get; private set; }
        public int PocetSilnic { get; private set; }
        public int Start { get; private set; }
        public int Cil { get; private set; }

        private Dictionary<int, List<(int soused, int delka, int cena)>> _seznamSousedu;

        public Graficek(string[] vstupniData)
        {
            if (vstupniData == null || vstupniData.Length == 0)
            {
                throw new ArgumentException("Špatný vstup");
            }
            _seznamSousedu = new Dictionary<int, List<(int soused, int delka, int cena)>>();
            OvereniDat(vstupniData);
        }

        private void OvereniDat(string[] radky)
        {
            string[] PrvniRadek = radky[0].Split(' ');

            if (PrvniRadek.Length != 2 || 
                !int.TryParse(PrvniRadek[0], out int PM) || 
                !int.TryParse(PrvniRadek[1], out int PS) || 
                PM < 0 || PS < 0)
            {
                throw new ArgumentException("špatný vstup - chyba v hlavičce");
            }

            PocetSilnic = PS;
            PocetMest = PM;

            for (int i = 0; i < PocetMest; i++)
            {
                _seznamSousedu[i] = new List<(int, int, int)>();
            }

            if (radky.Length < PocetSilnic + 2)
            {
                throw new ArgumentException("špatný vstup - chybí definice silnic nebo start/cíl");
            }

            for (int i = 1; i <= PocetSilnic; i++)
            {
                string[] casti = radky[i].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (casti.Length != 4)
                    throw new ArgumentException($"Špatný vstup na řádku {i}");

                if (!int.TryParse(casti[0], out int Mesto_Odkud) ||
                    !int.TryParse(casti[1], out int Mesto_Kam) ||
                    !int.TryParse(casti[2], out int Vzdalenost) ||
                    !int.TryParse(casti[3], out int Cena))
                {
                    throw new ArgumentException("Špatný vstup - čísla silnic");
                }
                PridejSilnici(Mesto_Odkud, Mesto_Kam, Vzdalenost, Cena);
            }

            int indexPoslednihoRadku = PocetSilnic + 1;
            string[] posledniRadek = radky[indexPoslednihoRadku].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (posledniRadek.Length != 2 ||
                !int.TryParse(posledniRadek[0], out int startMesto) ||
                !int.TryParse(posledniRadek[1], out int cilMesto))
            {
                throw new ArgumentException("Špatný vstup ");
            }

            if (startMesto < 0 || startMesto >= PocetMest || cilMesto < 0 || cilMesto >= PocetMest)
            {
                throw new ArgumentException("Špatný vstup");
            }

            Start = startMesto;
            Cil = cilMesto;
        }

        private void PridejSilnici(int Mesto_Odkud, int Mesto_Kam, int Vzdalenost, int Cena)
        {
            if (!_seznamSousedu.ContainsKey(Mesto_Odkud) || !_seznamSousedu.ContainsKey(Mesto_Kam))
            {
                 throw new ArgumentException("Město mimo rozsah");
            }

            if (!_seznamSousedu[Mesto_Odkud].Any(x => x.soused == Mesto_Kam))
            {
                _seznamSousedu[Mesto_Odkud].Add((Mesto_Kam, Vzdalenost, Cena));
                _seznamSousedu[Mesto_Kam].Add((Mesto_Odkud, Vzdalenost, Cena));
            }
        }

        public void NajdiNejkratsiCestu()
{
    //  Pole vzdáleností [město, početPlacených]
    int[,] vzdalenost = new int[PocetMest, 2];
    
    // Pole předchůdců pro rekonstrukci cesty [město, početPlacených]
    (int mesto, int placene)[,] predchudce = new (int, int)[PocetMest, 2];
    
    // všechny vzdálenosti na nekonečno
    for (int i = 0; i < PocetMest; i++)
    {
        vzdalenost[i, 0] = int.MaxValue;
        vzdalenost[i, 1] = int.MaxValue;
        predchudce[i, 0] = (-1, -1);  // -1 = nemá předchůdce
        predchudce[i, 1] = (-1, -1);
    }
    
    // Start má vzdálenost 0 (bez placené silnice)
    vzdalenost[Start, 0] = 0;
    
    // Prioritní fronta: (město, početPlacených), priorita = vzdálenost
    var fronta = new PriorityQueue<(int mesto, int placene), int>();
    fronta.Enqueue((Start, 0), 0);
    
    while (fronta.Count > 0)
    {
        // Vytáhni město s nejmenší vzdáleností
        var (mesto, placene) = fronta.Dequeue();
        
        // Pokud už jsme v cíli, můžeme skončit
        if (mesto == Cil)
        {
            break;
        }
        
        // Pokud jsme tuto kombinaci už zpracovali s lepší vzdáleností, přeskoč
        if (vzdalenost[mesto, placene] < vzdalenost[mesto, placene])
        {
            continue;
        }
        
        // Projdi všechny sousedy
        foreach (var (soused, delka, cena) in _seznamSousedu[mesto])
        {
            int novePlacene = placene + cena;
            
            // Pokud bychom použili víc než 1 placenou, přeskoč
            if (novePlacene > 1)
            {
                continue;
            }
            
            // Spočítej novou vzdálenost
            int novaVzdalenost = vzdalenost[mesto, placene] + delka;
            
            // Pokud je nová cesta kratší, aktualizuj
            if (novaVzdalenost < vzdalenost[soused, novePlacene])
            {
                vzdalenost[soused, novePlacene] = novaVzdalenost;
                predchudce[soused, novePlacene] = (mesto, placene);
                fronta.Enqueue((soused, novePlacene), novaVzdalenost);
            }
        }
    }
    
    // Najdi lepší výsledek (s 0 nebo 1 placenou)
    int vyslednaVzdalenost;
    int vyslednyStav;
    
    if (vzdalenost[Cil, 0] <= vzdalenost[Cil, 1])
    {
        vyslednaVzdalenost = vzdalenost[Cil, 0];
        vyslednyStav = 0;
    }
    else
    {
        vyslednaVzdalenost = vzdalenost[Cil, 1];
        vyslednyStav = 1;
    }
    
    // Zkontroluje jestli cesta existuje
    if (vyslednaVzdalenost == int.MaxValue)
    {
        Console.WriteLine("Cesta neexistuje");
        return;
    }
    
    // Rekonstrukce cesty (od cíle ke startu)
    List<int> cesta = new List<int>();
    int aktualniMesto = Cil;
    int aktualniPlacene = vyslednyStav;
    
    while (aktualniMesto != -1)
    {
        cesta.Add(aktualniMesto);
        var (predMesto, predPlacene) = predchudce[aktualniMesto, aktualniPlacene];
        aktualniMesto = predMesto;
        aktualniPlacene = predPlacene;
    }
    
    // je to jakoby naopak tak to musime otocit
    cesta.Reverse();
    
    // writee
    Console.WriteLine(string.Join(" -> ", cesta));
    Console.WriteLine($"vzdálenost: {vyslednaVzdalenost}");
}
    }
}