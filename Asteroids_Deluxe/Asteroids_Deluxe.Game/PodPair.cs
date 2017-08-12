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
        public List<TransformComponent> CenterPodTrans = new List<TransformComponent>();
        public List<Vector3> PodCenterVects = new List<Vector3>();

        public override void Start()
        {
            Initalize();
            LoadModelChild();
            UpdateActive(false);
            Radius = 1.29f;
            Points = 100;

            CenterPodTrans.Add(this.Entity.FindChild("PointOne").Get<TransformComponent>());
            CenterPodTrans.Add(this.Entity.FindChild("PointTwo").Get<TransformComponent>());

            for (int i = 0; i < 2; i++)
            {
                PodCenterVects.Add(Vector3.Zero);
            }

            base.Start();
        }

        public override void Update()
        {
            if (Active && !Paused)
            {
                for (int i = 0; i < 2; i++)
                {
                    PodCenterVects[i] = Vector3.TransformCoordinate(Position, CenterPodTrans[i].WorldMatrix);
                }

                UpdateDirection();
                CheckEdges();

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
            for (int i = 0; i < 2; i++)
            {
                Vector3 center = Vector3.TransformCoordinate(PodCenterVects[i],
                        Matrix.Invert(CenterPodTrans[i].WorldMatrix));

                if (target.CirclesIntersect(center, Radius))
                    return true;
            }

            return false;
        }
    }
}
