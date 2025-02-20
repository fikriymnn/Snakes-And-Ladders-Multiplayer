using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ScoreManager : MonoBehaviourPun
    {
        private Dictionary<int, (string pawnName, int points, int questions)> playerScores =
    new Dictionary<int, (string pawnName, int points, int questions)>();


        // Tambahkan tanda [PunRPC] pada metode yang akan dipanggil melalui RPC
        [PunRPC]
        public void AddPoints(int playerID, int points, string pawnName)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC(nameof(UpdateScoreRPC), RpcTarget.All, playerID, points, pawnName);
            }
        }

        [PunRPC]
        void UpdateScoreRPC(int playerID, int points, string pawnName)
        {
            if (playerScores.ContainsKey(playerID))
            {
                playerScores[playerID] = (pawnName, playerScores[playerID].points + points, playerScores[playerID].questions + 1);
            }
            else
            {
                playerScores[playerID] = (pawnName, points, 1);
            }

            Debug.Log($"[SYNC] Player {pawnName} now has {playerScores[playerID].points} points and has answered {playerScores[playerID].questions} questions.");
        }

        public (string pawnName, int points, int questions) GetPlayerStats(int playerID)
        {
            return playerScores.TryGetValue(playerID, out var stats) ? stats : (string.Empty, 0, 0);
        }

        public Dictionary<int, (string pawnName, int points, int questions)> GetAllScores()
        {
            return new Dictionary<int, (string pawnName, int points, int questions)>(playerScores);
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
