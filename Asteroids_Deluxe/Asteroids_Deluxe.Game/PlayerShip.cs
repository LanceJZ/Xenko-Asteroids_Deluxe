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
            Model.Enabled = false;
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
