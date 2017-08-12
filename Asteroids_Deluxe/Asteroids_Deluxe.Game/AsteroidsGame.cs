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
        readonly float PodTimerAmount = 20.25f; //30.25
        Random Random;
        Prefab RockPF;
        ClearToSpawn ClearToSpawnS = new ClearToSpawn();
        Player PlayerS;
        UFO UFOS;
        PodGroup PodGroupS;
        Explode UFOExplodeS;
        Numbers ScoreS;

        List<PlayerShip> PlayerLifeSs = new List<PlayerShip>();
        List<Rock> RockSs = new List<Rock>();
        List<Explode> RockExplodeSs = new List<Explode>();
        List<PodPair> PodPairSs = new List<PodPair>();
        List<Pod> PodSs = new List<Pod>();
        List<Explode> PodExplodeSs = new List<Explode>();

        int LargeRockAmount = 4;
        int RockCount = 0;
        int Wave = 0;
        int UFOSpawnCount = 0;
        int PlayerLives = 0;
        bool PodTimerStarted;
        bool GameOver = true;
        bool Paused;

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
            //PlayerS.Active = false;
            ClearToSpawnS.Start();

            Entity UFOE = UFOPF.Instantiate().First();
            SceneSystem.SceneInstance.RootScene.Entities.Add(UFOE);
            UFOS = UFOE.Get<UFO>();
            UFOS.RandomGenerator = this.Random;
            UFOS.PlayerRef = PlayerS;
            UFOExplodeS = UFOE.Get<Explode>();
            UFOExplodeS.RandomGenerator = this.Random;


            Entity podGroupE = podGroupPF.Instantiate().First();
            SceneSystem.SceneInstance.RootScene.Entities.Add(podGroupE);
            PodGroupS = podGroupE.Get<PodGroup>();
            PodGroupS.RandomGenerator = this.Random;
            PodGroupS.PlayerRef = PlayerS;
            PodGroupS.UFORef = UFOS;
            UFOS.PodGroupRef = PodGroupS;

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
                PodExplodeSs.Add(podE.Get<Explode>());
                PodExplodeSs.Last().RandomGenerator = this.Random;
            }

            Entity scoreE = new Entity { new Numbers() };
            SceneSystem.SceneInstance.RootScene.Entities.Add(scoreE);
            ScoreS = scoreE.Get<Numbers>();
            ScoreS.Initialize();
            PlayerS.ScoreRef = ScoreS;
            PlayerS.SetScore(0);
        }

        public override void Update()
        {
            if (!Paused)
            {
                RockController();
                UFOController();
                PodController();
            }

            if (GameOver)
            {
                if (Input.IsKeyPressed(Keys.N))
                    NewGame();
            }
            else
            {
                PlayerController();

                if (Input.IsKeyPressed(Keys.P))
                {
                    Paused = !Paused;
                    PlayerS.SetPause(!PlayerS.Paused);
                    UFOS.SetPause(!UFOS.Paused);
                    UFOSpawnTimer.SetPause(!UFOSpawnTimer.Paused);
                    PodSpawnTimer.SetPause(!PodSpawnTimer.Paused);

                    if (PodGroupS.Active)
                        PodGroupS.SetPause(!PodGroupS.Paused);

                    foreach (Rock rock in RockSs)
                    {
                        rock.Paused = !rock.Paused;
                    }

                    foreach (PodPair pair in PodPairSs)
                    {
                        if (pair.Active)
                            pair.SetPause(!pair.Paused);
                    }

                    foreach (Pod pod in PodSs)
                    {
                        if (pod.Active)
                            pod.SetPause(!pod.Paused);
                    }
                }
            }
        }

        void NewGame()
        {
            PlayerS.NewGame();

            foreach (Shot shot in PlayerS.ShotSs)
            {
                shot.Active = false;
            }

            foreach (Rock rock in RockSs)
            {
                rock.Active = false;
            }

            foreach (Pod pod in PodSs)
            {
                pod.Active = false;
            }

            foreach (PodPair podPair in PodPairSs)
            {
                podPair.Active = false;
            }

            PodGroupS.Activate(false);

            UFOS.Active = false;
            UFOS.ShotS.Active = false;
            UFOSpawnTimer.Reset();

            LargeRockAmount = 4;
            RockCount = 0;
            Wave = 0;
            UFOSpawnCount = 0;
            PlayerLives = 4;
            GameOver = false;
            PlayerS.Active = true;

            PlayerLifeDisplay();
        }

        void PlayerLifeDisplay()
        {
            if (PlayerLifeSs.Count < PlayerLives)
            {
                int ships = PlayerLives - PlayerLifeSs.Count;

                for (int i = 0; i < ships; i++)
                {
                    Entity shipE = new Entity { new PlayerShip() };
                    SceneSystem.SceneInstance.RootScene.Entities.Add(shipE);
                    PlayerLifeSs.Add(shipE.Get<PlayerShip>());
                    PlayerLifeSs.Last().LoadModel();
                    PlayerLifeSs.Last().Position(new Vector3((-PlayerLifeSs.Count * 2.5f) - 20, 28, 0));
                }

                return;
            }

            if (PlayerLifeSs.Count > PlayerLives)
            {
                PlayerLifeSs[PlayerLives].Active(false);
            }

            for (int i = 0; i < PlayerLives; i++)
            {
                PlayerLifeSs[i].Active(true);
            }
        }

        void PlayerController()
        {
            if (PlayerS.NewShip)
            {
                PlayerS.NewShip = false;
                PlayerLives++;
                PlayerLifeDisplay();
            }

            if (PlayerS.Active && PlayerS.Hit)
            {
                PlayerS.Disable();

                if (PlayerLives > 0)
                {
                    PlayerLives--;
                    PlayerLifeDisplay();

                    if (PlayerLives < 1)
                    {
                        GameOver = true;
                        return;
                    }
                }
            }

            if (!PlayerS.Active)
            {
                foreach (Rock rock in RockSs)
                {
                    if (rock.Active)
                    {
                        if (ClearToSpawnS.TargetHit(rock))
                        {
                            return;
                        }
                    }
                }

                foreach (Pod pod in PodSs)
                {
                    if (pod.Active)
                    {
                        if (ClearToSpawnS.TargetHit(pod))
                        {
                            return;
                        }
                    }
                }

                foreach (PodPair pair in PodPairSs)
                {
                    if (pair.Active)
                    {
                        if (ClearToSpawnS.TargetHit(pair))
                        {
                            return;
                        }
                    }
                }

                if (PodGroupS.Active)
                {
                    if (ClearToSpawnS.TargetHit(PodGroupS))
                    {
                        return;
                    }
                }

                if (UFOS.Active)
                {
                    if (ClearToSpawnS.TargetHit(UFOS))
                    {
                        return;
                    }
                }

                if (UFOS.ShotS.Active)
                {
                    return;
                }

                PlayerS.Active = true;
            }
        }

        void PodController()
        {
            if (RockCount < 4 && ArePodsDone()) //TODO: Should be 4.
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

            if (PodGroupS.Active && PodGroupS.Hit)
            {
                PodGroupS.Activate(false);

                PodPairSs[0].Spawn(PodGroupS.Position + new Vector3(0, 0.9f, 0), 0);
                PodPairSs[1].Spawn(PodGroupS.Position + new Vector3(1.256f, -1.256f, 0), MathUtil.Pi * 0.33333f);
                PodPairSs[2].Spawn(PodGroupS.Position + new Vector3(-1.256f, -1.256f, 0), -MathUtil.Pi * 0.33333f);
            }

            foreach (PodPair pair in PodPairSs)
            {
                if (pair.Active && pair.Hit)
                {
                    pair.Active = false;

                    for (int ipair = 0; ipair < 2; ipair++)
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            if (!PodSs[i].Active)
                            {
                                Vector3 pos = Vector3.TransformCoordinate(pair.PodCenterVects[ipair],
                                    Matrix.Invert(pair.CenterPodTrans[ipair].WorldMatrix));

                                PodSs[i].Spawn(pos, pair.Rotation + (MathUtil.Pi * ipair));
                                RockExplodeSs[i].Initilize((int)(PodSs[i].Radius * 2) * 5,
                                    (int)(PodSs[i].Radius * 2) * 10);

                                break;
                            }
                        }
                    }

                    break;
                }
            }

            for (int i = 0; i < PodSs.Count; i++)
            {
                if (PodSs[i].Active && PodSs[i].Hit)
                {
                    PodSs[i].Active = false;
                    PodExplodeSs[i].Spawn(PodSs[i].Radius * 0.5f);
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
                UFOS.Spawn(UFOSpawnCount++, Wave);
                UFOExplodeS.Initilize((int)UFOS.Radius * 10, (int)UFOS.Radius * 50);
            }

            if (UFOS.Done || UFOS.Hit)
            {
                UFOSpawnTimer.Reset();
                UFOExplodeS.Spawn(UFOS.Radius * 0.5f);
                UFOS.Active = false;
                UFOS.Hit = false;
                UFOS.Done = false;
            }
        }

        void RockController()
        {
            RockCount = 0;

            for (int rock = 0; rock < RockSs.Count; rock++)
            {
                if (RockSs[rock].Hit && RockSs[rock].Active)
                {
                    RockSs[rock].Active = false;
                    RockExplodeSs[rock].Spawn(RockSs[rock].Radius * 0.5f);

                    switch (RockSs[rock].SizeofRock)
                    {
                        case RockSize.Large:
                            SpawnRocks(RockSs, RockSs[rock].Position, RockSize.Medium, 2);
                            return;

                        case RockSize.Medium:
                            SpawnRocks(RockSs, RockSs[rock].Position, RockSize.Small, 2);
                            return;

                        case RockSize.Small:
                            return;
                    }
                }

                if (RockSs[rock].Active)
                {
                    RockCount++;
                }
            }

            if (RockCount == 0)
            {
                SpawnRocks(RockSs, Vector3.Zero, RockSize.Large, LargeRockAmount);

                if (LargeRockAmount < 12)
                    LargeRockAmount += 2;

                Wave++;

                PodTimerStarted = false;
                PodGroupS.NewRockWave = true;

                foreach (PodPair pair in PodPairSs)
                {
                    pair.NewRockWave = true;
                }

                foreach (Pod pod in PodSs)
                {
                    pod.NewRockWave = true;
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
                    if (!Rocks[rock].Active && !RockExplodeSs[rock].Active)
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
                    RockExplodeSs.Add(rockE.Components.Get<Explode>());
                    RockExplodeSs.Last().RandomGenerator = this.Random;
                    RockExplodeSs.Last().Start();
                    Rocks.Last().RandomGenerator = this.Random;
                    Rocks.Last().PlayerRef = PlayerS;
                    Rocks.Last().UFORef = UFOS;
                }

                Rocks[rockCount].Spawn(position, rockSize);
                RockExplodeSs[rockCount].Initilize((int)(Rocks[rockCount].Radius * 2) * 5,
                    (int)(Rocks[rockCount].Radius * 2) * 10);

                //Rocks[rockCount].GameOver = GameOver;
            }
        }
    }
}
