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
    public class PodGroup : Pods
    {
        bool Visable;
        List<ModelComponent> Models = new List<ModelComponent>();

        public override void Start()
        {
            Radius = 2.568f;
            Points = 50;

            for (int i = 0; i < 3; i++)
            {
                Models.Add(this.Entity.GetChild(i).Get<ModelComponent>());
            }

            Activate(false);

            base.Start();
        }

        public override void Update()
        {
            if (Active && !Paused)
            {
                CheckEdges();
            }
            else if (NewRockWave && Visable)
            {
                Activate(false);
                Visable = false;
            }

            base.Update();
        }

        public void Activate(bool active)
        {
            Active = active;

            foreach (ModelComponent model in Models)
            {
                model.Enabled = active;
            }
        }

        public void Spawn()
        {
            Hit = false;
            Visable = true;
            NewRockWave = false;
            Activate(true);

            Velocity = VelocityFromAngle(6);

            Position.Y = RandomHeight();

            if (Velocity.X > 0)
                Position.X = -Edge.X;
            else
                Position.X = Edge.X;
        }

    }
}
