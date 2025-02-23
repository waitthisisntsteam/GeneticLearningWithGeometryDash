using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticLearningWithGeometryDash
{
    public class LearningObject
    {
        public double Fitness;        //fitness based off ticks/time alive, coins collected
        public Gamemode Player;       
        public NeuralNetwork Network; //inputs: position of player, position of closest object(s), bounds before death

        public LearningObject(double fitness, Gamemode player, NeuralNetwork neuralNetwork)
        {
            Fitness = fitness;
            Player = player;
            Network = neuralNetwork;
        }

        public void SetBestObj()
        {
           // Network.Layers[1].Neurons[0].
        }
    }
}
