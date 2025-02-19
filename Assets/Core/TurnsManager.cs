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

namespace Game
{
    public class TurnsManager : MonoBehaviourPun
    {
        public Core Core { get { return Core.Instance; } }

        public NetworkManager Network { get { return Core.Network; } }

        public PlayGrid Grid { get { return Core.Grid; } }

        public PawnsManager Pawns { get { return Core.Pawns; } }
        public NetworkPlayers NetworkPlayers { get { return Core.Network.Players; } }

        public int PawnIndex { get; protected set; }

        public PopupQuestion popupQuestion;
        public LeaderboardManager leaderboardManager;
        private ScoreManager scoreManager;
        public int ClampToPlayerIndex(int value)
        {
            if (value >= Pawns.Count) value = 0;

            return value;
        }

        public void Init()
        {
            PawnIndex = 0;

            coroutines = new Coroutines(this);

            Core.Match.OnBegin += OnBeginMatch;

            Pawns.OnRemove += OnPlayerLeft;
        }

        private void Awake()
        {
            //popupQuestion = FindObjectOfType<PopupQuestion>();
            leaderboardManager = FindObjectOfType<LeaderboardManager>();
            scoreManager = FindObjectOfType<ScoreManager>();

            if (popupQuestion == null)
            {
                Debug.LogError("Error: PopupQuestion is missing in the scene.");
            }

            if (scoreManager == null)
            {
                Debug.LogError("Error: ScoreManager is missing in the scene.");
            }

            if (Core.ScoreManager == null)
            {
                Debug.LogError("Error: ScoreManager is not initialized in Core.");
            }

            if (leaderboardManager == null)
            {
                Debug.LogError("LeaderboardManager is missing in the scene.");
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.W) && PhotonNetwork.IsMasterClient)
            {
                var pawn = Pawns.Locals.Last();

                pawn.SyncProgress(95);
                Roll(pawn, 99 - pawn.Progress);
            }
        }

