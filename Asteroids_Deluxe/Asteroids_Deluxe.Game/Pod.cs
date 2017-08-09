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

        public override void Start()
        {
            LoadModelChild();
            UpdateActive(false);

            base.Start();
        }

        public override void Update()
        {
            if (Active)
            {
                CheckEdges();


            }

            base.Update();
        }
    }
}
