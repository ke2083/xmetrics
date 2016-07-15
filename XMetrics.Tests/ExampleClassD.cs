using System;
using System.Collections.Generic;
using System.Linq;

namespace XMetrics.Tests
{
    public class ExampleClassD : IInterfaceE
    {
        private readonly ExampleClassA classA;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExampleClassD"/> class.
        /// </summary>
        public ExampleClassD(ExampleClassA classA)
        {
            this.classA = classA;
        }

        public bool MustRun(ExampleClassB element)
        {
            return true;
        }
    }
}

