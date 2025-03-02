using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;


namespace GeneticLearningWithGeometryDash
{
    public class LearningWrapper
    {
        public LearningObject[] Population;

        public LearningWrapper(int count, LearningObject[] learningObject)
        {
            Population = new LearningObject[count];

            for (int i = 0; i < learningObject.Length; i++)
            {
                Population[i] = learningObject[i];
            }
        }


        public double NextDouble(Random random, double min, double max) => (random.NextDouble() * (max - min)) + min;
        public void Mutate(NeuralNetwork network, Random random, double mutationRate)
        {
            foreach (Layer layer in network.Layers)
            {
                foreach (Neuron neuron in layer.Neurons)
                {
                    for (int i = 0; i < neuron.Dendrites.Length; i++)
                    {
                        if (random.NextDouble() < mutationRate)
                        {
                            if (random.Next(0, 2) == 0)
                            {
                                neuron.Dendrites[i].Weight *= NextDouble(random, 0, 1);
                            }
                            else
                            {
                                neuron.Dendrites[i].Weight *= -1;
                            }
                        }
                    }

                    if (random.NextDouble() < mutationRate)
                    {
                        if (random.Next(0, 2) == 0)
                        {
                            neuron.Bias *= NextDouble(random, 0, 1);
                        }
                        else
                        {
                            neuron.Bias *= -1;
                        }
                    }
                }
            }
        }
        public void Crossover(NeuralNetwork winner, NeuralNetwork loser, Random random)
        {
            for (int i = 0; i < winner.Layers.Length; i++)
            {
                Layer winnerLayer = winner.Layers[i];
                Layer childLayer = loser.Layers[i];

                int crossoverPoint = random.Next(0, winnerLayer.Neurons.Length);
                bool flipOrNot = random.Next(0, 2) == 0;

                for (int j = (flipOrNot ? 0 : crossoverPoint); j < (flipOrNot ? crossoverPoint : winnerLayer.Neurons.Length); j++)
                {
                    Neuron winnerNeuron = winnerLayer.Neurons[j];
                    Neuron childNeuron = childLayer.Neurons[j];

                    for (int k = 0; k < winnerNeuron.Dendrites.Length; k++)
                    {
                        childNeuron.Dendrites[k].Weight = winnerNeuron.Dendrites[k].Weight;
                    }
                    childNeuron.Bias = winnerNeuron.Bias;
                }
            }
        }
        public void Train(Random random, double mutationRate)
        {
            Array.Sort(Population, (a, b) => b.Fitness.CompareTo(a.Fitness));

            for (int i = 0; i < Population.Length; i++)
            {
                int start = (int)(Population.Length * .1);
                int end = (int)(Population.Length * .9);

                for (int j = start; j < end; j++)
                {
                    Crossover(Population[random.Next(0, start)].Network, Population[j].Network, random);
                    Mutate(Population[j].Network, random, mutationRate);
                }

                for (int j = end; j < Population.Length; j++)
                {
                    Population[j].Network.Randomize(new Random(), 0, 1);
                }
            }
        }
    }
}
