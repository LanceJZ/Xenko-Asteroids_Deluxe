using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xenko.Core.Mathematics;
using Xenko.Input;
using Xenko.Engine;
using Xenko.Games.Time;
using Xenko.Graphics;
using Xenko.Rendering;
using Xenko.Audio;

namespace Asteroids_Deluxe
{
    class ClearToSpawn : PO
    {
        public override void Start()
        {
            Radius = 10;

            base.Start();
        }

        public bool TargetHit(PO target)
        {
            if (CirclesIntersect(target.Position, target.Radius))
            {
                return true;
            }

            return false;
        }
    }
}
