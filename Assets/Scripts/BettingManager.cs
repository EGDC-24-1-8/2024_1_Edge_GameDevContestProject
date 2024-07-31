using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BettingManager : MonoBehaviour
{
    [SerializeField] private Player[] playerArray = null;
    [SerializeField] private int ante = 10; //참여금
    [SerializeField] private const int defaultBet = 10; //기본 베팅 단위
    [SerializeField] private int roundBet = 0; //기본 베팅 단위
    [SerializeField] private int pot = 0; //판돈
    [SerializeField] private int prize = 0; //라운드 상금
    [SerializeField] private int eliminationCriteria = 0;
    [SerializeField] private List<int> winner = null;
    [SerializeField] private int[] playerCardSum = null;
    [SerializeField] private int dealtCardCount = 0;
    [SerializeField] private int dealOrder = 0;
    [SerializeField] private int endOrder = 0;

    [SerializeField] private int maxBet = 10;

    [SerializeField] private Text[] playerBetText = null;
    [SerializeField] private Text potText = null;
    [SerializeField] private Text[] winnerText = null;
    [SerializeField] private Text[] playerFoldText = null;
    [SerializeField] private Text[] playerSeedText = null;

    [SerializeField] public bool[] isFold = null;
    [SerializeField] public bool isBetOver = false;

    void Start()
    {
        playerArray = GameManager.Instance.playerArray;
        playerCardSum = GameManager.Instance.playerCardSum;
        roundBet = defaultBet;
    }

    void Update()
    {
        if (isBetOver)
        {
            StopCoroutine("Betting");
            UpdateUIText();
        }
    }

    public void resetBet()
    {
        for (int i = 0; i < playerArray.Length; i++)
        {
            playerArray[i].playerBettingMoney = 0;
            
        }
        pot = 0;
    }

    public void UpdateUIText()
    {
        for (int i = 0; i < playerArray.Length; i++)
        {
            playerSeedText[i].text = playerArray[i].playerMoney.ToString();
            playerBetText[i].text = playerArray[i].playerBettingMoney.ToString();
            playerFoldText[i].text = isFold[i] ? "FOLD" : "IN";
            potText.text = pot.ToString();
        }
    }

    public IEnumerator Betting()
    {
        endOrder = playerArray.Length;
        GameManager.Instance.SetIsAbleToDeal(false);
        for (int i = 0; i != endOrder && !isBetOver; i = (i+1)%4) //한 턴이 돌 때마다 베팅 진행
        {

            yield return new WaitForSeconds(1);
            bet(i);
            playerSeedText[i].text = playerArray[i].playerMoney.ToString();
        }
        yield return new WaitForSeconds(1);
        GameManager.Instance.CheckDealOrder(); //베팅 이후에 fold 플레이어 순번을 제외해야하기때문에 실행
        if (GameManager.Instance.GetDealtCardCount() > 2)
            isBetOver = true;
        GameManager.Instance.SetIsAbleToDeal(true);
    }

    #region betting options
    public void entranceBet(int playerIdx)
    {
        maxBet = ante;
        playerArray[playerIdx].playerBettingMoney += ante;
        playerArray[playerIdx].playerMoney -= ante;
        pot += ante;
    }

    public void bet(int playerIdx)
    {
        dealtCardCount = GameManager.Instance.GetDealtCardCount();
        dealOrder = GameManager.Instance.GetDealOrder();

        if (isBetOver)
            return;

        if (isFold[playerIdx] == true)
            return;

        if (dealtCardCount == 2 
            && (playerCardSum[playerIdx] < 7 || 17 < playerCardSum[playerIdx]))
        {
            fold(playerIdx);
            return;
        }

        if (dealtCardCount == 3)
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
        call(playerIdx);
    }

    public void fold(int playerIdx)
    {
        isFold[playerIdx] = true;
        GameManager.Instance.foldPlayerCnt++;
        GameManager.Instance.CheckWinnerByFold();
        UpdateUIText();
    }
    #endregion

    public List<int> decideWinner(int[] sum)
    {
        int max = 0;
        for(int i = 0; i < sum.Length; i++)
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
        for (int i = 0; i < winner.Count; i++)
            winnerText[i].text = winner[i].ToString();
        return winner;
    }

    public List<int> decideWinnerByFold(int playerIdx)
    {
        isBetOver = true;
        winner.Clear();
        winner.Add(playerIdx);
        winnerText[0].text = playerIdx.ToString();
        return winner;
    }

    public void calculateResult(List<int> playerIdx)
    {
        isBetOver = true;
        if(winner.Count != 0 )
            prize = pot / winner.Count;
        for ( ; winner.Count > 0; winner.RemoveAt(0))
            playerArray[winner[0]].playerMoney += prize;
        pot = 0;
        roundBet = defaultBet;

        for(int i = 0; i < playerArray.Length; i++)
        {
            playerArray[i].playerBettingMoney = 0;

            if (eliminationCriteria > playerArray[i].playerMoney)
            {
                GameManager.Instance.EliminatePlayer(i, GameManager.NO_MONEY_ELIMINATED);
            }
        }

        GameManager.Instance.TriggerNextTurn();

    }
}
