using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;

    //巨切昔鳶渡掻 縮越宕 鳶渡

    public Player player;
    public GameObject Player;

    private void Awake()
    {
        if (null == Instance)
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
        // AudioManager
        //けいしいいけしけいしけいしいけしけいけいしけいしけい TEST
    }

    // Update is called once per frame
    void Update()
    {
        //けいしいいけしけいしけいしいけしけいけいしけいしけい TEST//けいしいいけしけいしけいしいけしけいけいしけいしけい TEST
        //けいしいいけしけいしけいしいけしけいけいしけいしけい TEST
        //けいしいいけしけいしけいしいけしけいけいしけいしけい TEST
    }


    public void GameOver()
    {
        Debug.Log("惟績 魁!");
    }
}
