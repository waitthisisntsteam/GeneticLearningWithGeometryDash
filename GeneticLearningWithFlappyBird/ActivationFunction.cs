using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticLearningWithGeometryDash
{
    public class ActivationFunction
    {
        Func<double, double> Function;
        Func<double, double> Derivative;

        public ActivationFunction (Func<double, double> function, Func<double, double> derivative)
        {
            Function = function;
            Derivative = derivative;
        }

        public double FunctionFunc(double input) => Function(input);

        public double DerivativeFunc(double input) => Derivative(input);
    }
}
