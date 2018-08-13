using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public class PodPair : Pods
    {
        public List<TransformComponent> CenterPodTrans = new List<TransformComponent>();

        public override void Start()
        {
            Initalize();
            LoadModelChild();
            UpdateActive(false);
            Radius = 1.45f;
            Points = 100;
            Paired = true;

            Entity warmTimerE = new Entity { new Timer() };
            SceneSystem.SceneInstance.RootScene.Entities.Add(warmTimerE);

            base.Start();
        }

        public override void Update()
        {
            if (Active && !Paused && !Hit && Paired)
            {
                UpdateDirection();
                CheckEdges();

                if (PlayerCollide())
                {
                    SetScore();
                    Hit = true;
                }

                if (UFOCollide())
                {
                    Hit = true;
                }
            }

            base.Update();
        }

        public override void Spawn(Vector3 position, float rotation)
        {
            base.Spawn(position, rotation);

            for (int i = 0; i < 2; i++)
            {
                CenterPodTrans[i].WorldMatrix.TranslationVector = position;
            }
        }

        bool UFOCollide()
        {
            if (UFORef.Active && !UFORef.Hit)
            {
                if (Collide(UFORef))
                {
                    UFORef.Hit = true;
                    return true;
                }
            }

            if (UFORef.ShotS.Active)
            {
                if (Collide(UFORef.ShotS))
                {
                    UFORef.ShotS.Active = false;
                    return true;
                }
            }

            return false;
        }

        bool PlayerCollide()
        {
            if (PlayerRef.Active && !PlayerRef.Hit)
            {
                if (PlayerRef.ShieldOn)
                {
                    if (CirclesIntersect(PlayerRef.Position, PlayerRef.ShieldRadius))
                    {
                        PlayerRef.ShieldHit(Position, Velocity);
                    }
                }
                else if (Collide(PlayerRef))
                {
                    PlayerRef.Hit = true;
                    return true;
                }
            }

            foreach (Shot shot in PlayerRef.ShotSs)
            {
                if (shot.Active)
                {
                    if (Collide(shot))
                    {
                        shot.Active = false;
                        return true;
                    }
                }
            }

            return false;
        }

        public bool Collide(PO target)
        {
            for (int i = 0; i < 2; i++)
            {
                if (target.CirclesIntersect(CenterPodTrans[i].WorldMatrix.TranslationVector, Radius))
                    return true;
            }

            return false;
        }
    }
}
