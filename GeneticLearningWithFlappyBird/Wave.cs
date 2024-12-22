using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticLearningWithGeometryDash
{
    public class Wave
    {
        public bool Alive;
        public Point Position;
        private Rectangle Hitbox;
        private int Gravity;
        private int Speed;

        public Wave(int gravity, int speed) 
        {
            Alive = true;
            Position = new Point(200, 0);
            Hitbox = new Rectangle(200, 0, 15, 15);
            Gravity = gravity;
            Speed = speed;
        }

        public void Up() => Position.Y -= Speed;

        public void Down() => Position.Y += Gravity;

        public Rectangle getHitbox()
        {
            Hitbox.X = Position.X;
            Hitbox.Y = Position.Y;
            return Hitbox;
        }
    }
}
