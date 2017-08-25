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
    struct HighScoreData
    {
        public string Name;
        public int Score;
    }

    struct HighScoreModel
    {
        public Entity Rank;
        public Entity Score;
        public Entity Name;
    }

    public class HighScores : ScriptComponent
    {
        HighScoreData[] HighScoreList = new HighScoreData[10];
        HighScoreModel[] HighScoreModelsList = new HighScoreModel[10];
        Entity[] EnterInitialsTextEs = new Entity[4];
        string[] EnterInitialsText = new string[4];
        char[] HighScoreSelectedLetters = new char[3];

        Entity NewHighScoreLettersE;
        Entity HighScoreTextE;
        Stream stream;

        bool newHighScore;
        int HighScoreSelector = 0;
        int NewHighScoreRank = 0;
        string m_FileName = "/roaming/Score.sav";

        public Vector2 Edge;

        public int HighScore { get => HighScoreList[0].Score; }
        //{
        //    get { return HighScoreList[0].Score; }
        //}

        public bool NewHighScore { get => newHighScore; }

        public void Start()
        {
            EnterInitialsText[0] = "YOUR SCORE IS ONE OF THE TEN BEST";
            EnterInitialsText[1] = "PLEASE ENTER YOUR INITIALS";
            EnterInitialsText[2] = "ROTATE TO SELECT LETTER";
            EnterInitialsText[3] = "SHIELD WHEN LETTER IS CURRECT";

            for (int i = 0; i < 10; i ++)
            {
                HighScoreList[i].Name = "";
                HighScoreList[i].Score = 0;
            }

            NewHighScoreLettersE = new Entity { new Words() };
            SceneSystem.SceneInstance.RootScene.Entities.Add(NewHighScoreLettersE);
            NewHighScoreLettersE.Get<Words>().Initialize();

            HighScoreTextE = new Entity { new Words() };
            SceneSystem.SceneInstance.RootScene.Entities.Add(HighScoreTextE);
            HighScoreTextE.Get<Words>().Initialize();
            HighScoreTextE.Get<Words>().ProcessWords("HIGH SCORES", new Vector3(-10, 22, 0), 1);

            for (int i = 0; i < 4; i++)
            {
                EnterInitialsTextEs[i] = new Entity { new Words() };
                SceneSystem.SceneInstance.RootScene.Entities.Add(EnterInitialsTextEs[i]);
                EnterInitialsTextEs[i].Get<Words>().Initialize();
                EnterInitialsTextEs[i].Get<Words>().ProcessWords(EnterInitialsText[i], new Vector3(-37.5f, -6 + (-i * 2.5f), 0), 1);
                EnterInitialsTextEs[i].Get<Words>().ShowWords(false);
            }

            for (int i = 0; i < 10; i++)
            {
                HighScoreModelsList[i].Rank = new Entity { new Numbers() };
                HighScoreModelsList[i].Score = new Entity { new Numbers() };
                HighScoreModelsList[i].Name = new Entity { new Words() };
                SceneSystem.SceneInstance.RootScene.Entities.Add(HighScoreModelsList[i].Rank);
                HighScoreModelsList[i].Rank.Get<Numbers>().Initialize();
                SceneSystem.SceneInstance.RootScene.Entities.Add(HighScoreModelsList[i].Score);
                HighScoreModelsList[i].Score.Get<Numbers>().Initialize();
                SceneSystem.SceneInstance.RootScene.Entities.Add(HighScoreModelsList[i].Name);
                HighScoreModelsList[i].Name.Get<Words>().Initialize();
            }

            SetupHighScoreList();
        }

        public void EndingScore(int score)
        {
            for (int rank = 0; rank < 10; rank++)
            {
                if (score > HighScoreList[rank].Score)
                {
                    if (rank < 9)
                    {
                        // Move High Score at rank list to make room for new High Score.
                        HighScoreData[] oldScores = new HighScoreData[10];

                        for (int oldranks = rank; oldranks < 10; oldranks++)
                        {
                            oldScores[oldranks].Score = HighScoreList[oldranks].Score;
                            oldScores[oldranks].Name = HighScoreList[oldranks].Name;
                        }

                        for (int oldranks = rank; oldranks < 9; oldranks++)
                        {
                            HighScoreList[oldranks + 1].Score = oldScores[oldranks].Score;
                            HighScoreList[oldranks + 1].Name = oldScores[oldranks].Name;
                        }
                    }

                    HighScoreList[rank].Score = score;
                    //SaveNewHighScoreList();
                    NewHighScoreRank = rank;
                    newHighScore = true;
                    NewHighScoreLettersE.Components.Get<Words>().ShowWords(true);
                    HighScoreSelectedLetters = "___".ToCharArray();

                    for (int line = 0; line < 4; line++)
                    {
                        EnterInitialsTextEs[line].Components.Get<Words>().ShowWords(true);
                    }

                    break;
                }
            }

            if (!NewHighScore)
            {
                DisplayHighScoreList(true);
            }
        }

        public void AddNewHighScore()
        {
            string name = "";

            for (int i = 0; i < 3; i++)
            {
                name += HighScoreSelectedLetters[i];
            }

            NewHighScoreLettersE.Components.Get<Words>().ProcessWords(name, new Vector3(0, 0, 0), 1);

            if (Input.IsKeyPressed(Keys.Right))
            {
                HighScoreSelectedLetters[HighScoreSelector]++;

                if (HighScoreSelectedLetters[HighScoreSelector] == 96)
                {
                    HighScoreSelectedLetters[HighScoreSelector] = (char)65;
                    return;
                }

                if (HighScoreSelectedLetters[HighScoreSelector] > 90)
                {
                    HighScoreSelectedLetters[HighScoreSelector] = (char)95;
                    return;
                }
            }

            if (Input.IsKeyPressed(Keys.Left))
            {
                HighScoreSelectedLetters[HighScoreSelector]--;

                if (HighScoreSelectedLetters[HighScoreSelector] == 94)
                {
                    HighScoreSelectedLetters[HighScoreSelector] = (char)90;
                    return;
                }

                if (HighScoreSelectedLetters[HighScoreSelector] < 65)
                {
                    HighScoreSelectedLetters[HighScoreSelector] = (char)95;
                    return;
                }
            }

            if (Input.IsKeyPressed(Keys.Down))
            {
                HighScoreSelector++;

                if (HighScoreSelector > 2)
                {
                    HighScoreSelector = 0;
                    newHighScore = false;
                    HighScoreList[NewHighScoreRank].Name = name;
                    SaveNewHighScoreList();
                    UpdateHighScoreList();
                    NewHighScoreLettersE.Components.Get<Words>().ShowWords(false);

                    for (int line = 0; line < 4; line++)
                    {
                        EnterInitialsTextEs[line].Components.Get<Words>().ShowWords(false);
                    }

                    DisplayHighScoreList(true);
                }
            }
        }

        public void DisplayHighScoreList(bool show)
        {
            for (int i = 0; i < 10; i++)
            {
                if (HighScoreList[i].Score > 0)
                {
                    HighScoreModelsList[i].Rank.Get<Numbers>().ShowNumbers(show);
                    HighScoreModelsList[i].Score.Get<Numbers>().ShowNumbers(show);
                    HighScoreModelsList[i].Name.Get<Words>().ShowWords(show);
                }

                HighScoreTextE.Get<Words>().ShowWords(show);
            }
        }

        void UpdateHighScoreList()
        {
            Vector3 loc = new Vector3(0, 18, 0);

            for (int i = 0; i < 10; i++)
            {
                if (HighScoreList[i].Score > 0)
                {
                    HighScoreModelsList[i].Rank.Get<Numbers>().
                        ProcessNumber(i + 1, new Vector3(loc.X - 10, loc.Y - i * 2.5f, 0), 0.75f);
                    HighScoreModelsList[i].Score.Get<Numbers>().
                        ProcessNumber(HighScoreList[i].Score, new Vector3(loc.X + 6, loc.Y - i * 2.5f, 0), 0.75f);
                    HighScoreModelsList[i].Name.Get<Words>().
                        ProcessWords(HighScoreList[i].Name, new Vector3(loc.X + 9, loc.Y - i * 2.5f, 0), 0.75f);
                }
            }
        }

        bool DoesFileExist()
        {
            return VirtualFileSystem.FileExists(m_FileName);
        }

        void OpenForWrite()
        {
            stream = VirtualFileSystem.OpenStream(m_FileName, VirtualFileMode.OpenOrCreate, VirtualFileAccess.Write);
        }

        void OpenForRead()
        {
            stream = VirtualFileSystem.OpenStream(m_FileName, VirtualFileMode.Open, VirtualFileAccess.Read);
        }

        void Close()
        {
            byte[] marker = new UTF8Encoding(true).GetBytes("*");
            stream.Write(marker, 0, marker.Length);

            stream.Flush();
            stream.Dispose();
        }

        void SetupHighScoreList()
        {
            if (DoesFileExist())
            {
                OpenForRead();
                string scoreData = Read();

                int score = 0;
                int letter = 0;
                bool isLetter = true;
                string fromNumber = "";

                foreach (char ch in scoreData)
                {
                    if (ch.ToString() == "*")
                        break;

                    if (isLetter)
                    {
                        letter++;
                        HighScoreList[score].Name += ch;

                        if (letter == 3)
                            isLetter = false;
                    }
                    else
                    {
                        if (ch.ToString() == ":")
                        {
                            HighScoreList[score].Score = int.Parse(fromNumber);

                            score++;
                            letter = 0;
                            fromNumber = "";
                            isLetter = true;
                        }
                        else
                        {
                            fromNumber += ch.ToString();
                        }
                    }
                }

                UpdateHighScoreList();
            }
        }

        void SaveNewHighScoreList()
        {
            OpenForWrite();

            for (int i = 0; i < 10; i++)
            {
                if (HighScoreList[i].Score > 0 && HighScoreList[i].Name != "")
                    Write(HighScoreList[i]);
            }

            Close();
        }

        void Write(HighScoreData data)
        {
            byte[] name = new UTF8Encoding(true).GetBytes(data.Name);
            stream.Write(name, 0, name.Length);

            byte[] score = new UTF8Encoding(true).GetBytes(data.Score.ToString());
            stream.Write(score, 0, score.Length);

            byte[] marker = new UTF8Encoding(true).GetBytes(":");
            stream.Write(marker, 0, marker.Length);
        }

        string Read()
        {
            string data = "";

            byte[] b = new byte[1024];
            UTF8Encoding buffer = new UTF8Encoding(true);

            while (stream.Read(b, 0, b.Length) > 0)
            {
                data += buffer.GetString(b, 0, b.Length);
            }

            stream.Dispose();

            return data;
        }
    }
}
