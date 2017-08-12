using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Engine;

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
