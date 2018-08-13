using System;
using System.Collections.Generic;
using System.Linq;
using Xenko.Core.Mathematics;
using Xenko.Input;
using Xenko.Engine;
using Xenko.Games.Time;
using Xenko.Graphics;
using Xenko.Rendering;
using Xenko.Audio;

namespace Asteroids_Deluxe
{
    public class AsteroidsGame : RockSizes
    {
        Timer DisplayTimer;
        Timer UFOSpawnTimer;
        Timer PodSpawnTimer;
        readonly float UFOTimerAmount = 10.15f;
        readonly float PodTimerAmount = 20.25f; //30.25
        Random Random;
        Prefab RockPF;
        ClearToSpawn ClearToSpawnS = new ClearToSpawn();
        GameOverDisplay GameOverS;
        Player PlayerS;
        UFO UFOS;
        PodGroup PodGroupS;
        Explode UFOExplodeS;
        Sound RockExplodeSound;
        SoundInstance PlayerStartSI;
        SoundInstance PodSpawnSI;

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
        bool Display;
        bool NewWave;

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

            Entity dispTimer = new Entity { new Timer() };
            SceneSystem.SceneInstance.RootScene.Entities.Add(dispTimer);
            DisplayTimer = PodTimerE.Get<Timer>();
            DisplayTimer.Reset(5);

            Entity display = new Entity { new GameOverDisplay() };
            SceneSystem.SceneInstance.RootScene.Entities.Add(display);
            GameOverS = display.Get<GameOverDisplay>();
            GameOverS.Start();
            GameOverS.Display(false);

            Prefab playerPF = Content.Load<Prefab>("Player");
            Prefab UFOPF = Content.Load<Prefab>("UFO");
            Prefab podGroupPF = Content.Load<Prefab>("PodGroup");
            Prefab podPairPF = Content.Load<Prefab>("PodPair");
            Prefab podPF = Content.Load<Prefab>("Pod");
            RockPF = Content.Load<Prefab>("Rock");
            RockExplodeSound = Content.Load<Sound>("RockExplode");

            Entity playerE = playerPF.Instantiate().First();
            SceneSystem.SceneInstance.RootScene.Entities.Add(playerE);
            PlayerS = playerE.Get<Player>();
            PlayerS.RandomGenerator = this.Random;
            ClearToSpawnS.Start();
            PlayerStartSI = Content.Load<Sound>("ADPlayerStart").CreateInstance();
            PlayerStartSI.Volume = 0.15f;

            Entity UFOE = UFOPF.Instantiate().First();
            SceneSystem.SceneInstance.RootScene.Entities.Add(UFOE);
            UFOS = UFOE.Get<UFO>();
            UFOS.RandomGenerator = this.Random;
            UFOS.PlayerRef = PlayerS;
            UFOS.GameOver = true;
            UFOExplodeS = UFOE.Get<Explode>();
            UFOExplodeS.RandomGenerator = this.Random;
            UFOExplodeS.ExplodeSI = Content.Load<Sound>("UFOExplode").CreateInstance();
            UFOExplodeS.ExplodeSI.Volume = 0.25f;
            UFOExplodeS.GameOver = true;

            Entity podGroupE = podGroupPF.Instantiate().First();
            SceneSystem.SceneInstance.RootScene.Entities.Add(podGroupE);
            PodGroupS = podGroupE.Get<PodGroup>();
            PodGroupS.RandomGenerator = this.Random;
            PodGroupS.PlayerRef = PlayerS;
            PodGroupS.UFORef = UFOS;
            UFOS.PodGroupRef = PodGroupS;
            PodSpawnSI = Content.Load<Sound>("ADPodSpawn").CreateInstance();
            PodSpawnSI.Volume = 0.25f;

            InitializeAudio();

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
                PodExplodeSs.Last().ExplodeSI = Content.Load<Sound>("ADPodExplode").CreateInstance();
            }
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

                if (DisplayTimer.Expired && !PlayerS.NewHighScore)
                {
                    DisplayTimer.Reset();
                    GameOverDisplayChange();
                }
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

        void GameOverDisplayChange()
        {
            Display = !Display;

            GameOverS.Display(Display);
            PlayerS.HighDisplay = !Display;
        }

