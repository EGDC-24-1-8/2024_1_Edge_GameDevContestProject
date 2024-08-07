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
    [SerializeField] public MousePointState mousePointState = MousePointState.normal;
    //Card UI
    [Header("Card UI")]
    [SerializeField] private Text[] playerCard0Text;
    [SerializeField] private Text[] playerCard1Text;
    [SerializeField] private Text[] playerCard2Text;
    [SerializeField] private Text[] playerSumText;
    [SerializeField] private Text TopCardText;
    [SerializeField] private Text BottomCardText;

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
    [SerializeField] private List<int> CardDeck = null;

    [Header("In Game Counts")]
    [SerializeField] public int foldPlayerCnt = 0;
    [SerializeField] public int IngamePlayerCnt = 4;

    [Header("Turn Info")]
    [SerializeField] private int gameTurn = 0;
    //[SerializeField] private int dealtCardCount = 0; // 카드 나눠주는 턴
    [SerializeField] private int dealOrder = 0; // 현재 카드 나눠줄 플레이어 순서

    [Header("etc")]
    [SerializeField] private GameObject[] Player;
    [SerializeField] private List<PlayerData> playerDataSet = null;
    [SerializeField] public Player[] playerArray = null;
    
    [SerializeField] public BettingManager betMan;
    [SerializeField] private IEnumerator[] cheatCoroutine = new IEnumerator[4];
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
    }

    void Start()
    {
        InitPlayer();
        IngamePlayerCnt = 4;
        SetStateStart();
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            mousePointState = MousePointState.normal;
        }
        else if(Input.GetKeyDown(KeyCode.W))
        {
            if(mousePointState == MousePointState.code)
            {
                if(codeType == 0)
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
        else if(Input.GetKeyDown(KeyCode.E))
        {
            mousePointState = MousePointState.detect;
        }
    }

    #region set state

    public void SetStateStart()
    {
        gameState = GameState.start;
        CardDeck = InitDeck();
        CardDeck = shuffleDeck(CardDeck);
        TopCardText.text = CardDeck[0].ToString();
        BottomCardText.text = CardDeck[CardDeck.Count - 1].ToString();
        for (int i = 0; i < playerArray.Length; i++) //입장 베팅
        {
            betMan.entranceBet(i);
            playerIsCheat[i] = false;
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
        ShowCardSum();
        if (foldPlayerCnt == IngamePlayerCnt - 1)
        {
            for (int i = 0; i < betMan.isFold.Length; i++)
            {
                if (betMan.isFold[i] == false)
                {
                    betMan.CalculateResult(betMan.DecideWinnerByFold(i));
                    return;
                }
            }
        }
        else
        {
            betMan.CalculateResult(betMan.DecideWinner(playerCardSum));
        }
        betMan.isBetOver = false;
        //SetStateStart();
    }

    #endregion


    #region start

    private List<int> InitDeck()
    {
        int temp;
        List<int> initDeck = new List<int>();

        for (int i = 0; i < 52; i++) //카드 생성
        {
            temp = i % 13 + 1;
            temp = temp > 10 ? 10 : temp;
            initDeck.Add(temp);
        }

        return initDeck;
    }

    private void InitPlayer()
    {
        for (int i = 0; i < 4; i++)
        {
            int temp = UnityEngine.Random.Range(0, playerDataSet.Count);
            playerArray[i].playerData = playerDataSet[temp];
            playerArray[i].initData();
            playerDataSet.RemoveAt(temp);
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
            if (betMan.isFold[dealOrder])
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

        switch (betMan.dealtCardCount)
        {
            case 0:
                playerCard0Num[dealOrder] = CardDeck[0];
                playerCardSum[dealOrder] += playerCard0Num[dealOrder];
                playerCard0Text[dealOrder].text = playerCard0Num[dealOrder].ToString();
                break;
            case 1:
                playerCard1Num[dealOrder] = CardDeck[0];
                playerCardSum[dealOrder] += playerCard1Num[dealOrder];
                playerCard1Text[dealOrder].text = playerCard1Num[dealOrder].ToString();
                break;
            case 2:
                playerCard2Num[dealOrder] = CardDeck[0];
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

        switch (betMan.dealtCardCount)
        {
            case 0:
                playerCard0Num[dealOrder] = CardDeck[CardDeck.Count - 1];
                playerCardSum[dealOrder] += playerCard0Num[dealOrder];
                playerCard0Text[dealOrder].text = playerCard0Num[dealOrder].ToString();
                break;
            case 1:
                playerCard1Num[dealOrder] = CardDeck[CardDeck.Count - 1];
                playerCardSum[dealOrder] += playerCard1Num[dealOrder];
                playerCard1Text[dealOrder].text = playerCard1Num[dealOrder].ToString();
                break;
            case 2:
                playerCard2Num[dealOrder] = CardDeck[CardDeck.Count - 1];
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

    public void EliminatePlayer(int idx, int type) //0이면 베팅금 부족, 1이면 고발
    {
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
        IngamePlayerCnt--;


        //idx 플레이어 제거처리
    }

    private IEnumerator NextTurn()
    {
        yield return new WaitForSeconds(5f);
        gameTurn++;
        ResetDealtCard();
        betMan.ResetBet();
        SetStateStart();
    }

    public void TriggerNextTurn()
    {
        StartCoroutine(NextTurn());
    }

    public void GameOver()
    {
        Debug.Log("게임 끝!");
    }

    #endregion


    private IEnumerator CheatCycle(int playerIdx) //코루틴
    {

        while (true)
        {
            if (playerArray[playerIdx].isAlly)
            {
                break;
            }
            DecideToSwitch(playerIdx);
            yield return new WaitForSeconds(UnityEngine.Random.Range(10, 30));
        }
    }
    // 1번 플레이어 받았을때 실행(1번으로)
    // 2번 플레이어 받았을때 실행(2번으로)
    // ,...
    // 4번 플레이ㅓ~~

    


    #region Cheat
    public void DecideToSwitch(int idx)
    {
        if (playerIsCheat[idx] == true)
        {
            SwitchCard(idx);
        }


        if (playerCardSum[idx] > 21)
        {

            if ((20f + playerArray[idx].cheatFrequency) > UnityEngine.Random.Range(0, 101))
            {
                SwitchCard(idx);
                StopCoroutine(cheatCoroutine[idx]);

            }


        }
    }


    public void SwitchCard(int idx)
    {
        //사기치는 애니메이션 재생


        Debug.Log("CHEAT! " + idx);
        playerArray[idx].Start_DoCheat();
        playerCard0Num[idx] = 5;
        playerCard1Num[idx] = 6;
        playerCard2Num[idx] = 10; //숨긴 카드 3장을 가지고 특정 몇 장만 바꾸는 식으로 조작하도록 수정
        playerCardSum[idx] = 21;


        playerCard0Text[idx].text = playerCard0Num[idx].ToString();
        playerCard1Text[idx].text = playerCard1Num[idx].ToString();
        playerCard2Text[idx].text = playerCard2Num[idx].ToString();

    }
    #endregion

    public void DoCheatCycle()
    {
        cheatCoroutine[dealOrder] = CheatCycle(dealOrder);
        StartCoroutine(cheatCoroutine[dealOrder]);
    }

    //- > 내가 받은 직후부터 자기가 내가 베팅하기 직전(개발 편의를 위해..) 까지 치팅 가능성 있음




    public void CodingAllyToFold()
    {
        betMan.isAllyFold = true;
        betMan.isAllyRaise = false;
    }
    public void CodingAllyToRaise()
    {
        betMan.isAllyFold = false;
        betMan.isAllyRaise = true;

    }

    public void OnMouseClickPlayer(int playerIdx) //플레이어 버튼에 할당
    {
        if (!playerArray[playerIdx].isAlly)
        {
            return;
        }

        if(mousePointState == MousePointState.code)
        {
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
            if (playerIsCheat[playerIdx] == true)
            {
                EliminatePlayer(playerIdx , 1);
            }
            else
            {
                GameOver();
            }
        }
        


    }
}
