using System;
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
        public int Points { get => points; set => points = value; }
        public float Radius { get => radius; set => radius = value; }
        public float RotationVelocity { get => rotationVelocity; set => rotationVelocity = value; }

        public float Rotation
        {
            get => rotation;
            set
            {
                if (value < 0)
                    value += MathUtil.TwoPi;
                if (value > MathUtil.TwoPi)
                    value -= MathUtil.TwoPi;

                rotation = value;
            }
        }

        public Random RandomGenerator { get => random; set => random = value; }

        bool hit;
        bool active;
        bool pause;
        bool gameOver;
        int points;
        float radius;
        float rotation = 0;
        float rotationVelocity = 0;
        Random random;

        public ModelComponent Model;
        public Vector3 Position = Vector3.Zero;
        public Vector3 Velocity = Vector3.Zero;
        public Vector3 Acceleration = Vector3.Zero;
        public Vector3 Scale = Vector3.Zero;
        public float Deceleration = 0;
        protected Vector2 Edge = new Vector2(44, 32);

        public override void Start()
        {

        }

        public override void Update()
        {
            if (Active)
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

                UpdatePR();
            }

            UpdateActive(Active);
        }

        public void LoadModel()
        {
            Model = this.Entity.Get<ModelComponent>();
        }

        public void LoadModelChild()
        {
            Model = this.Entity.GetChild(0).Get<ModelComponent>();
        }

        public void UpdateActive(bool active)
        {
            Active = active;

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

        public bool CheckForEdge()
        {
            if (Position.X > Edge.X)
            {
                Position.X = -Edge.X;
                return true;
            }

            if (Position.X < -Edge.X)
            {
                Position.X = Edge.X;
                return true;
            }

            if (Position.Y > Edge.Y)
            {
                Position.Y = -Edge.Y;
                return true;
            }

            if (Position.Y < -Edge.Y)
            {
                Position.Y = Edge.Y;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Get a random float between min and max
        /// </summary>
        /// <param name="min">the minimum random value</param>
        /// <param name="max">the maximum random value</param>
        /// <returns>float</returns>
        public float RandomMinMax(float min, float max)
        {
            return min + (float)RandomGenerator.NextDouble() * (max - min);
        }
        /// <summary>
        /// Returns random number from zero to Pi times two.
        /// </summary>
        /// <returns>float</returns>
        public float RandomRadian()
        {
            return RandomMinMax(0, MathUtil.TwoPi);
        }

        /// <summary>
        /// Returns the velocity by direction in radian.
        /// </summary>
        /// <param name="speed">The velocity of object.</param>
        /// <param name="radian">The direction of target object.</param>
        public Vector3 VelocityFromRadian(float speed, float radian)
        {
            return new Vector3((float)Math.Cos(radian) * speed, (float)Math.Sin(radian) * speed, 0);
        }

        /// <summary>
        /// Returns a velocity in a random direction with a random speed.
        /// </summary>
        /// <param name="speedMax">Maximum speed.</param>
        /// <param name="speedMin">Minimum speed.</param>
        public Vector3 RandomVelocity(float speedMin, float speedMax)
        {
            float rad = RandomRadian();
            float amt = RandomMinMax(speedMin, speedMax);
            return new Vector3((float)Math.Cos(rad) * amt, (float)Math.Sin(rad) * amt, 0);
        }

        /// <summary>
        /// Returns a velocity of magnitude in a random direction.
        /// </summary>
        /// <param name="magnitude">Units per second.</param>
        public Vector3 RandomVelocity(float magnitude)
        {
            float rad = RandomRadian();
            return new Vector3((float)Math.Cos(rad) * magnitude, (float)Math.Sin(rad) * magnitude, 0);
        }

        public float AngleFromVectors(Vector3 origin, Vector3 target)
        {
            return (float)(Math.Atan2(target.Y - origin.Y, target.X - origin.X));
        }

        /// <summary>
        /// Returns a velocity using rotation as direction, at magnitude.
        /// </summary>
        /// <param name="rotation">Direction in radians.</param>
        /// <param name="magnitude">Speed in units per second.</param>
        /// <returns></returns>
        public Vector3 VelocityFromAngle(float rotation, float magnitude)
        {
            return new Vector3((float)Math.Cos(rotation) * magnitude, (float)Math.Sin(rotation) * magnitude, 0);
        }

        public Vector3 VelocityFromAngle(float magnitude)
        {
            float ang = RandomRadian();
            return new Vector3((float)Math.Cos(ang) * magnitude, (float)Math.Sin(ang) * magnitude, 0);
        }

        public float RandomHeight()
        {
            return RandomMinMax(-Edge.Y * 0.9f, Edge.Y * 0.9f);
        }

        public Vector3 RandomXEdge()
        {
            return new Vector3(Edge.X, RandomMinMax(-Edge.Y * 0.9f, Edge.Y * 0.9f), 0);
        }

        public float AimAtTarget(Vector3 origin, Vector3 target, float facingAngle, float magnitude)
        {
            float turnVelocity = 0;
            float targetAngle = AngleFromVectors(origin, target);
            float targetLessFacing = targetAngle - facingAngle;
            float facingLessTarget = facingAngle - targetAngle;

            if (Math.Abs(targetLessFacing) > Math.PI)
            {
                if (facingAngle > targetAngle)
                {
                    facingLessTarget = ((MathUtil.TwoPi - facingAngle) + targetAngle) * -1;
                }
                else
                {
                    facingLessTarget = (MathUtil.TwoPi - targetAngle) + facingAngle;
                }
            }

            if (facingLessTarget > 0)
            {
                turnVelocity = -magnitude;
            }
            else
            {
                turnVelocity = magnitude;
            }

            return turnVelocity;
        }
    }
}
