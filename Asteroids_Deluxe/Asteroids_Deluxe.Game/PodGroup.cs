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

        List<ModelComponent> Models = new List<ModelComponent>();

        public override void Start()
        {
            Radius = 2.568f;

            for (int i = 0; i < 3; i++)
            {
                Models.Add(this.Entity.GetChild(i).Get<ModelComponent>());
            }

            Activate(false);

            base.Start();
        }

        public override void Update()
        {
            if (Active)
            {

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
            NewRockWave = false;
            Activate(true);

            Velocity = VelocityFromAngle(6);
            Position = RandomXEdge();
        }

    }
}
