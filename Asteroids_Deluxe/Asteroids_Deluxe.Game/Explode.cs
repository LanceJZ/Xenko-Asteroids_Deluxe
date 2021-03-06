﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xenko.Core.Mathematics;
using Xenko.Input;
using Xenko.Engine;
using Xenko.Games.Time;
using Xenko.Graphics;
using Xenko.Rendering;
using Xenko.Audio;

namespace Asteroids_Deluxe
{
    public class Explode : PO
    {
        public SoundInstance ExplodeSI;
        List<Dot> Dots = new List<Dot>();
        Prefab DotP;

        public override void Start()
        {
            base.Start();

            DotP = Content.Load<Prefab>("Particle");
        }

        public override void Update()
        {
            if (CheckDone())
            {
                Active = false;
            }

            base.Update();
        }

        public void Initilize(int minCount, int maxCount)
        {
            int count = RandomGenerator.Next(minCount, maxCount);

            if (count > Dots.Count)
            {
                int more = count - Dots.Count;

                for (int i = 0; i < count; i++)
                {
                    Entity dotE = DotP.Instantiate().First();
                    SceneSystem.SceneInstance.RootScene.Entities.Add(dotE);
                    Dots.Add(dotE.Get<Dot>());
                }
            }
        }

        public void Spawn(float radius)
        {
            if (!GameOver)
            {
                ExplodeSI.Play();
            }

            Active = true;
            Position = Entity.Transform.Position;

            foreach (Dot dot in Dots)
            {
                dot.Spawn(Position + new Vector3(RandomMinMax(-radius, radius),
                    RandomMinMax(-radius, radius), 0), RandomVelocity(RandomMinMax(1, 5)),
                    RandomMinMax(1, 6), RandomMinMax(0.5f, 1.5f));
            }

        }

        public void Clear()
        {
            foreach (Dot dot in Dots)
            {
                dot.Active = false;
            }

            Active = false;
        }

        bool CheckDone()
        {
            foreach (Dot dot in Dots)
            {
                if (dot.Active)
                    return false;
            }

            return true;
        }
    }
}
