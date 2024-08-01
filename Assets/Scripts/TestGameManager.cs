using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TestGameManager : MonoBehaviour
{
    public const int NO_MONEY_ELIMINATED = 0;
    public static TestGameManager Instance = null;//싱글톤 패턴

    public enum GameState
    {
        start,
        deal,
        afterDeal,
        bet,
        afterBet,
        end
    };

    [Header("Game State")]
    [SerializeField] public GameState gameState;
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
    
    [SerializeField] public TestBettingManager betMan;


    private void Awake()
    {
        if (null == Instance) //디자인패턴중 싱글톤 패턴
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
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
}
