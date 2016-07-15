using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMetrics
{
    public class Metrics
    {
        private readonly double degreeOfCohesion;
        private readonly IEnumerable<Type> referencedTypes;
        private readonly IEnumerable<Type> referencingTypes;
        private readonly Type elementType;

        public IEnumerable<Type> ReferencedTypes
        {
            get
            {
                return referencedTypes;
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

        public double DegreeOfCoupling
        {
            get
            {
                if (referencedTypes.Count() == 0)
                    return 0;

                return referencingTypes.Count() / referencedTypes.Count();
            }
        }

        public double DegreeOfCohesion
        {
            get
            {
                return degreeOfCohesion;
            }
        }

        public Metrics(Type elementType, IEnumerable<Type> referencedTypes, IEnumerable<Type> referencingTypes, double cohesion)
        {
            this.elementType = elementType;
            this.referencedTypes = referencedTypes;
            this.referencingTypes = referencingTypes;
            degreeOfCohesion = cohesion;
        }
    }
}
