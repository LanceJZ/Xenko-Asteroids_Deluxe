using System;
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
    public class PlayerExplodeControl : PO
    {
        public SoundInstance ExplodeSI;

        PlayerExplode ExplodeFrontS;

        List<PlayerExplode> ExplodeLinesS = new List<PlayerExplode>();
        List<PlayerExplode> ExplodeWingsS = new List<PlayerExplode>();
        List<PlayerExplode> ExplodeRearsS = new List<PlayerExplode>();


        public override void Start()
        {
            base.Start();

            Prefab lineP = Content.Load<Prefab>("ExplodeLine");
            Prefab wingP = Content.Load<Prefab>("ExplodeWing");
            Prefab rearP = Content.Load<Prefab>("ExplodeRear");
            Prefab frontP = Content.Load<Prefab>("ExplodeFront");

            Entity frontE = frontP.Instantiate().First();
            SceneSystem.SceneInstance.RootScene.Entities.Add(frontE);
            ExplodeFrontS = frontE.Get<PlayerExplode>();

            for (int i = 0; i < 6; i++)
            {
                Entity lineE = lineP.Instantiate().First();
                SceneSystem.SceneInstance.RootScene.Entities.Add(lineE);
                ExplodeLinesS.Add(lineE.Get<PlayerExplode>());
            }

            for (int i = 0; i < 2; i++)
            {
                Entity wingE = wingP.Instantiate().First();
                SceneSystem.SceneInstance.RootScene.Entities.Add(wingE);
                ExplodeWingsS.Add(wingE.Get<PlayerExplode>());
            }

            for (int i = 0; i < 2; i++)
            {
                Entity rearE = rearP.Instantiate().First();
                SceneSystem.SceneInstance.RootScene.Entities.Add(rearE);
                ExplodeRearsS.Add(rearE.Get<PlayerExplode>());
            }
        }

        public override void Update()
        {
            if (Active)
            {
                if (ExplosionDone())
                    Active = false;
            }

            base.Update();
        }

        public void Spawn(Vector3 position, Vector3 velocity, float rotation, float radius)
        {
            Active = true;
            ExplodeSI.Play();

            ExplodeFrontS.Spawn(position + VelocityFromRadian(Radius, Rotation),
                velocity += RandomVelocity(RandomMinMax(-1, 1)), rotation,
                RandomMinMax(-2, 2), RandomMinMax(1, 2.5f));

            foreach (PlayerExplode exp in ExplodeLinesS)
            {
                exp.Spawn(position + new Vector3(RandomMinMax(-1, 1)),
                    velocity += RandomVelocity(RandomMinMax(-2, 2)),
                    rotation, RandomMinMax(-5, 5), RandomMinMax(1, 2.5f));
            }

            foreach (PlayerExplode exp in ExplodeWingsS)
            {
                exp.Spawn(position + new Vector3(RandomMinMax(-1, 1)),
                    velocity += RandomVelocity(RandomMinMax(-2, 2)),
                    rotation + MathUtil.TwoPi, RandomMinMax(-4, 4), RandomMinMax(1, 2.5f));
            }

            foreach (PlayerExplode exp in ExplodeRearsS)
            {
                exp.Spawn(position - VelocityFromRadian(Radius, Rotation),
                    velocity += RandomVelocity(RandomMinMax(-2, 2)), rotation + MathUtil.Pi,
                    RandomMinMax(-3, 3), RandomMinMax(1, 2.5f));
            }
        }

        bool ExplosionDone()
        {
            if (ExplodeFrontS.Active)
                return false;

            foreach (PlayerExplode exp in ExplodeLinesS)
            {
                if (exp.Active)
                    return false;
            }

            foreach (PlayerExplode exp in ExplodeRearsS)
            {
                if (exp.Active)
                    return false;
            }

            foreach (PlayerExplode exp in ExplodeWingsS)
            {
                if (exp.Active)
                    return false;
            }

            return true;
        }
    }
}
