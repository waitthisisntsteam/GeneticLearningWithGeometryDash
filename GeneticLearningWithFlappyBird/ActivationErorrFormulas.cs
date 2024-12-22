using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticLearningWithGeometryDash
{
    public class ActivationErorrFormulas
    {
        // Activation Functions
        public double Sigmoid(double input) => 1 / (1 + Math.Pow(double.E, -input));
        public double SigmoidD(double input) => Sigmoid(input) * (1 - Sigmoid(input));

        public double TanH(double input) => Math.Tanh(input);

        public double TanHD(double input) => 1 - Math.Pow(TanH(input), 2);

        // Error Functions
        public double MeanSquared(double input, double desiredOutput) => Math.Pow(desiredOutput - input, 2);
        public double MeanSquaredD(double input, double desiredOutput) => -2 * (desiredOutput - input);
    }
}