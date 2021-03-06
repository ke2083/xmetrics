﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace XMetrics.Gatherers
{
    /// <summary>
    /// Finds all the references that a type makes to other types within the system.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    internal sealed class ReferencedGatherer : IDisposable
    {
        private Type typeToExamine;

        public ReferencedGatherer(Type typeForWhichToCheckReferences)
        {
            typeToExamine = typeForWhichToCheckReferences;
        }

        void Disposing(bool disposing)
        {
            if (disposing)
                typeToExamine = null;
        }

        private void FindReferencesInConstructors(List<Type> referencedTypes)
        {
            try
            {
                typeToExamine
                               .GetConstructors()
                                .Select(c => c.GetMethodBody())
                                .Where(c => c != null)
                                .SelectMany(c => c.LocalVariables)
                               .Select(c => c.LocalType)
                               .Where(c => c != typeToExamine)
                               .ToList()
                               .ForEach(referencedTypes.Add);
            }
            catch (FileNotFoundException)
            {
                // This is normal for some assemblies that are not part of the solution.
            }
        }

        private void FindReferencesInEvents(List<Type> referencedTypes)
        {
            try
            {
                typeToExamine
                            .GetEvents()
                            .Select(e => e.EventHandlerType)
                            .Where(e => e != typeToExamine)
                            .ToList()
                            .ForEach(referencedTypes.Add);
            }
            catch (FileNotFoundException)
            {
                // This is normal for some assemblies that are not part of the solution.
            }
        }

        private void FindReferencesInFields(List<Type> referencedTypes)
        {
            try
            {
                typeToExamine
                            .GetFields()
                            .Select(f => f.FieldType)
                            .Where(f => f != typeToExamine)
                            .ToList()
                            .ForEach(referencedTypes.Add);
            }
            catch (FileNotFoundException)
            {
                // This is normal for some assemblies that are not part of the solution.
            }
        }

        private void FindReferencesInMethodBodies(List<Type> referencedTypes)
        {
            try
            {
                typeToExamine
                            .GetMethods()
                            .Select(m => m.GetMethodBody())
                            .Where(m => m != null && m.LocalVariables != null && m.LocalVariables.Any())
                            .SelectMany(m => m.LocalVariables)
                            .Select(v => v.LocalType)
                            .Where(v => v != typeToExamine)
                            .ToList()
                            .ForEach(referencedTypes.Add);
            }
            catch (FileNotFoundException)
            {
                // This is normal for some assemblies that are not part of the solution.
            }
        }

        private void FindReferencesInMethodReturns(List<Type> referencedTypes)
        {
            try
            {
                typeToExamine
                            .GetMethods()
                            .Select(m => m.ReturnType)
                            .Where(m => m != typeToExamine)
                            .ToList()
                            .ForEach(referencedTypes.Add);
            }
            catch (FileNotFoundException)
            {
                // This is normal for some assemblies that are not part of the solution.
            }
        }

        private void FindReferencesInProperties(List<Type> referencedTypes)
        {
            try
            {
                typeToExamine
                            .GetProperties()
                            .Select(p => p.PropertyType)
                            .Where(p => p != typeToExamine)
                            .ToList()
                            .ForEach(referencedTypes.Add);
            }
            catch (FileNotFoundException)
            {
                // This is normal for some assemblies that are not part of the solution.
            }
        }

        private void FindReferencesInPropertyBodies(List<Type> referencedTypes)
        {
            try
            {
                typeToExamine
                                .GetProperties()
                                .Select(m => m.GetMethod.GetMethodBody())
                                .Where(m => m != null)
                                .SelectMany(m => m.LocalVariables)
                                .Select(m => m.LocalType)
                                .Where(m => m != typeToExamine)
                                .ToList()
                                .ForEach(referencedTypes.Add);
            }
            catch (FileNotFoundException)
            {
                // This is normal for some assemblies that are not part of the solution.
            }
        }

        public void Dispose()
        {
            Disposing(true);
        }
        public IEnumerable<Type> Examine()
        {
            var referencedTypes = new List<Type>();
            FindReferencesInFields(referencedTypes);
            FindReferencesInEvents(referencedTypes);
            FindReferencesInProperties(referencedTypes);
            FindReferencesInMethodReturns(referencedTypes);
            FindReferencesInMethodBodies(referencedTypes);
            FindReferencesInPropertyBodies(referencedTypes);
            FindReferencesInConstructors(referencedTypes);
            return referencedTypes;
        }
    }
}
