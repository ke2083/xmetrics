using System;
using System.Linq;
using System.Reflection.Emit;
using Mono.Reflection;

namespace XMetrics.Gatherers
{
    internal sealed class CohesionGatherer : IDisposable
    {
        private Type assemblyType;

        public CohesionGatherer(Type assemblyType)
        {
            this.assemblyType = assemblyType;
        }

        void Dispose(bool disposing)
        {
            if (disposing)
                assemblyType = null;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public double Examine()
        {
            var methods = assemblyType.GetMethods();
            var fields = assemblyType
                .GetFields();

            if (!fields.Any())
                return double.NaN;

            var fieldCount = (double)fields.Count();
            var degreeOfCohesion = assemblyType.GetMethods()
                .Where(m => m.GetMethodBody() != null)
                .Select(m =>
                    m.GetInstructions()
                    .Where(i => i.OpCode.OperandType == OperandType.InlineTok)
                    .Select(i => i.Operand).Distinct().Count() / fieldCount).Average();

            return degreeOfCohesion;
        }
    }
}