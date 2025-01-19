using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GeneticLearningWithGeometryDash
{
    public class Game1 : Game
    {
        private double ElapsedTime;

        private GraphicsDeviceManager gfx;
        private SpriteBatch spriteBatch;

        private LearningWrapper LearningWrappers;

        private NeuralNetwork[] Networks;
        private ActivationErorrFormulas Formulas;
        private ErrorFunction MeanSquared;
        private ActivationFunction Activation;
        Random rand;
        private Wave Wave;

        private List<Rectangle> HitBoxes;

        private Rectangle TopPillar;
        private Rectangle BottomPillar;

        KeyboardState state;
        public Game1()
        {
            gfx = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }
        
        protected override void Initialize()
        {
            

            base.Initialize();
        }

        protected override void LoadContent()
        {
            rand = new Random(1);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Formulas = new ActivationErorrFormulas();
            MeanSquared = new ErrorFunction(Formulas.MeanSquared, Formulas.MeanSquaredD);
            Activation = new ActivationFunction(Formulas.Sigmoid, Formulas.SigmoidD);
            
            Networks = new NeuralNetwork[100];
            for (int i = 0; i < Networks.Length; i++)
            {
                Networks[i] = new NeuralNetwork([Activation], MeanSquared, 6,4,2,1);
                Networks[i].Randomize(rand, 0, 1);
            }

            LearningObject[] learners = new LearningObject[100];
            for (int i = 0; i < learners.Length; i++) learners[i] = new LearningObject(0, new Wave(3, 3, new Point(0, 225)), Networks[i]);
            LearningWrappers = new(learners);

            HitBoxes = new List<Rectangle>();

            TopPillar = new Rectangle(1500, 0, 500, 150);
            BottomPillar = new Rectangle(1500, 300, 500, 250);

            HitBoxes.Add(TopPillar);
            HitBoxes.Add(BottomPillar);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            state = Keyboard.GetState();

            //Get Our Environment
            double y = GraphicsDevice.Viewport.Height;
            double nearestX1 = HitBoxes[0].X;
            double nearestX2 = HitBoxes[1].X;
            double nearestY1 = HitBoxes[0].Y;
            double nearestY2 = HitBoxes[1].Y;

            ElapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds;

            for (int i = 0; i < LearningWrappers.Population.Length; i++)
            {
                if (LearningWrappers.Population[i].Player.Alive)
                {
                    double distanceFromTop = (LearningWrappers.Population[i].Player.Position.Y); /// 1000000;
                    double distanceFromBottom = (y - LearningWrappers.Population[i].Player.Position.Y + 15); /// 1000000;
                    double distanceX1 = (nearestX1 - LearningWrappers.Population[i].Player.Position.X + 15); /// 1000000;
                    double distanceX2 = (nearestX2 - LearningWrappers.Population[i].Player.Position.X + 15); /// 1000000;
                    double distanceY1 = (nearestY1 - LearningWrappers.Population[i].Player.Position.Y + 15); /// 1000000;
                    double distanceY2 = (nearestY2 - LearningWrappers.Population[i].Player.Position.Y + 15); /// 1000000;

                    var result = LearningWrappers.Population[i].Network.Compute([distanceFromTop, distanceFromBottom, distanceX1, distanceX2, distanceY1, distanceY2]);
                    LearningWrappers.Population[i].Player.Action(result);

                    var test = result[0];

                    Rectangle currentHitbox = LearningWrappers.Population[i].Player.getHitbox();
                    for (int j = 0; j < HitBoxes.Count; j++)
                    {
                        if (HitBoxes[j].Right <= 0) HitBoxes[j] = new Rectangle(HitBoxes[j].X + 2000, HitBoxes[j].Y, HitBoxes[j].Width, HitBoxes[j].Height);

                        if (currentHitbox.Intersects(HitBoxes[j]) 
                            || currentHitbox.Y + 15 >= y 
                            || currentHitbox.Y <= 0
                            )
                        {
                            LearningWrappers.Population[i].Player.Alive = false;

                            LearningWrappers.Population[i].Fitness += ElapsedTime;
                        }
                    }
                }
            }

            for (int j = 0; j < HitBoxes.Count; j++) HitBoxes[j] = new Rectangle(HitBoxes[j].X - 5, HitBoxes[j].Y, HitBoxes[j].Width, HitBoxes[j].Height);

            /*if (Wave.Alive)
            {
                Wave.Update(state);

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

            if (state.IsKeyDown(Keys.Enter))
            {
                Wave.Position = new Point(200, 0);
                Wave.Alive = true;
                ticks = 0;

                HitBoxes.Clear();

                TopPillar = new Rectangle(1500, 0, 500, 150);
                BottomPillar = new Rectangle(1500, 250, 500, 250);

                HitBoxes.Add(TopPillar);
                HitBoxes.Add(BottomPillar);
            }*/

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Red);
            spriteBatch.Begin();

            for (int i = 0; i < LearningWrappers.Population.Length; i++)
            {
                if (LearningWrappers.Population[i].Player.Alive) 
                    spriteBatch.DrawRectangle
                        (new RectangleF(i * 10, LearningWrappers.Population[i].Player.Position.Y, 15, 15), Color.White, 1, 1);
                else 
                    spriteBatch.DrawRectangle
                        (new RectangleF(i * 10, LearningWrappers.Population[i].Player.Position.Y, 15, 15), Color.Purple, 1, 1);
            }

            foreach (Rectangle hb in HitBoxes) spriteBatch.FillRectangle(hb, Color.Black, 1);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
