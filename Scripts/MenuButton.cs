using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Project.Game
{
    public class MenuButton : MonoBehaviour
    {
        [SerializeField]
        private GameObject cardPanel;

        List<Tween> cardPanelTweens = new List<Tween>();


        void Start()
        {
            cardPanelTweens = cardPanel.GetComponent<DOTweenAnimation>().GetTweens();
            GameManager.OnGameStateChanged += SubscribeGameStateMethod;
        }

        public void OnClickedButton()
        {
            if (cardPanel.transform.localScale.x <= 0)
            {
                cardPanelTweens[0].Play(); //보드가 펴지는 트윈
            }
            else
            {
                cardPanelTweens[1].Play(); //보드가 접히는 트윈

            }
        }

        public void SubscribeGameStateMethod(GameManager.GameState gameState)
        {
            //if(gameState == GameManager.GameState.Select_Range || gameState == GameManager.GameState.Main)
            if (gameState == GameManager.GameState.Select_Range)
            {
                OnClickedButton();

            }
        }
    }
}
