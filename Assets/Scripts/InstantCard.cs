using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantCard : MonoBehaviour
{
    private int dealtCardCnt;

    private void Awake()
    {
        dealtCardCnt = GameManager.Instance.betMan.dealtCardCount;
    }
    void GetPlayerCard(int playerIdx)
    {
        GameManager.Instance.GetPlayerCard(playerIdx, dealtCardCnt);
        Debug.Log("Instant " + playerIdx + ", " + dealtCardCnt);
        //GameManager.Instance.isAnim = false;
        Destroy(this.gameObject);
    }
}
