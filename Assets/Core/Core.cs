﻿using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SocialPlatforms.Impl;

namespace Game
{
    [DefaultExecutionOrder(ExecutionOrder)]
    public class Core : MonoBehaviour
    {
        public const int ExecutionOrder = -200;

        public static Core Instance { get; protected set; }

        public NetworkManager Network { get; protected set; }

        public PlayGrid Grid { get; protected set; }

        public static PawnsManager Pawns { get; protected set; }

        public TurnsManager Turns { get; protected set; }

        public MatchManager Match { get; protected set; }

        public GameMenu Menu { get; protected set; }
        public Dice Dice { get { return Menu.Dice; } }
        public LeaderboardManager Leaderboard { get { return Menu.Leaderboard; } }
        public Popup Popup { get { return Menu.Popup; } }
        public PopupQuestion PopupQuestion { get { return Menu.PopupQuestion; } }

        public GameMode Mode { get; protected set; }

        public QuestionManager QuestionManager { get; protected set; }
        public int PlayerScore { get; private set; } = 0;
        public static ScoreManager ScoreManager { get; private set; }
        public LeaderboardManager LeaderboardManager { get; private set; }


        void Awake()
        {
            Instance = this;

            Network = Utility.GetDependancy<NetworkManager>();
            Grid = Utility.GetDependancy<PlayGrid>();
            Pawns = Utility.GetDependancy<PawnsManager>();
            Turns = Utility.GetDependancy<TurnsManager>();
            Match = Utility.GetDependancy<MatchManager>();
            Menu = Utility.GetDependancy<GameMenu>();
            Mode = Utility.GetDependancy<GameMode>();
            QuestionManager = Utility.GetDependancy<QuestionManager>();
            ScoreManager scoreManager = Utility.GetDependancy<ScoreManager>();
            LeaderboardManager = Utility.GetDependancy<LeaderboardManager>();

            if (ScoreManager == null)
            {
                ScoreManager = FindObjectOfType<ScoreManager>();
                if (ScoreManager == null)
                {
                    Debug.LogError("Error: ScoreManager is missing in the scene.");
                }
            }
        }

        IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();

            Network.Init();
            Grid.Init();
            Pawns.Init();
            Turns.Init();
            Match.Init();
            Menu.Init();
            Mode.Init();

            // Pastikan QuestionManager telah diinisialisasi
            if (QuestionManager != null)
            {
                QuestionManager.Init(); // Inisialisasi QuestionManager
            }
            else
            {
                Debug.LogError("QuestionManager is null during Start!");
            }

            PhotonNetwork.GameVersion = Application.version;
        }

        void OnApplicationQuit()
        {
            Network.Stop();
        }

        public void Reload()
        {
            Popup.Show("Reloading");

            Network.Stop(Utility.ReloadScene);
        }

        // Metode untuk menambahkan skor
        public void AddScore(int points)
        {
            PlayerScore += points;
            Debug.Log("Skor saat ini: " + PlayerScore);
        }

        public static class PlayerName
        {
            public static string Default { get { return "Player " + Random.Range(1, 999).ToString(); } }

            public static string PrefKey { get { return nameof(PlayerName); } }

            public static string Value
            {
                get
                {
                    return PlayerPrefs.GetString(PrefKey, Default);
                }
                set
                {
                    PlayerPrefs.SetString(PrefKey, value);

                    if (OnChange != null) OnChange(Value);
                }
            }

            public static event Action<string> OnChange;
        }
    }
}