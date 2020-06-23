using System;
using System.Collections.Generic;
using System.Text;

namespace AnalysisEngine
{
    public class ArchitectureNotMatchException:Exception
    {
        public ArchitectureNotMatchException()
        {

        }

        public ArchitectureNotMatchException(string message)
            :base(message)
        {

        }

        public ArchitectureNotMatchException(string message, Exception inner)
            :base(message, inner)
        {

        }
    }
}
