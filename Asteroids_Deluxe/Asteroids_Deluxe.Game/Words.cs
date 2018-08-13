using System;
using System.Linq;
using System.Collections.Generic;
using Xenko.Core.Mathematics;
using Xenko.Input;
using Xenko.Engine;
using Xenko.Games.Time;
using Xenko.Graphics;
using Xenko.Rendering;
using Xenko.Audio;

namespace Asteroids_Deluxe
{
    class Words : ScriptComponent
    {
        Prefab[] WordPFs = new Prefab[27];
        List<Entity> WordEs = new List<Entity>();
        Vector3 Position = Vector3.Zero;

        public void ProcessWords(string words, Vector3 locationStart, float scale)
        {
            DeleteWords();
            int textSize = words.Length;
            float charsize = 2.33f;
            float space = 0;// ((-scale * charsize) * (textSize - 1));
            Position = locationStart;

            foreach (char letter in words)
            {
                if ((int)letter > 64 && (int)letter < 91 || (int)letter == 95)
                {
                    int letval = (int)letter - 65;

                    if ((int)letter == 95)
                        letval = 26;

                    if (letval > -1 && letval < 27)
                    {
                        Entity letterE = InitiateLetter(letval);
                        letterE.Transform.Position = Position - new Vector3(space, 0, 0);
                        letterE.Transform.Scale = new Vector3(scale);
                    }
                }

                space -= scale * charsize;
            }
        }

        Entity InitiateLetter(int letter)
        {
            WordEs.Add(WordPFs[letter].Instantiate().First());
            SceneSystem.SceneInstance.RootScene.Entities.Add(WordEs.Last());
            return WordEs.Last();
        }

        public void DeleteWords()
        {
            foreach (Entity word in WordEs)
            {
                SceneSystem.SceneInstance.RootScene.Entities.Remove(word);
            }

            WordEs.Clear();
        }

        public void ShowWords(bool show)
        {
            if (WordEs != null)
            {
                foreach (Entity word in WordEs)
                {
                    word.Get<ModelComponent>().Enabled = show;
                }
            }
        }

        public void Initialize()
        {
            for (int i = 0; i < 26; i++)
            {
                char letter = (char)(i + 65);

                WordPFs[i] = Content.Load<Prefab>(letter.ToString());
            }

            WordPFs[26] = Content.Load<Prefab>("Underline");
        }
    }
}
