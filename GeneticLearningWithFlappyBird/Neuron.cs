using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace GeneticLearningWithGeometryDash
{
    public class Neuron
    {
        public double Bias;
        public Dendrite[] Dendrites;

        public double Output { get; set; }
        public double Input { get; private set; }
        public ActivationFunction Activation { get; set; }

        public Neuron(ActivationFunction activation, Neuron?[] previousNeurons)
        {
            Activation = activation;

            if (previousNeurons == null)
            {
                Dendrites = new Dendrite[0];                
            }
            else
            {
                Dendrites = new Dendrite[previousNeurons.Length];
                for (int i = 0; i < previousNeurons.Length; i++)
                {
                    Dendrites[i] = new Dendrite(null, previousNeurons[i], 0);
                    for (int j = 0; j < previousNeurons[i].Dendrites.Length; j++)
                    {
                        previousNeurons[i].Dendrites[j].Next = this;
                    }
                }
            }
        }

        public void Randomize(Random random, double min, double max)
        {
            for (int i = 0; i < Dendrites.Length; i++)
            {
                Dendrites[i].Weight = (random.NextDouble() * (max - min)) + min;
            }
            Bias = (random.NextDouble() * (max - min)) + min;
        }

        public double Compute()
        {
            double input = 0;
            if (Dendrites != null)
            {
                for (int i = 0; i < Dendrites.Length; i++)
                {
                    input += Dendrites[i].Compute();
                }
            }

            input += Bias;
            Input = input;
            Output = Activation.FunctionFunc(input);

            return Output;
        }
    }
}
