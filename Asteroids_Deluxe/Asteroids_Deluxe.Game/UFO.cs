using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Engine;

public enum UFOsizes
{
    Small,
    Large
}

namespace Asteroids_Deluxe
{
    public class UFO : PO
    {
        Vector3 RadiusOffset;
        int Points;
        float Speed = 5;
        public bool Done;
        UFOsizes Size;
        public Shot ShotS;
        public Player PlayerRef;

        Timer VectorTimer;
        Timer ShotTimer;

        public override void Start()
        {
            base.Start();

            Entity vectorTimerE = new Entity { new Timer() };
            SceneSystem.SceneInstance.RootScene.Entities.Add(vectorTimerE);
            VectorTimer = vectorTimerE.Get<Timer>();
            VectorTimer.Reset(3.15f);

            Entity shotTimerE = new Entity { new Timer() };
            SceneSystem.SceneInstance.RootScene.Entities.Add(shotTimerE);
            ShotTimer = shotTimerE.Get<Timer>();
            ShotTimer.Reset(2.75f);

            Prefab shotP = Content.Load<Prefab>("Shot");
            Entity ShotE = shotP.Instantiate().First();
            SceneSystem.SceneInstance.RootScene.Entities.Add(ShotE);
            ShotS = ShotE.Get<Shot>();
            ShotS.Active = false;

            Model = this.Entity.GetChild(0).Get<ModelComponent>();
            Active = false;
            UpdateActive();
        }

        public override void Update()
        {
            if (Active)
            {
                if (Position.X > Edge.X || Position.X < -Edge.X)
                    Done = true;
                else
                    CheckForEdge();

                if (VectorTimer.Expired)
                    ChangeVector();

                if (ShotTimer.Expired)
                    FireShot();

                if (PlayerCollide())
                    SetScore();
            }

            if (ShotS.Active)
            {
                if (ShotCollide())
                {

                }
            }

            base.Update();
        }

        public void Spawn(int SpawnCount, int Wave)
        {
            float spawnPercent = (float)(Math.Pow(0.915, (SpawnCount * 2) / ((Wave * 2) + 1)));

            if (RandomGenerator.Next(0, 99) < spawnPercent * 100)
            {
                Size = UFOsizes.Large;
                Points = 200;
                Scale = new Vector3(1);
                Radius = 1.3f;
                RadiusOffset = new Vector3(0.9f, 0, 0);
                //UFO is 2.6 in height (Radius 1.3). Use two circles offset on the horizontal for a 4.4 width (Radius 2.2).
            }
            else
            {
                Size = UFOsizes.Small;
                Points = 1000;
                Scale = new Vector3(0.5f);
                Radius = 0.65f;
                RadiusOffset = new Vector3(0.45f, 0, 0);
            }

            Position.Y = RandomHeight();

            if (RandomGenerator.Next(10) > 5)
            {
                Position.X = -Edge.X;
                Velocity.X = Speed;
            }
            else
            {
                Position.X = Edge.X;
                Velocity.X = -Speed;
            }

            UpdatePR();
            //ShotTimer.Reset();
            VectorTimer.Reset();
            Active = true;
            Done = false;
            Hit = false;
        }

        public bool Collide(PO target)
        {
            if (target.CirclesIntersect(Position, Radius * 2))
            {
                if (target.CirclesIntersect(Position - RadiusOffset, Radius) ||
                    target.CirclesIntersect(Position + RadiusOffset, Radius))
                {
                    Hit = true;
                    target.Hit = true;
                    return true;
                }
            }

            return false;
        }

        bool PlayerCollide()
        {
            if (PlayerRef.Active)
            {
                return Collide(PlayerRef);
                //if (PlayerRef.CirclesIntersect(Position, Radius * 2))
                //{
                //    if (PlayerRef.CirclesIntersect(Position - RadiusOffset, Radius) ||
                //        PlayerRef.CirclesIntersect(Position + RadiusOffset, Radius))
                //    {
                //        Hit = true;
                //        PlayerRef.Hit = true;
                //        return true;
                //    }
                //}
            }

            for (int shot = 0; shot < 4; shot++)
            {
                if (PlayerRef.ShotSs[shot].Active)
                {
                    if (Collide(PlayerRef.ShotSs[shot]))
                    {
                        PlayerRef.ShotSs[shot].Active = false;
                        return true;
                    }
                }
            }

            return false;
        }

        bool ShotCollide()
        {
            if (PlayerRef.Active)
            {
                if (ShotS.CirclesIntersect(PlayerRef.Position, PlayerRef.Radius))
                {
                    ShotS.Active = false;
                    PlayerRef.Hit = true;
                    return true;
                }
            }

            return false;
        }

        void FireShot()
        {
            //if (!m_Player.GameOver)
            //    m_FireShot.Play(0.4f, 0, 0);

            ShotTimer.Reset();
            float speed = 30;
            float rad = 0;

            //Adjust accuracy according to score. By the time the score reaches 30,000, percent = 0.
            float percent = 0.25f;// - (m_PlayerScore * 0.00001f);

            if (percent < 0)
                percent = 0;

            switch (Size)
            {
                case UFOsizes.Large:
                    if (RandomGenerator.Next(11) > 7)
                    {
                        rad = RandomRadian();
                    }
                    else
                    {
                        rad = AngleFromVectors(Position, PlayerRef.Position) + RandomMinMax(-percent, percent);
                    }
                    break;

                case UFOsizes.Small:
                    rad = AngleFromVectors(Position, PlayerRef.Position) + RandomMinMax(-percent, percent);
                    break;
            }

            ShotS.Spawn(Position + SetVelocity(Radius, rad), SetVelocity(speed, rad) + Velocity * 0.25f, 1.15f);
        }

        void SetScore()
        {

        }

        void ChangeVector()
        {
            VectorTimer.Reset();
            ShotTimer.Reset();
            float vChange = RandomGenerator.Next(10);

            if (vChange < 5)
            {
                if ((int)Velocity.Y == 0 && vChange < 2.5)
                    Velocity.Y = Speed;
                else if ((int)Velocity.Y == 0)
                    Velocity.Y = -Speed;
                else
                    Velocity.Y = 0;
            }
        }
    }
}