        void NewGame()
        {
            GameOverS.Display(false);
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

            foreach (Explode rockExp in RockExplodeSs)
            {
                rockExp.Clear();
            }

            foreach (Explode podExp in PodExplodeSs)
            {
                podExp.Clear();
            }

            PodGroupS.Activate(false);
            PodSpawnTimer.Reset();

            UFOS.Active = false;
            UFOS.ShotS.Active = false;
            UFOExplodeS.Clear();
            UFOSpawnTimer.Reset();

            LargeRockAmount = 4;
            Wave = 0;
            UFOSpawnCount = 0;
            PlayerLives = 4;
            PlayerController();
            PlayerLifeDisplay();

            IsGameOver(false);
            PlayerS.SetScore(0);
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

        void IsGameOver(bool over)
        {
            GameOver = over;
            UFOExplodeS.GameOver = over;
            UFOS.GameOver = over;
            PlayerS.GameOver = over;

            foreach (Explode rock in RockExplodeSs)
            {
                rock.GameOver = over;
            }

            foreach (Explode pod in PodExplodeSs)
            {
                pod.GameOver = over;
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
                PlayerS.ExplodeS.Spawn(PlayerS.Position, PlayerS.Velocity * 0.05f, PlayerS.Rotation, PlayerS.Radius);
                PlayerS.Disable();

                if (PlayerLives > 0)
                {
                    PlayerLives--;
                    PlayerLifeDisplay();

                    if (PlayerLives < 1)
                    {
                        IsGameOver(true);
                        return;
                    }
                }
            }

            if (!PlayerS.Active && !PlayerS.ExplodeS.Active)
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
                PlayerStartSI.Play();
            }
        }

        void PodController() //TODO: Add pod sound effects.
        {
            if (RockCount < LargeRockAmount) //Split to optimize code. Default is LargeRockAmount.
            {
                if (ArePodsDone())
                {
                    if (!PodTimerStarted)
                    {
                        PodSpawnTimer.Reset();
                        PodTimerStarted = true;
                    }

                    if (PodSpawnTimer.Expired)
                    {
                        if (!GameOver)
                            PodSpawnSI.Play();

                        PodSpawnTimer.Reset();
                        PodGroupS.Spawn();
                        NewWave = false;
                    }
                }
                else
                {
                    PodTimerStarted = false;
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

            for (int ipair = 0; ipair < PodPairSs.Count; ipair++)
            {
                PodPairSs[ipair].NewRockWave = NewWave;

                if (PodPairSs[ipair].Active && PodPairSs[ipair].Hit)
                {
                    PodPairSs[ipair].Active = false;

                    for (int ipod = (ipair * 2); ipod < (ipair * 2) + 2; ipod++)
                    {
                        Vector3 pos = new Vector3(PodPairSs[ipair].CenterPodTrans[ipod - (ipair * 2)].WorldMatrix.M41,
                            PodPairSs[ipair].CenterPodTrans[ipod - (ipair * 2)].WorldMatrix.M42, 0);

                        PodSs[ipod].Spawn(pos, PodPairSs[ipair].Rotation + (MathUtil.Pi * (ipod - ipair)));
                        PodExplodeSs[ipod].Initilize((int)(PodSs[ipod].Radius * 2) * 5,
                            (int)(PodSs[ipod].Radius * 2) * 10);
                        PodExplodeSs[ipod].ExplodeSI.Volume = 0.25f;
                    }

                    break;
                }
            }

            for (int i = 0; i < PodSs.Count; i++)
            {
                PodSs[i].NewRockWave = NewWave;

                if (PodSs[i].Active && PodSs[i].Hit)
                {
                    PodSs[i].Active = false;
                    PodExplodeSs[i].GameOver = GameOver;
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
                if (UFOS.Hit)
                    UFOExplodeS.Spawn(UFOS.Radius * 0.5f);

                UFOSpawnTimer.Reset();
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
                NewWave = true;

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
                    RockExplodeSs.Last().ExplodeSI = RockExplodeSound.CreateInstance();
                    RockExplodeSs.Last().ExplodeSI.Volume = 0.15f;
                    RockExplodeSs.Last().GameOver = GameOver;
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

        void InitializeAudio()
        {
            PlayerS.InitializeAudio(Content.Load<Sound>("ADPlayerFire").CreateInstance(),
                Content.Load<Sound>("ADPlayerThrust").CreateInstance(),
                Content.Load<Sound>("PlayerExplode").CreateInstance(),
                Content.Load<Sound>("ADBonusShip").CreateInstance(),
                Content.Load<Sound>("ADShield").CreateInstance());

            UFOS.InitializeAudio(Content.Load<Sound>("UFOFire").CreateInstance(),
                Content.Load<Sound>("UFOLarge").CreateInstance(),
                Content.Load<Sound>("UFOSmall").CreateInstance());
        }
    }
}
