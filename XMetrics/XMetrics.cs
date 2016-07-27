using EnvDTE;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using XMetrics.Gatherers;
using XMetrics.Reports;

namespace XMetrics
{
    public sealed class XMetrics
    {
        private static double GatherCohesion(Type assemblyType)
        {
            var degreeOfCohesion = 0d;
            using (var cohesionGatherer = new CohesionGatherer(assemblyType))
                degreeOfCohesion = cohesionGatherer.Examine();

            return degreeOfCohesion;
        }

        private static IEnumerable<Type> GatherReferencedTypes(Type T, string[] excludeAssembliesStartingWith)
        {
            var referencedTypes = new List<Type>();
            using (var referencedGatherer = new ReferencedGatherer(T))
                referencedTypes.AddRange(referencedGatherer.Examine());

            var typesOfInterest = referencedTypes
                .Where(r => !excludeAssembliesStartingWith
                    .Any(a => !string.IsNullOrEmpty(r.AssemblyQualifiedName) && r.AssemblyQualifiedName.StartsWith(a)));

            return typesOfInterest;
        }

        private static IEnumerable<Type> GatherReferencingTypes(Type searchType, Assembly assemblyToSearch)
        {
            var references = new ConcurrentQueue<Type>();
            var typesToSearch = assemblyToSearch.GetTypes();
            Parallel.ForEach(typesToSearch, searchable =>
            {
                using (var refGatherer = new ReferencesGatherer(searchType, searchable))
                    refGatherer.Examine().ToList().ForEach(references.Enqueue);
            });

            return references.ToList();
        }

        public IEnumerable<Type> Gather<T>(Assembly assembly)
        {
            return GatherReferencingTypes(typeof(T), assembly);
        }

        public IEnumerable<Type> Gather<T>(string[] excludeAssembliesStartingWith)
            where T : class
        {
            return GatherReferencedTypes(typeof(T), excludeAssembliesStartingWith);
        }

        public MetricReport GatherReport(Assembly[] assemblies, string[] excludeAssembliesStartingWith)
        {
            var metrics = new ConcurrentQueue<Metrics>();
            Parallel.ForEach(assemblies, assembly =>
            {
                Type[] types;
                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException)
                {
                    // This will happen periodically for types that are not part of the solution.
                    return;
                }

                var assemblyTypes = assembly
                    .GetTypes()
                    .Where(a => !excludeAssembliesStartingWith
                        .Any(excl => a.FullName.StartsWith(excl, StringComparison.InvariantCultureIgnoreCase)));

                Parallel.ForEach(assemblyTypes, assemblyType =>
                {
                    var referenced = GatherReferencedTypes(assemblyType, excludeAssembliesStartingWith);
                    var referencing = new List<Type>();
                    foreach (var indAssembly in assemblies)
                        referencing.AddRange(GatherReferencingTypes(assemblyType, assembly));

                    var cohesion = GatherCohesion(assemblyType);
                    metrics.Enqueue(new Metrics(assemblyType, referenced, referencing, cohesion));
                });
            });

            return new MetricReport(metrics.ToList());
        }
    }
}
