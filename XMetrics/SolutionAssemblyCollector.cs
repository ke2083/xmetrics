using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using EnvDTE;
using XMetrics.Reports;

namespace XMetrics
{
    public sealed class SolutionAssemblyCollector : IDisposable
    {
        private ConcurrentQueue<Assembly> assemblies;

        private bool disposedValue = false; // To detect redundant calls

        public SolutionAssemblyCollector(DTE dte)
        {
            var projects = dte.Solution.Projects;
            assemblies = new ConcurrentQueue<Assembly>();
            var allProjects = new List<Project>();
            GetProjects(projects, allProjects);
            Parallel.ForEach(allProjects.Where(p => !string.IsNullOrEmpty(p.FullName)), project =>
            {
                var path = GetAssemblyPath(project);
                if (File.Exists(path))
                    assemblies.Enqueue(Assembly.Load(File.ReadAllBytes(path)));
            });
        }

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    assemblies.ToList().ForEach(a => a = null);
                    assemblies = null;
                }

                disposedValue = true;
            }
        }

        /// <summary>
        /// Gets the assembly path.
        /// </summary>
        /// <param name="vsProject">The vs project.</param>
        /// <returns></returns>
        /// <remarks>
        /// Thanks to https://social.msdn.microsoft.com/Forums/vstudio/en-US/03d9d23f-e633-4a27-9b77-9029735cfa8d/how-to-get-the-right-output-path-from-envdteproject-by-code-if-show-advanced-build?forum=vsx
        /// </remarks>
        private static string GetAssemblyPath(Project vsProject)
        {
            var fullPath = vsProject.Properties.Item("FullPath").Value.ToString();
            var outputPath = vsProject.ConfigurationManager.ActiveConfiguration.Properties.Item("OutputPath").Value.ToString();
            var outputDir = Path.Combine(fullPath, outputPath);
            var outputFileName = vsProject.Properties.Item("OutputFileName").Value.ToString();
            return Path.Combine(outputDir, outputFileName);
        }

        private void GetProjects(Projects projects, ICollection<Project> allProjects)
        {
            if (projects == null)
                return;

            foreach (Project project in projects)
            {
                if (!string.IsNullOrEmpty(project.FileName))
                    allProjects.Add(project);

                if (project.ProjectItems != null)
                    GetProjectsRecursively(project.ProjectItems, allProjects);
            }
        }


        private void GetProjectsRecursively(ProjectItems projectItems, ICollection<Project> allProjects)
        {
            foreach (ProjectItem project in projectItems)
            {
                if (project.SubProject != null)
                    allProjects.Add(project.SubProject);

                if (project.ProjectItems != null)
                    GetProjectsRecursively(project.ProjectItems, allProjects);
            }
        }

        public MetricReport Analyse(string assembliesToIgnore)
        {
            var ignorables = assembliesToIgnore.Split(',').Select(t => t.Trim()).ToArray();
            var xmetrics = new XMetrics();
            var metrics = xmetrics.GatherReport(assemblies.ToArray(), ignorables);
            return metrics;
        }

        public void Dispose()
        {
            Dispose(true);
        }

    }
}
