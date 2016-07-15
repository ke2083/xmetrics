using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using Mono.Reflection;
using System.IO;

namespace XMetrics
{
    public class XMetrics
    {
        public static IEnumerable<Type> Gather<T>(Assembly assembly)
        {
            return GatherReferencingTypes(typeof(T), assembly);
        }

        private static IEnumerable<Type> GatherReferencingTypes(Type T, Assembly assembly)
        {
            var references = new List<Type>();
            var assemblyTypes = assembly.GetTypes();
            foreach (var assemblyType in assemblyTypes)
            {
                try
                {
                    assemblyType
                    .GetFields()
                    .Select(f => f.FieldType)
                    .Where(f => f == T)
                    .ToList()
                    .ForEach(references.Add);

                    assemblyType
                    .GetEvents()
                    .Select(e => e.EventHandlerType)
                    .Where(e => e == T)
                    .ToList()
                    .ForEach(references.Add);

                    assemblyType
                    .GetProperties()
                    .Select(p => p.PropertyType)
                    .Where(p => p == T)
                    .ToList()
                    .ForEach(references.Add);

                    assemblyType
                    .GetMethods()
                    .Select(m => m.ReturnType)
                    .Where(m => m == T)
                    .ToList()
                    .ForEach(references.Add);

                    assemblyType
                    .GetMethods()
                    .Select(m => m.GetMethodBody())
                    .Where(m => m != null)
                    .SelectMany(m => m.LocalVariables)
                    .Select(v => v.LocalType)
                    .Where(v => v == T)
                    .ToList()
                    .ForEach(references.Add);

                    assemblyType
                    .GetProperties()
                    .Select(m => m.GetMethod.GetMethodBody())
                    .Where(m => m != null)
                    .SelectMany(m => m.LocalVariables)
                    .Select(m => m.LocalType)
                    .Where(m => m != T)
                    .ToList()
                    .ForEach(references.Add);

                    assemblyType
                    .GetConstructors()
                    .SelectMany(c => c.GetMethodBody().LocalVariables)
                    .Select(c => c.LocalType)
                    .Where(c => c == T)
                    .ToList()
                    .ForEach(references.Add);
                }
                catch (FileNotFoundException)
                {
                }
            }

            return references;
        }

        private static double GatherCohesion(Type assemblyType)
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

        public static MetricReport GatherReport(Assembly[] assemblies, string[] excludeAssembliesStartingWith)
        {
            var metrics = new List<Metrics>();
            foreach (var assembly in assemblies)
            {
                Type[] types = null;
                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException)
                {
                    continue;
                }

                foreach (var assemblyType in assembly.GetTypes())
                {
                    var referenced = GatherReferencedTypes(assemblyType, excludeAssembliesStartingWith);
                    var referencing = new List<Type>();
                    foreach (var indAssembly in assemblies)
                    {
                        referencing.AddRange(GatherReferencingTypes(assemblyType, assembly));
                    }

                    var cohesion = GatherCohesion(assemblyType);
                    metrics.Add(new Metrics(assemblyType, referenced, referencing, cohesion));
                }
            }

            return new MetricReport(metrics);
        }

        public static IEnumerable<Type> Gather<T>(string[] excludeAssembliesStartingWith)
            where T: class
        {
            return GatherReferencedTypes(typeof(T), excludeAssembliesStartingWith);
        }

        private static IEnumerable<Type> GatherReferencedTypes(Type T, string[] excludeAssembliesStartingWith)
        {
            var referencedTypes = new List<Type>();
            try
            {
                T
                .GetFields()
                .Select(f => f.FieldType)
                .Where(f => f != T)
                .ToList()
                .ForEach(referencedTypes.Add);

                T
                .GetEvents()
                .Select(e => e.EventHandlerType)
                .Where(e => e != T)
                .ToList()
                .ForEach(referencedTypes.Add);

                T
                .GetProperties()
                .Select(p => p.PropertyType)
                .Where(p => p != T)
                .ToList()
                .ForEach(referencedTypes.Add);

                T
                .GetMethods()
                .Select(m => m.ReturnType)
                .Where(m => m != T)
                .ToList()
                .ForEach(referencedTypes.Add);


                T
                .GetMethods()
                .Select(m => m.GetMethodBody())
                .Where(m => m != null && m.LocalVariables != null && m.LocalVariables.Any())
                .SelectMany(m => m.LocalVariables)
                .Select(v => v.LocalType)
                .Where(v => v != T)
                .ToList()
                .ForEach(referencedTypes.Add);

                T
                    .GetProperties()
                    .Select(m => m.GetMethod.GetMethodBody())
                    .Where(m => m != null)
                    .SelectMany(m => m.LocalVariables)
                    .Select(m => m.LocalType)
                    .Where(m => m != T)
                    .ToList()
                    .ForEach(referencedTypes.Add);

                T
                   .GetConstructors()
                    .Select(c => c.GetMethodBody())
                    .Where(c => c != null)
                    .SelectMany(c => c.LocalVariables)
                   .Select(c => c.LocalType)
                   .Where(c => c != T)
                   .ToList()
                   .ForEach(referencedTypes.Add);
            }
            catch (FileNotFoundException)
            {
            }

            var typesOfInterest = referencedTypes
                .Where(r => !excludeAssembliesStartingWith
                    .Any(a => !string.IsNullOrEmpty(r.AssemblyQualifiedName) && r.AssemblyQualifiedName.StartsWith(a)));

            return typesOfInterest;
        }
    }
}