        void OnBeginMatch()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC(nameof(TurnInitiation), RpcTarget.All, Pawns[PawnIndex].ID);
            }
            else
            {

            }
        }

        [PunRPC]
        void TurnInitiation(int pawnID)
        {
            var pawn = Pawns.Get(pawnID);

            Core.Dice.Interactable = Pawns.IsLocal(pawn);

            if (OnTurnInitiation != null) OnTurnInitiation(pawn);
        }
        public event Action<Pawn> OnTurnInitiation;

        public void Roll(Pawn pawn, int roll)
        {
            Core.Dice.Interactable = false;

            photonView.RPC(nameof(RollRPC), RpcTarget.All, pawn.ID, roll);
        }
        [PunRPC]
        void RollRPC(int pawnID, int roll)
        {
            var pawn = Pawns.Get(pawnID);

            if (Pawns.IsLocal(pawn))
                Core.Dice.Interactable = false;
            else
                Core.Dice.Value = roll;

            if (PhotonNetwork.IsMasterClient)
            {
                if (pawn != Pawns[PawnIndex])
                {
                    Debug.LogWarning("Pawn: " + pawn.name + " Threw dice when it wasn't their turn, ignoring");
                    return;
                }

                if (InTurn)
                {
                    Debug.LogWarning("Pawn: " + pawn.name + " Threw dice mid-turn, ignoring");
                    return;
                }

                photonView.RPC(nameof(TurnStart), RpcTarget.All, pawn.ID, pawn.Progress, roll);
            }
        }

        [PunRPC]
        void TurnStart(int pawnID, int progress, int roll)
        {
            //Debug.Log($"TurnStart: Player {pawnID}, Roll: {roll}, Progress: {progress}");
            var pawn = Pawns.Get(pawnID);

            var target = Grid.Get(Grid[progress], roll);

            coroutines.Start(Procedure(pawn, Grid[progress], target));

            if (OnTurnStart != null) OnTurnStart(pawn, progress, roll);
        }
        public delegate void TurnStartDelegate(Pawn pawn, int progress, int roll);
        public event TurnStartDelegate OnTurnStart;

        Coroutines coroutines;
        public bool InTurn { get { return coroutines.Count > 0; } }
        IEnumerator Procedure(Pawn pawn, PlayGridElement current, PlayGridElement target)
        {
            if (target == null)
            {
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                yield return coroutines.Yield(pawn.Move(target));

                while (pawn.CurrentElement.Transition != null)
                    yield return coroutines.Yield(pawn.Transition(pawn.CurrentElement.Transition));
            }

            if (PhotonNetwork.IsMasterClient)
                photonView.RPC(nameof(HandleQuestion), pawn.photonView.Owner, pawn.ID);
        }

        [PunRPC]
        void HandleQuestion(int pawnID)
        {
            var pawn = Pawns.Get(pawnID);

            if (!Pawns.IsLocal(pawn)) return;
            //Debug.Log($"HandleQuestion: Displaying question for Player {pawnID}");

            // Ambil pertanyaan acak
            global::Question question = Core.QuestionManager.GetRandomQuestion();

            if (question != null)
            {
                if (popupQuestion == null || scoreManager == null)
                {
                    Debug.LogError("Error: Dependencies are not set.");
                    return;
                }
                popupQuestion.ShowQuestion(question, (selectedIndex) =>
                {
                    // Cek jawaban pemain
                    bool isCorrect = question.CorrectAnswerIndex == selectedIndex;
                    Debug.Log(isCorrect ? "Jawaban benar!" : "Jawaban salah!");

                    if (isCorrect)
                    {
                        if (Core.ScoreManager == null)
                        {
                            Debug.LogError("Error: Core.ScoreManager is null.");
                            return;
                        }
                        if (pawn == null)
                        {
                            Debug.LogError("Error: Pawn is null.");
                            return;
                        }

                        if (string.IsNullOrEmpty(pawn.ID.ToString()))
                        {
                            Debug.LogError("Error: Pawn ID is null or empty.");
                            return;
                        }

                        Core.ScoreManager.AddPoints(pawn.ID, 1, pawn.name);
                        Debug.Log($"Player {pawn.name} mendapatkan 1 poin.");
                    }
                    else
                    {
                        Core.ScoreManager.AddPoints(pawn.ID, 0, pawn.name);
                        Debug.Log($"Player {pawn.name} tidak mendapatkan poin.");
                    }
                    // Hitung persentase skor pemain
                    leaderboardManager.UpdateLeaderboard();


                    // Lanjutkan giliran
                    photonView.RPC(nameof(TurnEnd), RpcTarget.MasterClient, pawnID, pawn.Progress);
                });
            }
            else
            {
                Debug.Log("Tidak ada pertanyaan yang tersedia.");
                photonView.RPC(nameof(TurnEnd), RpcTarget.MasterClient, pawnID, pawn.Progress);
            }

        }

        [PunRPC]
        void TurnEnd(int pawnID, int progress)
        {
            if (!PhotonNetwork.IsMasterClient) return;
            //Debug.Log($"TurnEnd: Player {pawnID}, Progress: {progress}");
            var pawn = Pawns.Get(pawnID);

            coroutines.StopAll();

            if (PhotonNetwork.IsMasterClient)
            {
                if (pawn.CurrentElement == Grid.Last)
                    Core.Match.End(pawn);
                else
                    PawnIndex = ClampToPlayerIndex(PawnIndex + 1);
            }
            else
                pawn.SyncProgress(progress);

            pawn.Land();

            if (OnTurnEnd != null) OnTurnEnd(pawn, progress);

            if (PhotonNetwork.IsMasterClient)
                photonView.RPC(nameof(TurnInitiation), RpcTarget.All, Pawns[PawnIndex].ID);
        }
        public delegate void TurnEndDelegate(Pawn pawn, int progress);
        public event TurnEndDelegate OnTurnEnd;

        void OnPlayerLeft(Pawn pawn)
        {
            if (PhotonNetwork.IsConnected)
            {
                if (PawnIndex >= Pawns.Count)
                {
                    coroutines.StopAll();

                    Debug.LogWarning("Player In Turn Disconnected");
                }

                if (PhotonNetwork.IsMasterClient)
                {
                    PawnIndex = ClampToPlayerIndex(PawnIndex);

                    photonView.RPC(nameof(TurnInitiation), RpcTarget.All, Pawns[PawnIndex].ID);
                }
            }
        }

        void OnDestroy()
        {
            Core.Match.OnBegin -= OnBeginMatch;

            Pawns.OnRemove -= OnPlayerLeft;
        }
    }
}
