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
        Timer UFOSpawnTimer;
        readonly float UFOTimerAmount = 10.15f;
        Random Random;
        Prefab RockPF;
        Player PlayerS;
        UFO UFOS;
        List<Rock> LargeRocks = new List<Rock>();
        List<Rock> MedRocks = new List<Rock>();
        List<Rock> SmallRocks = new List<Rock>();

        int LargeRockAmount = 4;
        //bool GameOver;

        public override void Start()
        {
            this.Random = new Random(DateTime.UtcNow.Millisecond * 666);

            Entity UFOTimerE = new Entity { new Timer() };
            SceneSystem.SceneInstance.RootScene.Entities.Add(UFOTimerE);
            UFOSpawnTimer = UFOTimerE.Get<Timer>();
            UFOSpawnTimer.Reset(UFOTimerAmount);

            Prefab playerPF = Content.Load<Prefab>("Player");
            Prefab UFOPF = Content.Load<Prefab>("UFO");
            RockPF = Content.Load<Prefab>("Rock");

            Entity playerE = playerPF.Instantiate().First();
            SceneSystem.SceneInstance.RootScene.Entities.Add(playerE);
            PlayerS = playerE.Get<Player>();

            Entity UFOE = UFOPF.Instantiate().First();
            SceneSystem.SceneInstance.RootScene.Entities.Add(UFOE);
            UFOS = UFOE.Get<UFO>();
            UFOS.RandomGenerator = this.Random;
        }

        public override void Update()
        {
            CountRocks();

            if (UFOSpawnTimer.Expired && !UFOS.Active)
            {
                SpawnUFO();
            }

            if (UFOS.Done || UFOS.Hit)
            {
                UFOSpawnTimer.Reset();
                UFOS.Active = false;
                UFOS.Done = false;
                UFOS.Hit = false;
            }
        }

        void SpawnUFO()
        {
            UFOS.Spawn(1, 1);
        }

        void CountRocks()
        {
            int rockCount = 0;

            foreach (Rock rock in LargeRocks)
            {
                if (rock.Hit)
                {
                    rock.Active = false;
                    rock.Hit = false;
                    SpawnRocks(MedRocks, rock.Position, RockSize.Medium, 2);
                }

                if (rock.Active)
                {
                    rockCount++;
                    //lgrockCount++;
                }
            }

            foreach (Rock rock in MedRocks)
            {
                if (rock.Hit)
                {
                    rock.Active = false;
                    rock.Hit = false;
                    SpawnRocks(SmallRocks, rock.Position, RockSize.Small, 2);
                }

                if (rock.Active)
                {
                    rockCount++;
                    //mdrockCount++;
                }
            }

            foreach (Rock rock in SmallRocks)
            {
                if (rock.Hit)
                {
                    rock.Active = false;
                    rock.Hit = false;
                }

                if (rock.Active)
                {
                    rockCount++;
                    //smrockCount++;
                }
            }

            if (rockCount == 0)
            {
                SpawnRocks(LargeRocks, Vector3.Zero, RockSize.Large, LargeRockAmount);

                if (LargeRockAmount < 12)
                    LargeRockAmount += 2;
            }

        }

        void SpawnRocks(List<Rock> Rocks, Vector3 position, RockSize rockSize, int count)
        {
            for (int i = 0; i < count; i++)
            {
                bool spawnNewRock = true;
                int rockCount = Rocks.Count;

                for (int rock = 0; rock < rockCount; rock++)
                {

                    if (!Rocks[rock].Active)// && !Rocks[rock].Exploding)
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
                    Rocks.Add(rockE.Components.Get<Rock>());
                    Rocks[rockCount].RandomGenerator = this.Random;
                    Rocks[rockCount].PlayerRef = PlayerS;
                }

                Rocks[rockCount].Spawn(position, rockSize);
                //Rocks[rockCount].GameOver = GameOver;
            }
        }
    }
}
