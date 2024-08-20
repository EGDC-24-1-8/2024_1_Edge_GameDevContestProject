using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
    //Card UI
    [Header("Card UI")]
    [SerializeField] private Text[] playerCard0Text;
    [SerializeField] private Text[] playerCard1Text;
    [SerializeField] private Text[] playerCard2Text;
    [SerializeField] private Text[] playerSumText;
    [SerializeField] private Text TopCardText;
    [SerializeField] private Text BottomCardText;
    [SerializeField] private Text GameDayText;
    [SerializeField] private Text GameTurnText;

    //Card Value
    [Header("Card Value")]
    //public string[] playerName;
    //[SerializeField] private int[] playerCardNum;
    [SerializeField] private int[] playerCard0Num;
    [SerializeField] private int[] playerCard1Num;
    [SerializeField] private int[] playerCard2Num;
    [SerializeField] public int[] playerCardSum;
    [SerializeField] public int codeType = 0; //신호 타입 0 : fold , 1 : raise   


    [SerializeField] public bool[] playerIsCheat = new bool[4] { false, false, false, false };
    [SerializeField] public bool[] playerIsDetectable = new bool[4] { false, false, false, false };
    [SerializeField] public List<int> CardDeck = null;

    [Header("In Game Counts")]
    [SerializeField] public int foldPlayerCnt = 0;
    [SerializeField] public int IngamePlayerCnt = 4;

    [Header("Turn Info")]
    [SerializeField] private int gameDay = 1;
    [SerializeField] private int gameTurn = 0;
    //[SerializeField] private int dealtCardCount = 0; // 카드 나눠주는 턴
    [SerializeField] private int dealOrder = 0; // 현재 카드 나눠줄 플레이어 순서

    [Header("etc")]
    [SerializeField] private GameObject[] Player;
    [SerializeField] private List<PlayerData> enemyPlayerDataSet = null;
    [SerializeField] private List<PlayerData> allyPlayerDataSet = null;
    [SerializeField] private List<PlayerData> tempEnemyPlayerDataSet = null;
    [SerializeField] private List<PlayerData> tempAllyPlayerDataSet = null;
    [SerializeField] public Player[] playerArray = null;

    [SerializeField] public BettingManager betMan;
    [SerializeField] private IEnumerator[] cheatCoroutine = new IEnumerator[4];

    //[SerializeField] private AudioClip hoverSound = null;
    
    private void Awake()
    {
        if (null == Instance) //디자인패턴중 싱글톤 패턴
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            //Destroy(this.gameObject);
        }
        InitPlayer();
        CardDeck = InitDeck();
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
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            if (mousePointState == MousePointState.code)
            {
                if (codeType == 0)
                {
                    codeType = 1;
                }
                else
                {
                    codeType = 0;
                }
                mousePointState = MousePointState.code;
            }
            else
            {
                mousePointState = MousePointState.code;
                codeType = 0;
            }
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            mousePointState = MousePointState.detect;
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(isPause)
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
    }

    #region set state

    public void SetStateStart()
    {
        if(gameTurn > 2 || IngamePlayerCnt < 2)
        {
            betMan.ResetPlayer();
            InitPlayer();
            gameTurn = 0;
            gameDay++;
            IngamePlayerCnt = 4;
        }
        GameDayText.text = "Day " + gameDay;
        GameTurnText.text = "Turn " + (gameTurn + 1);
        gameState = GameState.start;
        CardDeck = InitDeck();
        CardDeck = shuffleDeck(CardDeck);
        TopCardText.text = CardDeck[0].ToString();
        BottomCardText.text = CardDeck[CardDeck.Count - 1].ToString();

        DialogSystem.Instance.TriggerNextSentence(1, DialogSystem.TextType.start);
        for (int i = 0; i < playerArray.Length; i++) //입장 베팅
        {
            betMan.entranceBet(i);
            playerIsCheat[i] = false;
            playerIsDetectable[i] = false;
            playerArray[i].dealtCardCount = 0;
            if (cheatCoroutine[i] != null)
            {
                StopCoroutine(cheatCoroutine[i]);
            }
        }
        betMan.UpdateUIText();
        SetStateDeal();
    }

    public void SetStateDeal()
    {
        gameState = GameState.deal;
        dealOrder = 0;
        BottomCardText.text = CardDeck[CardDeck.Count - 1].ToString();
    }
    
    public void SetStateAfterDeal()
    {
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
        tempEnemyPlayerDataSet.Clear();
        for (int i = 0; i < allyPlayerDataSet.Count; i++)
        {
            tempAllyPlayerDataSet.Add(allyPlayerDataSet[i]);
        }
        for (int i = 0; i < enemyPlayerDataSet.Count; i++)
        {
            tempEnemyPlayerDataSet.Add(enemyPlayerDataSet[i]);
        }
        for (int i = 0; i < 4; i++)
        {
            if(playerArray[i].isAlly)
            {
                int temp = UnityEngine.Random.Range(0, tempAllyPlayerDataSet.Count);
                playerArray[i].playerData = tempAllyPlayerDataSet[temp];
                playerArray[i].initData();
                tempEnemyPlayerDataSet.RemoveAt(temp);
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
    }



    public void NormalDeal()
    {
        CheckDealOrder();

        if (gameState != GameState.deal)
        {
            return;
        }
        if(DialogSystem.Instance.isDialog)
        {
            return;
        }
        playerArray[dealOrder].dealtCardCount++;
        switch (betMan.dealtCardCount)
        {
            case 0:
                playerCard0Num[dealOrder] = (CardDeck[0] % 13 + 1 > 10) ? 10 : CardDeck[0] % 13 + 1;
                playerCardSum[dealOrder] += playerCard0Num[dealOrder];
                playerCard0Text[dealOrder].text = playerCard0Num[dealOrder].ToString();
                break;
            case 1:
                playerCard1Num[dealOrder] = (CardDeck[0] % 13 + 1 > 10) ? 10 : CardDeck[0] % 13 + 1;
                playerCardSum[dealOrder] += playerCard1Num[dealOrder];
                playerCard1Text[dealOrder].text = playerCard1Num[dealOrder].ToString();
                betMan.SetIsPlayerCheat(dealOrder);
                break;
            case 2:
                playerCard2Num[dealOrder] = (CardDeck[0] % 13 + 1 > 10) ? 10 : CardDeck[0] % 13 + 1;
                playerCardSum[dealOrder] += playerCard2Num[dealOrder];
                playerCard2Text[dealOrder].text = playerCard2Num[dealOrder].ToString();

                cheatCoroutine[dealOrder] = CheatCycle(dealOrder);
                StartCoroutine(cheatCoroutine[dealOrder]);

                break;
            default:
                break;
        }
        CardDeck.Remove(CardDeck[0]);
        TopCardText.text = CardDeck[0].ToString();
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
        if (DialogSystem.Instance.isDialog)
        {
            return;
        }
        playerArray[dealOrder].dealtCardCount++;
        switch (betMan.dealtCardCount)
        {
            case 0:
                playerCard0Num[dealOrder] = (CardDeck[CardDeck.Count - 1] % 13 + 1 > 10) ? 10 : CardDeck[CardDeck.Count - 1] % 13 + 1;
                playerCardSum[dealOrder] += playerCard0Num[dealOrder];
                playerCard0Text[dealOrder].text = playerCard0Num[dealOrder].ToString();
                break;
            case 1:
                playerCard1Num[dealOrder] = (CardDeck[CardDeck.Count - 1] % 13 + 1 > 10) ? 10 : CardDeck[CardDeck.Count - 1] % 13 + 1;
                playerCardSum[dealOrder] += playerCard1Num[dealOrder];
                playerCard1Text[dealOrder].text = playerCard1Num[dealOrder].ToString();
                betMan.SetIsPlayerCheat(dealOrder);
                break;
            case 2:
                playerCard2Num[dealOrder] = (CardDeck[CardDeck.Count - 1] % 13 + 1 > 10) ? 10 : CardDeck[CardDeck.Count - 1] % 13 + 1;
                playerCardSum[dealOrder] += playerCard2Num[dealOrder];
                playerCard2Text[dealOrder].text = playerCard2Num[dealOrder].ToString();
                cheatCoroutine[dealOrder] = CheatCycle(dealOrder);
                StartCoroutine(cheatCoroutine[dealOrder]);
                break;
        }
        CardDeck.RemoveAt(CardDeck.Count - 1);
        BottomCardText.text = "Unknown";
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
            playerSumText[i].text = playerCardSum[i].ToString();
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
                break;
            case DETECTED_CHEAT_ELIMINATED:
                betMan.EliminatePlayer(idx, type);
                break;
            default:
                Debug.Log("-");
                break;
        }
        //idx 플레이어 제거처리
    }

    public void GameOver()
    {
        Debug.Log("게임 끝!");
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
        //사기치는 애니메이션 재생
        playerIsCheat[idx] = false;
        Debug.Log("CHEAT! " + idx);
        playerArray[idx].Start_DoCheat(idx);
        playerCard0Num[idx] = 5;
        playerCard1Num[idx] = 6;
        playerCard2Num[idx] = 10; //숨긴 카드 3장을 가지고 특정 몇 장만 바꾸는 식으로 조작하도록 수정
        playerCardSum[idx] = 21;

        playerCard0Text[idx].text = playerCard0Num[idx].ToString();
        playerCard1Text[idx].text = playerCard1Num[idx].ToString();
        playerCard2Text[idx].text = playerCard2Num[idx].ToString();
    }
    #endregion

    public void DoCheatCycle() //안쓰는중
    {
        cheatCoroutine[dealOrder] = CheatCycle(dealOrder);
        StartCoroutine(cheatCoroutine[dealOrder]);
    }

    //- > 내가 받은 직후부터 자기가 내가 베팅하기 직전(개발 편의를 위해..) 까지 치팅 가능성 있음




    public void CodingAllyToFold()
    {
        betMan.isAllyCodedFold = true;
        betMan.isAllyCodedRaise = false;
    }
    public void CodingAllyToRaise()
    {
        betMan.isAllyCodedFold = false;
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
            if (codeType == 0)
            {
                CodingAllyToFold();
            }
            else //1
            {
                CodingAllyToRaise();
            }
        }
       
        if(mousePointState == MousePointState.detect)
        {
            if (playerArray[playerIdx].isAlly)
            {
                return;
            }
            if (playerIsDetectable[playerIdx] == true)
            {
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
                GameOver();
            }
        }
        


    }
}
