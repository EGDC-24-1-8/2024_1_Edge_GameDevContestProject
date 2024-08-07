using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public PlayerData playerData;
    public Image playerImage;
    public int playerMoney;
    public int playerBettingMoney;
    public string playerName;
    public int highPreference;
    public int lowPreference;
    public float cheatFrequency;


    public bool isAlly = false; 

    public bool isCheat = false;

    public void initData()
    {
        playerMoney = playerData.PlayerMoney;
        playerBettingMoney = playerData.PlayerBettingMoney;
        playerName = playerData.PlayerName;
        highPreference = playerData.HighPreference;
        lowPreference = playerData.LowPreference;
        cheatFrequency = playerData.CheatFrequency;
    }
    public void Start_DoCheat()
    {
        StartCoroutine(DoCheat());
    }

    private IEnumerator DoCheat()
    {
        isCheat = true;
        playerImage.color = Color.red;
        yield return new WaitForSeconds(3f);
        playerImage.color = Color.white;
        isCheat = false;

    }
}
