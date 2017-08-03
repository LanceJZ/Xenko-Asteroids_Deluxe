﻿using System;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Engine;

namespace Asteroids_Deluxe
{
    public class PO : RockSizes
    {
        public bool Hit { get => hit; set => hit = value; }
        public bool Active { get => active; set => active = value; }
        public bool Pause { get => pause; set => pause = value; }
        public bool GameOver { get => gameOver; set => gameOver = value; }
        public float Radius { get => radius; set => radius = value; }
        public Random RandomGenerator { get => random; set => random = value; }
        protected float RotationVelocity { get => rotationVelocity; set => rotationVelocity = value; }
        protected float Rotation
        {
            get => rotation;
            set
            {
                if (value < 0)
                    value = MathUtil.TwoPi;
                if (value > MathUtil.TwoPi)
                    value = 0;

                rotation = value;
            }
        }

        private bool hit;
        private bool active;
        private bool pause;
        private bool gameOver;
        private float radius;

        public ModelComponent Model;
        public Vector3 Position = Vector3.Zero;
        public Vector3 Velocity = Vector3.Zero;
        public Vector3 Acceleration = Vector3.Zero;
        public Vector3 Scale = Vector3.Zero;
        public float Deceleration = 0;
        protected Vector2 Edge = new Vector2(44, 32);
        private Random random;
        private float rotation = 0;
        private float rotationVelocity = 0;

        public override void Start()
        {

        }

        public override void Update()
        {
            //Calculate movement this frame according to velocity and acceleration.
            float elapsed = (float)Game.UpdateTime.Elapsed.TotalSeconds;

            if (Acceleration == Vector3.Zero)
            {
                Acceleration = -Velocity * Deceleration;
            }

            Velocity += Acceleration;
            Position += Velocity * elapsed;
            Rotation += rotationVelocity * elapsed;

            UpdateActive();
            UpdatePR();
        }

        public void UpdateActive()
        {
            if (Model != null)
                Model.Enabled = Active;
        }

        public void UpdatePR()
        {
            this.Entity.Transform.Position = Position;
            this.Entity.Transform.RotationEulerXYZ = new Vector3(0, 0, rotation);
        }

        public void UpdateScale()
        {
            this.Entity.Transform.Scale = Scale;
        }

        public bool CirclesIntersect(Vector3 Target, float TargetRadius)
        {
            float dx = Target.X - Position.X;
            float dy = Target.Y - Position.Y;
            float rad = Radius + TargetRadius;

            if ((dx * dx) + (dy * dy) < rad * rad)
                return true;

            return false;
        }

        public void CheckForEdge()
        {
            if (Position.X > Edge.X)
            {
                Position.X = -Edge.X;
                UpdatePR();
            }

            if (Position.X < -Edge.X)
            {
                Position.X = Edge.X;
                UpdatePR();
            }

            if (Position.Y > Edge.Y)
            {
                Position.Y = -Edge.Y;
                UpdatePR();
            }

            if (Position.Y < -Edge.Y)
            {
                Position.Y = Edge.Y;
                UpdatePR();
            }
        }

        /// <summary>
        /// Returns random number from zero to Pi times two.
        /// </summary>
        /// <returns>float</returns>
        public float RandomRadian()
        {
            return (float)random.NextDouble() * (float)(MathUtil.TwoPi);
        }

        public float RandomHieght()
        {
            return random.Next((int)-Edge.Y, (int)Edge.Y);
        }

        /// <summary>
        /// Sets the velocity by direction in radian.
        /// </summary>
        /// <param name="speed">The velocity of object.</param>
        /// <param name="radian">The direction of object.</param>
        public void SetVelocity(float speed, float radian)
        {
            Velocity = new Vector3((float)Math.Cos(radian) * speed, (float)Math.Sin(radian) * speed, 0);
        }

        /// <summary>
        /// Sets the velocity in a random direction with a random speed.
        /// </summary>
        /// <param name="speedMax">Maximum speed.</param>
        /// <param name="speedMin">Minimum speed.</param>
        public void SetRandomVelocity(float speedMax, float speedMin)
        {
            float rad = RandomRadian();
            float amt = (float)random.NextDouble() * speedMax + (speedMin);
            Velocity = new Vector3((float)Math.Cos(rad) * amt, (float)Math.Sin(rad) * amt, 0);
        }
    }
}
