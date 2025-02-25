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

namespace Game
{
    public class GameMenu : MonoBehaviour
    {
        [SerializeField]
        protected StartMenu start;
        public StartMenu Start { get { return start; } }

        [SerializeField]
        protected VersusMenu versus;
        public VersusMenu Versus { get { return versus; } }

        [SerializeField]
        protected MultiplayerMenu multiplayer;
        public MultiplayerMenu Multiplayer { get { return multiplayer; } }

        [SerializeField]
        protected RoomMenu room;
        public RoomMenu Room { get { return room; } }

        [SerializeField]
        protected Dice dice;
        public Dice Dice { get { return dice; } }
        protected LeaderboardManager leaderboard;
        public LeaderboardManager Leaderboard { get { return leaderboard; } }

        [SerializeField]
        protected PauseMenu pause;
        public PauseMenu Pause { get { return pause; } }

        [SerializeField]
        protected Popup popup;
        public Popup Popup { get { return popup; } }
        [SerializeField]
        protected PopupQuestion popupQuestion;
        public PopupQuestion PopupQuestion { get { return popupQuestion; } }

        public QuestionManager QuestionManager;

        void Awake()
        {
            QuestionManager = Core.Instance.QuestionManager;
        }

        public void Init()
        {
            pause.Init();

            popup.Init();
        }

        void Update()
        {
            pause.Process();
        }
    }
}