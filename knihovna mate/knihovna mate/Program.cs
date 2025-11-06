using System;
using System.Collections.Generic;

namespace Knihovna_Mate
{
    internal class Program
    {
        static void Main(string[] args)
        {

            List<MathFunction> functions = new()
            {
                new LinearFunction(2, 3),
                new AbsoluteValueLinearFunction(1, -2),
                new RationalFunction(1, 2, 1, -1),
                new QuadraticFunction(1, -2, 1)
            };

            double x = 2;
            Console.WriteLine("=== TEST===\n");
            
            foreach (var f in functions)
            {
                // Základní informace - volá se virtuální metoda
                f.PrintInfo();
                Console.WriteLine($"f({x}) = {f.Calculate(x)}");
                
                // Derivace (pokud ji funkce podporuje)
                if (f is IDifferentiable differentiable)
                {
                    Console.WriteLine($"Derivace: f'(x) = {differentiable.OutputDerivative()}");
                }
                
                // Inverze (pokud ji funkce podporuje)
                if (f is IInvertible invertible)
                {
                    Console.WriteLine($"Inverzní funkce: {invertible.OutputInversion()}");
                }
                
                Console.WriteLine(); // Prázdný řádek mezi funkcemi
            }

            Console.WriteLine("\nStiskněte libovolnou klávesu pro ukončení...");
            Console.ReadKey();
        }
    }
    
    interface IDifferentiable
    {
        string OutputDerivative();
    }

    interface IInvertible
    {
        string OutputInversion();
    }
    

    struct Interval
    {
        // PROPERTIES s gettery - enkapsulace dat
        public double UpperBoundValue { get; }
        public double LowerBoundValue { get; }
        public char UpperBoundBracket { get; }
        public char LowerBoundBracket { get; }

        /// <summary>
        /// Konstruktor intervalu
        /// </summary>
        /// <param name="lbb">Lower Bound Bracket - levá závorka '(' nebo '['</param>
        /// <param name="lbv">Lower Bound Value - dolní mez</param>
        /// <param name="ubv">Upper Bound Value - horní mez</param>
        /// <param name="ubb">Upper Bound Bracket - pravá závorka ')' nebo ']'</param>
        public Interval(char lbb, double lbv, double ubv, char ubb)
        {
            UpperBoundBracket = ubb;
            LowerBoundBracket = lbb;
            UpperBoundValue = ubv;
            LowerBoundValue = lbv;
        }

        /// <summary>
        /// Přepsání metody ToString pro pěkný výpis intervalu
        /// </summary>
        public override string ToString()
        {
            // Speciální zpracování nekonečna
            string lower = double.IsNegativeInfinity(LowerBoundValue) ? "-∞" : LowerBoundValue.ToString();
            string upper = double.IsPositiveInfinity(UpperBoundValue) ? "∞" : UpperBoundValue.ToString();
            return $"{LowerBoundBracket}{lower},{upper}{UpperBoundBracket}";
        }
    }
    /// Abstraktní základní třída pro všechny matematické funkce

    abstract class MathFunction
    {
        //  public přístup, ale kontrolovaný
        public string Name { get; }
        public string Description { get; }
        public Interval Domain { get; protected set; } // protected set - jen potomci mohou měnit
        public Interval Range { get; protected set; }


        /// Konstruktor - protected, protože tuto třídu nelze přímo instancovat
 
        protected MathFunction(string name, string description)
        {
            Name = name;
            Description = description;
        }
        /// Každá funkce počítá hodnotu jinak
        public abstract double Calculate(double x);
        
        public virtual void PrintInfo()
        {
            Console.WriteLine($"{Name}: {Description} na D(f) = {Domain}");
        }
    }

    /// Lineární funkce: f(x) = ax + b
    /// Dedicnost : odvozena z MathFunction
    /// rozhrani: implementuje IInvertible a IDifferentiable
    class LinearFunction : MathFunction, IInvertible, IDifferentiable
    {
        // zapouzdreni - private datové položky
        private double a, b;
        public LinearFunction(double a, double b) 
            : base("Lineární funkce", $"f(x) = {a}x + {b}")
        {
            this.a = a;
            this.b = b;
            // Lineární funkce je definována na celé R
            Domain = new Interval('(', double.NegativeInfinity, double.PositiveInfinity, ')');
            Range = new Interval('(', double.NegativeInfinity, double.PositiveInfinity, ')');
        }
        
        public override double Calculate(double x) => a * x + b;

        /// Implementace rozhraní IDifferentiable
        /// Derivace lineární funkce je konstanta a
        public string OutputDerivative() => $"{a}";

        /// Implementace rozhraní IInvertible
        /// Inverzní funkce: x = (y - b) / a
        public string OutputInversion()
        {
            if (a == 0)
                return "Nelze invertovat, protože a = 0 (konstanta).";
            return $"f^(-1)(x) = (x - {b}) / {a}, D(f^(-1)) = {Range}";
        }
    }

