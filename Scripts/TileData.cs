using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

namespace Project.Game
{
    public class TileData : MonoBehaviour
    {
        [SerializeField]
        private Color normalColor, offsetColor, damageColor;
        [SerializeField]
        private SpriteRenderer spriteRenderer;
        [SerializeField]
        private GameObject highLight;
        [SerializeField]
        private GameObject attackAreaParticle;
        
        private Vector2Int ownCellPosition;
        private bool attackFlag = false;
        [SerializeField]
        private bool targetOnTile = false;
        private int skillDamage;

        private event Action<bool> OnAttackFlagChanged;

        public Vector2Int OwnCellPosition
        { get { return ownCellPosition; } set { ownCellPosition = value; } }

        public int SkillDamage 
        { get { return skillDamage; } set { skillDamage = value; } }

        public bool AttackFlag 
        { get { return attackFlag; } set { attackFlag = value; } }

        public void UpdateAttackFlag(bool booleanValue)
        {
            this.AttackFlag = booleanValue;
            OnAttackFlagChanged?.Invoke(booleanValue);
        }

        public void InitializeTileData(bool isOffset, float tileSizeWidth, float tileSizeHeight)
        {
            spriteRenderer.color = isOffset ? normalColor : offsetColor;

            this.GetComponent<Transform>().localScale = new Vector3(tileSizeWidth, tileSizeHeight);

            OnAttackFlagChanged += SubscribeOnChangeAttackFlagMethod;

        }

        private void OnMouseEnter()
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                highLight?.SetActive(true);
            }

        }
        private void OnMouseExit()
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                highLight?.SetActive(false);
            }

        }
        private void OnMouseDown()
        {
            //todo 게임메니저에 페이즈설정 다하면 스킬카드 범위설정 페이즈에만 실핼하도록 할것
            if (GameManager.gameManagerInstance.gameState != GameManager.GameState.Select_Range)
            {
                return;
            }
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                //GridManager.gridManagerInstance.clickedPosition = this.transform.position;
                GridManager.gridManagerInstance.SetClickedTileData(this);
            }
        }

        private void SubscribeOnChangeAttackFlagMethod(bool booleanValue)
        {
            if(booleanValue)
            {
                Debug.Log(" atk !! ");
                Debug.Log((GameManager.gameManagerInstance.MyCharacterObject.GetPhotonView().ViewID));
                //여기에서 공격을 실행 할것임
                //해당 타일 위에 캐릭터가 있는지 체크..하는 기능필요함
                if (targetOnTile)
                {
                    //myCharacterObject.GetComponent<PhotonView>().RPC("GetDamaged_Health", RpcTarget.All);

                    //GameManager.gameManagerInstance.EnemyCharacter.GetComponent<Character_BaseClass>().GetDamaged_Health(this.SkillDamage);
                    GameManager.gameManagerInstance.EnemyCharacter.GetComponent<PhotonView>().RPC
                        ("GetDamaged_Health", RpcTarget.AllBuffered, this.SkillDamage);

                    //GameManager.gameManagerInstance.EnemyCharacter.GetComponent<PhotonView>().RPC("AttackAreaParticleActive", RpcTarget.AllBuffered);


                }
            }
        }

        public void AttackAreaParticleActive()
        {
            attackAreaParticle.SetActive(true);
        }
       

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "Player" && GameManager.gameManagerInstance.EnemyCharacter == collision.gameObject)
            {
                this.targetOnTile = true;
                Debug.Log("on tile emeny" + this.OwnCellPosition);
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.tag == "Player" && GameManager.gameManagerInstance.EnemyCharacter == collision.gameObject)
            {
                this.targetOnTile = false;
            }
        }
    }
}

