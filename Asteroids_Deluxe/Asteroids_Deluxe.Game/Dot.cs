﻿using System;
using System.Collections.Generic;
using Xenko.Core.Mathematics;
using Xenko.Input;
using Xenko.Engine;
using Xenko.Games.Time;
using Xenko.Graphics;
using Xenko.Rendering;
using Xenko.Audio;

namespace Asteroids_Deluxe
{
    public class Dot : PO
    {
        Timer LifeTimer;

        public override void Start()
        {
            base.Start();

            Entity lifeTimerE = new Entity { new Timer() };
            SceneSystem.SceneInstance.RootScene.Entities.Add(lifeTimerE);
            LifeTimer = lifeTimerE.Get<Timer>();
            LoadModel();
            UpdateActive(false);
        }

        public override void Update()
        {
            if (Active)
            {
                if (LifeTimer.Expired)
                {
                    Active = false;
                }
            }

            base.Update();
        }

        public void Spawn(Vector3 position, Vector3 velocity, float rotationV, float life)
        {
            LifeTimer.Reset(life);
            Active = true;
            Position = position;
            Velocity = velocity;
            RotationVelocity = rotationV;
            UpdatePR();
        }
    }
}
