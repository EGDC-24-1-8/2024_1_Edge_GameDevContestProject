using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public const int NO_MONEY_ELIMINATED = 0;
    public const int DETECTED_CHEAT_ELIMINATED = 1;
    public static GameManager Instance = null;//싱글톤 패턴

    public enum GameState
    {
        start,
        deal,
        afterDeal,
        bet,
        afterBet,
        end
    };
    public enum MousePointState
    {
        normal,
        code,
        detect
    };
    [Header("Game State")]
    [SerializeField] public GameState gameState;
    [SerializeField] public bool isPause = false;
    [SerializeField] public MousePointState mousePointState = MousePointState.normal;
    //[SerializeField] public InputField inputDay;
    //Card UI
    [Header("Card UI")]
    [SerializeField] private Text[] playerCard0Text;
    [SerializeField] private Text[] playerCard1Text;
    [SerializeField] private Text[] playerCard2Text;
    [SerializeField] private Sprite[] cardFace;
    [SerializeField] private GameObject[] playerCard0Face;
    [SerializeField] private GameObject[] playerCard1Face;
    [SerializeField] private GameObject[] playerCard2Face;
    [SerializeField] private GameObject[] playerCardFacePosition;
    [SerializeField] private Text[] playerSumText;
    [SerializeField] private Text TopCardText;
    [SerializeField] private Text BottomCardText;
    [SerializeField] private Text GameDayText;
    [SerializeField] private Text GameTurnText;
    [SerializeField] private Slider timeBar;

    //Card Value
    [Header("Card Value")]
    //public string[] playerName;
    //[SerializeField] private int[] playerCardNum;
    [SerializeField] private int[] playerCard0;
    [SerializeField] private int[] playerCard1;
    [SerializeField] private int[] playerCard2;
    [SerializeField] private int[] playerCard0Num;
    [SerializeField] private int[] playerCard1Num;
    [SerializeField] private int[] playerCard2Num;
    [SerializeField] public int[] playerCardSum;
    [SerializeField] public int codeType = 0; //신호 타입 0 : fold , 1 : raise   


    [SerializeField] public bool[] playerIsCheat = new bool[4] { false, false, false, false };
    [SerializeField] public bool[] playerIsDetectable = new bool[4] { false, false, false, false };
    [SerializeField] public List<int> CardDeck = null;

    [SerializeField] public GameObject[] playerCard0Obj;
    [SerializeField] public GameObject[] playerCard1Obj;
    [SerializeField] public GameObject[] playerCard2Obj;

    [Header("In Game Counts")]
    [SerializeField] public int allyPlayerPosition;
    [SerializeField] public int foldPlayerCnt = 0;
    [SerializeField] public int IngamePlayerCnt = 4;

    [Header("Turn Info")]
    [SerializeField] private int gameDay = 1;
    [SerializeField] private int gameTurn = 0;
    //[SerializeField] private int dealtCardCount = 0; // 카드 나눠주는 턴
    [SerializeField] private int dealOrder = 0; // 현재 카드 나눠줄 플레이어 순서
    [SerializeField] private float dealTimeCur = 0;
    [SerializeField] private float dealTimeMax = 3.5f; // 카드 제한 시간

    [Header("etc")]
    [SerializeField] private AudioClip BGM;
    [SerializeField] private AudioClip NormalDealingSound;
    [SerializeField] private AudioClip SecondDealingSound;
    [SerializeField] private AudioClip BottomDealingSound;
    [SerializeField] private AudioClip DetectSound;
    [SerializeField] private Slider suspicionBar;
    [SerializeField] private float suspicionLevelMax = 100f;
    [SerializeField] private float suspicionLevelCur = 0f;
    [SerializeField] private GameObject[] Player;
    [SerializeField] private List<PlayerData> enemyPlayerDataSet = null;
    [SerializeField] private List<PlayerData> allyPlayerDataSet = null;
    [SerializeField] private List<PlayerData> tempEnemyPlayerDataSet = null;
    [SerializeField] private List<PlayerData> tempAllyPlayerDataSet = null;
    [SerializeField] public Player[] playerArray = null;

    [SerializeField] public BettingManager betMan;
    [SerializeField] private IEnumerator[] cheatCoroutine = new IEnumerator[4];
    private bool isFade = false;
    //[SerializeField] private AudioClip hoverSound = null;
    [SerializeField] private Texture2D normal_cursor;
    [SerializeField] private Texture2D code_cursor;
    [SerializeField] private Texture2D code_hover_cursor;
    [SerializeField] private Texture2D detect_cursor;
    [SerializeField] private Texture2D detect_hover_cursor;



    public void ChangeHoverCursor()
    {
        if (mousePointState == MousePointState.code)
        {
            Cursor.SetCursor(code_hover_cursor, new Vector2(0, 0), CursorMode.Auto);
        }
        else if (mousePointState == MousePointState.detect)
        {
            Cursor.SetCursor(detect_hover_cursor, new Vector2(32, 32), CursorMode.Auto);
        }
    }

    public void ChangeNormalCursor()
    {
        if (mousePointState == MousePointState.code)
        {
            Cursor.SetCursor(code_cursor, new Vector2(0, 0), CursorMode.Auto);
        }
        else if (mousePointState == MousePointState.detect)
        {
            Cursor.SetCursor(detect_cursor, new Vector2(32, 32), CursorMode.Auto);
        }
    }

    private void Awake()
    {

        if (null == Instance) //디자인패턴중 싱글톤 패턴
        {
            Instance = this;
            //DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
        allyPlayerPosition = UnityEngine.Random.Range(0, playerArray.Length);
        InitPlayer();
        InitCardFace();
        CardDeck = InitDeck();
        AudioManager.GetOrCreate().SetBGMVolume(0.1f);
        AudioManager.GetOrCreate().PlayBGM(BGM);
        if(PlayerPrefs.HasKey("Day"))
            gameDay = PlayerPrefs.GetInt("Day");
        else
            gameDay = 1;
    }

    void Start()
    {
        IngamePlayerCnt = 4;
        SetStateStart();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            //AudioManager.GetOrCreate().PlayEffectSound(hoverSound);
            mousePointState = MousePointState.normal;
            Cursor.SetCursor(normal_cursor, new Vector2(0, 0), CursorMode.Auto);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            mousePointState = MousePointState.code;
            Cursor.SetCursor(code_cursor, new Vector2(0, 0), CursorMode.Auto);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            Cursor.SetCursor(detect_cursor, new Vector2(0, 0), CursorMode.Auto);
            mousePointState = MousePointState.detect;
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPause)
            {
                isPause = false;
                Time.timeScale = 1;
            }
            else
            {
                isPause = true;
                Time.timeScale = 0;
            }
        }

        if (gameState == GameState.deal)
        {
            if (DialogManager.Instance.isDialogMiddlePriority ||
               DialogManager.Instance.isDialogHighPriority)
            {
                dealTimeCur = Time.time;
            }
            else
            {
                TimeBarUpdate();
            }
        }
    }

    #region set state

    public void SetStateStart()
    {
        /*if (gameTurn > 2 || IngamePlayerCnt < 2)
        {
            gameDay++;
            PlayerPrefs.SetInt("Day", gameDay);
            betMan.ResetPlayer(); //여기부터
            InitPlayer();
            InitCardFace();
            gameTurn = 0;
            IngamePlayerCnt = 4; //여기까지 싹다 날리고 컷신으로 넘길 거임
        }*/
        GameDayText.text = "Day " + gameDay;
        GameTurnText.text = "Turn " + (gameTurn + 1);
        gameState = GameState.start;
        CardDeck = InitDeck();
        CardDeck = shuffleDeck(CardDeck);
        DealingManager.Instance.DestroyChild();
        DealingManager.Instance.SetImage();
        DealingManager.Instance.TopNewCardCreate(true);
        DealingManager.Instance.SecondNewCardCreate(true);
        DealingManager.Instance.BottomNewCardCreate(true);
        TopCardText.text = (CardDeck[0] % 13 + 1).ToString();
        BottomCardText.text = (CardDeck[CardDeck.Count - 1] % 13 + 1).ToString();

        mousePointState = MousePointState.normal;
        Cursor.SetCursor(normal_cursor, new Vector2(0, 0), CursorMode.Auto);
        for (int i = 0; i < playerArray.Length; i++) //입장 베팅
        {
            playerCard0Face[i].GetComponent<SpriteRenderer>().sprite = null;
            playerCard1Face[i].GetComponent<SpriteRenderer>().sprite = null;
            playerCard2Face[i].GetComponent<SpriteRenderer>().sprite = null;
            playerCardFacePosition[i].SetActive(false);
            playerIsCheat[i] = false;
            playerIsDetectable[i] = false;
            playerArray[i].dealtCardCount = 0;
            if (cheatCoroutine[i] != null)
            {
                StopCoroutine(cheatCoroutine[i]);
            }
        }
        betMan.UpdateUIText();
        DialogManager.Instance.TriggerNextSentence_MiddlePriority(allyPlayerPosition, DialogManager.TextType.start);
        //SetStateDeal();
    }

    public void SetStateDeal()
    {
        gameState = GameState.deal;
        dealOrder = 0;
        dealTimeCur = Time.time;
        timeBar.gameObject.SetActive(true);
        SetTimeBarPosition();
        BottomCardText.text = (CardDeck[CardDeck.Count - 1] % 13 + 1).ToString();
    }

    public void SetStateAfterDeal()
    {
        timeBar.gameObject.SetActive(false);
        gameState = GameState.afterDeal;
        SetStateBet();
    }

    public void SetStateBet()
    {
        gameState = GameState.bet;
        betMan.TriggerBetting();
    }

    public void SetStateAfterBet()
    {
        gameState = GameState.afterBet;
        IsBetOver();
    }

    public void SetStateEnd()
    {
        gameState = GameState.end;
        for (int i = 0; i < playerArray.Length; i++)
        {
            if (!betMan.isFold[i] && playerIsCheat[i] && playerCardSum[i] != 21)
            {
                SwitchCard(i);
            }
        }
        TriggerCardOpen();
    }

    #endregion


    #region start
    private List<int> InitDeck()
    {
        List<int> initDeck = new List<int>();

        for (int i = 0; i < 52; i++) //카드 생성
        {
            initDeck.Add(i);
        }
        return initDeck;
    }

    private void InitPlayer()
    {
        tempAllyPlayerDataSet.Clear();
        tempEnemyPlayerDataSet.Clear();
        tempAllyPlayerDataSet.AddRange(allyPlayerDataSet);
        tempEnemyPlayerDataSet.AddRange(enemyPlayerDataSet);
        for (int i = 0; i < 4; i++)
        {
            if (i == allyPlayerPosition)
            {
                playerArray[i].isAlly = true;
                int temp = UnityEngine.Random.Range(0, tempAllyPlayerDataSet.Count);
                playerArray[i].playerData = tempAllyPlayerDataSet[temp];
                playerArray[i].initData();
                tempAllyPlayerDataSet.RemoveAt(temp);
            }
            else
            {
                int temp = UnityEngine.Random.Range(0, tempEnemyPlayerDataSet.Count);
                playerArray[i].playerData = tempEnemyPlayerDataSet[temp];
                playerArray[i].initData();
                tempEnemyPlayerDataSet.RemoveAt(temp);
            }
        }
    }

    private void InitCardFace()
    {
        for (int i = 0; i < playerArray.Length; i++)
        {
            GameObject newPlayer = Instantiate(playerArray[i].playerSprite, playerArray[i].transform);
            playerCard0Obj[i] = newPlayer.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject;
            playerCard1Obj[i] = newPlayer.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).gameObject;
            playerCard2Obj[i] = newPlayer.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(2).gameObject;
        }
    }

    private List<int> shuffleDeck(List<int> deck)
    {
        int random1, random2;
        int temp;
        for (int i = 0; i < deck.Count; ++i)
        {
            random1 = UnityEngine.Random.Range(0, deck.Count);
            random2 = UnityEngine.Random.Range(0, deck.Count);

            temp = deck[random1];
            deck[random1] = deck[random2];
            deck[random2] = temp;
        }
        return deck;
    }

    #endregion


    #region deal

    private void SetTimeBarPosition()
    {
        CheckDealOrder();
        if (gameState != GameState.deal)
            return;
        timeBar.transform.position = playerCard2Text[dealOrder].GetComponent<RectTransform>().position;
    }

    private void TimeBarUpdate()
    {
        if (Time.time - dealTimeCur >= dealTimeMax)
        {
            IncreaseSuspicionByDealTime();
            DialogManager.Instance.TriggerNextSentence_LowPriority(dealOrder, DialogManager.TextType.time);
            dealTimeCur = Time.time;
        }
        timeBar.value = (Time.time - dealTimeCur) / dealTimeMax;
    }

    public void CheckDealOrder()
    {
        if (gameState != GameState.deal)
        {
            return;
        }
        while (true)
        {
            //if (betMan.isFold.Length > dealOrder)
            if (betMan.isFold[dealOrder] || betMan.isEliminated[dealOrder])
            {
                ++dealOrder;
                IsDealOver();
            }
            else
            {
                return;
            }
        }
    }

    public void IsDealOver()
    {
        if (dealOrder >= betMan.isFold.Length)
        {
            SetStateAfterDeal();
            return;
        }
        if (betMan.isFold[dealOrder] == true)
        {
            ++dealOrder;
            IsDealOver();
        }
        SetTimeBarPosition();
    }

    public void GetPlayerCard(int playerIdx, int dealtCardCnt)
    {
        switch (dealtCardCnt)
        {
            case 0:
                playerCard0Obj[playerIdx].SetActive(true);
                break;
            case 1:
                playerCard1Obj[playerIdx].SetActive(true);
                break;
            case 2:
                playerCard2Obj[playerIdx].SetActive(true);
                break;
            default:
                break;
        }
    }

    public void NormalDeal()
    {
        CheckDealOrder();

        if (gameState != GameState.deal)
        {
            return;
        }
        if (DialogManager.Instance.isDialogMiddlePriority)
        {
            return;
        }
        playerArray[dealOrder].dealtCardCount++;
        switch (betMan.dealtCardCount)
        {
            case 0:
                playerCard0[dealOrder] = CardDeck[0];
                playerCard0Num[dealOrder] = (CardDeck[0] % 13 + 1 > 10) ? 10 : CardDeck[0] % 13 + 1;
                playerCardSum[dealOrder] += playerCard0Num[dealOrder];
                playerCard0Text[dealOrder].text = playerCard0Num[dealOrder].ToString();
                break;
            case 1:
                playerCard1[dealOrder] = CardDeck[0];
                playerCard1Num[dealOrder] = (CardDeck[0] % 13 + 1 > 10) ? 10 : CardDeck[0] % 13 + 1;
                playerCardSum[dealOrder] += playerCard1Num[dealOrder];
                playerCard1Text[dealOrder].text = playerCard1Num[dealOrder].ToString();
                betMan.SetIsPlayerCheat(dealOrder);
                break;
            case 2:
                playerCard2[dealOrder] = CardDeck[0];
                playerCard2Num[dealOrder] = (CardDeck[0] % 13 + 1 > 10) ? 10 : CardDeck[0] % 13 + 1;
                playerCardSum[dealOrder] += playerCard2Num[dealOrder];
                playerCard2Text[dealOrder].text = playerCard2Num[dealOrder].ToString();

                cheatCoroutine[dealOrder] = CheatCycle(dealOrder);
                StartCoroutine(cheatCoroutine[dealOrder]);

                break;
            default:
                break;
        }
        AudioManager.GetOrCreate().SetEffectVolume(1);
        AudioManager.GetOrCreate().PlayEffectSound(NormalDealingSound);
        CardDeck.Remove(CardDeck[0]);
        dealTimeCur = Time.time;
        TopCardText.text = (CardDeck[0] % 13 + 1).ToString();
        if (50 >= UnityEngine.Random.Range(0, 101)) //50% 확률로 대사 재생
            DialogManager.Instance.TriggerNextSentence_LowPriority(dealOrder, DialogManager.TextType.recieveCard);
        DealingManager.Instance.InstantNewCardCreate(dealOrder);
        dealOrder++;
        IsDealOver();
        //카드 애니메이션을 실행해야한다.. 그 애니메이션에 이벤트를 달아놓고
        // - >>>>>GetPlayerCard();
    }

    public void SecondDeal()
    {
        CheckDealOrder();

        if (gameState != GameState.deal)
        {
            return;
        }
        if (DialogManager.Instance.isDialogMiddlePriority)
        {
            return;
        }
        playerArray[dealOrder].dealtCardCount++;
        switch (betMan.dealtCardCount)
        {
            case 0:
                playerCard0[dealOrder] = CardDeck[1];
                playerCard0Num[dealOrder] = (CardDeck[1] % 13 + 1 > 10) ? 10 : CardDeck[1] % 13 + 1;
                playerCardSum[dealOrder] += playerCard0Num[dealOrder];
                playerCard0Text[dealOrder].text = playerCard0Num[dealOrder].ToString();
                break;
            case 1:
                playerCard1[dealOrder] = CardDeck[1];
                playerCard1Num[dealOrder] = (CardDeck[1] % 13 + 1 > 10) ? 10 : CardDeck[1] % 13 + 1;
                playerCardSum[dealOrder] += playerCard1Num[dealOrder];
                playerCard1Text[dealOrder].text = playerCard1Num[dealOrder].ToString();
                betMan.SetIsPlayerCheat(dealOrder);
                break;
            case 2:
                playerCard2[dealOrder] = CardDeck[1];
                playerCard2Num[dealOrder] = (CardDeck[1] % 13 + 1 > 10) ? 10 : CardDeck[1] % 13 + 1;
                playerCardSum[dealOrder] += playerCard2Num[dealOrder];
                playerCard2Text[dealOrder].text = playerCard2Num[dealOrder].ToString();
                cheatCoroutine[dealOrder] = CheatCycle(dealOrder);
                StartCoroutine(cheatCoroutine[dealOrder]);
                break;
        }
        AudioManager.GetOrCreate().SetEffectVolume(1);
        AudioManager.GetOrCreate().PlayEffectSound(SecondDealingSound);
        CardDeck.RemoveAt(1);
        dealTimeCur = Time.time;
        if (50 >= UnityEngine.Random.Range(0, 101)) //50% 확률로 대사 재생
            DialogManager.Instance.TriggerNextSentence_LowPriority(dealOrder, DialogManager.TextType.recieveCard);
        DealingManager.Instance.InstantNewCardCreate(dealOrder);
        dealOrder++;
        IsDealOver();
    }
    public void BottomDeal()
    {
        CheckDealOrder();

        if (gameState != GameState.deal)
        {
            return;
        }
        if (DialogManager.Instance.isDialogMiddlePriority)
        {
            return;
        }
        playerArray[dealOrder].dealtCardCount++;
        switch (betMan.dealtCardCount)
        {
            case 0:
                playerCard0[dealOrder] = CardDeck[CardDeck.Count - 1];
                playerCard0Num[dealOrder] = (CardDeck[CardDeck.Count - 1] % 13 + 1 > 10) ? 10 : CardDeck[CardDeck.Count - 1] % 13 + 1;
                playerCardSum[dealOrder] += playerCard0Num[dealOrder];
                playerCard0Text[dealOrder].text = playerCard0Num[dealOrder].ToString();
                break;
            case 1:
                playerCard1[dealOrder] = CardDeck[CardDeck.Count - 1];
                playerCard1Num[dealOrder] = (CardDeck[CardDeck.Count - 1] % 13 + 1 > 10) ? 10 : CardDeck[CardDeck.Count - 1] % 13 + 1;
                playerCardSum[dealOrder] += playerCard1Num[dealOrder];
                playerCard1Text[dealOrder].text = playerCard1Num[dealOrder].ToString();
                betMan.SetIsPlayerCheat(dealOrder);
                break;
            case 2:
                playerCard2[dealOrder] = CardDeck[CardDeck.Count - 1];
                playerCard2Num[dealOrder] = (CardDeck[CardDeck.Count - 1] % 13 + 1 > 10) ? 10 : CardDeck[CardDeck.Count - 1] % 13 + 1;
                playerCardSum[dealOrder] += playerCard2Num[dealOrder];
                playerCard2Text[dealOrder].text = playerCard2Num[dealOrder].ToString();
                cheatCoroutine[dealOrder] = CheatCycle(dealOrder);
                StartCoroutine(cheatCoroutine[dealOrder]);
                break;
        }
        AudioManager.GetOrCreate().SetEffectVolume(1);
        AudioManager.GetOrCreate().PlayEffectSound(BottomDealingSound);
        CardDeck.RemoveAt(CardDeck.Count - 1);
        dealTimeCur = Time.time;
        BottomCardText.text = "Unknown";
        if (50 >= UnityEngine.Random.Range(0, 101)) //50% 확률로 대사 재생
            DialogManager.Instance.TriggerNextSentence_LowPriority(dealOrder, DialogManager.TextType.recieveCard);
        DealingManager.Instance.InstantNewCardCreate(dealOrder);
        dealOrder++;
        IsDealOver();
    }

    #endregion


    #region afterDeal

    private void GlimpseBottomCard()
    {
        //추후 개발
    }

    #endregion


    #region afterBet

    private void IsBetOver()
    {
        if (betMan.dealtCardCount == 3 || foldPlayerCnt == IngamePlayerCnt - 1)
        {
            SetStateEnd();
        }
        else
        {
            SetStateDeal();
        }
    }

    #endregion


    #region end

    private void ShowCardSum()
    {
        for (int i = 0; i < 4; i++)
        {
            if (betMan.isFold[i])
                continue;
            playerCard0Face[i].GetComponent<SpriteRenderer>().sprite = cardFace[playerCard0[i]];
            playerCard1Face[i].GetComponent<SpriteRenderer>().sprite = cardFace[playerCard1[i]];
            playerCard2Face[i].GetComponent<SpriteRenderer>().sprite = cardFace[playerCard2[i]];
            playerCardFacePosition[i].SetActive(true);
            playerSumText[i].text = playerCardSum[i].ToString();
        }
    }

    private void ResetDealtCard()
    {
        Array.Clear(playerCard0Num, 0, playerCard0Num.Length);
        Array.Clear(playerCard1Num, 0, playerCard1Num.Length);
        Array.Clear(playerCard2Num, 0, playerCard2Num.Length);
        Array.Clear(playerCardSum, 0, playerCardSum.Length);
        dealOrder = 0;
        foldPlayerCnt = 0;
        for (int i = 0; i < 4; i++)
        {
            playerCard0Text[i].text = " - ";
            playerCard1Text[i].text = " - ";
            playerCard2Text[i].text = " - ";
            playerSumText[i].text = "Sum";
            playerCard0Obj[i].SetActive(false);
            playerCard1Obj[i].SetActive(false);
            playerCard2Obj[i].SetActive(false);
        }
    }

    private void TriggerCardOpen()
    {
        StartCoroutine(CardOpen());
    }

    private IEnumerator CardOpen()
    {
        ShowCardSum();
        yield return new WaitForSeconds(2.5f);
        if (foldPlayerCnt == IngamePlayerCnt - 1)
        {
            for (int i = 0; i < betMan.isFold.Length; i++)
            {
                if (betMan.isFold[i] == false)
                {
                    betMan.CalculateResult(betMan.DecideWinnerByFold(i));
                }
            }
        }
        else
        {
            betMan.CalculateResult(betMan.DecideWinner(playerCardSum));
        }
        TriggerNextTurn();
        for (int i = 0; i < playerArray.Length; i++)
            playerIsDetectable[i] = false;
        betMan.isBetOver = false;
    }

    public void TriggerNextTurn()
    {
        StartCoroutine(NextTurn());
    }

    private IEnumerator NextTurn()
    {
        yield return new WaitForSeconds(4f);
        gameTurn++;
        ResetDealtCard();
        betMan.ResetBet();
        if(gameTurn > 2 || IngamePlayerCnt < 2)
        {
            if (betMan.DayResult())
            {
                DialogManager.Instance.TriggerNextSentence_HighPriority(allyPlayerPosition, DialogManager.TextType.dayWin);
                yield return StartCoroutine(DialogManager.Instance.WaitForHighDialog());
                if (!isFade)
                {
                    isFade = true;
                    Fade.Out(2.5f, () =>
                    {
                        Debug.Log(PlayerPrefs.GetInt("Day") + "일차 승리");
                        PlayerPrefs.SetInt("Day", PlayerPrefs.GetInt("Day") + 1);
                        if (PlayerPrefs.GetInt("Day") == 2)
                        {
                            PlayerPrefs.SetInt("CutSceneBegin", 9);
                            PlayerPrefs.SetInt("CutSceneEnd", 12);
                        }
                        if (PlayerPrefs.GetInt("Day") == 3)
                        {
                            PlayerPrefs.SetInt("CutSceneBegin", 13);
                            PlayerPrefs.SetInt("CutSceneEnd", 16);
                        }
                        SceneManager.LoadScene("CutScene");
                    });
                }
            }
            else
            {
                DialogManager.Instance.TriggerNextSentence_HighPriority(allyPlayerPosition, DialogManager.TextType.dayLose);
                yield return StartCoroutine(DialogManager.Instance.WaitForHighDialog());
                if (!isFade)
                {
                    isFade = true;
                    Fade.Out(2.5f, () =>
                    {
                        Debug.Log("2일차 패배, 즉시 재도전");
                        SceneManager.LoadScene("Game Scene");
                    });
                }
            }
        }
        else
            SetStateStart();
    }
    #endregion

    public void EliminatePlayer(int idx, int type) //0이면 베팅금 부족, 1이면 고발
    {
        IngamePlayerCnt--;
        switch (type)
        {
            case NO_MONEY_ELIMINATED:
                betMan.EliminatePlayer(idx, type);
                DialogManager.Instance.TriggerNextSentence_HighPriority(idx, DialogManager.TextType.busted);
                break;
            case DETECTED_CHEAT_ELIMINATED:
                betMan.EliminatePlayer(idx, type);
                DialogManager.Instance.TriggerNextSentence_HighPriority(idx, DialogManager.TextType.detected);
                break;
            default:
                Debug.Log("-");
                break;
        }
        //idx 플레이어 제거처리
    }

    public void GameOver()
    {
        if (!isFade)
        {
            //철컥 탕~ 또는 비명소리
            // 너 졋음 UI
            isFade = true;
            Fade.Out(2.5f, () =>
            {
                SceneManager.LoadScene("Game Scene");
                Debug.Log("게임 끝!");
            });
        }
    }

    IEnumerator GameOverByMissDetect(int idx)
    {
        DialogManager.Instance.TriggerNextSentence_HighPriority(idx, DialogManager.TextType.missDetected);
        yield return StartCoroutine(DialogManager.Instance.WaitForHighDialog());
        GameOver();
    }

    IEnumerator GameOverBySuspicion()
    {
        if (gameState == GameState.deal)
        {
            DialogManager.Instance.TriggerNextSentence_HighPriority(dealOrder, DialogManager.TextType.suspicion);
            yield return StartCoroutine(DialogManager.Instance.WaitForHighDialog());
        }
        else
        {
            for (int i = 0; i < playerArray.Length; i++)
            {
                if (betMan.isEliminated[i] || playerArray[i].isAlly)
                    continue;
                DialogManager.Instance.TriggerNextSentence_HighPriority(dealOrder, DialogManager.TextType.suspicion);
                yield return StartCoroutine(DialogManager.Instance.WaitForHighDialog());
                break;
            }
        }
        GameOver();
    }


    #region Cheat

    private IEnumerator CheatCycle(int playerIdx) //코루틴
    {
        if (playerArray[playerIdx].isAlly)
        {
            yield break;
        }
        DecideToSwitch(playerIdx); //2번째 장 받고 결심한 경우, 3번째장 받자마자 스위치
        betMan.SetIsPlayerCheat(playerIdx); //3번째 장 받고 결심 여부 결정
        yield return new WaitForSeconds(UnityEngine.Random.Range(2, 4));
        DecideToSwitch(playerIdx); //랜덤 2~4초 뒤 스위치
        yield break;
    }
    // 1번 플레이어 받았을때 실행(1번으로)
    // 2번 플레이어 받았을때 실행(2번으로)
    // ,...
    // 4번 플레이ㅓ~~

    public void DecideToSwitch(int idx)
    {
        if (playerIsCheat[idx] == true)
        {
            SwitchCard(idx);
        }
    }


    public void SwitchCard(int idx)
    {
        if(betMan.isFold[idx])
        {
            playerIsCheat[idx] = false;
            return;
        }
        //사기치는 애니메이션 재생
        playerIsCheat[idx] = false;
        Debug.Log("CHEAT! " + idx);
        playerArray[idx].Start_DoCheat(idx);
        if(playerCard0Num[idx] + playerCard1Num[idx] > 10)
        {
            playerCard2[idx] = 21 - playerCard0Num[idx] - playerCard1Num[idx] - 1 + UnityEngine.Random.Range(0, 4) * 13;
            playerCard2Num[idx] = playerCard2[idx] % 13 + 1;
        }
        else
        {
            playerCard2[idx] = UnityEngine.Random.Range(9, 13) - 1 + UnityEngine.Random.Range(0, 4) * 13;
            playerCard2Num[idx] = 10;
        }
        playerCardSum[idx] = playerCard0Num[idx] +
                             playerCard1Num[idx] +
                             playerCard2Num[idx];

        //playerCard0Text[idx].text = playerCard0Num[idx].ToString();
        //playerCard1Text[idx].text = playerCard1Num[idx].ToString();
        playerCard2Text[idx].text = playerCard2Num[idx].ToString();
    }
    #endregion

    public void DoCheatCycle() //안쓰는중
    {
        cheatCoroutine[dealOrder] = CheatCycle(dealOrder);
        StartCoroutine(cheatCoroutine[dealOrder]);
    }

    //- > 내가 받은 직후부터 자기가 내가 베팅하기 직전(개발 편의를 위해..) 까지 치팅 가능성 있음




    /*public void CodingAllyToFold()
    {
        betMan.isAllyCodedFold = true;
        betMan.isAllyCodedRaise = false;
    }*/
    public void CodingAllyToRaise()
    {
        //betMan.isAllyCodedFold = false;
        betMan.isAllyCodedRaise = true;

    }
    public void OnMouseClickPlayer(int playerIdx) //플레이어 버튼에 할당
    {
        if(mousePointState == MousePointState.code)
        {
            if (!playerArray[playerIdx].isAlly)
            {
                return;
            }
            //if (codeType == 0)
            //{
            //    CodingAllyToFold();
            //}
            //else //1
            CodingAllyToRaise();
        }
       
        if(mousePointState == MousePointState.detect)
        {
            if (playerArray[playerIdx].isAlly)
            {
                return;
            }
            AudioManager.GetOrCreate().SetEffectVolume(1);
            AudioManager.GetOrCreate().PlayEffectSound(DetectSound);
            AudioManager.GetOrCreate().SetEffectVolume(0.2f);
            if (playerIsDetectable[playerIdx] == true)
            {
                DialogManager.Instance.TriggerNextSentence_HighPriority(playerIdx, DialogManager.TextType.detected);
                EliminatePlayer(playerIdx , 1);
                playerIsDetectable[playerIdx] = false;
                Debug.Log("GOTCHA!");
                playerCard0Num[playerIdx] = 0;
                playerCard1Num[playerIdx] = 0;
                playerCard2Num[playerIdx] = 0;
                playerCardSum[playerIdx] = 0;
            }
            else
            {
                StartCoroutine(GameOverByMissDetect(playerIdx));
            }
        }
    }

    private void SetSuspicionBar()
    {
        if (suspicionBar != null)
            suspicionBar.value = suspicionLevelCur / suspicionLevelMax;
    }

    public void IncreaseSuspicionByDealTime()
    {
        suspicionLevelCur += 5f;
        SetSuspicionBar();
    }

    public void IncreaseSuspicionByDragTime()
    {
        if (suspicionLevelCur >= suspicionLevelMax)
            return;
        suspicionLevelCur += 0.2f;
        SetSuspicionBar();
        if (suspicionLevelCur >= suspicionLevelMax)
        {
            StartCoroutine(GameOverBySuspicion());
        }
    }

    public void IncreaseSuspicionByDragButDontDeal()
    {
        if (suspicionLevelCur >= suspicionLevelMax)
            return;
        suspicionLevelCur += 10f;
        SetSuspicionBar();
        if (suspicionLevelCur >= suspicionLevelMax)
        {
            StartCoroutine(GameOverBySuspicion());
        }
    }

    public void IncreaseSuspicionByGauge(int gauge)
    {
        if (suspicionLevelCur >= suspicionLevelMax)
            return;
        switch(gauge)
        {
            case 0: //Green
                break;
            case 1: //Yellow
                suspicionLevelCur += 10f;
                break;
            case 2: //Red
                suspicionLevelCur += 30f;
                break;
            default:
                break;
        }
        SetSuspicionBar();
        if (suspicionLevelCur >= suspicionLevelMax)
        {
            StartCoroutine(GameOverBySuspicion());
        }
    }
}
