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

        private KeyboardState state;

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
            InputCount = 2;

            Rand = new Random(1);
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Formulas = new ActivationErorrFormulas();
            MeanSquared = new ErrorFunction(Formulas.MeanSquared, Formulas.MeanSquaredD);
            Activation = new ActivationFunction(Formulas.TanH, Formulas.TanHD);
            
            Networks = new NeuralNetwork[PopulationCount];
            for (int i = 0; i < Networks.Length; i++)
            {
                Networks[i] = new NeuralNetwork([Activation], MeanSquared, InputCount, 4, 2, 1);
                Networks[i].Randomize(Rand, 0, 1);
            }

            LearningObject[] learners = new LearningObject[PopulationCount];
            for (int i = 0; i < learners.Length; i++)
            {
                learners[i] = new LearningObject(0, new Wave(5, 5, new Point(0, 100)), Networks[i]);
            }
            LearningWrappers = new(PopulationCount, learners);

            HitBoxes = new List<Rectangle>();

            TopPillar = new Rectangle(500, 0, 500, 150);
            BottomPillar = new Rectangle(500, 325, 500, 250);

            HitBoxes.Add(TopPillar);
            HitBoxes.Add(BottomPillar);

            PlayersAlive = PopulationCount;
        }

        protected override void Update(GameTime gameTime)
        {
            
                //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                //Exit();
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                for (int t = 0; t < 4; t++)
                {
                    state = Keyboard.GetState();

                    //Get Our Environment
                    for (int j = 0; j < HitBoxes.Count; j++)
                    {
                        HitBoxes[j] = new Rectangle(HitBoxes[j].X - 5, HitBoxes[j].Y, HitBoxes[j].Width, HitBoxes[j].Height);
                    }

                    double y = GraphicsDevice.Viewport.Height;
                    //double nearestX1 = HitBoxes[0].X;
                    //double nearestX2 = HitBoxes[1].X;
                    //double nearestY1 = HitBoxes[0].Y;
                    //double nearestY2 = HitBoxes[1].Y;

                    if (PlayersAlive > 0)
                    {
                       

                        for (int i = 0; i < LearningWrappers.Population.Length; i++)
                        {
                            if (LearningWrappers.Population[i].Player.Alive)
                            {
                                double distanceFromTop = (LearningWrappers.Population[i].Player.Position.Y);
                                double distanceFromBottom = (y - LearningWrappers.Population[i].Player.Position.Y + LearningWrappers.Population[i].Player.Hitbox.Height);
                                //double distanceX1 = (nearestX1 - LearningWrappers.Population[i].Player.Position.X + LearningWrappers.Population[i].Player.Hitbox.Width);
                                //double distanceX2 = (nearestX2 - LearningWrappers.Population[i].Player.Position.X + LearningWrappers.Population[i].Player.Hitbox.Width);
                                //double distanceY1 = (nearestY1 - LearningWrappers.Population[i].Player.Position.Y + LearningWrappers.Population[i].Player.Hitbox.Height);
                                //double distanceY2 = (nearestY2 - LearningWrappers.Population[i].Player.Position.Y + LearningWrappers.Population[i].Player.Hitbox.Height);

                                var result = LearningWrappers.Population[i].Network.Compute([distanceFromTop, distanceFromBottom]); 
                                //, distanceX1, distanceX2, distanceY1, distanceY2]);
                                LearningWrappers.Population[i].Player.Action(result); //! this is the problem, never changes decision no matter the inputs

                                var test = result[0];

                                Rectangle currentHitbox = LearningWrappers.Population[i].Player.getHitbox();
                                for (int j = 0; j < HitBoxes.Count; j++)
                                {
                                    if (HitBoxes[j].Right <= 0)
                                    {
                                        HitBoxes[j] = new Rectangle(HitBoxes[j].X + 2000, HitBoxes[j].Y, HitBoxes[j].Width, HitBoxes[j].Height);
                                    }

                                    if (currentHitbox.Intersects(HitBoxes[j])
                                        || currentHitbox.Y + LearningWrappers.Population[i].Player.Hitbox.Height >= y
                                        || currentHitbox.Y <= 0
                                        )
                                    {
                                        LearningWrappers.Population[i].Player.Alive = false;

                                        float time = gameTime.GetElapsedSeconds();
                                        ;
                                        LearningWrappers.Population[i].Fitness += time;
                                        PlayersAlive--;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        LearningWrappers.Train(Rand, 0.1);


                        PlayersAlive = PopulationCount;

                        for (int i = 0; i < LearningWrappers.Population.Length; i++)
                        {
                            LearningWrappers.Population[i].Player.Position = new Point(0, 100);
                            LearningWrappers.Population[i].Player.Alive = true;
                        }


                        HitBoxes.Clear();

                        TopPillar = new Rectangle(500, 0, 500, 150);
                        BottomPillar = new Rectangle(500, 325, 500, 250);

                        HitBoxes.Add(TopPillar);
                        HitBoxes.Add(BottomPillar);

                        gameTime = new GameTime();
                        ;
                    }

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
