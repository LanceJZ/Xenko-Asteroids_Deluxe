using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiliconStudio.Xenko.Input;
using SiliconStudio.Core.IO;
using System.IO;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Engine;

namespace Asteroids_Deluxe
{
    public class GameOverDisplay : ScriptComponent
    {
        string GameOverText = "GAME OVER";
        string CoinPlayText = "COIN   PLAY";
        string PushStartText = "PUSH START";
        string AtariText = "ATARI";
        int AtariDate = 1980;

        Entity GameOverE;
        Entity CoinPlayE;
        Entity PushStartE;
        Entity AtariE;
        Entity AtariDateE;
        Entity CoinE;
        Entity PlayE;

        public void Start()
        {
            GameOverE = new Entity { new Words() };
            SceneSystem.SceneInstance.RootScene.Entities.Add(GameOverE);
            GameOverE.Get<Words>().Initialize();

            CoinPlayE = new Entity { new Words() };
            SceneSystem.SceneInstance.RootScene.Entities.Add(CoinPlayE);
            CoinPlayE.Get<Words>().Initialize();

            PushStartE = new Entity { new Words() };
            SceneSystem.SceneInstance.RootScene.Entities.Add(PushStartE);
            PushStartE.Get<Words>().Initialize();

            PushStartE = new Entity { new Words() };
            SceneSystem.SceneInstance.RootScene.Entities.Add(PushStartE);
            PushStartE.Get<Words>().Initialize();

            AtariE = new Entity { new Words() };
            SceneSystem.SceneInstance.RootScene.Entities.Add(AtariE);
            AtariE.Get<Words>().Initialize();

            AtariDateE = new Entity { new Numbers() };
            SceneSystem.SceneInstance.RootScene.Entities.Add(AtariDateE);
            AtariDateE.Get<Numbers>().Initialize();

            CoinE = new Entity { new Numbers() };
            SceneSystem.SceneInstance.RootScene.Entities.Add(CoinE);
            CoinE.Get<Numbers>().Initialize();

            PlayE = new Entity { new Numbers() };
            SceneSystem.SceneInstance.RootScene.Entities.Add(PlayE);
            PlayE.Get<Numbers>().Initialize();

            GameOverE.Get<Words>().ProcessWords(GameOverText, new Vector3(-18.15f, 10, 0), 2);
            PushStartE.Get<Words>().ProcessWords(PushStartText, new Vector3(-9, -28, 0), 1);
            AtariE.Get<Words>().ProcessWords(AtariText, new Vector3(2, -32, 0), 0.666f);
            CoinPlayE.Get<Words>().ProcessWords(CoinPlayText, new Vector3(-8, -20, 0), 1);
            CoinE.Get<Numbers>().ProcessNumber(1, new Vector3(-12, -20, 0), 1);
            PlayE.Get<Numbers>().ProcessNumber(1, new Vector3(4, -20, 0), 1);
            AtariDateE.Get<Numbers>().ProcessNumber(AtariDate, new Vector3(-2, -32, 0), 0.666f);
        }

        public void Display(bool show)
        {
            GameOverE.Get<Words>().ShowWords(show);
            CoinPlayE.Get<Words>().ShowWords(show);
            PushStartE.Get<Words>().ShowWords(show);
            AtariE.Get<Words>().ShowWords(show);
            AtariDateE.Get<Numbers>().ShowNumbers(show);
            CoinE.Get<Numbers>().ShowNumbers(show);
            PlayE.Get<Numbers>().ShowNumbers(show);

        }
    }
}