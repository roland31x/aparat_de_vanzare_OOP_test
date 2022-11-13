using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Cache;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace aparat_de_vanzare_obiect_test
{
    // https://refactoring.guru/design-patterns/state/csharp/example - folosind de sablonul dat.

    // The Context defines the interface of interest to clients. It also
    // maintains a reference to an instance of a State subclass, which
    // represents the current state of the Context.
    public class Coins
    {
        public static readonly decimal[] coins = { 0.05m, 0.10m, 0.25m }; // pentru a adauga noi monede acceptate
        public static readonly int[] coinsavailable = { 0, 0, 0 }; // pentru referinta, sau daca vrem sa folosim un anumit nr de monede disponibile in aparat in intreaga functionare a acestuia.
        public static readonly decimal cost = 0.20m; // pentru a schimba costul produsului
        public static readonly decimal balance = 3m; // pentru a schimba banii de buzunar
    }
    class Rest
    {
        public static decimal RestCalc(decimal input) // calculeaza cat rest trebuie sa dea / cat poate da
        {
            decimal r;
            r = input;
            int[] coinsgiven = new int[3] { 0, 0, 0 }; // monede date ca rest
            int[] coinsavailable = new int[3] { 1, 1, 0 }; //  { nickels, dimes , quarters } 
                                                           // monede disponibile de dat ca rest intr-un ciclu.     
            for (int i = 0; i < Coins.coins.Length; i++)
            {
                while (r > 0 && coinsavailable[i] > 0)
                {
                    coinsgiven[i]++;
                    coinsavailable[i]--;
                    r -= Coins.coins[i];
                }
            }
            Console.WriteLine();
            Console.Write($"Aparatul a dat rest:");
            for (int j = 0; j < Coins.coins.Length; j++)
            {
                if (coinsgiven[j] > 0)
                {
                    if (j > 0)
                    {
                        Console.Write(",");
                    }
                    Program.output[j] = true;
                    Console.Write($" {coinsgiven[j]} x {Coins.coins[j]} ");
                }              
            }
            Console.WriteLine();
            //for (int i = 0; i < Coins.coins.Length; i++)
            //{
            //    balance += coinsgiven[i] * Coins.coins[i];
            //}
            input = r;
            Console.WriteLine("In aparat este: " + input);// se actualizeaza banii ramasi in aparat.
            return input;
        }
    }
    class Context
    {
        // A reference to the current state of the Context.
        private State _state = null;

        public Context(State state)
        {
            this.TransitionTo(state);
        }

        // The Context allows changing the State object at runtime.
        public void TransitionTo(State state)
        {
            //Console.WriteLine($"Context: Transition to {state.GetType().Name}.");
            this._state = state;
            this._state.SetContext(this);
        }

        // The Context delegates part of its behavior to the current State
        // object.
        public void Request(int n)
        {
            this._state.Handle(Coins.coins[n]);
        }
    }   
    // The base State class declares methods that all Concrete State should
    // implement and also provides a backreference to the Context object,
    // associated with the State. This backreference can be used by States to
    // transition the Context to another State.
    abstract class State
    {
        protected Context _context;

        public void SetContext(Context context)
        {
            this._context = context;
        }

        public abstract void Handle(decimal input);
    }
    

    // Concrete States implement various behaviors, associated with a state of
    // the Context.
    class ConcreteStateA : State // 0.00 IN APARAT
    {
        public override void Handle(decimal input)
        {
            Console.WriteLine($"Ati introdus in aparat: {input}");
            if(input >= Coins.cost)
            {
                Console.WriteLine("Ati primit un produs");
                Program.returnMerch = true;
                input -= Coins.cost;
                if(input > 0)
                {
                    input = Rest.RestCalc(input);
                }
                else
                {
                    Console.WriteLine("Aparatul este gol.");
                }
            }
            else
            {
                //Console.WriteLine("In aparat este: " + input);
                Console.WriteLine("Mai introduceti o moneda!");
            }
            switch (input) 
            {
                case 0m:                   
                    this._context.TransitionTo(new ConcreteStateA());
                    break;
                case 0.05m:
                    this._context.TransitionTo(new ConcreteStateB());
                    break;
                case 0.10m:
                    this._context.TransitionTo(new ConcreteStateC());
                    break;
                case 0.15m:
                    this._context.TransitionTo(new ConcreteStateD());
                    break;
            }
        }
    }

    class ConcreteStateB : State // 0.05 
    {
        private readonly decimal balance = 0.05m;
        public override void Handle(decimal input)
        {
            input += balance;
            ConcreteStateA B = new ConcreteStateA();
            B.SetContext(this._context);
            B.Handle(input);
        }
    }
    class ConcreteStateC : State // IN APARAT ESTE 0.10
    {
        private readonly decimal balance = 0.10m;
        public override void Handle(decimal input)
        {
            input += balance;
            ConcreteStateA B = new ConcreteStateA();
            B.SetContext(this._context);
            B.Handle(input);
        }
    }
    class ConcreteStateD : State // in APARAT ESTE 0.15
    {
        private readonly decimal balance = 0.15m;
        public override void Handle(decimal input)
        {
            input += balance;
            ConcreteStateA B = new ConcreteStateA();
            B.SetContext(this._context);
            B.Handle(input);
        }
    }

    class Program
    {
        public static bool returnMerch = false; // RETURN MERCH 
        public static bool[] output = { false, false }; // RETURN NICKEL / RETURN DIME
        static void Main(string[] args)
        {
            // The client code.
            var context = new Context(new ConcreteStateA());
            while (true)
            {
                //Console.WriteLine("Dime: " + Convert.ToString(returnD));
                //Console.WriteLine("Nickel: " + Convert.ToString(returnN));
                //Console.WriteLine("Produs: "+ Convert.ToString(returnMerch));
                Console.WriteLine("Introduceti o moneda! ( Q: 0.25 | D: 0.10 | N: 0.05 ).");
                string Button = Console.ReadLine();
                Console.Clear();
                switch (Button)
                {
                    case "Q":
                        context.Request(2);
                        break;
                    case "D":
                        context.Request(1);
                        break;
                    case "N":
                        context.Request(0);
                        break;
                    default:
                        break;
                }
            }
        }
     
    }
}
