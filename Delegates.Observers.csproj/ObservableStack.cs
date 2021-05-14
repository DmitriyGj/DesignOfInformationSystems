using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Observers
{
<<<<<<< HEAD

=======
>>>>>>> 648966bea90d11772e2a96eeb6b4f702c2667a03
	public class StackOperationsLogger
	{
		private readonly Observer observer = new Observer();
		public void SubscribeOn<T>(ObservableStack<T> stack)
		{
			stack.Notify += observer.HandleEvent;
		}

		public string GetLog()
		{
			return observer.Log.ToString();
		}
	}

	public class Observer 
	{
		public StringBuilder Log = new StringBuilder();

		public void HandleEvent(object eventData)
		{
			Log.Append(eventData);
		}
	}

	public delegate void Notifications(object data);

	public class ObservableStack<T> 
	{
		public event Notifications Notify;

<<<<<<< HEAD
=======

>>>>>>> 648966bea90d11772e2a96eeb6b4f702c2667a03
		List<T> data = new List<T>();

		public void Push(T obj)
		{
			data.Add(obj);
			Notify?.Invoke(new StackEventData<T> { IsPushed = true, Value = obj });
		}

		public T Pop()
		{
			if (data.Count == 0)
				throw new InvalidOperationException();
			var result = data[data.Count - 1];
<<<<<<< HEAD
			Notify?.Invoke(new StackEventData<T> { IsPushed = false, Value = data[data.Count - 1] });
			return result;

		}
	}


=======
			Notify?.Invoke(new StackEventData<T> { IsPushed = false, Value = result });
			return result;
		}
	}
>>>>>>> 648966bea90d11772e2a96eeb6b4f702c2667a03
}
