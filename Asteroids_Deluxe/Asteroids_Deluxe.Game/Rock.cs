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
        public UFO UFORef;

        float Speed;
        RockSize Size;
        int Points;

        public RockSize SizeofRock { get => Size; set => Size = value; }

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
                        PlayerRef.ShotSs[shot].Hit = true;
                        SetScore();
                        return true;
                    }
                }
            }

            if (PlayerRef.Active)
            {
                if (CirclesIntersect(PlayerRef.Position, PlayerRef.Radius))
                {
                    PlayerRef.Hit = true;
                    SetScore();
                    return true;
                }
            }

            if (UFORef.ShotS.Active)
            {
                if (CirclesIntersect(UFORef.ShotS.Position, UFORef.ShotS.Radius))
                {
                    UFORef.ShotS.Hit = true;
                    return true;
                }
            }

            if (UFORef.Active)
            {
                return UFORef.Collide(this);
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
                    Position = RandomXEdge();
                    Radius = 3.25f;
                    Points = 20;
                    Speed = 5;
                    break;

                case RockSize.Medium:
                    Scale = new Vector3(0.5f);
                    Radius = 1.625f;
                    Points = 50;
                    Speed = 8;
                    break;

                case RockSize.Small:
                    Scale = new Vector3(0.25f);
                    Radius = 0.8125f;
                    Points = 100;
                    Speed = 15;
                    break;
            }

            RandomVelocity(Speed * 0.25f, Speed);

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

        void SetScore()
        {

        }
    }
}
