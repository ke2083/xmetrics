using System;
using System.Collections.Generic;
using System.Linq;

namespace XMetrics.Tests
{
    public class ExampleClassA
    {
        public ExampleClassB B { get; set; }

        public ExampleClassA()
        {
            this.B = new ExampleClassB();
        }
    }
}

