using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TempoIQ
{
    public class Pipeline
    {
        public IList<PipelineFunction> Functions { get; private set; }

        public Pipeline AddFunction(PipelineFunction function)
        {
            this.Functions.Add(function);
            return this;
        }

        public Pipeline(IList<PipelineFunction> functions)
        {
            this.Functions = functions;
        }

        public Pipeline()
        {
            this.Functions = new List<PipelineFunction>();
        }
    }

    public interface PipelineFunction
    {
        string Name { get; }

        IList<string> Arguments { get; }

    }
}