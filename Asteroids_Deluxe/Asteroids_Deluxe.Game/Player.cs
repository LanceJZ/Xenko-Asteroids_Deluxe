using System;
using System.Linq;
using System.Collections.Generic;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Input;
using SiliconStudio.Xenko.Engine;
using SiliconStudio.Xenko.Games.Time;
using SiliconStudio.Xenko.Graphics;
using SiliconStudio.Xenko.Rendering;
using SiliconStudio.Xenko.Audio;

namespace Asteroids_Deluxe
{
    public class Player : PO
    {
        Timer FlameTimer;

        public Numbers ScoreRef;
        ModelComponent FlameM;
        ModelComponent ShieldM;

        public List<Shot> ShotSs;

        float ShieldPower;
        float shieldRadius;
        int score = 0;
        int NextNewShip = 5000;
        int NewShipCount = 0;
        bool newShip;
        bool shieldOn;

        public int Score { get => score; }
        public bool NewShip { get => newShip; set => newShip = value; }
        public bool ShieldOn { get => shieldOn; }
        public float ShieldRadius { get => shieldRadius; }

        public override void Start()
        {
            base.Start();

            Entity flameTimerE = new Entity { new Timer() };
            SceneSystem.SceneInstance.RootScene.Entities.Add(flameTimerE);
            FlameTimer = flameTimerE.Get<Timer>();
            FlameTimer.Reset(0.01666f);

            Radius = 1.25f;
            shieldRadius = 2.1f;
            ShotSs = new List<Shot>();

            Prefab shotP = Content.Load<Prefab>("Shot");

            for (int i = 0; i < 4; i++)
            {
                Entity ShotE = shotP.Instantiate().First();
                SceneSystem.SceneInstance.RootScene.Entities.Add(ShotE);
                ShotSs.Add(ShotE.Get<Shot>());
            }

            Deceleration = 0.01f;
            LoadModelChild();

            FlameM = this.Entity.FindChild("PlayerFlame").Get<ModelComponent>();
            ShieldM = this.Entity.FindChild("PlayerShield").Get<ModelComponent>();
            Disable();
        }

        public override void Update()
        {
            if (Active && !Paused)
            {
                GetInput();

                if (CheckForEdge())
                    UpdatePR();
            }

            if (!Active)
            {
                ShieldPower = 100;
            }

            base.Update();
        }

        public void ShieldHit(Vector3 position, Vector3 velocity)
        {
            Acceleration = Vector3.Zero;
            Velocity = (Velocity * 0.01f) * -1;
            Velocity += velocity * 0.095f;
            Velocity += VelocityFromAngle(AngleFromVectors(position, Position), 7.5f);

            ShieldPower -= 20;
        }

        public void ShieldHit()
        {
            ShieldPower -= 40;
        }

        public void SetPause(bool pause)
        {
            FlameTimer.SetPause(pause);
            Paused = pause;

            foreach (Shot shot in ShotSs)
            {
                shot.SetPause(pause);
            }
        }

        /// <summary>
        /// Add points to the current score.
        /// </summary>
        /// <param name="number">Number of points to add.</param>
        public void SetScore(int number)
        {
            score += number;

            if (score > NextNewShip)
            {
                NextNewShip += (NextNewShip * (1 + NewShipCount));
                NewShip = true;
            }

            ScoreRef.ProcessNumber(score, new Vector3(-23, Edge.Y - 1, 0), 1);
        }

        public void Disable()
        {
            Hit = false;
            Active = false;
            Position = Vector3.Zero;
            Velocity = Vector3.Zero;
            Acceleration = Vector3.Zero;
            FlameM.Enabled = false;
            ShieldM.Enabled = false;
        }

        public void NewGame()
        {
            score = 0;
            NewShipCount = 0;
            ShieldPower = 100;
            SetScore(0);
        }

        void Shield(bool active)
        {
            if (active)
            {
                if (ShieldPower > 0)
                {
                    shieldOn = true;
                    ShieldM.Enabled = true;
                    ShieldPower -= 10 * (float)Game.UpdateTime.Elapsed.TotalSeconds;
                }
                else
                {
                    Shield(false);
                }
            }
            else
            {
                shieldOn = false;
                ShieldM.Enabled = false;

                if (ShieldPower < 100)
                {
                    ShieldPower += 1 * (float)Game.UpdateTime.Elapsed.TotalSeconds;
                }
            }
        }

        void GetInput()
        {
            if (Input.IsKeyDown(Keys.Left))
            {
                RotationVelocity = 3.5f;
            }
            else if (Input.IsKeyDown(Keys.Right))
            {
                RotationVelocity = -3.5f;
            }
            else
                RotationVelocity = 0;

            if (Input.IsKeyDown(Keys.Up))
            {
                Thrust();
            }
            else
            {
                ThrustOff();
            }

            if (Input.IsKeyDown(Keys.Down))
            {
                Shield(true);
            }
            else
            {
                Shield(false);
            }

            if (Input.IsKeyPressed(Keys.LeftCtrl) || Input.IsKeyPressed(Keys.Space))
            {
                FireShot();
            }
        }

        void FireShot()
        {
            for (int shot = 0; shot < 4; shot++)
            {
                if (!ShotSs[shot].Active)
                {
                    //m_FireSoundInstance.Stop();
                    //m_FireSoundInstance.Play();
                    float speed = 35;

                    ShotSs[shot].Spawn(Position + VelocityFromRadian(Radius, Rotation),
                        VelocityFromRadian(speed, Rotation) + Velocity * 0.75f, 1.55f);

                    break;
                }
            }
        }

        void Thrust()
        {
            //ThurstSoundInstance.Play();
            float maxPerSecond = 50;
            float thrustAmount = 0.55f;
            float testX;
            float testY;

            if (Velocity.Y < 0)
                testY = -Velocity.Y;
            else
                testY = Velocity.Y;

            if (Velocity.X < 0)
                testX = -Velocity.X;
            else
                testX = Velocity.X;

            if (Velocity.Y < 0)
                testY = -Velocity.Y;
            else
                testY = Velocity.Y;

            if (Velocity.X < 0)
                testX = -Velocity.X;
            else
                testX = Velocity.X;

            if (testX + testY < maxPerSecond)
            {
                Acceleration = new Vector3((float)Math.Cos(Rotation) * thrustAmount, (float)Math.Sin(Rotation) * thrustAmount, 0);

                if (FlameTimer.Expired)
                {
                    FlameTimer.Reset();
                    FlameM.Enabled = !FlameM.Enabled;
                }
            }
            else
            {
                ThrustOff();
            }
        }

        void ThrustOff()
        {
            Acceleration = Vector3.Zero;
            FlameM.Enabled = false;
        }
    }
}
