using UnityEngine;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using System;
using ExitGames.Client.Photon;

namespace Project.Game
{
    public class Character_Sonycast : Character_BaseClass
    {
        private Character_Sonycast()
        {
            MoveSpeed = 5;
            HealthPoint = 100;
            SkillPoint = 70;

            Debug.Log("Character_Sonycast is Constructed");
        }

        private void Start()
        {
            PhotonView photonView;
            if (TryGetComponent(out PhotonView view))
            {
                photonView = view;
                if (photonView.IsMine == false)
                {
                    GameManager.gameManagerInstance.EnemyCharacter = this.gameObject;
                    Debug.Log("enemy setted");
                    GameManager.gameManagerInstance.EnemyNameEnum = GameManager.CharacterNameEnum.SonyCast;
                }
            }

            Hashtable Def_initialProps = new Hashtable() { { "Player_HP", HealthPoint }, { "Player_SP", SkillPoint } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(Def_initialProps);

        }

        

        public override async UniTask MoveCharacter(Vector3 moveToPosition, float duration)
        {
            float runTime = Time.time + duration;
            Vector3 tempVector = new Vector3(0, 0, 0);
            tempVector.Set(moveToPosition.x, moveToPosition.y, 0);

            while (Time.time < runTime)
            {
                transform.position = Vector3.Lerp(transform.position, tempVector, this.MoveSpeed * Time.deltaTime / duration);
                await UniTask.Yield();
            }
        }

        /*public override int GetDamaged_ReturnHealth(int damage)
        {
            int leftHealth;
            leftHealth = this.HealthPoint -= damage;
            this.HealthPoint = leftHealth;
            return leftHealth;
        }*/


    }
}
