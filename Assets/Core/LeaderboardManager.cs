using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class LeaderboardManager : UIElement
    {
        public GameObject leaderboardPanel;  // Panel untuk menampilkan leaderboard
        public GameObject leaderboardEntryPrefab;  // Prefab untuk setiap entri leaderboard
        public Transform leaderboardContent;  // Parent dari semua entri leaderboard
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

            // Urutkan skor berdasarkan poin
            foreach (var entry in allScores)
            {
                var playerID = entry.Key;
                var stats = entry.Value;

                string playerName = GetPlayerNameByID(playerID);

                // Buat entri baru di leaderboard
                var leaderboardEntry = Instantiate(leaderboardEntryPrefab, leaderboardContent);
                var entryText = leaderboardEntry.GetComponent<Text>();
                if (entryText != null)
                {
                    entryText.text = $"{Core.Pawns.name}: {stats.points} / {stats.questions} questions, Scores: {scoreManager.GetPlayerAccuracy(playerID):F2}";
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

        // Contoh struktur sederhana dalam Dictionary
        Dictionary<int, string> playerNames = new Dictionary<int, string>
        {
            { 1, "Alice" },
            { 2, "Bob" },
            { 3, "Charlie" },
    // Tambahkan data lainnya
        };

        private string GetPlayerNameByID(int playerID)
        {
            return playerNames.TryGetValue(playerID, out string name) ? name : $"Player {playerID}";
        }
    }
}
