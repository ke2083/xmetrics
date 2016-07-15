using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMetrics
{
    public static class MetricReportExtensions
    {
        public static IDictionary<string, IDictionary<string, IEnumerable<CouplingReport>>> Summarise(this MetricReport report)
        {
            var highlights = (int)Math.Floor((double)report.AllClasses.Count() / 2);
            if (highlights > 5)
                highlights = 5;

            var summary = new Dictionary<string, IDictionary<string, IEnumerable<CouplingReport>>>();

            var positive = new Dictionary<string, IEnumerable<CouplingReport>>();
            positive.Add("Most cohesive", report.ByCohesionLevel.Take(highlights));
            positive.Add("Least coupled", report.ByCouplingLevel
                                     .OrderBy(c => c.DegreeOfCoupling)
                                     .Where(c => c.DegreeOfCoupling > 0)
                                     .Take(highlights));

            var negative = new Dictionary<string, IEnumerable<CouplingReport>>();
            negative.Add("Least cohesive", report.ByCohesionLevel.OrderBy(c => c.DegreeOfCohesion).Take(highlights));
            negative.Add("Most coupled", report.ByCouplingLevel.Take(highlights));

            summary.Add("Be proud of", positive);
            summary.Add("Consider rethinking", negative);

            return summary;
        }
    }

    public class MetricReport
    {
        private readonly IEnumerable<Metrics> source;

        public MetricReport(IEnumerable<Metrics> metrics)
        {
            source = metrics;
        }

        public IEnumerable<CouplingReport> ByCohesionLevel
        {
            get
            {
                return source.Where(s => s.ReferencedTypes.Any()).OrderByDescending(s => s.DegreeOfCohesion).Select(s => new CouplingReport(s.ElementType, s.ReferencingTypes, s.DegreeOfCohesion, s.DegreeOfCoupling)).ToList();
            }
        }

        public int IndependentClasses
        {
            get
            {
                return source.Where(s => !s.ReferencedTypes.Any()).Count();
            }
        }

        public int DependentClasses
        {
            get
            {
                return source.Where(s => s.ReferencedTypes.Any()).Count();
            }
        }

        public IEnumerable<CouplingReport> AllClasses
        {
            get
            {
                return source.Select(s => new CouplingReport(s.ElementType, s.ReferencingTypes, s.DegreeOfCohesion, s.DegreeOfCoupling)).ToList();
            }
        }

        public IEnumerable<CouplingReport> ByCouplingLevel
        {
            get
            {
                return source.Where(s => s.ReferencedTypes.Any()).OrderByDescending(s => s.DegreeOfCoupling).Select(s => new CouplingReport(s.ElementType, s.ReferencingTypes, s.DegreeOfCohesion, s.DegreeOfCoupling)).ToList();
            }
        }
    }
}
