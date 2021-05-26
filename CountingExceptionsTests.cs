using System;
using System.Collections.Generic;
using Lab1KPI4;
using NUnit.Framework;

namespace Project1.UnitTests
{
    [TestFixture]
    public class CountingExceptionsTests
    {
        public static List<Type> CriticalExceptions = new List<Type>()
        {
            typeof(DivideByZeroException),
            typeof(OutOfMemoryException),
            typeof(StackOverflowException),
            typeof(InsufficientMemoryException),
            typeof(InsufficientExecutionStackException)
        };
        public static List<Type> NonCriticalExceptions = new List<Type>()
        {
            typeof(ArgumentNullException),
            typeof(ArgumentOutOfRangeException),
            typeof(NullReferenceException),
            typeof(AccessViolationException),
            typeof(IndexOutOfRangeException),
            typeof(InvalidOperationException)
        };

        [TestCase(typeof(DivideByZeroException), true)]
        [TestCase(typeof(OutOfMemoryException), true)]
        [TestCase(typeof(StackOverflowException), true)]
        [TestCase(typeof(InsufficientMemoryException), true)]
        [TestCase(typeof(InsufficientExecutionStackException), true)]
        [TestCase(typeof(ArgumentNullException), false)]
        [TestCase(typeof(ArgumentOutOfRangeException), false)]
        [TestCase(typeof(NullReferenceException), false)]
        [TestCase(typeof(AccessViolationException), false)]
        [TestCase(typeof(IndexOutOfRangeException), false)]
        [TestCase(typeof(InvalidOperationException), false)]
        public void IsCritical_CriticalityCheck_Correct(Type exceptionType, bool expectedResult)
        {
            // arrange
            var instance = (Exception)Activator.CreateInstance(exceptionType);
            try
            {
                // act
                throw instance;
            }
            catch (Exception e)
            {
                // assert
                Assert.AreEqual(expectedResult, new CountingExceptions().IsCritical(e));
                return;
            }
        }

        [Test]
        public void CountExceptions_CounterValues_Correct()
        {
            var counterExceptions = new CountingExceptions();

            // act
            foreach (var item in criticalExceptions)
            {
                var instance = (Exception)Activator.CreateInstance(item);
                counterExceptions.CountExceptions(instance);
            }
            foreach (var item in nonCriticalExceptions)
            {
                var instance = (Exception)Activator.CreateInstance(item);
                counterExceptions.CountExceptions(instance);
            }

            // assert
            Assert.AreEqual(counterExceptions.CounterCriticalExceptions, criticalExceptions.Count);
            Assert.AreEqual(counterExceptions.CounterNotCriticalExceptions, nonCriticalExceptions.Count);
        }

        [Test]
        public void CountExceptions_InitCounters_Zero()
        {
            // arrange
            var counterExceptions = new CountingExceptions();

            // act: nothing

            // assert
            Assert.AreEqual(counterExceptions.CounterCriticalExceptions, 0);
            Assert.AreEqual(counterExceptions.CounterNotCriticalExceptions, 0);
        }
    }
} 