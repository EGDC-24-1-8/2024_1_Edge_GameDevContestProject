using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{

    public Animator anim;
    public Rigidbody rb;
    [SerializeField] private int test = 0;
    [SerializeField] private int PlayerHP = 10;
    //다른 스크립트에서 얘에 접근을 못하게 한거야.
    //버그를 막기위해서도 있고
    //무분별하게 다른 스크립트에서 다른 스크립트의 값을 건들지 못하도록 약간 습관을 들여놓음.

    void Start()
    {
        rb.useGravity = false;
    }

    void Update()
    {
        if(PlayerHP < 0)
        {
            GameManager.Instance.GameOver();
        }
        //anim.
    }


    public int getPlayerHP()
    {
        return PlayerHP;
    }

    public void setPlayerHp(int value)
    {
        PlayerHP = value;
    }


    
}
