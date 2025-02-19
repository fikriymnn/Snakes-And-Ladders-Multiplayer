using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ScoreManager : MonoBehaviour
    {
        private Dictionary<int, (int points, int questions)> playerScores = new Dictionary<int, (int points, int questions)>();

        public void AddPoints(int playerID, int points, string pawnName)
        {
            if (playerScores.ContainsKey(playerID))
            {
                playerScores[playerID] = (playerScores[playerID].points + points, playerScores[playerID].questions + 1);
            }
            else
            {
                playerScores[playerID] = (points, 1);
            }

            Debug.Log($"Player {pawnName} now has {playerScores[playerID].points} points and has answered {playerScores[playerID].questions} questions.");
        }

        public (int points, int questions) GetPlayerStats(int playerID)
        {
            return playerScores.TryGetValue(playerID, out var stats) ? stats : (0, 0);
        }

        public Dictionary<int, (int points, int questions)> GetAllScores()
        {
            return new Dictionary<int, (int points, int questions)>(playerScores);
        }

        public float GetPlayerAccuracy(int playerID)
        {
            if (playerScores.TryGetValue(playerID, out var stats))
            {
                return stats.questions > 0 ? (stats.points / (float)stats.questions) * 100 : 0;
            }
            return 0;
        }
    }
}
