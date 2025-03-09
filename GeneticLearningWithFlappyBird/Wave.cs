using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticLearningWithGeometryDash
{
    public class Wave:Gamemode
    {
        public Wave(int gravity, int speed, Point position)
            : base(gravity, speed, position) { }

        public void Up() => Position.Y -= Speed;

        public void Down() => Position.Y += Gravity;

        public override void Update(KeyboardState keyState)
        {
            if (keyState.IsKeyDown(Keys.Up))
            {
                Up();
            }
            else
            {
                Down();
            }
        }

        public override void Action(double[] array)
        {
            if (array[0] < .5)
            {
                Down();
            }
            else
            {
                Up();
            }
        }
    }
}
