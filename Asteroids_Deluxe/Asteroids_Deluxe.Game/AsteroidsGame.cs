using System;
using System.Collections.Generic;
using System.Linq;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Input;
using SiliconStudio.Xenko.Engine;
using SiliconStudio.Xenko.Games.Time;
using SiliconStudio.Xenko.Audio;

namespace Asteroids_Deluxe
{
    public class AsteroidsGame : RockSizes
    {
        Random Random;

        Prefab RockPF;
        Player PlayerS;
        List<Rock> LargeRocks = new List<Rock>();
        List<Rock> MedRocks = new List<Rock>();
        List<Rock> SmallRocks = new List<Rock>();

        bool GameOver;

        public override void Start()
        {
            this.Random = new Random(DateTime.UtcNow.Millisecond * 666);
            Prefab playerPF = Content.Load<Prefab>("Player");
            RockPF = Content.Load<Prefab>("Rock");

            Entity player = playerPF.Instantiate().First();
            SceneSystem.SceneInstance.RootScene.Entities.Add(player);
            PlayerS = player.Get<Player>();
            SpawnRocks(Vector3.Zero, RockSize.Large, 4);
        }

        public override void Update()
        {

        }

        void CountRocks()
        {

        }

        void SpawnLargeRocks(int count)
        {
            for (int i = 0; i < count; i++)
            {
                bool spawnNewRock = true;
                int rockCount = LargeRocks.Count;

                for (int rock = 0; rock < rockCount; rock++)
                {
                    if (!LargeRocks[rock].Active)// && !LargeRocks[rock].ExplosionActive())
                    {
                        spawnNewRock = false;
                        rockCount = rock;
                        break;
                    }
                }

                if (spawnNewRock)
                {
                    Entity rockE = RockPF.Instantiate().First();
                    SceneSystem.SceneInstance.RootScene.Entities.Add(rockE);
                    LargeRocks.Add(rockE.Components.Get<Rock>());
                    LargeRocks[rockCount].RandomGenerator = this.Random;
                }

                LargeRocks[rockCount].Spawn(Vector3.Zero, RockSize.Large);
                //LargeRocks[rock].GameOver = GameOver;
            }
        }

        void SpawnRocks(Vector3 position, RockSize rockSize, int count)
        {
            for (int i = 0; i < count; i++)
            {
                bool spawnNewRock = true;
                int rockCount = 0;

                switch (rockSize)
                {
                    case RockSize.Large:
                        rockCount = LargeRocks.Count;
                        break;

                    case RockSize.Medium:
                        rockCount = MedRocks.Count;
                        break;

                    case RockSize.Small:
                        rockCount = SmallRocks.Count;
                        break;
                }

                for (int rock = 0; rock < rockCount; rock++)
                {
                    switch (rockSize)
                    {
                        case RockSize.Large:
                            if (!LargeRocks[rock].Active)// && !LargeRocks[rock].ExplosionActive())
                            {
                                spawnNewRock = false;
                                rockCount = rock;
                                break;
                            }
                            break;

                        case RockSize.Medium:
                            if (!MedRocks[rock].Active)// && !MedRocks[rock].ExplosionActive())
                            {
                                spawnNewRock = false;
                                rockCount = rock;
                                break;
                            }
                            break;

                        case RockSize.Small:
                            if (!SmallRocks[rock].Active)// && !LargeRocks[rock].ExplosionActive())
                            {
                                spawnNewRock = false;
                                rockCount = rock;
                                break;
                            }
                            break;
                    }
                }

                if (spawnNewRock)
                {
                    Entity rockE = RockPF.Instantiate().First();
                    SceneSystem.SceneInstance.RootScene.Entities.Add(rockE);

                    switch (rockSize)
                    {
                        case RockSize.Large:
                            LargeRocks.Add(rockE.Components.Get<Rock>());
                            LargeRocks[rockCount].RandomGenerator = this.Random;
                            break;

                        case RockSize.Medium:
                            MedRocks.Add(rockE.Components.Get<Rock>());
                            MedRocks[rockCount].RandomGenerator = this.Random;
                            break;

                        case RockSize.Small:
                            SmallRocks.Add(rockE.Components.Get<Rock>());
                            SmallRocks[rockCount].RandomGenerator = this.Random;
                            break;
                    }
                }

                switch (rockSize)
                {
                    case RockSize.Large:
                        LargeRocks[rockCount].Spawn(position, rockSize);
                        //LargeRocks[rockCount].GameOver = GameOver;
                        break;

                    case RockSize.Medium:
                        MedRocks[rockCount].Spawn(position, rockSize);
                        //MedRocks[rockCount].GameOver = GameOver;
                        break;

                    case RockSize.Small:
                        SmallRocks[rockCount].Spawn(position, rockSize);
                        //SmallRocks[rockCount].GameOver = GameOver;
                        break;
                }
            }
        }
    }
}
