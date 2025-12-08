using System.Runtime.ExceptionServices;

nakalib

namespace Pisemná_práce
{
    internal class Program
    {
        static void Main(string[] args)
        {
        Hopsajici_Konik Cvalda = new Hopsajici_Konik();
        }

        class Hopsajici_Konik 
        {
            public int[,] Pole {get ; private set ;}
           public List<int> Prekazky { get; private set; }
           public List<int> Start{ get; private set; }
            public Hopsajici_Konik()
            {
                using (StreamReader soubor = new StreamReader("T:\\Mazna_IVT\\PrgSem2\\vstupni_soubory\\1.txt"))
                {
                    string[] nacteny_soubor = soubor.ReadAllLines();
                    string[] prvni_radek = nacteny_soubor[0].Split(' ');
                    int prekazky_count =  prvni_radek.int.Parse();
                    List<int> Prekazky= new List<int>(prekazky_count);
                    for (int i = 1; i <= prekazky_count; i++)
                    {
                        Prekazky[i-1] = nacteny_soubor[i].Split(" ").int.Parse ;
                    }
                    List<int> Start = new List<int>();
                    string[] lastline = nacteny_soubor[soubor.Length()].Split();
                    Start.Add(lastline[0]);
                    List<int> endeslus = new List<int>();
                    endeslus.Add(lastline[1]);
                }
                //pocatek v startu a vememe si mozne pohyby kone z pole start ,vytvorime si dictionary v kterem budou vktory pohybu kone),timpadem si pak souradnice kam muze skakat muzeme vyjadrovat spise jeho pohybama int (int , int ) 
                //fce na přepočítávání z tahu na souradnice by vzala start apricetla by k nemu vektory ktere jsou definovany v dictionary cisly
                //udelame si fci ktera bude ukladat mozne suradnice kam kun muze v tom tahu , a bude to ukladat jako souvislost tahu , ne jako souradnice jako takove  ,ale ulozi tah jen jakmile se prepocet souradnici nebude rovnat prekazce
                //v teto fci by se upadtoval list s moznymi kroky 
                //pobezi dokud nebude zpetny vypocet z fce na prepocitavani tahu vychazet na cil



            }
        }
    }
}
