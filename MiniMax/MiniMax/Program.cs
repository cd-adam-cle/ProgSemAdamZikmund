using System;
using System.Collections.Generic;
using System.Linq;

namespace MiniMax;
class Program
{
    static void Main(string[] args)
    {
        // Tady si muzu zmenit pocatecni hromadky, treba { 3, 4, 5 } pro delsi hru
        var initialPiles = new List<int> { 2, 2 };
        bool botStarts = true; // Kdo zacina

        var game = new NimGame(initialPiles, botStarts);
        GameState state;

        do
        {
            state = game.PlayTurn();
        } while (state == GameState.Ongoing);


        if (state == GameState.BotWon)
            Console.WriteLine("Vyhrál počítač!");
        else
            Console.WriteLine("Gratulujeme! Vyhráli jste!");
        
        // Aby se okno hned nezavrelo
        Console.ReadLine();
    }
}

public enum GameState
{
    Ongoing,
    BotWon,
    HumanWon
}

public class NimGameState
{
    public List<int> Piles { get; private set; }
    public int MatchesInGame { get; private set; }

    public NimGameState(List<int> initialPiles)
    {
        Piles = new List<int>(initialPiles);
        MatchesInGame = Piles.Sum();
    }

    public void MakeMove(int pileIndex, byte matchesToRemove)
    {
        if (IsValidMove(pileIndex, matchesToRemove))
        {
            Piles[pileIndex] -= matchesToRemove;
            MatchesInGame -= matchesToRemove;
        }
        else
        {
            throw new ArgumentException("Neplatný tah!");
        }
    }

    private bool IsValidMove(int pileIndex, byte matchesToRemove)
    {
        return pileIndex >= 0 &&
               pileIndex < Piles.Count &&
               Piles[pileIndex] >= matchesToRemove &&
               matchesToRemove > 0;
    }
}

public class NimGame
{
    private NimGameState _state; 
    private bool _botStarts;
    private bool _isBotTurn;

    public NimGame(List<int> initialPiles, bool botStarts)
    {
        _state = new NimGameState(initialPiles);
        _botStarts = botStarts;
        _isBotTurn = botStarts;
    }

    public GameState PlayTurn()
    {            
        PrintGameState();

        if (_isBotTurn)
        {
            var botMove = GetBestBotMove();
            MakeAndPrintBotMove(botMove);
        }
        else
        {
            var humanMove = GetHumanInput();
            _state.MakeMove(humanMove.Item1, humanMove.Item2);
        }

        _isBotTurn = !_isBotTurn;

        // Kontrola konce hry - kdo nema co vzit, vyhral (resp. ten kdo vzal posledni, prohral)
        if (_state.MatchesInGame == 0)
            if (_isBotTurn)
                return GameState.BotWon; // Hrac na tahu nemuze hrat -> Bot vyhral (clovek vzal posledni)
            else
                return GameState.HumanWon;
        else
            return GameState.Ongoing;
    }

