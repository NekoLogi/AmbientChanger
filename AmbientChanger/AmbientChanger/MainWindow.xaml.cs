using System.Threading;
using System.Windows;
using System.Collections.Generic;
using System.Xml;
using System;
using System.Windows.Input;
using System.Windows.Threading;
using System.IO;

namespace AmbientChanger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static bool STARTUP = true;
        bool isKeyPressed = false;
        private static bool THREAD_IS_RUNNING { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            if (STARTUP)
            {
                Start();
                THREAD_IS_RUNNING = true;
                STARTUP = false;

                Thread thread = new(VolumeHandler);
                thread.Start();
            }

            #region player1
            if (player1.player.Volume != Player.MAX_VOLUME && player1.PLAYER_STATE == Player.State.Playing)
            {
                if (!player1.VOLUME_COOLDOWN)
                {
                    player1.player.Volume += 0.1;
                    player1.Cooldown(Player.FADE_SPEED);
                }

            }
            else if (player1.player.Volume != 0 && player1.PLAYER_STATE == Player.State.Stopped)
            {
                if (!player1.VOLUME_COOLDOWN)
                {
                    player1.player.Volume -= 0.1;
                    player1.Cooldown(Player.FADE_SPEED);
                }

            }
            else if (player1.player.Volume == 0 && player1.PLAYER_STATE == Player.State.Stopped)
            {
                player1.Stop();
            }
            #endregion

            #region player2
            if (player2.player.Volume != Player.MAX_VOLUME + 0.1 && player2.PLAYER_STATE == Player.State.Playing)
            {
                if (!player2.VOLUME_COOLDOWN)
                {
                    player2.player.Volume += 0.1;
                    player2.Cooldown(Player.FADE_SPEED);
                }

            }
            else if (player2.player.Volume != 0 && player2.PLAYER_STATE == Player.State.Stopped)
            {
                if (!player2.VOLUME_COOLDOWN)
                {
                    player2.player.Volume -= 0.1;
                    player2.Cooldown(Player.FADE_SPEED);
                }

            }
            else if (player2.player.Volume == 0 && player2.PLAYER_STATE == Player.State.Stopped)
            {
                player2.Stop();
            }
            #endregion

        }

        private static string? PATH { get; set; }
        private static Media CURRENT_PLAYER { get; set; }
        private static List<List<string>> lists = new();

        private static Player player1 = new();
        private static Player player2 = new();

        enum Hotkey
        {
            NumPad1,
            NumPad2,
            NumPad3,
            NumPad4,
            NumPad5,
            NumPad6,
            NumPad7,
            NumPad8,
            NumPad9
        }

        enum Media
        {
            Player1,
            Player2
        }


        public void Start()
        {
            Save();
            Load();
            Init();
        }

        public void Change(int hotkey)
        {
            GetTracks(hotkey);
            if (lists[hotkey].Count != 0)
            {
                Random rnd = new();
                int track = rnd.Next(0, lists[hotkey].Count - 1);
                switch (CURRENT_PLAYER)
                {
                    case Media.Player1:
                        player2.Play(lists[hotkey][track]);
                        player2.PLAYER_STATE = Player.State.Playing;
                        player1.PLAYER_STATE = Player.State.Stopped;
                        CURRENT_PLAYER = Media.Player2;
                        break;

                    case Media.Player2:
                        player1.Play(lists[hotkey][track]);
                        player1.PLAYER_STATE = Player.State.Playing;
                        player2.PLAYER_STATE = Player.State.Stopped;
                        CURRENT_PLAYER = Media.Player1;
                        break;
                }
            }
        }

        public void KeyListener()
        {
            while (THREAD_IS_RUNNING)
            {
                if (!isKeyPressed)
                {
                    if (Keyboard.IsKeyDown(Key.NumPad0))
                        Dispatcher.Invoke(() =>
                        {
                            isKeyPressed = true;
                            Change(0);
                        });
                    else if (Keyboard.IsKeyDown(Key.NumPad1))
                        Dispatcher.Invoke(() =>
                        {
                            isKeyPressed = true;
                            Change(1);
                        });
                    else if (Keyboard.IsKeyDown(Key.NumPad2))
                        Dispatcher.Invoke(() =>
                        {
                            isKeyPressed = true;
                            Change(2);
                        });
                    else if (Keyboard.IsKeyDown(Key.NumPad3))
                        Dispatcher.Invoke(() =>
                        {
                            isKeyPressed = true;
                            Change(3);
                        });
                    else if (Keyboard.IsKeyDown(Key.NumPad4))
                        Dispatcher.Invoke(() =>
                        {
                            isKeyPressed = true;
                            Change(4);
                        });
                    else if (Keyboard.IsKeyDown(Key.NumPad5))
                        Dispatcher.Invoke(() =>
                        {
                            isKeyPressed = true;
                            Change(5);
                        });
                    else if (Keyboard.IsKeyDown(Key.NumPad6))
                        Dispatcher.Invoke(() =>
                        {
                            isKeyPressed = true;
                            Change(6);
                        });
                    else if (Keyboard.IsKeyDown(Key.NumPad7))
                        Dispatcher.Invoke(() =>
                        {
                            isKeyPressed = true;
                            Change(7);
                        });
                    else if (Keyboard.IsKeyDown(Key.NumPad8))
                        Dispatcher.Invoke(() =>
                        {
                            isKeyPressed = true;
                            Change(8);
                        });
                    else if (Keyboard.IsKeyDown(Key.NumPad9))
                        Dispatcher.Invoke(() =>
                        {
                            isKeyPressed = true;
                            Change(9);
                        });
                }
                else if (!Keyboard.IsKeyDown(Key.NumPad0) && !Keyboard.IsKeyDown(Key.NumPad1) && !Keyboard.IsKeyDown(Key.NumPad2) && !Keyboard.IsKeyDown(Key.NumPad3) && !Keyboard.IsKeyDown(Key.NumPad4) && !Keyboard.IsKeyDown(Key.NumPad5) && !Keyboard.IsKeyDown(Key.NumPad6) && !Keyboard.IsKeyDown(Key.NumPad7) && !Keyboard.IsKeyDown(Key.NumPad8) && !Keyboard.IsKeyDown(Key.NumPad9))
                    isKeyPressed = false;

                Thread.Sleep(50);
            }
        }

        private void Init()
        {
            CURRENT_PLAYER = Media.Player2;
            CreateLists();

            Thread thread1 = new(KeyListener);
            thread1.SetApartmentState(ApartmentState.STA);
            thread1.Start();
        }

        private static void CreateLists()
        {
            for (int i = 0; i < 10; i++)
                lists.Add(new List<string>());
        }

        private static void GetTracks(int hotkey)
        {
            DirectoryInfo info = new($"{PATH}/Tracks/Numpad{hotkey}");
            foreach (var item in info.GetFiles("*.mp3*"))
                lists[hotkey].Add(item.ToString());
        }

        private void VolumeHandler()
        {
            while (THREAD_IS_RUNNING)
            {
                Dispatcher.Invoke(() =>
                {
                    #region player1
                    if (player1.player.Volume != Player.MAX_VOLUME && player1.PLAYER_STATE == Player.State.Playing)
                    {
                        if (!player1.VOLUME_COOLDOWN)
                        {
                            player1.player.Volume += 0.1;
                            player1.Cooldown(Player.FADE_SPEED);
                        }

                    }
                    else if (player1.player.Volume != 0 && player1.PLAYER_STATE == Player.State.Stopped)
                    {
                        if (!player1.VOLUME_COOLDOWN)
                        {
                            player1.player.Volume -= 0.1;
                            player1.Cooldown(Player.FADE_SPEED);
                        }

                    }
                    else if (player1.player.Volume == 0 && player1.PLAYER_STATE == Player.State.Stopped)
                    {
                        player1.Stop();
                    }
                    #endregion

                    #region player2
                    if (player2.player.Volume != Player.MAX_VOLUME + 0.1 && player2.PLAYER_STATE == Player.State.Playing)
                    {
                        if (!player2.VOLUME_COOLDOWN)
                        {
                            player2.player.Volume += 0.1;
                            player2.Cooldown(Player.FADE_SPEED);
                        }

                    }
                    else if (player2.player.Volume != 0 && player2.PLAYER_STATE == Player.State.Stopped)
                    {
                        if (!player2.VOLUME_COOLDOWN)
                        {
                            player2.player.Volume -= 0.1;
                            player2.Cooldown(Player.FADE_SPEED);
                        }

                    }
                    else if (player2.player.Volume == 0 && player2.PLAYER_STATE == Player.State.Stopped)
                    {
                        player2.Stop();
                    }
                    #endregion
                });
                Thread.Sleep(50);
            }
        }

        #region Saving
        public static void Save()
        {
            try
            {
                if (!Directory.Exists("Player/"))
                {
                    Directory.CreateDirectory($"Player/Settings");
                    for (int i = 0; i < 10; i++)
                        Directory.CreateDirectory($"Player/Tracks/Numpad{i}");

                    File.WriteAllText("Player/READ ME.txt", "");
                }
                if (!File.Exists($"Player/Settings/Player.xml"))
                {
                    var document1 = new XmlDocument();
                    var main_node1 = document1.CreateElement("Settings");
                    document1.AppendChild(main_node1);

                    var path_node1 = document1.CreateElement("Path");
                    path_node1.InnerText = "Player";
                    main_node1.AppendChild(path_node1);

                    var maxVolume_node1 = document1.CreateElement("MaxVolume");
                    maxVolume_node1.InnerText = "0.80";
                    main_node1.AppendChild(maxVolume_node1);
                    var fadeSpeed_node1 = document1.CreateElement("FadeSpeed");
                    fadeSpeed_node1.InnerText = "150";
                    main_node1.AppendChild(fadeSpeed_node1);


                    document1.Save("Player/Settings/Player.xml");
                }
            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void Load()
        {
            using (var xml = new XmlTextReader($"Player/Settings/Player.xml"))
            {
                try
                {
                    while (xml.Read())
                    {
                        if (xml.NodeType == XmlNodeType.Element && xml.Name == "Path")
                        {
                            PATH = xml.ReadString();
                        }
                        else if (xml.NodeType == XmlNodeType.Element && xml.Name == "MaxVolume")
                        {
                            Player.MAX_VOLUME = double.Parse(xml.ReadString());
                        }
                        else if (xml.NodeType == XmlNodeType.Element && xml.Name == "FadeSpeed")
                        {
                            Player.FADE_SPEED = int.Parse(xml.ReadString());
                        }
                    }
                } catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

        }
        #endregion

        #region UI
        private void Window_Closed(object sender, EventArgs e)
        {
            THREAD_IS_RUNNING = false;
        }
        #endregion

    }
}
