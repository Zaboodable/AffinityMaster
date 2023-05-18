using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AffinityMaster
{
    public class ObjectPool<T>
    {
        public static ObjectPool<T> Instance = new ObjectPool<T>();
        private Stack<T> _stack;

        public ObjectPool()
        {
            this._stack = new Stack<T>();
        }

        public int Count { get { return _stack.Count; } }
        public T Next { get 
            {
                var value = _stack.Pop();
                System.Diagnostics.Debug.WriteLine($"Popped {value} from pool, count: {Count}");
                return value; 
            } 
        }
        public void Push(T value) 
        {
            _stack.Push(value); 
            System.Diagnostics.Debug.WriteLine($"Pushed {value} to pool, count: {Count}");
        }  

    }
}