    private Tuple<int, byte> GetBestBotMove()
    {
        int bestPile = 0; 
        byte matchesToRemove = 1; 

        // Hloubka prohledavani - cim vic, tim chytrejsi, ale pomalejsi. 10 bohate staci.
        int maxHloubka = 10;
        
        // Musim si pamatovat nejlepsi skore, abych vedel, ktery tah vybrat
        int bestScore = int.MinValue;

        // Projdu vsechny hromadky (tahy v koreni stromu resim tady, zbytek v rekurzi)
        for (int i = 0; i < _state.Piles.Count; i++)
        {
            // Muzu vzit 1 nebo 2 sirky, ale nesmim vzit vic nez tam je
            for (byte count = 1; count <= 2 && count <= _state.Piles[i]; count++)
            {
                // Pozor! Musim udelat kopii hromadek, jinak si rozbiju aktualni stav hry
                var nextPiles = new List<int>(_state.Piles);
                nextPiles[i] -= count;

                // Volam minimax pro soupere (human), takze minimizingPlayer = false
                // Alpha zacina na -nekonecno, Beta na +nekonecno
                int currentScore = minimax(nextPiles, maxHloubka - 1, false, int.MinValue, int.MaxValue);

                // Pokud je tenhle tah lepsi nez co jsem mel doted, ulozim si ho
                if (currentScore > bestScore)
                {
                    bestScore = currentScore;
                    bestPile = i;
                    matchesToRemove = count;
                }
            }
        }
        
        // --- Implementace Minimaxu s Alpha-Beta prorezavanim a hloubkou ---
        int minimax(List<int> piles, int depth, bool maximizingPlayer, int alpha, int beta)
        {
            // 1. Konec hry - nejsou sirky
            if (piles.Sum() == 0)
            {
                // Hrajeme Nim verzi Misere -> ten kdo vzal posledni, prohrava.
                // Pokud jsem ted na rade JA (maximizingPlayer) a nejsou sirky,
                // znamena to, ze souper vzal posledni. Takze souper prohral a JA vyhral.
                if (maximizingPlayer) return 1000; // Vyhra pro Maxe (bota)
                else return -1000;                 // Vyhra pro Min (cloveka)
            }

            // 2. Dosli jsme na konec hloubky (staticke ohodnoceni - BONUS)
            if (depth == 0)
            {
                // Nevime kdo vyhraje, tak vracim 0 (remiza/neznamo)
                return 0;
            }

            if (maximizingPlayer) // Jsem na tahu ja (Bot - Max)
            {
                int maxEval = int.MinValue;
                
                // Zkousim vsechny moznosti tahu
                for (int i = 0; i < piles.Count; i++)
                {
                    for (int count = 1; count <= 2 && count <= piles[i]; count++)
                    {
                        var simulacePiles = new List<int>(piles);
                        simulacePiles[i] -= count;

                        int eval = minimax(simulacePiles, depth - 1, false, alpha, beta);
                        maxEval = Math.Max(maxEval, eval);
                        
                        // Alpha-Beta orezavani (BONUS)
                        alpha = Math.Max(alpha, eval);
                        if (beta <= alpha)
                            break; // Beta rez - nema cenu dal hledat
                    }
                }
                return maxEval;
            }
            else // Na tahu je souper (Clovek - Min)
            {
                int minEval = int.MaxValue;

                for (int i = 0; i < piles.Count; i++)
                {
                    for (int count = 1; count <= 2 && count <= piles[i]; count++)
                    {
                        var simulacePiles = new List<int>(piles);
                        simulacePiles[i] -= count;

                        int eval = minimax(simulacePiles, depth - 1, true, alpha, beta);
                        minEval = Math.Min(minEval, eval);

                        // Alpha-Beta orezavani
                        beta = Math.Min(beta, eval);
                        if (beta <= alpha)
                            break; // Alpha rez
                    }
                }
                return minEval;
            }
        }

        return new Tuple<int, byte>(bestPile, matchesToRemove);
    }

    private void PrintGameState()
    {
        Console.WriteLine("Aktuální stav hry:");
        foreach (var pile in _state.Piles)
            Console.Write(pile + " ");
        Console.WriteLine();
    }

    private void MakeAndPrintBotMove(Tuple<int, byte> move)
    {
        _state.MakeMove(move.Item1, move.Item2);
        Console.WriteLine($"Počítač bere {move.Item2} sirky z hromádky {move.Item1}");
    }

    private Tuple<int, byte> GetHumanInput()
    {

        Console.Write("Z které hromádky chcete brát? (");
        for (int i = 0; i < _state.Piles.Count; i++)
        {
            if (_state.Piles[i] > 0)
                Console.Write($"{i} ");
        }   
        Console.Write(")");

        int pileIndex = Convert.ToInt32(Console.ReadLine());

        Console.WriteLine($"Kolik sirek chcete vzít? (1-{_state.Piles[pileIndex]})");
        byte matches = Convert.ToByte(Console.ReadLine());

        return new Tuple<int, byte>(pileIndex, matches);
    }
}