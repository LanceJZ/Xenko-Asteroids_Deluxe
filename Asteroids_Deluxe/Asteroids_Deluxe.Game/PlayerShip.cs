using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiliconStudio.Xenko.Engine;
using SiliconStudio.Core.Mathematics;

namespace Asteroids_Deluxe
{
    class PlayerShip : ScriptComponent
    {
        ModelComponent Model;
        Entity Ship;

        public void LoadModel()
        {
            Prefab shipP = Content.Load<Prefab>("PlayerLife");
            Ship = shipP.Instantiate().First();
            SceneSystem.SceneInstance.RootScene.Entities.Add(Ship);
            Model = Ship.FindChild("Ship").Get<ModelComponent>();
        }

        public void Position(Vector3 position)
        {
            Ship.Transform.Position = position;
        }

        public void Active(bool active)
        {
            Model.Enabled = active;

        }

        public bool Active()
        {
            return Model.Enabled;
        }
    }
}
