using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace AmbientChanger
{
    internal class Player
    {
        public static int FADE_SPEED { get; set; }
        public static double MAX_VOLUME { get; set; }
        public State PLAYER_STATE { get; set; }

        public bool VOLUME_COOLDOWN = false;

        public MediaPlayer player = new();

        public enum State
        {
            Playing,
            Stopped,
            Paused
        }


        public void Cooldown(int ms)
        {
            if (!VOLUME_COOLDOWN)
            {
                VOLUME_COOLDOWN = true;
                Task.Run(() =>
                {
                    Thread.Sleep(ms);
                    VOLUME_COOLDOWN = false;
                });
            }
        }

        public void Play(string path)
        {
            player.Volume = 0.4;

            player.Open(new Uri(path));
            player.Play();
        }

        public void Stop()
        {
            player.Stop();
        }

        public void Pause()
        {
            player.Pause();
        }
    }
}
