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
    public class PodGroup : PO
    {
        List<ModelComponent> Models = new List<ModelComponent>();

        public override void Start()
        {
            for (int i = 0; i < 3; i++)
            {
                Models.Add(this.Entity.GetChild(i).Get<ModelComponent>());
            }

            Active = false;
            Activate(Active);

            base.Start();
        }

        public override void Update()
        {
            CheckForEdge();

            base.Update();
        }

        public void Activate(bool active)
        {
            foreach (ModelComponent model in Models)
            {
                model.Enabled = active;
            }
        }

        public void Spawn()
        {
            Hit = false;
            Active = true;
            Activate(Active);

            Velocity = VelocityFromAngle(6);
            Position = RandomXEdge();
        }

    }
}
