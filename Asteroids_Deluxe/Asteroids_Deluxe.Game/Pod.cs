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
            if (Active && !Hit && !Paused)
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
