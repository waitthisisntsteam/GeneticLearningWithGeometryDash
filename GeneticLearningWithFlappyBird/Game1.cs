using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;

namespace GeneticLearningWithGeometryDash
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager gfx;
        private SpriteBatch spriteBatch;

        private NeuralNetwork[] Networks;
        private ActivationErorrFormulas Formulas;
        private ErrorFunction MeanSquared;
        private ActivationFunction TanH;

        private Wave Wave;

        private List<Rectangle> HitBoxes;

        private Rectangle TopPillar;
        private Rectangle BottomPillar;

        private int ticks; //fitness (how many updates have occurred)

        private double NextDouble(Random random, double min, double max) => (random.NextDouble() * (max - min)) + min;
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
                            if (random.Next(0, 2) == 0) neuron.Dendrites[i].Weight *= NextDouble(random, 0.5, 1.5);
                            else neuron.Dendrites[i].Weight *= -1;
                        }
                    }

                    if (random.NextDouble() < mutationRate)
                    {
                        if (random.Next(0, 2) == 0) neuron.Bias *= NextDouble(random, 0.5, 1.5);
                        else neuron.Bias *= -1;
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

                    for (int k = 0; k < winnerNeuron.Dendrites.Length; k++) childNeuron.Dendrites[k].Weight = winnerNeuron.Dendrites[k].Weight;
                    childNeuron.Bias = winnerNeuron.Bias;
                }
            }
        }
        public void Train((NeuralNetwork network, double fitness)[] population, Random random, double mutationRate)
        {
            Array.Sort(population, (a, b) => b.fitness.CompareTo(a.fitness));

            int start = (int)(population.Length * .1);
            int end = (int)(population.Length * .9);

            for (int i = start; i < end; i++)
            {
                Crossover(population[random.Next(0, start)].network, population[i].network, random);
                Mutate(population[i].network, random, mutationRate);
            }

            for (int i = end; i < population.Length; i++) population[i].network.Randomize(new Random(), 0, 1);
        }

        public Game1()
        {
            gfx = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }
        
        protected override void Initialize()
        {
            Formulas = new ActivationErorrFormulas();
            MeanSquared = new ErrorFunction(Formulas.MeanSquared, Formulas.MeanSquaredD);
            TanH = new ActivationFunction(Formulas.TanH, Formulas.TanHD);

            Networks = new NeuralNetwork[100];
            for (int i = 0; i < Networks.Length; i++)
            {
                Networks[i] = new NeuralNetwork([TanH], MeanSquared, [2, 4, 1]);
                Networks[i].Randomize(new Random(), 0, 1);
            }



            Wave = new Wave(7, 7);

            HitBoxes = new List<Rectangle>();

            TopPillar = new Rectangle(1500, 0, 500, 150);
            BottomPillar = new Rectangle(1500, 300, 500, 250);

            HitBoxes.Add(TopPillar);
            HitBoxes.Add(BottomPillar);

            ticks = 0;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Wave.Alive)
            {
                if (Wave.Position.Y >= 0 && Keyboard.GetState().IsKeyDown(Keys.Up)) Wave.Up();
                else if ((Wave.Position.Y + 20) <= GraphicsDevice.Viewport.Height) Wave.Down();

                if (Wave.Position.Y <= 0 || Wave.Position.Y + 20 >= GraphicsDevice.Viewport.Height) Wave.Alive = false;

                Rectangle currentHitbox = Wave.getHitbox();
                for (int i = 0; i < HitBoxes.Count; i++)
                {
                    HitBoxes[i] = new Rectangle(HitBoxes[i].X - 10, HitBoxes[i].Y, HitBoxes[i].Width, HitBoxes[i].Height);
                    if (currentHitbox.Intersects(HitBoxes[i])) Wave.Alive = false;

                    if (HitBoxes[i].Right <= 0) HitBoxes[i] = new Rectangle(HitBoxes[i].X + 2000, HitBoxes[i].Y, HitBoxes[i].Width, HitBoxes[i].Height);
                }
                ticks++;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                Wave.Position = new Point(200, 0);
                Wave.Alive = true;
                ticks = 0;

                HitBoxes.Clear();

                TopPillar = new Rectangle(1500, 0, 500, 150);
                BottomPillar = new Rectangle(1500, 250, 500, 250);

                HitBoxes.Add(TopPillar);
                HitBoxes.Add(BottomPillar);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Red);
            spriteBatch.Begin();

            if (Wave.Alive) spriteBatch.DrawRectangle(new RectangleF(Wave.Position.X, Wave.Position.Y, 20, 20), Color.White, 10, 1);

            foreach (Rectangle hb in HitBoxes) spriteBatch.FillRectangle(hb, Color.Black, 1);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
