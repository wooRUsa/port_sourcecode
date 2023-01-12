using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PlayerItem : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private TMP_Text playerName;
    private Image backgroundImage;
    public Color highlightColor;
    public GameObject leftArrowBotton;
    public GameObject rightArrowBotton;

    ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();
    [SerializeField]
    private Image playerAvatar;
    [SerializeField]
    private Sprite[] avatarsArray;
    //포톤 제공 클래스
    Player player;

    private void PlayerItemSetup()
    {
        backgroundImage = this.GetComponent<Image>();
        leftArrowBotton.SetActive(false);
        rightArrowBotton.SetActive(false);

        
        playerProperties["playerAvatar"] = 0;
        playerProperties["isReady"] = false;
        
        PhotonNetwork.SetPlayerCustomProperties(playerProperties);
    }

    public void SetplayerInfo(Player _player)
    {
        PlayerItemSetup();
        playerName.text = _player.NickName;
        player = _player;
        UpdatePlayerItem(player);
    }

    public void ApplyLocalChanges()
    {
        leftArrowBotton.SetActive(true);
        rightArrowBotton.SetActive(true);
        backgroundImage.color = highlightColor;
    }

    public void OnClickLeftArrowButton()
    {
        if ((int)playerProperties["playerAvatar"] == 0)
        {
            playerProperties["playerAvatar"] = avatarsArray.Length - 1;
        }
        else
        {
            playerProperties["playerAvatar"] = (int)playerProperties["playerAvatar"] - 1;
        }
        PhotonNetwork.SetPlayerCustomProperties(playerProperties);
    }
    public void OnClickRightArrowButton()
    {
        if ((int)playerProperties["playerAvatar"] == avatarsArray.Length - 1)
        {
            playerProperties["playerAvatar"] = 0;
        }
        else
        {
            playerProperties["playerAvatar"] = (int)playerProperties["playerAvatar"] + 1;
        }
        PhotonNetwork.SetPlayerCustomProperties(playerProperties);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if(player == targetPlayer)
        {
            UpdatePlayerItem(targetPlayer);
        }

        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
    }

    private void UpdatePlayerItem(Player player)
    {
        if(player.CustomProperties.ContainsKey("playerAvatar"))
        {
            playerAvatar.sprite = avatarsArray[(int)player.CustomProperties["playerAvatar"]];
            playerProperties["playerAvatar"] = (int)player.CustomProperties["playerAvatar"];
        }
        else
        {
            playerProperties["playerAvatar"] = 0;
        }
    }
}
