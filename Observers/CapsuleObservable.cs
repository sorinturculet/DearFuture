using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DearFuture.Observers
{
    public class CapsuleObservable
    {
        private static readonly List<ICapsuleObserver> _observers = new();

        // ✅ Register an observer
        public static void AddObserver(ICapsuleObserver observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
        }

        // ✅ Remove an observer
        public static void RemoveObserver(ICapsuleObserver observer)
        {
            if (_observers.Contains(observer))
            {
                _observers.Remove(observer);
            }
        }

        // ✅ Notify all observers when capsules are updated
        public static void NotifyObservers()
        {
            foreach (var observer in _observers)
            {
                observer.UpdateCapsules();
            }
        }
    }
}
