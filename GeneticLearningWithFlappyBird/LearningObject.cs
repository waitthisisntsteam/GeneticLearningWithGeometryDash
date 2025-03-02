using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticLearningWithGeometryDash
{
    public class LearningObject
    {
        public double Fitness;
        public Gamemode Player;       
        public NeuralNetwork Network;

        public LearningObject(double fitness, Gamemode player, NeuralNetwork neuralNetwork)
        {
            Fitness = fitness;
            Player = player;
            Network = neuralNetwork;
        }
    }
}
