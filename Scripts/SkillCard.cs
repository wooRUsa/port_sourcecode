using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
namespace Project.Game
{
    //�ڵ忡 ������ ī�� ��ü
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
            //�ؽ�Ʈ �� �����տ� �Է�..
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

                if (this.isNeedSelectRange)  // �������� �ʿ䰡 �ִ°�.. �ٽø��� �̵�ī���ΰ�?
                {
                    CardSelected_SelectRange();
                }

                // if ����ī����?
                // ����ī�� task�� ť�� �־���Ѵ�
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
                    GameManager.OnGameStateChanged -= SpendSelectedCards;   //��������.. �̺�Ʈ���� ������Ѵ�
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
