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
        public static readonly decimal[] coins = new decimal[3] { 0.05m, 0.10m, 0.25m }; // pentru a adauga noi monede acceptate
        public static readonly int[] coinsavailable = new int[3] { 0, 0, 0 }; // pentru referinta, sau daca vrem sa folosim un anumit nr de monede disponibile in aparat in intreaga functionare a acestuia.
        public static readonly decimal cost = 0.20m; // pentru a schimba costul produsului
        public static readonly decimal balance = 3m; // pentru a schimba banii de buzunar
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
        public void RequestN()
        {
            this._state.HandleN(ref Coins.coins[0]);
        }
        public void RequestD()
        {
            this._state.HandleD(ref Coins.coins[1]);
        }
        public void RequestQ()
        {
            this._state.HandleQ(ref Coins.coins[2]);
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

        public abstract void HandleQ(ref decimal input);
        public abstract void HandleD(ref decimal input);
        public abstract void HandleN(ref decimal input);
    }

    // Concrete States implement various behaviors, associated with a state of
    // the Context.
    class ConcreteStateA : State // 0.00 IN APARAT
    {
        public override void HandleQ(ref decimal input)
        {
            Console.WriteLine($"Ati introdus {input} in aparat.");
            Console.WriteLine("Ati primit un produs.");
            Console.WriteLine("Ati primit rest: 1x 0.05 .");
            Console.WriteLine("Aparatul este gol.");
            this._context.TransitionTo(new ConcreteStateA());
        }
        public override void HandleD(ref decimal input)
        {
            Console.WriteLine($"Ati introdus {input} in aparat.");
            Console.WriteLine("Mai introduceti o moneda!.");
            this._context.TransitionTo(new ConcreteStateC());
        }
        public override void HandleN(ref decimal input)
        {
            Console.WriteLine($"Ati introdus {input} in aparat.");
            Console.WriteLine("Mai introduceti o moneda!.");
            this._context.TransitionTo(new ConcreteStateB());
        }
    }

    class ConcreteStateB : State // 0.05 IN APARAT
    {
        public override void HandleQ(ref decimal input)
        {
            Console.WriteLine($"Ati introdus {input} in aparat.");
            Console.WriteLine("Ati primit un produs.");
            Console.WriteLine("Ati primit rest: 1x 0.10 .");
            Console.WriteLine("Aparatul este gol.");
            this._context.TransitionTo(new ConcreteStateA());
        }
        public override void HandleD(ref decimal input)
        {
            Console.WriteLine($"Ati introdus {input} in aparat.");
            Console.WriteLine("In aparat este 0.15.");
            Console.WriteLine("Mai introduceti o moneda!.");
            this._context.TransitionTo(new ConcreteStateD());
        }
        public override void HandleN(ref decimal input)
        {
            Console.WriteLine($"Ati introdus {input} in aparat.");
            Console.WriteLine("In aparat este 0.10 .");
            Console.WriteLine("Mai introduceti o moneda!.");
            this._context.TransitionTo(new ConcreteStateC());
        }
    }
    class ConcreteStateC : State // IN APARAT ESTE 0.10
    {
        public override void HandleQ(ref decimal input)
        {
            Console.WriteLine($"Ati introdus {input} in aparat.");
            Console.WriteLine("Ati primit un produs.");
            Console.WriteLine("Ati primit rest: 1x 0.10, 1x 0.05 .");
            Console.WriteLine("Aparatul este gol.");
            this._context.TransitionTo(new ConcreteStateA());
        }
        public override void HandleD(ref decimal input)
        {
            Console.WriteLine($"Ati introdus {input} in aparat.");
            Console.WriteLine("Ati primit un produs.");
            Console.WriteLine("Aparatul este gol.");
            this._context.TransitionTo(new ConcreteStateA());
        }
        public override void HandleN(ref decimal input)
        {
            Console.WriteLine($"Ati introdus {input} in aparat.");
            Console.WriteLine("In aparat este 0.15 .");
            Console.WriteLine("Mai introduceti o moneda!.");
            this._context.TransitionTo(new ConcreteStateD());
        }
    }
    class ConcreteStateD : State // in APARAT ESTE 0.15
    {
        public override void HandleQ(ref decimal input)
        {
            Console.WriteLine($"Ati introdus {input} in aparat.");
            Console.WriteLine("Ati primit un produs");
            Console.WriteLine("Ati primit rest: 1x 0.10, 1x 0.05");
            Console.WriteLine("In aparat este 0.05");
            this._context.TransitionTo(new ConcreteStateB());
        }
        public override void HandleD(ref decimal input)
        {
            Console.WriteLine($"Ati introdus {input} in aparat.");
            Console.WriteLine("Ati primit un produs");
            Console.WriteLine("Ati primit rest: 1x 0.05");
            Console.WriteLine("Aparatul este gol.");
            this._context.TransitionTo(new ConcreteStateA());
        }
        public override void HandleN(ref decimal input)
        {
            Console.WriteLine($"Ati introdus {input} in aparat.");
            Console.WriteLine("Ati primit un produs.");
            Console.WriteLine("Aparatul este gol.");
            this._context.TransitionTo(new ConcreteStateA());
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // The client code.
            var context = new Context(new ConcreteStateA());
            while (true)
            {
                Console.WriteLine("Introduceti o moneda! ( Q: 0.25 | D: 0.10 | N: 0.05 ).");
                string Button = Console.ReadLine();
                Console.Clear();
                switch (Button)
                {
                    case "Q":
                        context.RequestQ();
                        break;
                    case "D":
                        context.RequestD();
                        break;
                    case "N":
                        context.RequestN();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
