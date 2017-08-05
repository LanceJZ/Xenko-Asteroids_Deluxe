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
        public Player PlayerRef;

        float Speed;
        RockSize Size;
        int Points;

        public override void Start()
        {
            base.Start();

            Model = this.Entity.GetChild(0).Get<ModelComponent>();

        }

        public override void Update()
        {
            if (Active)
            {
                if (Collide())
                {
                    Hit = true;
                }

                CheckForEdge();
            }

            base.Update();
        }

        public bool Collide()
        {
            for (int shot = 0; shot < 4; shot++)
            {
                if (PlayerRef.ShotSs[shot].Active)
                {
                    if (CirclesIntersect(PlayerRef.ShotSs[shot].Position, PlayerRef.ShotSs[shot].Radius))
                    {
                        PlayerRef.ShotSs[shot].Active = false;
                        //SetScore();
                        return true;
                    }
                }
            }

            return false;
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
                    Position.Y = RandomHeight();
                    Position.X = Edge.X;
                    Radius = 2.9f;
                    Points = 20;
                    Speed = 5;
                    break;

                case RockSize.Medium:
                    Scale = new Vector3(0.5f);
                    Radius = 1.45f;
                    Points = 50;
                    Speed = 8;
                    break;

                case RockSize.Small:
                    Scale = new Vector3(0.25f);
                    Radius = 0.725f;
                    Points = 100;
                    Speed = 15;
                    break;
            }

            SetRandomVelocity(Speed * 0.25f, Speed);

            float rotV = 0;

            if (RandomGenerator.Next(10) > 5)
            {
                rotV = RandomGenerator.Next(15) * 0.1f + 0.25f;
            }
            else
            {
                rotV = RandomGenerator.Next(15) * -0.1f - 0.25f;
            }

            RotationVelocity = rotV;
            UpdatePR();
            UpdateScale();
        }
    }
}
