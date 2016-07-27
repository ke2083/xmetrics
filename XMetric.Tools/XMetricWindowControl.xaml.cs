//------------------------------------------------------------------------------
// <copyright file="XMetricWindowControl.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace XMetric.Tools
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using XMetrics;



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
            using (var assemblyCollector = new SolutionAssemblyCollector())
            {
                var metrics = assemblyCollector.Analyse(txtIgnore.Text);
                var totalClasses = metrics.AllClasses.Count();
                dataGrid.ItemsSource = metrics.AllClasses.Select(c => new
                {
                    Name = c.ElementType.FullName,
                    Cohesion = double.IsNaN(c.DegreeOfCohesion) ? "Not applicable" : string.Format("{0}%", Math.Round(c.DegreeOfCohesion * 100, 2)),
                    Coupling = c.DegreeOfCoupling
                }).ToList();
            }


        }
    }
}
