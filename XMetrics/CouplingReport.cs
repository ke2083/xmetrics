using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMetrics
{
    public class CouplingReport
    {
        private readonly double degreeOfCohesion;
        private readonly double degreeOfCoupling;
        private readonly Type elementType;
        private readonly IEnumerable<Type> referencingTypes;

        public CouplingReport(Type elementType, IEnumerable<Type> referencingTypes, double degreeOfCohesion, double degreeOfCoupling)
        {
            this.elementType = elementType;
            this.referencingTypes = referencingTypes;
            this.degreeOfCohesion = degreeOfCohesion;
            this.degreeOfCoupling = degreeOfCoupling;
        }

        public double DegreeOfCohesion
        {
            get
            {
                return degreeOfCohesion;
            }
        }

        public double DegreeOfCoupling
        {
            get
            {
                return degreeOfCoupling;
            }
        }

        public Type ElementType
        {
            get
            {
                return elementType;
            }
        }

        public IEnumerable<Type> ReferencingTypes
        {
            get
            {
                return referencingTypes;
            }
        }
    }
}
