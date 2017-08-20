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

        public RockSize SizeofRock { get => Size; set => Size = value; }

        public override void Start()
        {
            base.Start();

            //Model = this.Entity.GetChild(0).Get<ModelComponent>();
            LoadModelChild();
        }

        public override void Update()
        {
            if (Active && !Paused)
            {
                if (Collide())
                {
                    Hit = true;
                }

                if (CheckForEdge())
                    UpdatePR();
            }

            base.Update();
        }

        public bool Collide()
        {
            foreach (Shot shot in PlayerRef.ShotSs)
            {
                if (shot.Active)
                {
                    if (CirclesIntersect(shot.Position, shot.Radius))
                    {
                        shot.Active = false;
                        SetScore();
                        return true;
                    }
                }
            }

            if (PlayerRef.Active && !PlayerRef.Hit)
            {
                if (PlayerRef.ShieldOn)
                {
                    if (CirclesIntersect(PlayerRef.Position, PlayerRef.ShieldRadius))
                    {
                        PlayerRef.ShieldHit(Position, Velocity);
                    }
                }
                else if (CirclesIntersect(PlayerRef.Position, PlayerRef.Radius))
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

            if (UFORef.Active && !UFORef.Hit)
            {
                UFORef.Collide(this);
            }

            return false;
        }

        public void Spawn(Vector3 position, RockSize size)
        {
            Size = size;
            Position = position;
            Active = true;
            Hit = false;

            switch (Size)
            {
                case RockSize.Large:
                    Scale = new Vector3(1);
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
            Velocity = RandomVelocity(Speed * 0.25f, Speed);

            if (Size == RockSize.Large)
            {
                Position.Y = RandomHeight();

                if (Velocity.X > 0)
                    Position.X = -Edge.X;
                else
                    Position.X = Edge.X;
            }

            UpdatePR();
            UpdateScale();
        }

        void SetScore()
        {
            PlayerRef.SetScore(Points);
        }
    }
}
