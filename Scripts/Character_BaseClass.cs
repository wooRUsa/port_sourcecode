using Cysharp.Threading.Tasks;
using Photon.Pun;
using System.Linq;
using UnityEngine;
using static Project.Game.GameManager;

using UnityEngine.UI;
using System;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using DG.Tweening;

namespace Project.Game
{
    public abstract class Character_BaseClass : MonoBehaviourPunCallbacks, IPunObservable
    {
       // public event Action<int> OnPlayerHealthChanged;

        public enum CharacterState
        {
            Idle,
            Action
        }
        private CharacterState currentState;
        private int drawPoint;
        private int healthPoint;
        private int skillPoint;
        private int moveSpeed;
        private Image myHealthBar;

        public int DrawPoint
        { get { return drawPoint; } set { drawPoint = value; } }
        public int HealthPoint
        { get { return healthPoint; } set { healthPoint = value; } }
        public int SkillPoint
        { get { return skillPoint; } set { skillPoint = value; } }
        public int MoveSpeed
        { get { return moveSpeed; } set { moveSpeed = value; } }
        public CharacterState CurrentState
        { get { return currentState; } set { currentState = value; } }
        public Image MyHealthBar
        { get { return myHealthBar; } set { myHealthBar = value; } }


        public abstract UniTask MoveCharacter(Vector3 moveToPosition, float duration);

        [PunRPC]
        public void GetDamaged_Health(int damage)//tileData에서 호출
        {
            int leftHealth;
            leftHealth = this.HealthPoint -= damage;
            this.HealthPoint = leftHealth;

            // todo 서버 중심으로 전환 
            //  Hashtable Def_initialProps = new Hashtable() { { "Player_HP", 0 }, { "Player_SP", 0 } };
            //  PhotonNetwork.LocalPlayer.SetCustomProperties(Def_initialProps);

            myHealthBar.DOFillAmount(HealthPoint / 100.0f, 1.1f);


            

            Debug.Log(" UnderAttack ! Left : " + leftHealth );

        }

        [PunRPC]
        public void CheckAllPlayersReady()// 함수명 string으로 RPC함수에서 그냥 냅다 호출이 가능하여 편리하긴 한데 디버깅이 어려움
                                          // GameManager에서 OnClickIamReady() 함수에서 호출
        {

            var players = PhotonNetwork.PlayerList;


            if (players.All(p => p.CustomProperties.ContainsKey("Ready") && (bool)p.CustomProperties["Ready"]))
            {
                Debug.Log("All players are ready!");

                gameManagerInstance.RequestStartQueue();

            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(this.HealthPoint);
            }
            else
            {
                this.HealthPoint = (int)stream.ReceiveNext();
            }

        }

        // public abstract void EntryAsEnemyCharacter();

        //public abstract UniTask Attack1();
    }
}