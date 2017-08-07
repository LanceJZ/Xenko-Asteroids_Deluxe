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
    public class Pod : PO
    {


        public override void Start()
        {
            LoadModelChild();
            Active = false;
            UpdateActive();

            base.Start();
        }

        public override void Update()
        {


            base.Update();
        }
    }
}
