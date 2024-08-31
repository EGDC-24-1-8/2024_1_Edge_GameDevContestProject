using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DealingManager : MonoBehaviour
{
    public static DealingManager Instance = null;//ΩÃ±€≈Ê ∆–≈œ

    [Header("Card Image")]
    [SerializeField] private Sprite[] cardImage = null;
    [SerializeField] private Transform topCard;
    [SerializeField] private Transform secondCard;
    [SerializeField] private Transform secondArrow;
    [SerializeField] private Transform bottomCard;
    [SerializeField] private Transform bottomArrow;

    [Header("Top Card Create")]
    [SerializeField] private GameObject topCardPrefab;
    [SerializeField] private Transform topCardSpawnPosition;

    [Header("Top Card Create")]
    [SerializeField] private GameObject secondCardPrefab;
    [SerializeField] private Transform secondCardSpawnPosition;

    [Header("Bottom Card Create")]
    [SerializeField] private GameObject bottomCardPrefab;
    [SerializeField] private Transform bottomCardSpawnPosition;

    [Header("Instant Dealt Card Create")]
    [SerializeField] private GameObject instantCard;
    [SerializeField] private Transform instantCardSpawnPosition;
    [SerializeField] public bool isAnim = false;

    private void Awake()
    {
        if (null == Instance) //µ¿⁄¿Œ∆–≈œ¡ﬂ ΩÃ±€≈Ê ∆–≈œ
        {
            Instance = this;
            //DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
    }
    private void Update()
    {
    }

    #region new card create
    public void TopNewCardCreate(bool state)
    {
        DestroyTopCardSpawnPositionChild();
        GameObject newCard = Instantiate(topCardPrefab,
            topCardSpawnPosition.transform.position,
            Quaternion.identity, 
            topCardSpawnPosition.transform);

        //Debug.Log("new Top Card Created");
        newCard.SetActive(true);
        newCard.GetComponent<TopCardController>().TopCardMoved += TopNewCardCreate;
    }
    public void SecondNewCardCreate(bool state)
    {
        DestroySecondCardSpawnPositionChild();
        GameObject newCard = Instantiate(secondCardPrefab,
            secondCardSpawnPosition.transform.position,
            Quaternion.identity,
            secondCardSpawnPosition.transform);

        //Debug.Log("new Second Card Created");
        newCard.SetActive(true);
        Transform child1 = newCard.transform.GetChild(0);
        Transform child2 = newCard.transform.GetChild(1);
        if (child1 != null) child1.gameObject.SetActive(true);
        if (child2 != null) child2.gameObject.SetActive(true);
        child2.GetComponent<SecondCardController>().secondCardMoved += SecondNewCardCreate;
    }
    public void BottomNewCardCreate(bool state)
    {
        DestroyBottomCardSpawnPositionChild();
        GameObject newCard = Instantiate(bottomCardPrefab,
            bottomCardSpawnPosition.transform.position, 
            Quaternion.identity, 
            bottomCardSpawnPosition.transform);

        //Debug.Log("new Bottom Card Created");
        newCard.SetActive(true);
        Transform child1 = newCard.transform.GetChild(0);
        Transform child2 = newCard.transform.GetChild(1);
        if (child1 != null) child1.gameObject.SetActive(true);
        if (child2 != null) child2.gameObject.SetActive(true);
        child2.GetComponent<BottomCardController>().bottomCardMoved += BottomNewCardCreate;
    }

    public void InstantNewCardCreate(int playerIdx)
    {
        //isAnim = true;
        GameObject newDealtCard = Instantiate(instantCard,
            instantCard.transform.position,
            Quaternion.identity,
            instantCardSpawnPosition.transform);
        newDealtCard.SetActive(true);
        switch (playerIdx)
        {
            case 0:
                newDealtCard.GetComponent<Animator>().SetTrigger("Player0");
                break;
            case 1:
                newDealtCard.GetComponent<Animator>().SetTrigger("Player1");
                break;
            case 2:
                newDealtCard.GetComponent<Animator>().SetTrigger("Player2");
                break;
            case 3:
                newDealtCard.GetComponent<Animator>().SetTrigger("Player3");
                break;
        }
    }

    #endregion

    #region set image

    public void SetImage()
    {
        TopSetImage();
        BottomSetImage();
    }
    public void TopSetImage()
    {
        topCard.gameObject.GetComponent<SpriteRenderer>().sprite
            = cardImage[GameManager.Instance.CardDeck[0] % 13];
        SecondSetImage();
        DestroySecondCardSpawnPositionChild();
        SecondNewCardCreate(true);
    }
    public void SecondSetImage()
    {
        secondCard.gameObject.GetComponent<SpriteRenderer>().sprite
           = cardImage[GameManager.Instance.CardDeck[1] % 13];
    }
    public void BottomSetImage()
    {
        bottomCard.gameObject.GetComponent<SpriteRenderer>().sprite
            = cardImage[GameManager.Instance.CardDeck[GameManager.Instance.CardDeck.Count - 1] % 13];
    }
    #endregion

    #region destroy child

    public void DestroyChild()
    {
        DestroyTopCardSpawnPositionChild();
        DestroySecondCardSpawnPositionChild();
        DestroyBottomCardSpawnPositionChild();
    }
    public void DestroyTopCardSpawnPositionChild()
    {
        foreach (Transform child in topCardSpawnPosition)
        {
            Destroy(child.gameObject);
        }
    }
    public void DestroySecondCardSpawnPositionChild()
    {
        foreach (Transform child in secondCardSpawnPosition)
        {
            Destroy(child.gameObject);
        }
    }
    public void DestroyBottomCardSpawnPositionChild()
    {
        foreach (Transform child in bottomCardSpawnPosition)
        {
            Destroy(child.gameObject);
        }
    }
    #endregion
}
