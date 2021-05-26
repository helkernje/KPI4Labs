using System;
using System.Collections.Generic;

namespace Lab1
{
	public class CountingExceptions
	{
        public static List<Type> _criticalExceptions = new List<Type>()
              {
                typeof(DivideByZeroException),
                typeof(OutOfMemoryException),
                typeof(StackOverflowException),
                typeof(InsufficientMemoryException),
                typeof(InsufficientExecutionStackException)
              };
        public int CounterCriticalExceptions { get; private set; }

        public int CounterNotCriticalExceptions { get; private set; }

        static void Main(string[] args)
        {

        }

        public bool IsCritical(Exception exception)
        {
            
            return _criticalExceptions.Contains(exception.GetType());
        }

        public void CountExceptions(Exception exception)
        {
            if (IsCritical(exception))
            {
                CounterCriticalExceptions += 1;
            }
            else
            {
                CounterNotCriticalExceptions += 1;
            }
        }
	}
}