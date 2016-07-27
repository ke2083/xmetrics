using FluentXUnit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace XMetrics.Tests
{
    public class TestGetReferencesToClass
    {
        private XMetrics XMetrics;

        public TestGetReferencesToClass()
        {
            XMetrics = new XMetrics();
        }

        [Fact]
        public void GetReferencesToOtherClasses()
        {
            var metrics = XMetrics.Gather<ExampleClassA>(new string[] { "Microsoft", "System" });
            XAssert.That(metrics, IsNot.Null());
            XAssert.That(metrics.Count(), Is.EqualTo(2));
        }

        [Fact]
        public void GetReferencesToThisClass()
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            var metrics = XMetrics.Gather<ExampleClassA>(currentAssembly);
            XAssert.That(metrics, IsNot.Null());
            XAssert.That(metrics.Count(), Is.EqualTo(1));
        }

        [Fact]
        public void GetIndependentClasses()
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            var metrics = XMetrics.GatherReport(new Assembly[] { currentAssembly }, new string[] { "Microsoft", "System" });
            XAssert.That(metrics, IsNot.Null());
            XAssert.That(metrics.IndependentClasses, Is.EqualTo(1));
        }

        [Fact]
        public void GetDependentClasses()
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            var metrics = XMetrics.GatherReport(new Assembly[] { currentAssembly }, new string[] { "Microsoft", "System" });
            XAssert.That(metrics, IsNot.Null());
            XAssert.That(metrics.DependentClasses, Is.EqualTo(3));
        }

        [Fact]
        public void GetClassesByCoupling()
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            var metrics = XMetrics.GatherReport(new Assembly[] { currentAssembly }, new string[] { "Microsoft", "System" });
            XAssert.That(metrics, IsNot.Null());
            XAssert.That(metrics.ByCouplingLevel.Count(), Is.EqualTo(3));
        }

        [Fact]
        public void GetClassesByCohesion()
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            var metrics = XMetrics.GatherReport(new Assembly[] { currentAssembly }, new string[] { "Microsoft", "System" });
            XAssert.That(metrics, IsNot.Null());
            XAssert.That(metrics.ByCohesionLevel.Count(), Is.EqualTo(3));
        }
    }
}