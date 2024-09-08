using System;
using System.Collections.Generic;
using System.Threading;

namespace Labb1DesignPattern.SingletonAdapterObserverIntegration
{
    // Singleton-klassen.
    public sealed class Singleton
    {
        private Singleton() { }

        private static Singleton _instance;

        public static Singleton GetInstance()
        {
            if (_instance == null)
            {
                _instance = new Singleton();
            }
            return _instance;
        }

        public void someBusinessLogic()
        {
            Console.WriteLine("Singleton logik utförd.");
        }
    }

    // Target-gränssnittet för Adapter-mönstret.
    public interface ITarget
    {
        string GetRequest();
    }

    // Adaptee-klassen för Adapter-mönstret.
    class Adaptee
    {
        public string GetSpecificRequest()
        {
            return "Specifik begäran.";
        }
    }

    // Adapter-klassen som kopplar Adaptee till ITarget-gränssnittet.
    class Adapter : ITarget
    {
        private readonly Adaptee _adaptee;

        public Adapter(Adaptee adaptee)
        {
            this._adaptee = adaptee;
        }

        public string GetRequest()
        {
            return $"Detta är '{this._adaptee.GetSpecificRequest()}'";
        }
    }

    // Observer-gränssnittet, som definierar en metod för att få uppdateringar från Subject.
    public interface IObserver
    {
        void Update(ISubject subject);
    }

    // Subject-gränssnittet som definierar metoder för att hantera observatörer.
    public interface ISubject
    {
        void Attach(IObserver observer);
        void Detach(IObserver observer);
        void Notify();
    }

    // Subject-klassen som hanterar observatörer och skickar notifieringar när tillståndet ändras.
    public class Subject : ISubject
    {
        public int State { get; set; } = -0;  // Subject's state som kan observeras.
        private List<IObserver> _observers = new List<IObserver>();  // Lista över observatörer.

        // Lägger till en observatör.
        public void Attach(IObserver observer)
        {
            Console.WriteLine("Subject: Attached an observer.");
            this._observers.Add(observer);
        }

        // Tar bort en observatör.
        public void Detach(IObserver observer)
        {
            this._observers.Remove(observer);
            Console.WriteLine("Subject: Detached an observer.");
        }

        // Notifierar alla observatörer om en förändring i state.
        public void Notify()
        {
            Console.WriteLine("Subject: Notifying observers...");

            foreach (var observer in _observers)
            {
                observer.Update(this);
            }
        }

        // Någon affärslogik som också kan leda till en notifiering av observatörer.
        public void SomeBusinessLogic()
        {
            Console.WriteLine("\nSubject: I'm doing something important.");
            this.State = new Random().Next(0, 10);

            Thread.Sleep(15);

            Console.WriteLine("Subject: My state has just changed to: " + this.State);
            this.Notify();  // Skickar notifiering till alla observatörer.
        }
    }

    // En konkret observerare som reagerar när Subject:s state ändras.
    class ConcreteObserverA : IObserver
    {
        public void Update(ISubject subject)
        {
            if ((subject as Subject).State < 3)
            {
                Console.WriteLine("ConcreteObserverA: Reacted to the event.");
            }
        }
    }

    // En annan konkret observerare som reagerar vid andra tillståndsändringar.
    class ConcreteObserverB : IObserver
    {
        public void Update(ISubject subject)
        {
            if ((subject as Subject).State == 0 || (subject as Subject).State >= 2)
            {
                Console.WriteLine("ConcreteObserverB: Reacted to the event.");
            }
        }
    }

    // Klassen som kopplar ihop Singleton, Adapter och Observer-mönstren.
    class SingletonAdapterObserverProgram
    {
        static void Main(string[] args)
        {
            // Singleton-logik.
            Singleton singleton = Singleton.GetInstance();
            singleton.someBusinessLogic();

            // Adapter-logik.
            Adaptee adaptee = new Adaptee();
            ITarget target = new Adapter(adaptee);

            Console.WriteLine("\nAdaptee-gränssnittet är inkompatibelt med klienten.");
            Console.WriteLine("Men med adapter kan klienten anropa dess metod.");
            Console.WriteLine(target.GetRequest());

            // Observer-mönstret i aktion.
            var subject = new Subject();

            var observerA = new ConcreteObserverA();
            subject.Attach(observerA);  // Kopplar observerare A till subject.

            var observerB = new ConcreteObserverB();
            subject.Attach(observerB);  // Kopplar observerare B till subject.

            subject.SomeBusinessLogic();  // Kör affärslogik som ändrar state och skickar notifiering.
            subject.SomeBusinessLogic();  // Kör affärslogik igen.

            subject.Detach(observerB);  // Tar bort observerare B.

            subject.SomeBusinessLogic();  // Kör affärslogik efter att observerare B tagits bort.
        }
    }
}
