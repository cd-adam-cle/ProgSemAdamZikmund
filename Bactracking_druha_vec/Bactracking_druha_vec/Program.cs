using System.IO;
using System.Linq;

namespace Bactracking_druha_vec;

class Program
{
    static string[] MozneNazvy = { "dite", "baterka", "jidlo", "voda", "nuz", "zapalovac", "obleceni", "negr" };
    static Random Rnd = new Random();

    static int MaxNalezenaEfektivita = 0;
    static List<veci> NejlepsiKombinace = new List<veci>();

    static void Main(string[] args)
    {
        List<space> batohy = nacti_pole();
        
        foreach (var batoh in batohy)
        {
            Console.WriteLine($"\n reseni pro batoh {batoh.kolikaty} (Kapacita: {batoh.Kapacita}) ===");
            
            batoh.Vsechny_vecicky = batoh.Vsechny_vecicky
                .OrderByDescending(v => v.PomernaHodnota)
                .ToList();

            MaxNalezenaEfektivita = 0;
            NejlepsiKombinace.Clear();

            SolveBranchAndBound(0, 0, 0, new List<veci>(), batoh);

            Console.WriteLine($"Nejlepsi nalezena efektivita: {MaxNalezenaEfektivita}");
            Console.WriteLine("Vybrane veci:");
            int celkovaVaha = 0;
            foreach (var vec in NejlepsiKombinace)
            {
                Console.WriteLine($" - {vec.nazev} (Vaha: {vec.vaha}, Efekt: {vec.efektivita})");
                celkovaVaha += vec.vaha;
            }
            Console.WriteLine($"Celkova vaha: {celkovaVaha}/{batoh.Kapacita}");
        }
    }

    static void SolveBranchAndBound(int index, int aktualniVaha, int aktualniEfektivita, List<veci> aktualniBatoh, space batoh)
    {
        if (aktualniVaha > batoh.Kapacita) return;

        if (aktualniEfektivita > MaxNalezenaEfektivita)
        {
            MaxNalezenaEfektivita = aktualniEfektivita;
            NejlepsiKombinace = new List<veci>(aktualniBatoh);
        }

        double bound = SpocitejBound(index, aktualniVaha, aktualniEfektivita, batoh);

        if (bound <= MaxNalezenaEfektivita)
        {
            return; 
        }

        if (index >= batoh.Vsechny_vecicky.Count) return;

        veci zvazovanaVec = batoh.Vsechny_vecicky[index];

        if (aktualniVaha + zvazovanaVec.vaha <= batoh.Kapacita)
        {
            aktualniBatoh.Add(zvazovanaVec);
            SolveBranchAndBound(index + 1, aktualniVaha + zvazovanaVec.vaha, aktualniEfektivita + zvazovanaVec.efektivita, aktualniBatoh, batoh);
            aktualniBatoh.RemoveAt(aktualniBatoh.Count - 1);
        }

        SolveBranchAndBound(index + 1, aktualniVaha, aktualniEfektivita, aktualniBatoh, batoh);
    }

    static double SpocitejBound(int index, int aktualniVaha, int aktualniEfektivita, space batoh)
    {
        double bound = aktualniEfektivita;
        int zbyvajiciKapacita = batoh.Kapacita - aktualniVaha;
        
        for (int i = index; i < batoh.Vsechny_vecicky.Count; i++)
        {
            veci vec = batoh.Vsechny_vecicky[i];

            if (zbyvajiciKapacita >= vec.vaha)
            {
                bound += vec.efektivita;
                zbyvajiciKapacita -= vec.vaha;
            }
            else
            {
                bound += (double)vec.efektivita * ((double)zbyvajiciKapacita / vec.vaha);
                break;
            }
        }

        return bound;
    }

    class veci
    {
        public string nazev { get; set; }
        public int vaha { get; set; }
        public int efektivita { get; set; }
        
        public double PomernaHodnota 
        { 
            get { return (double)efektivita / vaha; } 
        }

        public veci(string nazev, int vaha, int efektivita)
        {
            this.nazev = nazev;
            this.vaha = vaha;
            this.efektivita = efektivita;
        }
    }

    class space
    {
        public int kolikaty { get; set; }
        public int Kapacita { get; set; }
        public List<veci> Vsechny_vecicky { get; set; }

        public space(int kolikaty)
        {
            this.kolikaty = kolikaty;
            Vsechny_vecicky = new List<veci>();
        }
    }

    static List<space> nacti_pole()
    {
        List<space> seznamVybaveni = new List<space>();
        List<string> prepole = new List<string>();

        if (File.Exists("soubor.txt"))
        {
            using (StreamReader sr = new StreamReader("soubor.txt"))
            {
                string radek = "";
                int start = 7; 
                for (int i = 0; i < start; i++)
                {
                    sr.ReadLine();
                }
                while ((radek = sr.ReadLine()) != null)
                {
                    if (!string.IsNullOrEmpty(radek))
                    {
                        prepole.Add(radek);
                    }
                }
            }
        }
        else
        {
            Console.WriteLine("Soubor neexistuje!");
            return seznamVybaveni;
        }

        for (int i = 0; i < (prepole.Count / 5); i++)
        {
            int baseIndex = i * 5;
            if (baseIndex + 2 >= prepole.Count) break;

            space aktualniVybaveni = new space(i);

            string radekEfektivity = prepole[baseIndex];
            int[] efektivity = radekEfektivity.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();

            string radekVahy = prepole[baseIndex + 1];
            int[] vahy = radekVahy.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();

            string radekKapacita = prepole[baseIndex + 2];
            if (int.TryParse(radekKapacita, out int kap))
            {
                aktualniVybaveni.Kapacita = kap;
            }

            int pocetVeci = Math.Min(efektivity.Length, vahy.Length);

            for (int k = 0; k < pocetVeci; k++)
            {
                string nahodnyNazev = MozneNazvy[Rnd.Next(MozneNazvy.Length)];
                veci novaVec = new veci(nahodnyNazev, vahy[k], efektivity[k]);
                aktualniVybaveni.Vsechny_vecicky.Add(novaVec);
            }

            seznamVybaveni.Add(aktualniVybaveni);
        }

        return seznamVybaveni;
    }
}