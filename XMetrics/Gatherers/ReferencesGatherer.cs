using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace XMetrics.Gatherers
{
    internal sealed class ReferencesGatherer : IDisposable
    {
        private Type searchableItem;
        private Type searchItem;

        public ReferencesGatherer(Type typeForWhichToGatherReferences, Type typeWithinWhichToCheckForReferences)
        {
            this.searchItem = typeForWhichToGatherReferences;
            this.searchableItem = typeWithinWhichToCheckForReferences;
        }

        private void Disposing(bool disposing)
        {
            if (disposing)
            {
                searchItem = null;
                searchableItem = null;
            }
        }

        public void Dispose()
        {
            Disposing(true);
        }

        public IEnumerable<Type> Examine()
        {
            var references = new List<Type>();
            try
            {
                ReferencesInFields(references);
                ReferencesInEvents(references);
                ReferencesInProperties(references);
                ReferencesInMethodReturns(references);
                ReferencesInMethodBodies(references);
                ReferencesInPropertyBodies(references);
                ReferencesInConstructors(references);
            }
            catch (FileNotFoundException)
            {
                // This is normal for some assemblies that are not part of the solution.
            }

            return references;
        }

        private void ReferencesInConstructors(List<Type> references)
        {
            searchableItem
                        .GetConstructors()
                        .SelectMany(c => c.GetMethodBody().LocalVariables)
                        .Select(c => c.LocalType)
                        .Where(c => c == searchItem)
                        .ToList()
                        .ForEach(references.Add);
        }

        private void ReferencesInPropertyBodies(List<Type> references)
        {
            searchableItem
                        .GetProperties()
                        .Select(m => m.GetMethod.GetMethodBody())
                        .Where(m => m != null)
                        .SelectMany(m => m.LocalVariables)
                        .Select(m => m.LocalType)
                        .Where(m => m != searchItem)
                        .ToList()
                        .ForEach(references.Add);
        }

        private void ReferencesInMethodBodies(List<Type> references)
        {
            searchableItem
                        .GetMethods()
                        .Select(m => m.GetMethodBody())
                        .Where(m => m != null)
                        .SelectMany(m => m.LocalVariables)
                        .Select(v => v.LocalType)
                        .Where(v => v == searchItem)
                        .ToList()
                        .ForEach(references.Add);
        }

        private void ReferencesInMethodReturns(List<Type> references)
        {
            searchableItem
                        .GetMethods()
                        .Select(m => m.ReturnType)
                        .Where(m => m == searchItem)
                        .ToList()
                        .ForEach(references.Add);
        }

        private void ReferencesInProperties(List<Type> references)
        {
            searchableItem
                        .GetProperties()
                        .Select(p => p.PropertyType)
                        .Where(p => p == searchItem)
                        .ToList()
                        .ForEach(references.Add);
        }

        private void ReferencesInEvents(List<Type> references)
        {
            searchableItem
                        .GetEvents()
                        .Select(e => e.EventHandlerType)
                        .Where(e => e == searchItem)
                        .ToList()
                        .ForEach(references.Add);
        }

        private void ReferencesInFields(List<Type> references)
        {
            searchableItem
                        .GetFields()
                        .Select(f => f.FieldType)
                        .Where(f => f == searchItem)
                        .ToList()
                        .ForEach(references.Add);
        }
    }
}
