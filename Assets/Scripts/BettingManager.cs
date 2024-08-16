using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BettingManager : MonoBehaviour
{

    public const int NO_MONEY_ELIMINATED = 0;
    public const int DETECTED_CHEAT_ELIMINATED = 1;

    [SerializeField] private Player[] playerArray = null;
    [SerializeField] private int[] playerCardSum = null;

    [Header("UI")]
    [SerializeField] private Text[] playerBetText = null;
    [SerializeField] private Text potText = null;
    [SerializeField] private Text casinoMoneyText = null;
    [SerializeField] private Text[] playerFoldText = null;
    [SerializeField] private Text[] playerSeedText = null;
    [SerializeField] private Text winCriteriaText = null;

    [Header("Betting Amount")]
    [SerializeField] private int ante = 10; //참여금
    [SerializeField] private int defaultBet = 10; //기본 베팅 단위
    [SerializeField] private int roundBet = 0; //해당 베팅 페이즈의 베팅 단위
    [SerializeField] private int maxBet = 10; //라운드 최대 베팅 금액

    [SerializeField] private int pot = 0; //판돈
    [SerializeField] private int prize = 0; //라운드 상금
    [SerializeField] private int eliminationCriteria = 0; //소유금이 여기까지 줄어들면 해당 플레이어가 테이블을 떠남

    [SerializeField] public int casinoMoney = 0; //판돈
    [SerializeField] private int winCriteria = 0;



    [Header("In Game Data")]
    [SerializeField] private List<int> winner = null;
    [SerializeField] public int dealtCardCount = 0;
    [SerializeField] private int endOrder = 0;
    [SerializeField] public bool[] isFold = { false, false, false, false };
    [SerializeField] public bool[] isEliminated = { false, false, false, false };

    [SerializeField] public bool isBetOver = false;
    [SerializeField] public bool isAllyFold = false;
    [SerializeField] public bool isAllyRaise = false;


    void Start()
    {
        playerArray = GameManager.Instance.playerArray;
        playerCardSum = GameManager.Instance.playerCardSum;
        roundBet = defaultBet;
    }

    //베팅 페이즈 전반 관리
    //1. 베팅 관리
    //2. Winner By Fold 상시 체크

    /*
    public void CheckWinnerByFold()
    {
        if (foldPlayerCnt == IngamePlayerCnt-1)
        {
            for (int j = 0; j < betMan.isFold.Length; j++)
            {
                if (betMan.isFold[j] == false)
                {
                    betMan.CalculateResult(betMan.DecideWinnerByFold(j));
                }
            }
        }
    }
    */
    //3. 베팅 페이즈 종료 후 gameState afterBet으로 변경
    //4. 돈 정산까지 얘가 하자

    void Update()
    {
        if (isBetOver)
        {
            StopCoroutine("Betting");
            UpdateUIText();
        }
    }

    public void UpdateUIText()
    {
        for (int i = 0; i < playerArray.Length; i++)
        {
            playerSeedText[i].text = playerArray[i].playerMoney.ToString();
            playerBetText[i].text = playerArray[i].playerBettingMoney.ToString();
            playerFoldText[i].text = isFold[i] ? "FOLD" : "IN";
            potText.text = pot.ToString();
            casinoMoneyText.text = casinoMoney.ToString();
            winCriteriaText.text = (casinoMoney + playerArray[1].playerMoney) + " / " + winCriteria;
        }
    }

    public void TriggerBetting()
    {
        StartCoroutine(Betting());
    }

    public IEnumerator Betting()
    {
        if(GameManager.Instance.gameState != GameManager.GameState.bet || isBetOver == true)
        {
            yield break;
        }
        ++dealtCardCount;
        endOrder = playerArray.Length;
        int ctr = 0;
        for (int i = 0; i != endOrder && !isBetOver; i = (i+1)%4) //한 턴이 돌 때마다 베팅 진행
        {
            ctr++;
            if (ctr >= 1000)
            {
                Debug.Log("dkdkdkdk");
                break;
            }
            if (isFold[i])
                continue;
            yield return new WaitForSeconds(0.5f);
            bet(i);
            playerSeedText[i].text = playerArray[i].playerMoney.ToString();
        }
        yield return new WaitForSeconds(1);
        GameManager.Instance.SetStateAfterBet();
    }

    #region betting options
    public void entranceBet(int playerIdx)
    {
        if (isEliminated[playerIdx])
            return;
        maxBet = ante;
        playerArray[playerIdx].playerBettingMoney += ante;
        playerArray[playerIdx].playerMoney -= ante;
        pot += ante;
        UpdateUIText();
    }

    public void bet(int playerIdx)
    {
        if (isBetOver)
            return;

        if (isFold[playerIdx] == true)
            return;

        if (playerArray[playerIdx].isAlly == true)
        {
            if (isAllyFold == true)
            {
                fold(playerIdx);
                isAllyFold = false;
                return;
            }
            if(isAllyRaise == true)
            {
                raise(playerIdx);
                isAllyRaise = false;
                return;
            }
        }

        if (dealtCardCount == 2
            && (playerCardSum[playerIdx] < 8 || 16 < playerCardSum[playerIdx]))
        {
            fold(playerIdx);
            return;
        }
        else if (dealtCardCount == 3)
        {
            if (playerCardSum[playerIdx] < 17 || 21 < playerCardSum[playerIdx])
            {
                fold(playerIdx);
                return;
            }
            if ((playerCardSum[playerIdx] == 21) && (defaultBet == roundBet))
            {
                raise(playerIdx);
                return;
            }
        }
        call(playerIdx);
    }

    public void call(int playerIdx)
    {
        if (playerArray[playerIdx].playerBettingMoney == maxBet)
        {
            playerArray[playerIdx].playerBettingMoney += roundBet;
            playerArray[playerIdx].playerMoney -= roundBet;
            pot += roundBet;
            maxBet += roundBet;
            endOrder = playerIdx;
        }
        else
        {
            playerArray[playerIdx].playerMoney += playerArray[playerIdx].playerBettingMoney;
            pot -= playerArray[playerIdx].playerBettingMoney;
            playerArray[playerIdx].playerBettingMoney = maxBet;
            playerArray[playerIdx].playerMoney -= maxBet;
            pot += maxBet;
        }
        UpdateUIText();
    }

    public void raise(int playerIdx)
    {
        roundBet += defaultBet;
        endOrder = playerIdx;

        playerArray[playerIdx].playerBettingMoney += roundBet;
        playerArray[playerIdx].playerMoney -= roundBet;
        pot += roundBet;
        maxBet = playerArray[playerIdx].playerBettingMoney;
        endOrder = playerIdx;
        UpdateUIText();
    }

    public void fold(int playerIdx)
    {
        /*
        ~~
        TODO : 확률은 pot에 따라 변동되도록 변경
        ~~
        */

        if (playerArray[playerIdx].isAlly)
        {

        }
        else
        {
            if (GameManager.Instance.playerIsCheat[playerIdx])
            {
                call(playerIdx);
                return;
            }
        }
        


        isFold[playerIdx] = true;
        GameManager.Instance.foldPlayerCnt++;

        CheckWinnerByFold();
        UpdateUIText();
    }
    #endregion

    public void SetIsPlayerCheat(int playerIdx)
    {
        if ((playerArray[playerIdx].dealtCardCount == 2
            && (playerCardSum[playerIdx] < 8 || 16 < playerCardSum[playerIdx]))
            || 
            (playerArray[playerIdx].dealtCardCount == 3
            && (playerCardSum[playerIdx] < 17 || 21 < playerCardSum[playerIdx]))) // fold 조건
        {
            if (20f + GameManager.Instance.playerArray[playerIdx].cheatFrequency < UnityEngine.Random.Range(0, 101))
            {
                GameManager.Instance.playerIsCheat[playerIdx] = true;
                return;
            }
        }
    }


    #region win by fold

    private void CheckWinnerByFold()
    {
        if (GameManager.Instance.foldPlayerCnt == GameManager.Instance.IngamePlayerCnt - 1)
        {
            isBetOver = true;
        }
    }

    public List<int> DecideWinnerByFold(int playerIdx) //winner list 반환
    {
        isBetOver = true;
        winner.Clear();
        winner.Add(playerIdx);
        return winner;
    }

    #endregion

    #region afterBet

    public void ResetBet() //베팅금, 팟 등 데이터 리셋
    {
        dealtCardCount = 0;
        endOrder = playerArray.Length;
        isBetOver = false;
        for (int i = 0; i < playerArray.Length; i++)
        {
            playerArray[i].playerBettingMoney = 0;
            isFold[i] = isEliminated[i]? true : false;
        }
        roundBet = defaultBet;
        pot = 0;
        prize = 0;
    }

    public List<int> DecideWinner(int[] sum) //winner list 반환
    {
        int max = 0;
        for (int i = 0; i < sum.Length; i++)
        {
            if (sum[i] > 21 || 21 - sum[i] > 21 - max || isFold[i] == true)
                continue;
            else if (21 - sum[i] < 21 - max)
            {
                max = sum[i];
                winner.Clear();
            }
            winner.Add(i);
        }
        return winner;
    }

    public void CalculateResult(List<int> playerIdx) //winner에게 prize 전달, 
    {
        isBetOver = true;
        if (winner.Count != 0)
            prize = pot / winner.Count;
        for (; winner.Count > 0; winner.RemoveAt(0))
            playerArray[winner[0]].playerMoney += prize;
        UpdateUIText();
        for (int i = 0; i < playerArray.Length; i++)
        {
            if (eliminationCriteria > playerArray[i].playerMoney && !isEliminated[i])
            {
                GameManager.Instance.EliminatePlayer(i, GameManager.NO_MONEY_ELIMINATED);
            }
        }
        GameManager.Instance.TriggerNextTurn();
    }

    #endregion


    public void EliminatePlayer(int idx, int type) //0이면 베팅금 부족, 1이면 고발
    {
        switch (type)
        {
            case NO_MONEY_ELIMINATED:
                isEliminated[idx] = true;
                isFold[idx] = true;
                break;
            case DETECTED_CHEAT_ELIMINATED:
                playerArray[idx].playerMoney += playerArray[idx].playerBettingMoney;
                pot -= playerArray[idx].playerBettingMoney;
                playerArray[idx].playerBettingMoney = 0;
                if (GameManager.Instance.foldPlayerCnt == GameManager.Instance.IngamePlayerCnt)
                {
                    Debug.Log("DETECTED 4 FOLD");
                    for (int i = 0; i < 4; i++)
                    {
                        playerArray[i].playerMoney += playerArray[i].playerBettingMoney;
                        pot -= playerArray[i].playerBettingMoney;
                        playerArray[i].playerBettingMoney = 0;
                    }
                }
                isEliminated[idx] = true;
                isFold[idx] = true;
                casinoMoney += playerArray[idx].playerMoney;
                
                playerArray[idx].playerMoney = 0;
                break;
            default:
                Debug.Log("-");
                break;
        }
        CheckWinnerByFold();
        UpdateUIText();
        //idx 플레이어 제거처리
    }

    public void ResetPlayer()
    {
        for(int i = 0; i < 4; i++)
        {
            isEliminated[i] = false;
            isFold[i] = false;
            UpdateUIText();
        }
    }
}