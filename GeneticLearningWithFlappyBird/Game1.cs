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

        private GraphicsDeviceManager gfx;
        private SpriteBatch spriteBatch;

        private LearningWrapper LearningWrappers;

        private NeuralNetwork[] Networks;
        private ActivationErorrFormulas Formulas;
        private ErrorFunction MeanSquared;
        private ActivationFunction Activation;
        private Random Rand;

        private List<Rectangle> HitBoxes;

        private Rectangle TopPillar;
        private Rectangle BottomPillar;

        private Point CenterOfGap;

        private TimeSpan Timer;

        private int PlayersAlive;

        public int InputCount;
        public int PopulationCount;

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
            PopulationCount = 500;
            InputCount = 4;

            Rand = new Random(1);
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Formulas = new ActivationErorrFormulas();
            MeanSquared = new ErrorFunction(Formulas.MeanSquared, Formulas.MeanSquaredD);
            Activation = new ActivationFunction(Formulas.TanH, Formulas.TanHD);
            
            Networks = new NeuralNetwork[PopulationCount];
            for (int i = 0; i < Networks.Length; i++)
            {
                Networks[i] = new NeuralNetwork([Activation], MeanSquared, InputCount, 4, 1);
                Networks[i].Randomize(Rand, -1, 1);
            }

            LearningObject[] learners = new LearningObject[PopulationCount];
            for (int i = 0; i < learners.Length; i++)
            {
                learners[i] = new LearningObject(0, new Wave(5, 5, new Point(0, 250)), Networks[i]);
            }
            LearningWrappers = new(PopulationCount, learners);

            CenterOfGap = new Point(500, GraphicsDevice.Viewport.Height / 2);

            HitBoxes = new List<Rectangle>();

            TopPillar = new Rectangle(500, CenterOfGap.Y - 350, 500, 250);
            BottomPillar = new Rectangle(500, CenterOfGap.Y + 100, 500, 250);

            HitBoxes.Add(TopPillar);
            HitBoxes.Add(BottomPillar);

            PlayersAlive = PopulationCount;

            Timer = TimeSpan.FromMilliseconds(1);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
            {
                Timer += gameTime.ElapsedGameTime;
                for (int t = 0; t < 3; t++)
                {

                    //Set Environment
                    for (int j = 0; j < HitBoxes.Count; j++)
                    {
                        HitBoxes[j] = new Rectangle(HitBoxes[j].X-2, HitBoxes[j].Y, HitBoxes[j].Width, HitBoxes[j].Height);
                        CenterOfGap.X -= 2;
                    }

                    //Get Environment
                    double y = GraphicsDevice.Viewport.Height;
                    double nearestX1 = HitBoxes[0].X;
                    double nearestX2 = HitBoxes[1].X;
                    double nearestY1 = HitBoxes[0].Y;
                    double nearestY2 = HitBoxes[1].Y;

                    if (PlayersAlive > 0)
                    {
                        for (int i = 0; i < LearningWrappers.Population.Length; i++)
                        {
                            if (LearningWrappers.Population[i].Player.Alive)
                            {
                                //Get Current Player's Environment
                                double distanceFromTop = (LearningWrappers.Population[i].Player.Position.Y);
                                double distanceFromBottom = (y - LearningWrappers.Population[i].Player.Position.Y + LearningWrappers.Population[i].Player.Hitbox.Height);
                                
                                //double distanceX1 = (nearestX1 - LearningWrappers.Population[i].Player.Position.X + LearningWrappers.Population[i].Player.Hitbox.Width);
                                //double distanceX2 = (nearestX2 - LearningWrappers.Population[i].Player.Position.X + LearningWrappers.Population[i].Player.Hitbox.Width);
                                //double distanceY1 = (nearestY1 - LearningWrappers.Population[i].Player.Position.Y + LearningWrappers.Population[i].Player.Hitbox.Height);
                                //double distanceY2 = (nearestY2 - LearningWrappers.Population[i].Player.Position.Y + LearningWrappers.Population[i].Player.Hitbox.Height);

                                double distanceFromCenterOfGapX = CenterOfGap.X;
                                double distanceFromCenterOfGapY = CenterOfGap.Y;

                                //Act From Environment
                                //var result = LearningWrappers.Population[i].Network.Compute([distanceFromTop, distanceFromBottom, nearestX1, nearestY1, nearestX2, nearestY2]);
                                var result = LearningWrappers.Population[i].Network.Compute([distanceFromTop, distanceFromBottom, distanceFromCenterOfGapX, distanceFromCenterOfGapY]);

                                LearningWrappers.Population[i].Player.Action(result);

                                //Check Player's State
                                Rectangle currentHitbox = LearningWrappers.Population[i].Player.getHitbox();

                                bool reset = false;
                                for (int j = 0; j < HitBoxes.Count; j++)
                                {
                                    //Reset Environment
                                    if (!reset && HitBoxes[j].Right <= 0)
                                    {
                                        //HitBoxes[j] = new Rectangle(HitBoxes[j].X + 1200, Rand.Next(0, GraphicsDevice.Viewport.Height), HitBoxes[j].Width, HitBoxes[j].Height);

                                        CenterOfGap.Y = Rand.Next(100, GraphicsDevice.Viewport.Height - 200);
                                        TopPillar = new Rectangle(500, CenterOfGap.Y - 350, 500, 250);
                                        BottomPillar = new Rectangle(500, CenterOfGap.Y + 100, 500, 250);
                                        CenterOfGap.X = 500;

                                        reset = true;
                                    }

                                    //If Player Dies
                                    if (LearningWrappers.Population[i].Player.Alive&&(currentHitbox.Intersects(HitBoxes[j])
                                        || currentHitbox.Y + LearningWrappers.Population[i].Player.Hitbox.Height >= y
                                        || currentHitbox.Y <= 0)
                                        )
                                    {
                                        LearningWrappers.Population[i].Fitness += Timer.TotalMilliseconds;
                                        LearningWrappers.Population[i].Player.Alive = false;
                                        PlayersAlive--;
                                        
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        //When All Players Dead
                        LearningWrappers.Train(Rand, 0.01);


                        PlayersAlive = PopulationCount;
                        for (int i = 0; i < LearningWrappers.Population.Length; i++)
                        {
                            LearningWrappers.Population[i].Player.Position = new Point(0, 250);
                            LearningWrappers.Population[i].Player.Alive = true;
                        }

                        CenterOfGap = new Point(500, GraphicsDevice.Viewport.Height / 2);

                        HitBoxes.Clear();

                        TopPillar = new Rectangle(500, CenterOfGap.Y - 350, 500, 250);
                        BottomPillar = new Rectangle(500, CenterOfGap.Y + 100, 500, 250);

                        HitBoxes.Add(TopPillar);
                        HitBoxes.Add(BottomPillar);

                        Timer = TimeSpan.FromMilliseconds(1);
                    }
                    base.Update(gameTime);
                }
            }

        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Red);
            spriteBatch.Begin();


            foreach (Rectangle hb in HitBoxes)
            {
                spriteBatch.FillRectangle(hb, Color.Black, 1);
            }

            for (int i = 0; i < LearningWrappers.Population.Length; i++)
            {
                if (LearningWrappers.Population[i].Player.Alive)
                {
                    spriteBatch.DrawRectangle
                        (new RectangleF(LearningWrappers.Population[i].Player.Position.X, LearningWrappers.Population[i].Player.Position.Y, LearningWrappers.Population[i].Player.Hitbox.Width, LearningWrappers.Population[i].Player.Hitbox.Height), Color.White, 10, 1);
                }
                else
                {
                    spriteBatch.DrawRectangle
                        (new RectangleF(LearningWrappers.Population[i].Player.Position.X, LearningWrappers.Population[i].Player.Position.Y, LearningWrappers.Population[i].Player.Hitbox.Width, LearningWrappers.Population[i].Player.Hitbox.Height), Color.Purple, 10, 1);
                }
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
