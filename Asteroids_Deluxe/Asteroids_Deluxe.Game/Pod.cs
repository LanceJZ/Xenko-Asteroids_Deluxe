using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Input;
using SiliconStudio.Xenko.Engine;

namespace Asteroids_Deluxe
{
    public class Pod : Pods
    {
        public Explode ExplodeRef;

        public override void Start()
        {
            Initalize();
            LoadModelChild();
            UpdateActive(false);
            Radius = 1.45f;
            Points = 200;

            base.Start();
        }

        public override void Update()
        {
            if (Active)
            {
                UpdateDirection();
                CheckEdges();

                if (PlayerHit())
                {
                    SetScore();
                    Hit = true;
                }
            }

            base.Update();
        }

    }
}
