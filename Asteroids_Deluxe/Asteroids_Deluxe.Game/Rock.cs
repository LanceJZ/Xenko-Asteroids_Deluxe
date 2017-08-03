using System;
using System.Collections.Generic;
using System.Linq;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Engine;
using SiliconStudio.Xenko.Games.Time;
using SiliconStudio.Xenko.Audio;

namespace Asteroids_Deluxe
{
    public class Rock : PO
    {
        float Speed;
        RockSize Size;
        int Points;

        public override void Start()
        {
            base.Start();

            Model = this.Entity.Get<ModelComponent>();
            UpdateActive();

        }

        public override void Update()
        {
            if (Active)
            {
                if (Hit)
                {
                    Active = false;
                }

                CheckForEdge();
                base.Update();
            }
        }

        public void Spawn(Vector3 position, RockSize size)
        {
            Size = size;
            Position = position;
            Active = true;

            switch (Size)
            {
                case RockSize.Large:
                    Scale = new Vector3(1);
                    Position.Y = RandomHieght();
                    Position.X = Edge.X;
                    Points = 20;
                    Speed = 5;
                    break;

                case RockSize.Medium:
                    Scale = new Vector3(0.5f);
                    Points = 50;
                    Speed = 10;
                    break;

                case RockSize.Small:
                    Scale = new Vector3(0.25f);
                    Points = 100;
                    Speed = 20;
                    break;
            }

            SetRandomVelocity(Speed, Speed * 0.5f);
            UpdatePR();
            UpdateScale();
        }
    }
}
