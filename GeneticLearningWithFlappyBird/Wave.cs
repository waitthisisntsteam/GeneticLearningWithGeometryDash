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

        private void Up() => Position.Y -= Speed;

        private void Down() => Position.Y += Gravity;

        public override void Update(Keys? keyPressed)
        {
            if (keyPressed == null) Down();
            else if (keyPressed == Keys.Up) Up();
            else Down();
        }
    }
}
