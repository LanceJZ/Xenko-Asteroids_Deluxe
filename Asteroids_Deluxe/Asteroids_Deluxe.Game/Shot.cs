using System;
using System.Collections.Generic;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Engine;

namespace Asteroids_Deluxe
{
    public class Shot : PO
    {
        Timer LifeTimer;

        public override void Start()
        {
            base.Start();

            Entity lifeTimerE = new Entity { new Timer() };
            SceneSystem.SceneInstance.RootScene.Entities.Add(lifeTimerE);
            LifeTimer = lifeTimerE.Get<Timer>();
            LoadModel();
            Radius = 0.25f;
            UpdateActive(false);
        }

        public override void Update()
        {
            if (Active)
            {
                if (LifeTimer.Expired || Hit)
                {
                    Active = false;
                }

                if (CheckForEdge())
                    UpdatePR();
            }

            base.Update();
        }

        public void Spawn(Vector3 position, Vector3 velocity, float timer)
        {
            Position = position;
            Velocity = velocity;
            LifeTimer.Reset(timer);
            Active = true;
            Hit = false;
            UpdatePR();
        }
    }
}
