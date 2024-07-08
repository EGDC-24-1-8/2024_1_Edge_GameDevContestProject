using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    
    [SerializeField] private int PlayerHP = 10;
    void Start()
    {
        
    }

    void Update()
    {
        if(PlayerHP < 0)
        {
            GameManager.Instance.GameOver();
        }
    }
}
