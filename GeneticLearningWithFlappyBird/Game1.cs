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

        private LearningWrapper LearningWrappers;

        private NeuralNetwork[] Networks;
        private ActivationErorrFormulas Formulas;
        private ErrorFunction MeanSquared;
        private ActivationFunction TanH;

        private Wave Wave;

        private List<Rectangle> HitBoxes;

        private Rectangle TopPillar;
        private Rectangle BottomPillar;

        private int ticks; //fitness (how many updates have occurred)

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
           
            
            Wave = new(7, 7,new Point());

            LearningObject[] learners = new LearningObject[100];
            for (int i = 0; i < learners.Length; i++) learners[i] = new LearningObject(0, Wave, Networks[i]);
            LearningWrappers = new(learners);

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
                //if (Wave.Position.Y >= 0 && Keyboard.GetState().IsKeyDown(Keys.Up)) Wave.Up();
                //else if ((Wave.Position.Y + 20) <= GraphicsDevice.Viewport.Height) Wave.Down();

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

           // if (Wave.Alive) spriteBatch.DrawRectangle(new RectangleF(Wave.Position.X, Wave.Position.Y, 20, 20), Color.White, 10, 1);

            foreach (Rectangle hb in HitBoxes) spriteBatch.FillRectangle(hb, Color.Black, 1);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
