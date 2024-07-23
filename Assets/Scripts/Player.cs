using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerData playerData;


    public int playerMoney;
    public int playerBettingMoney;
    public string playerName;
    public int highPreference;
    public int lowPreference;
    public float cheatFrequnce;

    public bool isCheat = false;

    public void initData()
    {
        playerBettingMoney = playerData.PlayerBettingMoney;
        playerBettingMoney = playerData.PlayerBettingMoney;
        playerName = playerData.PlayerName;
        highPreference = playerData.HighPreference;
        lowPreference = playerData.LowPreference;
        cheatFrequnce = playerData.CheatFrequnce;
    }
    public void Start_DoCheat()
    {
        StartCoroutine(DoCheat());
    }

    private IEnumerator DoCheat()
    {
        isCheat = true;
        yield return new WaitForSeconds(3f);
        isCheat = false;
    }
}
