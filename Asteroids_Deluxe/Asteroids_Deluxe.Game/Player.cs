using System;
using System.Linq;
using System.Collections.Generic;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Input;
using SiliconStudio.Xenko.Engine;
using SiliconStudio.Xenko.Graphics;
using SiliconStudio.Xenko.Rendering;
using SiliconStudio.Xenko.Audio;

namespace Asteroids_Deluxe
{
    class Player : PO
    {
        ModelComponent flame;
        ModelComponent shield;

        List<Shot> shotSs;
        List<Entity> shotEs;

        public override void Start()
        {
            base.Start();

            Radius = 1.15f;

            shotSs = new List<Shot>();
            shotEs = new List<Entity>();

            //Content.Load<Prefab>("MyBulletPrefab");
            //SceneSystem.SceneInstance.RootScene.Entities.Add(bullet);
            Prefab shotP = Content.Load<Prefab>("Shot");

            for (int i = 0; i < 4; i++)
            {
                shotEs.Add(shotP.Instantiate().First());
                SceneSystem.SceneInstance.RootScene.Entities.Add(shotEs[i]);
                shotSs.Add(shotEs[i].Get<Shot>());
            }

            Deceleration = 0.01f;

            flame = this.Entity.FindChild("PlayerFlame").Get<ModelComponent>();
            shield = this.Entity.FindChild("PlayerShield").Get<ModelComponent>();
        }

        public override void Update()
        {
            base.Update();

            GetInput();

            CheckForEdge();

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

            //FireShot();

            if (Input.IsKeyDown(Keys.Down))
            {
                Shield();
            }
            else
            {
                ShieldOff();
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
                if (!shotSs[shot].Active)
                {
                    //m_FireSoundInstance.Stop();
                    //m_FireSoundInstance.Play();
                    float speed = 35;
                    Vector3 dir = new Vector3((float)Math.Cos(Rotation) * speed, (float)Math.Sin(Rotation) * speed, 0);
                    Vector3 offset = new Vector3((float)Math.Cos(Rotation) * Radius, (float)Math.Sin(Rotation) * Radius, 0);
                    shotSs[shot].Spawn(Position + offset, dir + Velocity * 0.75f, 1.55f);
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
                flame.Enabled = true;

                //if (m_FlameTimer.TotalTime.Milliseconds > 18)
                //{
                //    m_FlameTimer.Reset();

                //    if (m_FlameMesh.Enabled)
                //        m_FlameMesh.Enabled = false;
                //    else
                //        m_FlameMesh.Enabled = true;
                //}
            }
            else
            {
                ThrustOff();
            }
        }

        void ThrustOff()
        {
            Acceleration = Vector3.Zero;
            flame.Enabled = false;
        }

        void Shield()
        {
            shield.Enabled = true;
        }

        void ShieldOff()
        {
            shield.Enabled = false;
        }
    }
}
