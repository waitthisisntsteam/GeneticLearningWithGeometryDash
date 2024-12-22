using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticLearningWithGeometryDash
{
    public class ErrorFunction
    {
        Func<double, double, double> Function;
        Func<double, double, double> Derivative;
        public ErrorFunction(Func<double, double, double> function, Func<double, double, double> derivative) 
        {
            Function = function;
            Derivative = derivative;
        }

        public double FunctionFunc(double output, double desiredOutput) => Function(output, desiredOutput);

        public double DerivativeFunc(double output, double desiredOutput) => Derivative(output, desiredOutput);
    }
}
