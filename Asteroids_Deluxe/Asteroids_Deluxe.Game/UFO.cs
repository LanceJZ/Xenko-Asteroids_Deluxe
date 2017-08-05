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
            }

            base.Update();
        }

        public void Spawn(int SpawnCount, int Wave)
        {
            float spawnPercent = (float)(Math.Pow(0.915, (SpawnCount * 2) / ((Wave * 2) + 1)));

            // Size 0 is the large one.
            if (RandomGenerator.Next(0, 99) < spawnPercent * 100)
            {
                Size = UFOsizes.Large;
                Points = 200;
                Scale = new Vector3(1);
                Radius = 1.9f;
            }
            else
            {
                Size = UFOsizes.Small;
                Points = 1000;
                Scale = new Vector3(0.5f);
                Radius = 0.95f;
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

        void FireShot()
        {

        }

        void ChangeVector()
        {
            VectorTimer.Reset();
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
