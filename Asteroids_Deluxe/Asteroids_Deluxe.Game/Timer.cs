using System;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Engine;
using SiliconStudio.Xenko.Games.Time;

namespace Asteroids_Deluxe
{
    class Timer : SyncScript
    {
        TimerTick timerTick = new TimerTick();

        float amount = 0;
        bool Paused = false;

        public Timer(float amount)
        {
            this.amount = amount;
        }

        public Timer()
        {

        }

        public float Seconds
        {
            get { return (float)timerTick.TotalTime.TotalSeconds; }
        }

        public float Amount
        {
            get=> amount;

            set
            {
                Reset(value);
            }
        }

        public bool Expired
        {
            get
            {
                return timerTick.TotalTime.TotalSeconds > amount;
            }
        }

        public override void Update()
        {
            if (!Paused)
                timerTick.Tick();
        }

        public void Pause(bool pause)
        {
            Paused = pause;

            if (Paused)
            {
                timerTick.Pause();
            }
            else
            {
                timerTick.Resume();
            }
        }

        public void Reset(float length)
        {
            timerTick.Reset();
            amount = length;
        }

        public void Reset()
        {
            timerTick.Reset();
        }
    }
}
