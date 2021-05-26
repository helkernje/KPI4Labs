using System;
using System.ComponentModel;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;

namespace LW2.UnitTests
{
    public class TestData
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
    }

    [SetUpFixture]
    public class TestSetup
    {
        public static System.ComponentModel.IListSource ListSource;
        public static ITelemetryReporter NormalTelemetryReporter;
        public static ITelemetryReporter FailingTelemetryReporter;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            ListSource = Substitute.For<IListSource>();
            ListSource.GetList().Returns(TestData.CriticalExceptions);

            NormalTelemetryReporter = Substitute.For<ITelemetryReporter>();
            NormalTelemetryReporter.Report(Arg.Any<String>()).Returns(true);
            FailingTelemetryReporter = Substitute.For<ITelemetryReporter>();
            FailingTelemetryReporter.Report(Arg.Any<String>()).Returns(false);
        }
    }
    [TestFixture]
    public class LW2Tests
    {
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
            // 1. Use a LW2Factory
            var factory = new Factory()
                .WithListSource(TestSetup.ListSource)
                .WithTelemetryReporter(TestSetup.NormalTelemetryReporter)
                .Build();

            try
            {
                // act
                throw instance;
            }
            catch (Exception e)
            {
                // assert
                Assert.AreEqual(expectedResult, factory.IsCritical(e));
                return;
            }
        }

        [Test]
        public void CountExceptions_CounterValues_Correct()
        {
            // arrange
            // 2. Use constructor
            var counterExceptions = new CountingExceptions(TestSetup.ListSource, TestSetup.NormalTelemetryReporter);

            // act
            foreach (var item in TestData.CriticalExceptions)
            {
                var instance = (Exception)Activator.CreateInstance(item);
                counterExceptions.CountExceptions(instance);
            }
            foreach (var item in TestData.NonCriticalExceptions)
            {
                var instance = (Exception)Activator.CreateInstance(item);
                counterExceptions.CountExceptions(instance);
            }

            // assert
            Assert.AreEqual(counterExceptions.CounterCriticalExceptions, TestData.CriticalExceptions.Count);
            Assert.AreEqual(counterExceptions.CounterNotCriticalExceptions, TestData.NonCriticalExceptions.Count);
        }

        [Test]
        public void CountExceptions_InitCounters_Zero()
        {
            // arrange
            // 3. Use property access
            var counterExceptions = new CountingExceptions();
            counterExceptions.ExceptionListSource = TestSetup.ListSource;
            counterExceptions.TelemetryReporter = TestSetup.NormalTelemetryReporter;

            // act: nothing

            // assert
            Assert.AreEqual(counterExceptions.CounterCriticalExceptions, 0);
            Assert.AreEqual(counterExceptions.CounterNotCriticalExceptions, 0);
        }

        [Test]
        public void TelemetryReport_FailureCounter_Correct()
        {
            // arrange
            var counterExceptionsNormalTelemetry = new CountingExceptions(TestSetup.ListSource, TestSetup.NormalTelemetryReporter);
            var counterExceptionsFailingTelemetry = new CountingExceptions(TestSetup.ListSource, TestSetup.FailingTelemetryReporter);

            // act
            foreach (var item in TestData.CriticalExceptions)
            {
                var instance = (Exception)Activator.CreateInstance(item);
                counterExceptionsNormalTelemetry.CountExceptions(instance);
                counterExceptionsFailingTelemetry.CountExceptions(instance);
            }

            // assert
            Assert.AreEqual(counterExceptionsNormalTelemetry.ReportFailures, 0);
            Assert.AreEqual(counterExceptionsFailingTelemetry.ReportFailures, TestData.CriticalExceptions.Count);
        }

        [Test]
        public void TelemetryReport_InitCounter_Zero()
        {
            // arrange
            var counterExceptionsFailingTelemetry = new CountingExceptions(TestSetup.ListSource, TestSetup.FailingTelemetryReporter);

            // act: nothing

            // assert
            Assert.AreEqual(counterExceptionsFailingTelemetry.ReportFailures, 0);
        }
    }
}