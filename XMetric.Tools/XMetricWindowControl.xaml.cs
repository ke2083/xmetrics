//------------------------------------------------------------------------------
// <copyright file="XMetricWindowControl.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace XMetric.Tools
{
    using EnvDTE;
    using Microsoft.VisualStudio.Shell;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;
    using System.Linq;
    using System.IO;
    using System.Reflection;
    using System.Collections.Generic;
    using XMetrics;
    using System;



    /// <summary>
    /// Interaction logic for XMetricWindowControl.
    /// </summary>
    public partial class XMetricWindowControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XMetricWindowControl"/> class.
        /// </summary>
        public XMetricWindowControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        protected void btnAnalyse_Click(object sender, RoutedEventArgs e)
        {
            var dte = (DTE)ServiceProvider.GlobalProvider.GetService(typeof(DTE));
            var projects = dte.Solution.Projects;
            var assemblies = new List<Assembly>();
            var allProjects = new List<Project>();
            GetProjects(projects, allProjects);
            foreach (var project in allProjects)
            {
                if (string.IsNullOrEmpty(project.FullName))
                    continue;

                var path = GetAssemblyPath(project);
                if (File.Exists(path))
                    assemblies.Add(Assembly.LoadFrom(path));
            }

            var ignorables = txtIgnore.Text.Split(',').Select(t => t.Trim()).ToArray();
            var metrics = XMetrics.GatherReport(assemblies.ToArray(), ignorables);
            var report = metrics.Summarise();
            var totalClasses = metrics.AllClasses.Count();
            dataGrid.ItemsSource = metrics.AllClasses.Select(c => new
            {
                Name = c.ElementType.FullName,
                Cohesion = double.IsNaN(c.DegreeOfCohesion) ? "Not applicable" : string.Format("{0}%", Math.Round(c.DegreeOfCohesion * 100, 2)),
                Coupling = c.DegreeOfCoupling
            }).ToList();
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
    }
}
