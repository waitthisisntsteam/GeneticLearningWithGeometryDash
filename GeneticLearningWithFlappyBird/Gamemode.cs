using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;

namespace GeneticLearningWithGeometryDash
{
    public abstract class Gamemode
    {
        public bool Alive;
        public Point Position;
        public Rectangle Hitbox;
        public int Gravity;
        public int Speed;

        public Gamemode(int gravity, int speed, Point position)
        {
            Gravity = gravity;
            Speed = speed;
            Position = position;

            Alive = true;
            Hitbox = new Rectangle(position.X, position.Y, 15, 15);
        }

        public Rectangle getHitbox()
        {
            Hitbox.X = Position.X;
            Hitbox.Y = Position.Y;
            return Hitbox;
        }

        public abstract void Update(Keys? keyPressed);
    }
}
