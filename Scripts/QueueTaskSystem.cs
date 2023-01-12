using Cysharp.Threading.Tasks;
//using Project.Grid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
namespace Project.Game
{
    public class QueueTaskSystem
    {
        private List<CharacterTask> taskList;

        public abstract class CharacterTask
        {
            private Vector3 previousPosition;
            private Vector3 moveToPosition;
            private TileData[] attackRange;
            private GridManager gridManager;

            protected void GetStaticGridManager()
            {
                gridManager = GridManager.ReturnStaticInstance();
            }

            public GridManager GridManager
            { get { return gridManager; } }
            public Vector3 MoveToPosition
            { get { return moveToPosition; } set { moveToPosition = value; } }
            public Vector3 PreviousPosition
            { get { return previousPosition; } set { previousPosition = value; } }
            public TileData[] AttackRangedTiles
            { get { return attackRange; } set { attackRange = value; } }

            public class MoveCardTask : CharacterTask
            {
                public MoveCardTask(Vector3 moveToPosition, Vector3 previousPosition)
                {
                    //GetStaticGridManager();
                    MoveToPosition = moveToPosition;
                    PreviousPosition = previousPosition;
                }

            }
            public class AttackCardTask : CharacterTask
            {
                private int damageData;
                public int DamageData
                { get { return damageData; } }
                public AttackCardTask(TileData[] tileDatas, int skillDamage)
                {
                    //GetStaticGridManager();
                    AttackRangedTiles = tileDatas;
                    damageData = skillDamage;
                }
            }
        }

        public QueueTaskSystem()
        {
            taskList = new List<CharacterTask>();

            Debug.Log("Queue task system ready");
        }

        public async void ExecuteNextQueue(Character_BaseClass character_object)
        {
            int loopCount = taskList.Count;

            for (int i = 0; i < loopCount; i++)
            {
                CharacterTask task = taskList[i];

                if (task is CharacterTask.MoveCardTask)
                {
                    ExecuteMoveTask(task, character_object);
                    //  GameManager.gameManagerInstance.UpdateGameState(GameManager.GameState.Main);
                    //await UniTask.WhenAll((ExecuteMoveTask(task, character_object)));
                    //await UniTask.WaitUntilValueChanged(character_object, x => Character_BaseClass.CharacterState.Idle);
                    // await UniTask.WaitUntilValueChanged(character_object.currentState, x => x.currentState);

                    // 상대 카드 1번큐와 내 카드 1번큐 비교
                    // 이동 0순위 방어 1순위 공격 2순위로 양측 스펠스피드 확인
                    // 빠른 순번을 가진 쪽 플레이어의 큐부터 처리 ... 완료 후 2번큐로

                    await UniTask.Delay(TimeSpan.FromSeconds(1.1f), ignoreTimeScale: false);
                }
                else if (task is CharacterTask.AttackCardTask)
                {
                    ExecuteAttackTask((CharacterTask.AttackCardTask)task, character_object);
                    await UniTask.Delay(TimeSpan.FromSeconds(1.1f), ignoreTimeScale: false);
                }
            }

            taskList.Clear();
            GameManager.gameManagerInstance.UpdateGameState(GameManager.GameState.End);

        }

        public void AddTask(CharacterTask task)
        {
            taskList.Add(task);

        }

        public void RemoveTaskQueue()
        {
            taskList.RemoveAt(taskList.Count - 1);
        }

        public CharacterTask GetRecentTask()
        {
            return taskList[taskList.Count - 1];
        }

        public bool IsLastTaskInList()
        {
            if (taskList.Count == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ExecuteMoveTask(CharacterTask task, Character_BaseClass character_object)
        {
            character_object.CurrentState = Character_BaseClass.CharacterState.Action;
            Debug.Log("Now Action");

            character_object.MoveCharacter(task.MoveToPosition, 0.8f);
            character_object.CurrentState = Character_BaseClass.CharacterState.Idle;

            Debug.Log("end Action return to Idle");
        }

        public void ExecuteAttackTask(CharacterTask.AttackCardTask task, Character_BaseClass character_object)
        {
            character_object.CurrentState = Character_BaseClass.CharacterState.Action;
            Debug.Log("Now Action");

            for (int i = 0; i < task.AttackRangedTiles.Length; i++)
            {
                if (task.AttackRangedTiles[i] != null)
                {
                    //각 타일에 데미지 데이터를 전달.. 공격판정 활성화
                    task.AttackRangedTiles[i].SkillDamage = task.DamageData;
                    task.AttackRangedTiles[i].UpdateAttackFlag(true);
                    task.AttackRangedTiles[i].AttackAreaParticleActive();
                }
            }

            // 공격 애니메이션 재생, 타격판정, damage..

            character_object.CurrentState = Character_BaseClass.CharacterState.Idle;

            Debug.Log("end Action return to Idle");
        }


    }
}
