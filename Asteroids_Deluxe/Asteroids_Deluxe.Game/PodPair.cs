using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Engine;

namespace Asteroids_Deluxe
{
    public class PodPair : Pods
    {
        public List<TransformComponent> CenterPodPoints = new List<TransformComponent>();

        public override void Start()
        {
            Initalize();
            LoadModelChild();
            UpdateActive(false);
            Radius = 2.9f;

            CenterPodPoints.Add(this.Entity.FindChild("PointOne").Get<TransformComponent>());
            CenterPodPoints.Add(this.Entity.FindChild("PointTwo").Get<TransformComponent>());

            base.Start();
        }

        public override void Update()
        {
            if (Active)
            {
                UpdateDirection();

                if (PlayerCollide())
                {
                    SetScore();
                    Hit = true;
                }
            }

            base.Update();
        }

        bool PlayerCollide()
        {
            if (PlayerRef.Active)
            {
                if (Collide(PlayerRef))
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
            foreach (TransformComponent point in CenterPodPoints)
            {
                if (target.CirclesIntersect(point.Position, Radius))
                    return true;
            }

            return false;
        }
    }
}
