using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BettingManager : MonoBehaviour
{
    [SerializeField] private Player[] playerArray;
    [SerializeField] private int Ante; //참여금

    [SerializeField] private int pot;//참여금
    [SerializeField] private int EliminatedMoney;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    

    public void Betting(int playerIdx)
    {

        playerArray[playerIdx].playerBettingMoney += Ante;
        playerArray[playerIdx].playerMoney -= Ante;
        pot += Ante;

    }

    public void calculateResult(int playerIdx)
    {
        playerArray[playerIdx].playerMoney += pot;


        pot = 0;
        for(int i = 0; i < playerArray.Length; i++)
        {
            playerArray[i].playerBettingMoney = 0;

            if (EliminatedMoney > playerArray[i].playerMoney)
            {
                GameManager.Instance.EliminatePlayer(i, GameManager.NO_MONEY_ELIMINATED);
            }

        }
    }
}
