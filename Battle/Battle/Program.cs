namespace Battle;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
    }
}
public class Human
{
    protected string Name { get; set; }
    protected int Health { get; set; }
    protected int Power { get; set; }


    public Human(string name)
    {
        string Name = name;
    }
    public void Attack (int health, int power) {

    }
    public class Wizzard : Human
    {
        public Wizzard(string name) : base(name)
        {
            int Health = 5;
            int Power = 10;
        }
    }
    public class Warrior : Human
    {
        public Warrior(string name) : base(name)
        {
            int Health = 10;
            int Power = 7;
        }
    }
    public class Archer : Human
    {
        public Archer(string name) : base(name)
        {
            int Health = 7;
            int Power = 8;
        }
    }
    
}