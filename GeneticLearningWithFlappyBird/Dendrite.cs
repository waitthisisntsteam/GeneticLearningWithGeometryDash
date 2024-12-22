using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticLearningWithGeometryDash
{
    public class Dendrite
    {
        public Neuron Next { get; }
        public Neuron Previous { get; }
        public double Weight { get; set; }

        public Dendrite(Neuron next, Neuron previous, double weight) => (Next, Previous, Weight) = (next, previous ,weight);

        public double Compute() => Previous.Output * Weight;
    }
}
