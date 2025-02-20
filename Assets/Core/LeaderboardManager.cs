using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

namespace Game
{
    public class LeaderboardManager : UIElement
    {
        public GameObject leaderboardPanel;
        public GameObject leaderboardEntryPrefab;
        public Transform leaderboardContent;
        private ScoreManager scoreManager;

        public GameObject btnShowLeaderboard;
        public GameObject btnHideLeaderboard;

        private void Start()
        {
            scoreManager = FindObjectOfType<ScoreManager>();
            if (scoreManager == null)
            {
                Debug.LogError("ScoreManager is missing in the scene.");
            }
        }

        public void UpdateLeaderboard()
        {
            // Hapus semua entri leaderboard sebelumnya
            foreach (Transform child in leaderboardContent)
            {
                Destroy(child.gameObject);
            }

            // Ambil semua skor dari ScoreManager
            var allScores = scoreManager.GetAllScores();

            // Urutkan berdasarkan akurasi (jumlah jawaban benar / total pertanyaan) dari tertinggi ke terendah
            var sortedScores = allScores.OrderByDescending(entry => scoreManager.GetPlayerAccuracy(entry.Key)).ToList();

            foreach (var entry in sortedScores)
            {
                var playerID = entry.Key;
                var stats = entry.Value;
                string pawnName = stats.pawnName;

                string playerName = GetPlayerNameByID(playerID);
                float accuracy = scoreManager.GetPlayerAccuracy(playerID);

                // Buat entri baru di leaderboard
                var leaderboardEntry = Instantiate(leaderboardEntryPrefab, leaderboardContent);
                var entryText = leaderboardEntry.GetComponent<Text>();
                if (entryText != null)
                {
                    entryText.text = $"{pawnName}: {stats.points} / {stats.questions} questions = {accuracy:F2}";
                }
            }
        }

        public void ShowLeaderboard()
        {
            leaderboardPanel.SetActive(true);
            btnShowLeaderboard.SetActive(false);
            btnHideLeaderboard.SetActive(true);
            UpdateLeaderboard();
        }

        public void HideLeaderboard()
        {
            leaderboardPanel.SetActive(false);
            btnHideLeaderboard.SetActive(false);
            btnShowLeaderboard.SetActive(true);
        }

        private string GetPlayerNameByID(int playerID)
        {
            Player photonPlayer = PhotonNetwork.CurrentRoom.Players.Values.FirstOrDefault(p => p.ActorNumber == playerID);
            return photonPlayer != null ? photonPlayer.NickName : $"Player {playerID}";
        }
    }
}
