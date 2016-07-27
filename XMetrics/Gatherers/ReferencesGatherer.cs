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
            ReferencesInFields(references);
            ReferencesInEvents(references);
            ReferencesInProperties(references);
            ReferencesInMethodReturns(references);
            ReferencesInMethodBodies(references);
            ReferencesInPropertyBodies(references);
            ReferencesInConstructors(references);
            return references;
        }

        private void ReferencesInConstructors(List<Type> references)
        {
            try
            {
                searchableItem
                            .GetConstructors()
                            .SelectMany(c => c.GetMethodBody().LocalVariables)
                            .Select(c => c.LocalType)
                            .Where(c => c == searchItem)
                            .ToList()
                            .ForEach(references.Add);
            }
            catch (FileNotFoundException)
            {
                // This is normal for some assemblies that are not part of the solution.
            }
        }

        private void ReferencesInPropertyBodies(List<Type> references)
        {
            try
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
            catch (FileNotFoundException)
            {
                // This is normal for some assemblies that are not part of the solution.
            }
        }

        private void ReferencesInMethodBodies(List<Type> references)
        {
            try
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
            catch (FileNotFoundException)
            {
                // This is normal for some assemblies that are not part of the solution.
            }
        }

        private void ReferencesInMethodReturns(List<Type> references)
        {
            try
            {
                searchableItem
                            .GetMethods()
                            .Select(m => m.ReturnType)
                            .Where(m => m == searchItem)
                            .ToList()
                            .ForEach(references.Add);
            }
            catch (FileNotFoundException)
            {
                // This is normal for some assemblies that are not part of the solution.
            }
        }

        private void ReferencesInProperties(List<Type> references)
        {
            try
            {
                searchableItem
                            .GetProperties()
                            .Select(p => p.PropertyType)
                            .Where(p => p == searchItem)
                            .ToList()
                            .ForEach(references.Add);
            }
            catch (FileNotFoundException)
            {
                // This is normal for some assemblies that are not part of the solution.
            }
        }

        private void ReferencesInEvents(List<Type> references)
        {
            try
            {
                searchableItem
                            .GetEvents()
                            .Select(e => e.EventHandlerType)
                            .Where(e => e == searchItem)
                            .ToList()
                            .ForEach(references.Add);
            }
            catch (FileNotFoundException)
            {
                // This is normal for some assemblies that are not part of the solution.
            }
        }

        private void ReferencesInFields(List<Type> references)
        {
            try
            {
                searchableItem
                            .GetFields()
                            .Select(f => f.FieldType)
                            .Where(f => f == searchItem)
                            .ToList()
                            .ForEach(references.Add);
            }
            catch (FileNotFoundException)
            {
                // This is normal for some assemblies that are not part of the solution.
            }
        }
    }
}
