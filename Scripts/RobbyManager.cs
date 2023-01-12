using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System.Security.Cryptography;
using System;

public class RobbyManager : MonoBehaviourPunCallbacks
{
    private List<PlayerItem> playerItemList = new List<PlayerItem>();
    public PlayerItem playerItemPrefab;
    public Transform playerItemParentTransform;
    [SerializeField]
    private GameObject startPlayButton;

    private int requiredPlayerCountToStartGame = 0;
    private int maxPlayerCount = 2;

    public static RobbyManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            
        }
        else
        {
            Destroy(this);
        }
    }


    private void Start()
    {
        JoinRandomOrCreateRoom();

    }
    private void Update()
    {
        if(PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount >= requiredPlayerCountToStartGame)
        {
            startPlayButton.SetActive(true);
        }
        else
        {
            startPlayButton.SetActive(false);
        }
    }

    private void JoinRandomOrCreateRoom()
    {
        string randomRoomName = this.CreateRandomRoomname();
        
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)maxPlayerCount;
        roomOptions.BroadcastPropsChangeToAll = true;

        PhotonNetwork.JoinRandomOrCreateRoom(new ExitGames.Client.Photon.Hashtable(), roomOptions.MaxPlayers,MatchmakingMode.FillRoom,
            null,null,randomRoomName,roomOptions:roomOptions);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("success join");
        Debug.Log(PhotonNetwork.CurrentRoom.Name);

        UpdatePlayerList();
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log("success create");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        UpdatePlayerList();
    }
    public override void OnLeftRoom()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("0_Entrance");
        base.OnLeftRoom();
    }
    public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    public void OnClickPlayButton()
    {
        PhotonNetwork.LoadLevel("2_Game");
    }

    private string CreateRandomRoomname()
    {
        byte[] random = new Byte[10];
        var randomRoomName = System.Security.Cryptography.RandomNumberGenerator.Create();
        randomRoomName.GetBytes(random);

        Debug.Log("Room Name : " + BitConverter.ToString(random));
        return BitConverter.ToString(random);
    }

    private void UpdatePlayerList()
    {
        foreach(PlayerItem item in playerItemList)
        {
            Destroy(item.gameObject);
        }
        playerItemList.Clear();

        if(PhotonNetwork.CurrentRoom == null)
        {
            return;
        }

        foreach (KeyValuePair<int, Player> players in PhotonNetwork.CurrentRoom.Players)
        {
            PlayerItem newplayerItem = Instantiate(playerItemPrefab, playerItemParentTransform);
            
            newplayerItem.SetplayerInfo(players.Value);

            if(players.Value == PhotonNetwork.LocalPlayer)
            {
                newplayerItem.ApplyLocalChanges();
            }

            playerItemList.Add(newplayerItem);
        }
    }
}
