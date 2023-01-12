using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
namespace Project.Game
{
    //핸드에 생성된 카드 객체
    public class SkillCard : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text cardName;
        [SerializeField]
        private TMP_Text description;
        [SerializeField]
        private Image artWork;
        [SerializeField]
        private TMP_Text cost;
        [SerializeField]
        private TMP_Text skillDamage;
        private int damageData;
        [SerializeField]
        private GameObject skillCardPrefab;
        private bool isNeedSelectRange = false;
        private SkillDataTable.KeyCode_CardName keyCode_;

        private Transform handSlotTransform;
        private Transform selectedSlotTransform;

        private bool hasBeenPlayed = false;
        //private int moveRange = 0;
        private Vector2Int[] attackRange;

        public bool HasBeenPlayed
        { get { return hasBeenPlayed; } set { hasBeenPlayed = value; } }
        public Vector2Int[] AttackRange
        { get { return attackRange; } set { attackRange = value; } }
        public int AttackDamage
        { get { return damageData; } }

        public void SetSkillCardData(Transform handSlotTransform, Transform selectedSlotTransform, SkillDataTable.KeyCode_CardName keyCode_Card,
            string cardKindName, string cardCost, string damage, string cardDescription, bool isNeedSelectRange, Vector2Int[] attackRange)
        {
            //텍스트 등 프리팹에 입력..
            keyCode_ = keyCode_Card;
            cardName.SetText(cardKindName);
            description.SetText(cardDescription);
            cost.SetText(cardCost);
            skillDamage.SetText(damage);
            this.damageData = Int32.Parse(damage);
            this.handSlotTransform = handSlotTransform;
            this.selectedSlotTransform = selectedSlotTransform;

            if (attackRange != null)
            {
                AttackRange = attackRange;
            }
            else
            {
                AttackRange = null;
            }

            if (isNeedSelectRange)
            {
                this.isNeedSelectRange = true;
            }
            GameManager.OnGameStateChanged += SpendSelectedCards;


        }

        public void Selected_DuplicateSelf()
        {
            // GameObject gameObject = null;
            //GameManager gameManager = GameManager.gameManagerInstance;
            // int temp = 0;


            if (this.transform.parent.transform == this.selectedSlotTransform)

            {
                Debug.Log(GameManager.gameManagerInstance.GetAvailableSeletedSlot.IndexOf(this));
                if (GameManager.gameManagerInstance.GetAvailableSeletedSlot.IndexOf(this) ==
                    GameManager.gameManagerInstance.GetAvailableSeletedSlot.Count - 1)
                {
                    /*gameObject = Instantiate(skillCardPrefab, this.handSlotTransform);
                    gameObject.GetComponentInChildren<Button>().onClick.AddListener(() =>
                    gameObject.GetComponent<SkillCard>().Selected_DuplicateSelf(this.selectedSlotTransform));*/

                    //   temp = GameManager.gameManagerInstance.AvailableSeletedSlot + 1;
                    //  GameManager.gameManagerInstance.AvailableSeletedSlot = temp;

                    this.gameObject.transform.SetParent(this.handSlotTransform, false);

                    GameManager.gameManagerInstance.RemoveSelectedSlot(this);

                    GameManager.gameManagerInstance.UndoTask();
                }
            }

            else
            {
                if (GameManager.gameManagerInstance.GetAvailableSeletedSlot.Count >= 4)
                {
                    return;
                }

                /* gameObject = Instantiate(skillCardPrefab, this.selectedSlotTransform);
                 gameObject.GetComponentInChildren<Button>().onClick.AddListener(() =>
                 gameObject.GetComponent<SkillCard>().Selected_DuplicateSelf(this.handSlotTransform));*/

                // temp = GameManager.gameManagerInstance.AvailableSeletedSlot - 1;
                // GameManager.gameManagerInstance.AvailableSeletedSlot = temp;

                this.gameObject.transform.SetParent(this.selectedSlotTransform, false);


                GameManager.gameManagerInstance.AddSelectedSlot(this);

                if (this.isNeedSelectRange)  // 범위지정 필요가 있는가.. 다시말해 이동카드인가?
                {
                    CardSelected_SelectRange();
                }

                // if 공격카드라면?
                // 공격카드 task를 큐에 넣어야한다
                if (keyCode_ == SkillDataTable.KeyCode_CardName.attackCard_1 ||
                    keyCode_ == SkillDataTable.KeyCode_CardName.attackCard_2 ||
                    keyCode_ == SkillDataTable.KeyCode_CardName.attackCard_3)
                {


                    QueueTaskSystem.CharacterTask testTask = new QueueTaskSystem.CharacterTask.AttackCardTask(
                        GameManager.gameManagerInstance.GetTargettedTiles(this.attackRange), this.damageData);

                    GameManager.gameManagerInstance.MakeTaskEnqueue(testTask);
                }
            }
            /*if(gameObject != null)
            {
                gameObject.GetComponent<SkillCard>().SetSkillCardData(cardName.text, cost.text, description.text, this.handSlotTransform, this.selectedSlotTransform);
                Destroy(this.gameObject);
            }
            else
            {
                return;
            }*/

        }

        public void SpendSelectedCards(GameManager.GameState gameState)
        {
            if (gameState == GameManager.GameState.Battle)
            {
                if (this.transform.parent == this.selectedSlotTransform)
                {
                    GameManager.gameManagerInstance.RemoveSelectedSlot(this);
                    GameManager.OnGameStateChanged -= SpendSelectedCards;   //잊지말것.. 이벤트에서 빼줘야한다
                    Destroy(this.gameObject);
                }
            }
        }

        private void CardSelected_SelectRange()
        {

            GameManager.gameManagerInstance.UpdateGameState(GameManager.GameState.Select_Range);


        }

    }
}
