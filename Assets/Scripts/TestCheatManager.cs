using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCheatManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /*
    private IEnumerator CheatCycle() //코루틴
    {
        while (true)
        {
            for(int i = 0; i < 4; i++)
            {
                DecideToSwitch(i);
            }
            yield return new WaitForSeconds(UnityEngine.Random.Range(10, 30));
        }
    }

    #region Cheat
    public void DecideToSwitch(int idx)
    {
        if (playerCardSum[idx] > 21)
        {

            if (20f + playerArray[idx].cheatFrequency < UnityEngine.Random.Range(0, 101))
            {
                SwitchCard(idx);
            }


        }
    }
    public void SwitchCard(int idx)
    {
        //사기치는 애니메이션 재생
        playerArray[idx].Start_DoCheat();
        playerCard0Num[idx] = 5;
        playerCard1Num[idx] = 6;
        playerCard2Num[idx] = 10; //숨긴 카드 3장을 가지고 특정 몇 장만 바꾸는 식으로 조작하도록 수정
    }
    #endregion
    
    */
}
