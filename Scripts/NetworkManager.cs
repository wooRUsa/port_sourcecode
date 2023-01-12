
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.IO;
//using Photon.Pun.Demo.Asteroids;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Project.Game
{
    public class NetworkManager : MonoBehaviourPunCallbacks
    {
        public GameObject[] playerPrefabs;
        public Transform[] spawnPoints;
        private void Awake()
        {
            int spawnIndex;
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                spawnIndex = 0;
            }
            else
            {
                spawnIndex = 1;
            }
            GameObject playerPrefabToSpawn = playerPrefabs[(int)PhotonNetwork.LocalPlayer.CustomProperties["playerAvatar"]];

            Hashtable Def_initialProps = new Hashtable() { { "Player_HP", 0 }, { "Player_SP", 0 } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(Def_initialProps);


            GameObject playerObject = PhotonNetwork.Instantiate(Path.Combine("Prefab", playerPrefabToSpawn.name), spawnPoints[spawnIndex].position,
                Quaternion.identity);

            //적유닛 생성도 같이 해줘야 하나..?

            GameManager.gameManagerInstance.MyCharacterObject = playerObject;
        }
    }
}
