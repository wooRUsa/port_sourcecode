using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Data;
using Photon.Pun;
using Random = UnityEngine.Random;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
//using Project.Grid;
using DG.Tweening;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Linq;
using ExitGames.Client.Photon;
using Cysharp.Threading.Tasks.CompilerServices;
using Photon.Pun.Demo.Asteroids;

namespace Project.Game
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        public static GameManager gameManagerInstance;

        private GameManager()
        {
            if (gameManagerInstance == null)
            {
                gameManagerInstance = this;
            }
            else
            {
                Destroy(this);
            }

        }
        public GameState gameState;


        public enum CharacterNameEnum
        {
            SonyCast,
            ZzamTiger,
            Kimdoe
        }
        public enum GameState
        {
            Draw,
            Stand_By,
            Main,
            Select_Range,
            Battle,
            End
        }

        public static event Action<GameState> OnGameStateChanged;
       // public static event Action<int> OnPlayerHealthChanged;

        private int moveCard = 10;
        private int attackCard_1 = 10;
        private int attackCard_2 = 0;
        private int attackCard_3 = 0;
        private int guardCard = 0;
        private int restoreCard = 0;

        private QueueTaskSystem taskSystem;

        private GameObject myCharacterObject;
        private GameObject enemyCharacterObject;
        private CharacterNameEnum enemyCharacterEnum;
        private GameObject DummyCharacterInstance;

        [SerializeField]
        private Transform handSlotTransform;
        [SerializeField]
        private Transform selectedSlotTransform;
        [SerializeField]
        private GameObject skillCardPrefab;
        [SerializeField]
        private bool[] availableHandSlots;
        [SerializeField]
        private List<SkillCard> availableSeletedSlots;
        [SerializeField]
        GameObject[] DummyPrefabs;

        [SerializeField]
        Image LeftHealthBar;
        [SerializeField]
        Image LeftSkillPointBar;
        [SerializeField]
        Image RightHealthBar;
        [SerializeField]
        Image RightSkillPointBar;

        public Dictionary<int, GameObject> playerListEntries = new Dictionary<int, GameObject>();

        public List<SkillCard> GetAvailableSeletedSlot
        { get { return availableSeletedSlots; } }
        public GameObject EnemyCharacter
        { set { enemyCharacterObject = value; } get { return enemyCharacterObject; } }
        public CharacterNameEnum EnemyNameEnum
        { set { enemyCharacterEnum = value; } get { return enemyCharacterEnum; } }

        public void AddSelectedSlot(SkillCard skillCard)
        {
            availableSeletedSlots.Add(skillCard);
        }
        public void RemoveSelectedSlot(SkillCard skillCard)
        {
            availableSeletedSlots.Remove(skillCard);
        }

        // private Dictionary<int, SkillDataTable.KeyCode_CardName> myDeck = new Dictionary<int, SkillDataTable.KeyCode_CardName>();
        private List<SkillDataTable.KeyCode_CardName> myDeck = new List<SkillDataTable.KeyCode_CardName>();

        SkillCardFactory cardFactory;

        private void Awake()
        {
           /* playerListEntries = new Dictionary<int, GameObject>();

            foreach (Player p in PhotonNetwork.PlayerList)
            {
                GameObject entry = 
               

                playerListEntries.Add(p.ActorNumber, entry);
            }*/

            SkillDataTable.SetSkillDataTable();

        }
        // Start is called before the first frame update
        void Start()
        {

            BuildDeckData();
            ShuffleDeck(myDeck);

            InitializeSkillCardFactory();

            // DrawCard();
            /*foreach (Player p in PhotonNetwork.PlayerList)
            {
                GameObject entry = Instantiate(PlayerOverviewEntryPrefab);
                

                playerListEntries.Add(p.ActorNumber, entry);
            }*/

            //UpdateGameState(GameState.Draw);
            LateStart();
            taskSystem = new QueueTaskSystem();

        }

        private async void LateStart()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), ignoreTimeScale: false);
            UpdateGameState(GameState.Draw);
        }

        public GameObject MyCharacterObject
        {
            get { return myCharacterObject; }
            set { myCharacterObject = value; }
        }

        public void SetCharacterStatusBars()
        {
            Vector2Int leftSpawnPoint = new Vector2Int(1, 1);
            //Vector2Int rightSpawnPoint = new Vector2Int(6, 1);
            
            if (myCharacterObject == null || enemyCharacterObject == null)
            {
                Debug.LogWarning("missing character objec");
                return;
            }
            else
            {
                if (GridManager.gridManagerInstance.GetCellPositionFromVec3(myCharacterObject.transform.position) == leftSpawnPoint)
                {
                    myCharacterObject.GetComponent<Character_BaseClass>().MyHealthBar = LeftHealthBar;
                    enemyCharacterObject.GetComponent<Character_BaseClass>().MyHealthBar = RightHealthBar;
                    SetupStatusLeft(MyCharacterObject);
                    SetupStatusRight(enemyCharacterObject);

                }
                else
                {
                    myCharacterObject.GetComponent<Character_BaseClass>().MyHealthBar = RightHealthBar;
                    enemyCharacterObject.GetComponent<Character_BaseClass>().MyHealthBar = LeftHealthBar;
                    SetupStatusLeft(enemyCharacterObject);
                    SetupStatusRight(MyCharacterObject);

                }
            }
        }

        

        private void SetupStatusLeft(GameObject characterObject)
        {
            if (characterObject == null)
            {
                Debug.LogError("missing character object");
                return;
            }

            if (LeftHealthBar.fillAmount <= 0)
            {
                LeftHealthBar.DOFillAmount(characterObject.GetComponent<Character_BaseClass>().HealthPoint / 100.0f, 1.1f);
                LeftSkillPointBar.DOFillAmount(characterObject.GetComponent<Character_BaseClass>().SkillPoint / 100.0f, 1.1f);
                /*LeftHealthBar.DOFillAmount((float)PhotonNetwork.CurrentRoom.GetPlayer(characterObject.GetPhotonView().ControllerActorNr)
                    .CustomProperties["Player_HP"] / 100.0f, 1.1f);
                LeftSkillPointBar.DOFillAmount((float)PhotonNetwork.CurrentRoom.GetPlayer(characterObject.GetPhotonView().ControllerActorNr)
                    .CustomProperties["Player_SP"] / 100.0f, 1.1f);*/
            }

        }

        private void SetupStatusRight(GameObject characterObject)
        {
            if (characterObject == null)
            {
                Debug.LogError("missing character object");
                return;
            }

            if (LeftHealthBar.fillAmount <= 0)
            {
                RightHealthBar.DOFillAmount(characterObject.GetComponent<Character_BaseClass>().HealthPoint / 100.0f, 1.1f);
                RightSkillPointBar.DOFillAmount(characterObject.GetComponent<Character_BaseClass>().SkillPoint / 100.0f, 1.1f);
                /*RightHealthBar.DOFillAmount((float)PhotonNetwork.CurrentRoom.GetPlayer(characterObject.GetPhotonView().OwnerActorNr)
                    .CustomProperties["Player_HP"] / 100.0f, 1.1f);
                RightSkillPointBar.DOFillAmount((float)PhotonNetwork.CurrentRoom.GetPlayer(characterObject.GetPhotonView().ControllerActorNr)
                    .CustomProperties["Player_SP"] / 100.0f, 1.1f);*/
            }
        }
        

        public void UpdateGameState(GameState newState)
        {
            this.gameState = newState;

            switch (newState)
            {
                case GameState.Draw:
                    Debug.Log(" == Draw Phase == ");
                    HandleDrawPhase();
                    break;
                case GameState.Stand_By:
                    Debug.Log(" == Stand By Phase == ");
                    HandleStandByPhase();
                    break;
                case GameState.Main:
                    Debug.Log(" == Main Phase == ");
                    break;
                case GameState.Select_Range:
                    Debug.Log(" == Select Range Phase == ");
                    break;
                case GameState.Battle:
                    HandleBattlePhase();
                    Debug.Log(" == Battle Phase == ");
                    break;
                case GameState.End:
                    Debug.Log(" == End Phase == ");
                    break;
                default:
                    break;
            }

            OnGameStateChanged?.Invoke(newState);
        }
        private void InitializeSkillCardFactory()
        {
            cardFactory = GetFactoryType((int)PhotonNetwork.LocalPlayer.CustomProperties["playerAvatar"]);
            //SkillCardFactory factory = GetFactoryType((int)PhotonNetwork.LocalPlayer.CustomProperties["playerAvatar"]);
            //factory.makeSkillCard("aa", this.transform);

        }

        private void HandleDrawPhase()
        {
            DrawCard();
        }

        private void HandleStandByPhase()
        {

            SetCharacterStatusBars();


            UpdateGameState(GameState.Main);
        }

        private void HandleBattlePhase()
        {
            if (DummyCharacterInstance != null)
            {
                Destroy(DummyCharacterInstance);
            }
        }

        private void HandleSelectRangePhase()
        {
            //todo 번위선택시에만 마우스 위치에 반투명한 커서 표시하도록 연출
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            GameObject entry;
            if (playerListEntries.TryGetValue(targetPlayer.ActorNumber, out entry))
            {
                //entry.GetComponent<Text>().text = string.Format("{0}\nScore: {1}\nLives: {2}", targetPlayer.NickName, targetPlayer.GetScore(), targetPlayer.CustomProperties[AsteroidsGame.PLAYER_LIVES]);
                
            }
        }

        public SkillCardFactory GetFactoryType(int characterName)
        {
            SkillCardFactory factory = null;
            switch (characterName)
            {
                case (int)CharacterNameEnum.SonyCast:

                    factory = new SonycastSkillFactory();
                    break;
                case (int)CharacterNameEnum.ZzamTiger:

                    break;
                case (int)CharacterNameEnum.Kimdoe:

                    break;

            }
            return factory;
        }

        private void BuildDeckData()
        {
            myDeck.Clear();

            for (int i = 0; i < moveCard; i++)
            {
                myDeck.Add(SkillDataTable.KeyCode_CardName.moveCard);

            }
            for (int i = 0; i < attackCard_1; i++)
            {
                myDeck.Add(SkillDataTable.KeyCode_CardName.attackCard_1);

            }
            for (int i = 0; i < attackCard_2; i++)
            {
                myDeck.Add(SkillDataTable.KeyCode_CardName.attackCard_2);

            }
            for (int i = 0; i < attackCard_3; i++)
            {
                myDeck.Add(SkillDataTable.KeyCode_CardName.attackCard_3);

            }
            for (int i = 0; i < guardCard; i++)
            {
                myDeck.Add(SkillDataTable.KeyCode_CardName.guardCard);

            }
            for (int i = 0; i < restoreCard; i++)
            {
                myDeck.Add(SkillDataTable.KeyCode_CardName.restoreCard);

            }
        }

        private void DrawCard(bool isExtraOneDraw = false)
        {
            if (myDeck.Count > 0)
            {

                //todo 신규 드로우시 애니메이션 연출

                for (int i = 0; i < availableHandSlots.Length; i++)
                {
                    SkillDataTable.KeyCode_CardName cardName = myDeck[0];
                    if (availableHandSlots[i] == true)
                    {
                        DataRow cardDataRow;
                        cardDataRow = cardFactory.makeSkillCardData(cardName);

                        InstantiateCardFunction(cardDataRow);

                        availableHandSlots[i] = false;
                        myDeck.RemoveAt(0);
                        if (isExtraOneDraw)
                        {
                            return;
                        }
                        if (myDeck.Count == 0)
                        {
                            return;
                        }
                    }
                }
            }
            UpdateGameState(GameState.Stand_By);
        }
        private void InstantiateCardFunction(DataRow cardData)
        {
            GameObject gameObject;
            gameObject = Instantiate(skillCardPrefab, handSlotTransform);

            gameObject.GetComponent<SkillCard>().SetSkillCardData(handSlotTransform, selectedSlotTransform,
                (SkillDataTable.KeyCode_CardName)cardData.ItemArray[0], cardData.ItemArray[1].ToString(),
                cardData.ItemArray[2].ToString(), cardData.ItemArray[3].ToString(), cardData.ItemArray[4].ToString(),
                (bool)cardData.ItemArray[5], (Vector2Int[])cardData.ItemArray[6]);

            gameObject.GetComponentInChildren<Button>().onClick.AddListener(() =>
                gameObject.GetComponent<SkillCard>().Selected_DuplicateSelf());
        }

        public List<T> ShuffleDeck<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int rand = Random.Range(0, i);

                T temp = list[i];
                list[i] = list[rand];
                list[rand] = temp;
            }

            foreach (var item in list)
            {
                // Debug.Log(item);
            }

            return list;
        }

        // Update is called once per frame
        void Update()
        {
            //todo 임시조치 조건분기는 나중에 정밀조정함
            //todo 게임메니저에 페이즈설정 다하면 스킬카드 범위설정 페이즈에만 실핼하도록 할것
            if (Input.GetMouseButtonDown(0))
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    Vector2Int currentCellPos;
                    Vector2Int clickedCellPos;
                    Vector3 currentPosTemp;
                    if (DummyCharacterInstance != null)
                    {
                        currentCellPos = GridManager.gridManagerInstance.GetCellPositionFromVec3(DummyCharacterInstance.transform.position);
                        currentPosTemp = DummyCharacterInstance.transform.position;
                    }
                    else
                    {
                        currentCellPos = GridManager.gridManagerInstance.GetCellPositionFromVec3(myCharacterObject.transform.position);
                        currentPosTemp = myCharacterObject.transform.position;
                    }


                    if (GridManager.gridManagerInstance.clickedTileData != null)
                    {
                        clickedCellPos = GridManager.gridManagerInstance.clickedTileData.OwnCellPosition;
                    }
                    else
                    {
                        Debug.Log(" Clicked TileData missing e ");
                        return;
                    }

                    if (GridManager.gridManagerInstance.CheckAvailableMoveRange(currentCellPos, clickedCellPos))
                    {
                        if (myCharacterObject.GetComponent<Character_BaseClass>().CurrentState == Character_BaseClass.CharacterState.Idle)
                        {
                            QueueTaskSystem.CharacterTask testTask =
                            new QueueTaskSystem.CharacterTask.MoveCardTask(GridManager.gridManagerInstance.clickedTileData.transform.position, currentPosTemp);
                            taskSystem.AddTask(testTask);
                            // Debug.Log(currentPosTemp);
                            UpdateGameState(GameState.Main);

                            Vector3 temp = new Vector3(GridManager.gridManagerInstance.clickedTileData.transform.position.x,
                                GridManager.gridManagerInstance.clickedTileData.transform.position.y, 0);

                            if (DummyCharacterInstance != null)
                            {
                                Destroy(DummyCharacterInstance);
                            }

                            DummyCharacterInstance = Instantiate(DummyPrefabs[(int)PhotonNetwork.LocalPlayer.CustomProperties["playerAvatar"]],
                           temp, Quaternion.identity);



                            GridManager.gridManagerInstance.SetClickedTileData(); // ClickedTileData초기화

                        }
                    }

                }

            }
        }

        public void UndoTask()
        {
            QueueTaskSystem.CharacterTask task = taskSystem.GetRecentTask();
            if (task == null)
            {
                return;
            }
            if (task is QueueTaskSystem.CharacterTask.MoveCardTask)
            {
                if (DummyCharacterInstance != null)
                {
                    Destroy(DummyCharacterInstance);
                    if (taskSystem.IsLastTaskInList())
                    {
                        taskSystem.RemoveTaskQueue();
                        return;
                    }
                    DummyCharacterInstance = Instantiate(DummyPrefabs[(int)PhotonNetwork.LocalPlayer.CustomProperties["playerAvatar"]],
                    task.PreviousPosition, Quaternion.identity);

                    taskSystem.RemoveTaskQueue();
                }
            }
            else if (task is QueueTaskSystem.CharacterTask.AttackCardTask)
            {
                taskSystem.RemoveTaskQueue();
            }
        }

        public void testFuncQ()
        {
            // PhotonNetwork.LocalPlayer.GetPlayerNumber();
            // PhotonNetwork.LocalPlayer.CustomProperties["isReady"] = true;

            OnClickIamReady();


        }

        public void OnClickIamReady()
        {

            var hash = PhotonNetwork.LocalPlayer.CustomProperties;
            hash["Ready"] = true;
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            //IamReadyButton.gameObject.SetActive(false);

            //  if (!PhotonNetwork.IsMasterClient) return; //이거 지울까?

            // 1초정도 비동기 대기 후에 실행?

            //CheckAllPlayersReady();
            if (myCharacterObject.GetComponent<PhotonView>().IsMine)
            {
                myCharacterObject.GetComponent<PhotonView>().RPC("CheckAllPlayersReady", RpcTarget.All);
            }

        }

        public void RequestStartQueue()
        {
            taskSystem.ExecuteNextQueue(myCharacterObject.GetComponent<Character_BaseClass>());

            UpdateGameState(GameState.Battle);
        }

        public void MakeTaskEnqueue(QueueTaskSystem.CharacterTask task)
        {
            taskSystem.AddTask(task);
        }
        public TileData[] GetTargettedTiles(Vector2Int[] attackRange)
        {
            TileData[] tileDatas = new TileData[attackRange.Length];

            // 더미 캐릭터가 존재하는 상태(이동카드로 이동을 먼저 선택한 경우)
            if (DummyCharacterInstance != null)
            {
                Vector2Int currentCellPos = GridManager.gridManagerInstance.GetCellPositionFromVec3(DummyCharacterInstance.transform.position);


                for (int i = 0; i < attackRange.Length; i++)
                {
                    Vector2Int calculatedRange = new Vector2Int(attackRange[i].x + currentCellPos.x, attackRange[i].y + currentCellPos.y);
                    tileDatas[i] = GridManager.gridManagerInstance.GetTileDataFromCellpos(calculatedRange);
                }

                // TileData a = GridManager.gridManagerInstance.GetTileDataFromCellpos(currentCellPos);

            }
            else
            {
                Vector2Int currentCellPos = GridManager.gridManagerInstance.GetCellPositionFromVec3(myCharacterObject.transform.position);


                for (int i = 0; i < attackRange.Length; i++)
                {
                    Vector2Int calculatedRange = new Vector2Int(attackRange[i].x + currentCellPos.x, attackRange[i].y + currentCellPos.y);
                    tileDatas[i] = GridManager.gridManagerInstance.GetTileDataFromCellpos(calculatedRange);
                }
            }

            foreach (var tile in tileDatas)
            {
                Debug.Log(tile);
            }

            return tileDatas;
        }


    }

    //todo
    // 캐릭터 인스턴스를 포톤인스턴스로 바꾸고 그걸 취득해오도록 바꿔야함
    // 조건분기 정리하기
    // addTask 호출하는 시기조정.. 카드사용시로
}
