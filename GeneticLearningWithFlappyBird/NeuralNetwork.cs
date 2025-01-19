using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticLearningWithGeometryDash
{
    public class NeuralNetwork
    {
        public Layer[] Layers;
        public ErrorFunction Error;

        public NeuralNetwork(ActivationFunction[] activation, ErrorFunction error, params int[] neuronsPerLayer)
        {
            Error = error;
            Layers = new Layer[neuronsPerLayer.Length];

            Layer inputLayer = new Layer(activation[0], neuronsPerLayer[0], null);
            Layer previousLayer = inputLayer;
            Layers[0] = inputLayer; 
            for (int i = 1; i < neuronsPerLayer.Length; i++)
            {
                Layer currentLayer = new Layer(activation[0], neuronsPerLayer[i], previousLayer);
                previousLayer = currentLayer;
                Layers[i] = currentLayer;
            }
        }

        public void Randomize(Random random, double min, double max)
        {
            for (int i = 0; i < Layers.Length; i++) Layers[i].Randomize(random, min, max);
        }

        public double[] Compute(double[] inputs)
        {
            for (int i = 0; i < Layers[0].Neurons.Length; i++) Layers[0].Neurons[i].Output = inputs[i];
            for (int i = 0; i < Layers.Length - 1; i++) Layers[i].Compute();
            ;
            return Layers[Layers.Length - 1].Compute();
        }

        public double GetError(double[] inputs, double[] desiredOutputs)
        {
            double errorSum = 0;
            for (int i = 0; i < inputs.Length; i++) errorSum += Error.FunctionFunc(inputs[i], desiredOutputs[i]);

            return errorSum;
        }
    }
}
