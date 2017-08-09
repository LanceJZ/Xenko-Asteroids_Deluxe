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
        Timer PodSpawnTimer;
        readonly float UFOTimerAmount = 10.15f;
        readonly float PodTimerAmount = 5.25f; //30.25
        Random Random;
        Prefab RockPF;
        Player PlayerS;
        UFO UFOS;
        PodGroup PodGroupS;
        List<Rock> RockSs = new List<Rock>();
        List<PodPair> PodPairSs = new List<PodPair>();
        List<Pod> PodSs = new List<Pod>();

        int LargeRockAmount = 4;
        int RockCount = 0;
        bool PodTimerStarted;
        //bool GameOver;

        public override void Start()
        {
            this.Random = new Random(DateTime.UtcNow.Millisecond * 666);

            Entity UFOTimerE = new Entity { new Timer() };
            SceneSystem.SceneInstance.RootScene.Entities.Add(UFOTimerE);
            UFOSpawnTimer = UFOTimerE.Get<Timer>();
            UFOSpawnTimer.Reset(UFOTimerAmount);

            Entity PodTimerE = new Entity { new Timer() };
            SceneSystem.SceneInstance.RootScene.Entities.Add(PodTimerE);
            PodSpawnTimer = PodTimerE.Get<Timer>();
            PodSpawnTimer.Reset(PodTimerAmount);

            Prefab playerPF = Content.Load<Prefab>("Player");
            Prefab UFOPF = Content.Load<Prefab>("UFO");
            Prefab podGroupPF = Content.Load<Prefab>("PodGroup");
            Prefab podPairPF = Content.Load<Prefab>("PodPair");
            Prefab podPF = Content.Load<Prefab>("Pod");
            RockPF = Content.Load<Prefab>("Rock");

            Entity playerE = playerPF.Instantiate().First();
            SceneSystem.SceneInstance.RootScene.Entities.Add(playerE);
            PlayerS = playerE.Get<Player>();

            Entity UFOE = UFOPF.Instantiate().First();
            SceneSystem.SceneInstance.RootScene.Entities.Add(UFOE);
            UFOS = UFOE.Get<UFO>();
            UFOS.RandomGenerator = this.Random;
            UFOS.PlayerRef = PlayerS;

            Entity podGroupE = podGroupPF.Instantiate().First();
            SceneSystem.SceneInstance.RootScene.Entities.Add(podGroupE);
            PodGroupS = podGroupE.Get<PodGroup>();
            PodGroupS.RandomGenerator = this.Random;
            PodGroupS.PlayerRef = PlayerS;
            PodGroupS.UFORef = UFOS;

            for (int i = 0; i < 3; i++)
            {
                Entity podPairE = podPairPF.Instantiate().First();
                SceneSystem.SceneInstance.RootScene.Entities.Add(podPairE);
                PodPairSs.Add(podPairE.Get<PodPair>());
                PodPairSs[i].RandomGenerator = this.Random;
                PodPairSs[i].PlayerRef = PlayerS;
                PodPairSs[i].UFORef = UFOS;
            }

            for (int i = 0; i < 6; i++)
            {
                Entity podE = podPF.Instantiate().First();
                SceneSystem.SceneInstance.RootScene.Entities.Add(podE);
                PodSs.Add(podE.Get<Pod>());
                PodSs[i].RandomGenerator = this.Random;
                PodSs[i].PlayerRef = PlayerS;
                PodSs[i].UFORef = UFOS;
            }
        }

        public override void Update()
        {
            RockController();
            UFOController();
            PodController();
        }

        void PodController()
        {
            if (RockCount < 4 && ArePodsDone())
            {
                if (!PodTimerStarted)
                {
                    PodSpawnTimer.Reset();
                    PodTimerStarted = true;
                }

                if (PodSpawnTimer.Expired)
                {
                    PodSpawnTimer.Reset();
                    PodGroupS.Spawn();
                }
            }
            else
            {
                PodTimerStarted = false;
            }

            if (PodGroupS.Active)
            {
                if (PodGroupS.Hit)
                {
                    PodGroupS.Activate(false);

                    PodPairSs[0].Spawn(PodGroupS.Position + new Vector3(0, 0.9f, 0), 0);
                    PodPairSs[1].Spawn(PodGroupS.Position + new Vector3(1.256f, -1.256f, 0), MathUtil.Pi * 0.33333f);
                    PodPairSs[2].Spawn(PodGroupS.Position + new Vector3(-1.256f, -1.256f, 0), -MathUtil.Pi * 0.33333f);
                }
            }

            foreach (PodPair pair in PodPairSs)
            {
                if (pair.Active && pair.Hit)
                {
                    pair.Active = false;
                    pair.Hit = false;
                }
            }
        }

        bool ArePodsDone()
        {

            if (PodGroupS.Active)
            {
                return false;
            }

            foreach (PodPair podPair in PodPairSs)
            {
                if (podPair.Active)
                {
                    return false;
                }
            }

            foreach (Pod pod in PodSs)
            {
                if (pod.Active)
                {
                    return false;
                }
            }

            return true;
        }

        void UFOController()
        {
            if (UFOSpawnTimer.Expired && !UFOS.Active)
            {
                UFOS.Spawn(1, 1);
            }

            if (UFOS.Done || UFOS.Hit)
            {
                UFOSpawnTimer.Reset();
                UFOS.Active = false;
                UFOS.Done = false;
            }
        }

        void RockController()
        {
            RockCount = 0;

            foreach (Rock rock in RockSs)
            {
                if (rock.Hit && rock.Active)
                {
                    rock.Active = false;

                    RockSize size = rock.SizeofRock;

                    switch (rock.SizeofRock)
                    {
                        case RockSize.Large:
                            SpawnRocks(RockSs, rock.Position, RockSize.Medium, 2);
                            return;

                        case RockSize.Medium:
                            SpawnRocks(RockSs, rock.Position, RockSize.Small, 2);
                            return;
                    }
                }

                if (rock.Active)
                {
                    RockCount++;
                    //lgrockCount++;
                }
            }

            if (RockCount == 0)
            {
                SpawnRocks(RockSs, Vector3.Zero, RockSize.Large, LargeRockAmount);

                if (LargeRockAmount < 12)
                    LargeRockAmount += 2;

                PodTimerStarted = false;
                PodGroupS.NewRockWave = true;

                for (int i = 0; i < 3; i++)
                {
                    PodPairSs[i].NewRockWave = true;
                }
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
                    Rocks[rockCount].UFORef = UFOS;
                }

                Rocks[rockCount].Spawn(position, rockSize);
                //Rocks[rockCount].GameOver = GameOver;
            }
        }
    }
}
