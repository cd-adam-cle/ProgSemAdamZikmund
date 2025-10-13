using System;

namespace BeastInLabyrinth
{
    // Enum pro směry
    public enum Direction
    {
        Up = 0,    // ^
        Right = 1, // >
        Down = 2,  // v
        Left = 3   // <
    }

    // Třída pro pozici
    public class Position
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    // Třída pro příšeru
    public class Beast
    {
        public Position Position { get; set; }
        public Direction Direction { get; set; }

        public Beast(int x, int y, Direction direction)
        {
            Position = new Position(x, y);
            Direction = direction;
        }

        // Otočení doprava
        public void TurnRight()
        {
            Direction = (Direction)(((int)Direction + 1) % 4);
        }

        // Otočení doleva
        public void TurnLeft()
        {
            Direction = (Direction)(((int)Direction + 3) % 4);
        }

        // Získání pozice před příšerou
        public Position GetForwardPosition()
        {
            switch (Direction)
            {
                case Direction.Up:
                    return new Position(Position.X, Position.Y - 1);
                case Direction.Right:
                    return new Position(Position.X + 1, Position.Y);
                case Direction.Down:
                    return new Position(Position.X, Position.Y + 1);
                case Direction.Left:
                    return new Position(Position.X - 1, Position.Y);
                default:
                    return Position;
            }
        }

        // Získání pozice napravo od příšery
        public Position GetRightPosition()
        {
            switch (Direction)
            {
                case Direction.Up:
                    return new Position(Position.X + 1, Position.Y);
                case Direction.Right:
                    return new Position(Position.X, Position.Y + 1);
                case Direction.Down:
                    return new Position(Position.X - 1, Position.Y);
                case Direction.Left:
                    return new Position(Position.X, Position.Y - 1);
                default:
                    return Position;
            }
        }

        // Pohyb dopředu
        public void MoveForward()
        {
            Position forwardPos = GetForwardPosition();
            Position.X = forwardPos.X;
            Position.Y = forwardPos.Y;
        }

        // Získání znaku pro aktuální směr
        public char GetDirectionChar()
        {
            switch (Direction)
            {
                case Direction.Up: return '^';
                case Direction.Right: return '>';
                case Direction.Down: return 'v';
                case Direction.Left: return '<';
                default: return '?';
            }
        }
    }

    // Třída pro bludiště
    public class Labyrinth
    {
        private char[,] map;
        private int width;
        private int height;
        private Beast beast;

        public Labyrinth(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.map = new char[height, width];
        }

        // Načtení mapy z řádků
        public void LoadMap(string[] mapLines)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    char cell = mapLines[y][x];
                    map[y, x] = cell;

                    // Najít příšeru
                    if (cell == '^' || cell == '>' || cell == 'v' || cell == '<')
                    {
                        Direction dir = Direction.Up;
                        switch (cell)
                        {
                            case '^': dir = Direction.Up; break;
                            case '>': dir = Direction.Right; break;
                            case 'v': dir = Direction.Down; break;
                            case '<': dir = Direction.Left; break;
                        }
                        beast = new Beast(x, y, dir);
                        map[y, x] = '.'; // Původní pozice je volná
                    }
                }
            }
        }

        // Kontrola, zda je pozice zeď
        public bool IsWall(Position pos)
        {
            if (pos.X < 0 || pos.X >= width || pos.Y < 0 || pos.Y >= height)
                return true;
            return map[pos.Y, pos.X] == 'X';
        }

        // Jeden krok příšery (algoritmus pravá ruka na zdi)
        public void MakeBeastMove()
        {
            // 1. Zkus se otočit doprava a jít dopředu
            beast.TurnRight();
            Position rightForward = beast.GetForwardPosition();
            
            if (!IsWall(rightForward))
            {
                // Můžeme jít doprava dopředu
                beast.MoveForward();
                return;
            }

            // 2. Otočit se zpět doleva a zkusit jít dopředu
            beast.TurnLeft();
            Position forward = beast.GetForwardPosition();
            
            if (!IsWall(forward))
            {
                // Můžeme jít dopředu
                beast.MoveForward();
                return;
            }

            // 3. Otočit se doleva a zkusit jít dopředu
            beast.TurnLeft();
            Position leftForward = beast.GetForwardPosition();
            
            if (!IsWall(leftForward))
            {
                // Můžeme jít doleva dopředu
                beast.MoveForward();
                return;
            }

            // 4. Otočit se ještě jednou doleva (úplně dozadu)
            beast.TurnLeft();
            Position backForward = beast.GetForwardPosition();
            
            if (!IsWall(backForward))
            {
                // Můžeme jít dozadu
                beast.MoveForward();
                return;
            }

            // Pokud se nemůžeme nikam pohnout, zůstaneme na místě
        }

        // Výpis mapy
        public void PrintMap()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (x == beast.Position.X && y == beast.Position.Y)
                    {
                        Console.Write(beast.GetDirectionChar());
                    }
                    else
                    {
                        Console.Write(map[y, x]);
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine(); // Prázdný řádek na konci
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Načtení rozměrů
            int width = int.Parse(Console.ReadLine());
            int height = int.Parse(Console.ReadLine());

            // Vytvoření bludiště
            Labyrinth labyrinth = new Labyrinth(width, height);

            // Načtení mapy
            string[] mapLines = new string[height];
            for (int i = 0; i < height; i++)
            {
                mapLines[i] = Console.ReadLine();
            }
            labyrinth.LoadMap(mapLines);

            // 20 kroků simulace
            for (int step = 1; step <= 20; step++)
            {
                labyrinth.MakeBeastMove();
                Console.WriteLine($"{step}. krok");
                labyrinth.PrintMap();
            }
        }
    }
}