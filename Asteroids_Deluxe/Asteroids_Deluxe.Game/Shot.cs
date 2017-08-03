using System;
using System.Collections.Generic;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Engine;

namespace Asteroids_Deluxe
{
    public class Shot : PO
    {
        Timer LifeTimer;
        Entity LifeTimerE = new Entity { new Timer() };

        public override void Start()
        {
            base.Start();

            SceneSystem.SceneInstance.RootScene.Entities.Add(LifeTimerE);
            LifeTimer = LifeTimerE.Get<Timer>();
            Model = this.Entity.Get<ModelComponent>();
            Active = false;
            UpdateActive();
        }

        public override void Update()
        {
            if (Active)
            {
                if (LifeTimer.Expired || Hit)
                {
                    Active = false;
                }

                CheckForEdge();
                base.Update();
            }
        }

        public void Spawn(Vector3 position, Vector3 velocity, float timer)
        {
            Position = position;
            Velocity = velocity;
            LifeTimer.Reset(timer);
            Active = true;
            UpdatePR();
        }
    }
}
