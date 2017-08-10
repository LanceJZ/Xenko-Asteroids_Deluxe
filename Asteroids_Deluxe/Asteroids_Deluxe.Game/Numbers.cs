﻿using System;
using System.Linq;
using System.Collections.Generic;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Engine;

namespace Asteroids_Deluxe
{
    public class Numbers : ScriptComponent
    {
        Prefab[] NumberPFs = new Prefab[10];
        List<Entity> NumberEs = new List<Entity>();
        Vector3 Position = Vector3.Zero;

        public void Initialize()
        {
            NumberPFs[0] = Content.Load<Prefab>("Zero");
            NumberPFs[1] = Content.Load<Prefab>("One");
            NumberPFs[2] = Content.Load<Prefab>("Two");
            NumberPFs[3] = Content.Load<Prefab>("Three");
            NumberPFs[4] = Content.Load<Prefab>("Four");
            NumberPFs[5] = Content.Load<Prefab>("Five");
            NumberPFs[6] = Content.Load<Prefab>("Six");
            NumberPFs[7] = Content.Load<Prefab>("Seven");
            NumberPFs[8] = Content.Load<Prefab>("Eight");
            NumberPFs[9] = Content.Load<Prefab>("Nine");
        }

        public void ProcessNumber(int number, Vector3 locationStart, float scale)
        {
            Position = locationStart;
            int numberIn = number;
            float space = 0;

            ClearNumbers();

            do
            {
                //Make digit the modulus of 10 from number.
                int digit = numberIn % 10;
                //This sends a digit to the draw function with the location and size.
                //MakeNumberMesh(space, digit, scale);
                Entity numberE = InitiateNumber(digit);
                numberE.Transform.Position = Position - new Vector3(space, 0, 0);
                numberE.Transform.Scale = new Vector3(scale);
                // Dividing the int by 10, we discard the digit that was derived from the modulus operation.
                numberIn /= 10;
                // Move the location for the next digit location to the left. We start on the right hand side
                // with the lowest digit.
                space += scale * 2;

            } while (numberIn > 0);
        }

        Entity InitiateNumber(int number)
        {
            NumberEs.Add(NumberPFs[number].Instantiate().First());
            SceneSystem.SceneInstance.RootScene.Entities.Add(NumberEs.Last());
            return NumberEs.Last();
        }

        void RemoveNumber(Entity numberE)
        {
            NumberEs.Remove(numberE);
            SceneSystem.SceneInstance.RootScene.Entities.Remove(numberE);
        }

        void ClearNumbers()
        {
            foreach (Entity numberE in NumberEs)
            {
                SceneSystem.SceneInstance.RootScene.Entities.Remove(numberE);
            }

            NumberEs.Clear();
        }
    }
}