    /// Lineární funkce s absolutní hodnotou: f(x) = a|x + b| + c
    /// Tato funkce nemá inverzi (není prostá), ale má derivaci (po částech)
    class AbsoluteValueLinearFunction : MathFunction, IDifferentiable
    {
        private double a, b, c;

        public AbsoluteValueLinearFunction(double a, double b, double c = 0) 
            : base("Lineární funkce s absolutní hodnotou", $"f(x) = {a}|x + {b}| + {c}")
        {
            this.a = a;
            this.b = b;
            this.c = c;
            Domain = new Interval('(', double.NegativeInfinity, double.PositiveInfinity, ')');
            
            // Obor hodnot závisí na znaménku a
            if (a > 0)
                Range = new Interval('[', c, double.PositiveInfinity, ')');
            else
                Range = new Interval('(', double.NegativeInfinity, c, ']');
        }

        public override double Calculate(double x) => a * Math.Abs(x + b) + c;

        public string OutputDerivative()
        {
            // Derivace absolutní hodnoty existuje všude kromě bodu nespojitosti
            return $"f'(x) = {a} · sgn(x + {b}) pro x ≠ {-b}, derivace neexistuje v bodě x = {-b}";
        }
        /// PŘEPSÁNÍ (Override) virtuální metody PrintInfo
        /// Přidává dodatečné informace specifické pro tuto funkci
        public override void PrintInfo()
        {
            base.PrintInfo(); // Zavolá původní metodu z předka
            Console.WriteLine($"Poznámka: Funkce má vrchol v bodě x = {-b}");
        }
    }
    /// Lineární lomená funkce: f(x) = (ax + b) / (cx + d)
    /// Má hyperbolický průběh a asymptoty
    class RationalFunction : MathFunction, IDifferentiable
    {
        private double a, b, c, d;

        public RationalFunction(double a, double b, double c, double d) 
            : base("Lineární lomená funkce", $"f(x) = ({a}x + {b}) / ({c}x + {d})")
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
            
            // Definiční obor: vše kromě kořene jmenovatele (x ≠ -d/c)
            Domain = new Interval('(', double.NegativeInfinity, double.PositiveInfinity, ')');
            
            // Obor hodnot: vše kromě horizontální asymptoty (y ≠ a/c)
            Range = new Interval('(', double.NegativeInfinity, double.PositiveInfinity, ')');
        }

        public override double Calculate(double x)
        {
            double denominator = c * x + d;
            if (Math.Abs(denominator) < 0.0001)
                throw new DivideByZeroException($"Funkce není definována v bodě x = {x}");
            return (a * x + b) / denominator;
        }

        public string OutputDerivative()
        {
            // Derivace (ax+b)/(cx+d) = (ad-bc)/(cx+d)²
            double numerator = a * d - b * c;
            return $"f'(x) = {numerator} / ({c}x + {d})²";
        }

        public override void PrintInfo()
        {
            base.PrintInfo();
            double verticalAsymptote = -d / c;
            double horizontalAsymptote = a / c;
            Console.WriteLine($"Poznámka: Funkce má hyperbolický průběh");
            Console.WriteLine($"  Svislá asymptota: x = {verticalAsymptote}");
            Console.WriteLine($"  Vodorovná asymptota: y = {horizontalAsymptote}");
        }
    }

    /// Kvadratická funkce: f(x) = ax² + bx + c
    /// Má parabolický průběh
   
    class QuadraticFunction : MathFunction, IDifferentiable
    {
        private double a, b, c;

        public QuadraticFunction(double a, double b, double c) 
            : base("Kvadratická funkce", $"f(x) = {a}x² + {b}x + {c}")
        {
            this.a = a;
            this.b = b;
            this.c = c;
            Domain = new Interval('(', double.NegativeInfinity, double.PositiveInfinity, ')');
            
            // Vrchol paraboly: x = -b/(2a), y = f(x_vrchol)
            double xVertex = -b / (2 * a);
            double yVertex = Calculate(xVertex);
            
            // Obor hodnot závisí na znaménku a (parabola otevřená nahoru/dolů)
            if (a > 0)
                Range = new Interval('[', yVertex, double.PositiveInfinity, ')');
            else
                Range = new Interval('(', double.NegativeInfinity, yVertex, ']');
        }

        public override double Calculate(double x) => a * x * x + b * x + c;

        public string OutputDerivative() => $"f'(x) = {2 * a}x + {b}";

        public override void PrintInfo()
        {
            base.PrintInfo();
            double xVertex = -b / (2 * a);
            double yVertex = Calculate(xVertex);
            string direction = a > 0 ? "nahoru" : "dolů";
            Console.WriteLine($"Poznámka: Funkce má parabolický průběh (otevřená {direction})");
            Console.WriteLine($"  Vrchol paraboly: V = [{xVertex}, {yVertex}]");
        }
    }
}