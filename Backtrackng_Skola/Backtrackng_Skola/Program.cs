namespace Backtrackng_Skola;

class Program
{
    static void Main(string[] args)
    {
        Zalozeni();
    }

    static void Zalozeni()
    {
        Console.WriteLine("napiste vstupni informace , nejdrive typy minci :");
        string[] prvni = Console.ReadLine().Split(' ');
        Console.WriteLine("a ted sumu:");
        string niger = Console.ReadLine();
        int druhy = int.Parse(niger);
        int[] typy_minci = new int [prvni.Length];
        for (int i = 0; i < prvni.Length; i++)
        {
            typy_minci[i] = int.Parse(prvni[i]);
        }
        Mincicky(typy_minci , druhy);
        return;
    }
    
    
    static void Mincicky(int[] mince, int castka)
    {
        List<List<int>> vsechnaReseni = new List<List<int>>();
        List<int> aktualniSesti = new List<int>();
    
        Backtrack(mince, castka, 0, aktualniSesti, vsechnaReseni);
    
        Console.WriteLine($"Částka {castka}, mince {string.Join(",", mince)}:");
        if (vsechnaReseni.Count == 0)
        {
            Console.WriteLine("Žádné řešení.");
        }
        else
        {
            for (int i = 0; i < vsechnaReseni.Count; i++)
            {
                Console.WriteLine($"  Řešení {i + 1}: {string.Join(" + ", vsechnaReseni[i])}");
            }
        }
        Console.WriteLine($"Celkem: {vsechnaReseni.Count} řešení\n");
    }

    static void Backtrack(
        int[] mince, 
        int zbytek,           
        int startIdx,         
        List<int> cesta,     
        List<List<int>> vsechnaReseni)
    {

        if (zbytek == 0)
        {
            vsechnaReseni.Add(new List<int>(cesta));  
            return;
        }
        if (zbytek < 0)
            return;
    
        for (int i = startIdx; i < mince.Length; i++)
        {
            cesta.Add(mince[i]);
        
            Backtrack(mince, zbytek - mince[i], i, cesta, vsechnaReseni);
        
            cesta.RemoveAt(cesta.Count - 1);
        }
    }


}

