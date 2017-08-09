using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Engine;

namespace Asteroids_Deluxe
{
    public class Pods : PO
    {
        protected float RotateMagnitude;
        protected float Speed;
        protected float RandomDirection;

        float score;
        Timer DirectionTimer;

        bool rockWave;
        public bool NewRockWave { get => rockWave; set => rockWave = value; }
        protected float Score { get => score; set => score = value; }

        public Player PlayerRef;
        public UFO UFORef;

        public override void Update()
        {
            if (Active)
            {
                CheckEdges();

                if (PlayerHit())
                {
                    Hit = true;
                    SetScore();
                }
            }

            base.Update();
        }

        protected void Initalize()
        {
            Entity TimerE = new Entity { new Timer() };
            SceneSystem.SceneInstance.RootScene.Entities.Add(TimerE);
            DirectionTimer = TimerE.Get<Timer>();
            DirectionTimer.Reset(3.15f);
        }

        protected void SetScore()
        {

        }

        public void Spawn(Vector3 position, float rotation)
        {
            Position = position;
            Rotation = rotation;
            UpdatePR();

            Active = true;
            Hit = false;
            NewRockWave = false;

            RotateMagnitude = RandomMinMax(-0.25f + MathUtil.PiOverTwo, 0.25f + MathUtil.PiOverTwo);
            Speed = RandomMinMax(8, 10);
        }

        protected bool PlayerHit()
        {
            if (PlayerRef.Active)
            {
                if (CirclesIntersect(PlayerRef.Position, PlayerRef.Radius))
                {
                    PlayerRef.Hit = true;
                    return true;
                }
            }

            foreach (Shot shot in PlayerRef.ShotSs)
            {
                if (shot.Active)
                {
                    if (CirclesIntersect(shot.Position, shot.Radius))
                    {
                        shot.Active = false;
                        return true;
                    }
                }
            }

            return false;
        }

        protected void CheckEdges()
        {
            if (CheckForEdge())
            {
                if (NewRockWave)
                {
                    Active = false;
                }
                else
                {
                    UpdatePR();
                }
            }
        }

        protected void UpdateDirection()
        {
            RotationVelocity = 0;

            if (!NewRockWave)
            {
                if (PlayerRef.Active)// && !PlayerRef.Hit)
                    ChaseObject(PlayerRef.Position);
                else if (UFORef.Active)
                    ChaseObject(UFORef.Position);
            }

            Velocity = VelocityFromRadian(Speed, Rotation);
        }

        void ChaseObject(Vector3 position)
        {
            RotationVelocity = AimAtTarget(Position, position, Rotation + RandomDirection, RotateMagnitude);

            if (DirectionTimer.Expired)
            {
                DirectionTimer.Reset(RandomMinMax(2, 4));
                RandomDirection = RandomMinMax(-0.5f, 0.5f);
            }
        }
    }
}
