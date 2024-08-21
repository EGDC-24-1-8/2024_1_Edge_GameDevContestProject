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
    [SerializeField] private Transform bottomCard;
    [SerializeField] private Transform bottomArrow;

    [Header("Top Card Create")]
    [SerializeField] private GameObject topCardPrefab;
    [SerializeField] private GameObject secondCardPrefab;
    [SerializeField] private Transform topCardSpawnPosition;

    [Header("Bottom Card Create")]
    [SerializeField] private GameObject bottomCardPrefab;
    [SerializeField] private Transform bottomCardSpawnPosition;

    private void Awake()
    {
        if (null == Instance) //µ¿⁄¿Œ∆–≈œ¡ﬂ ΩÃ±€≈Ê ∆–≈œ
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
    }
    private void Update()
    {
    }

    #region new card create
    public void TopNewCardCreate(bool state)
    {
        GameObject newCard = Instantiate(topCardPrefab,
            topCardSpawnPosition.transform.position,
            Quaternion.identity, 
            topCardSpawnPosition.transform);

        Debug.Log("new Top Card Created");
        newCard.SetActive(true);
        newCard.GetComponent<TopCardController>().TopCardMoved += TopNewCardCreate;
    }
    public void BottomNewCardCreate(bool state)
    {
        GameObject newCard = Instantiate(bottomCardPrefab,
            bottomCardSpawnPosition.transform.position, 
            Quaternion.identity, 
            bottomCardSpawnPosition.transform);

        Debug.Log("new Bottom Card Created");
        newCard.SetActive(true);
        Transform child1 = newCard.transform.GetChild(0);
        Transform child2 = newCard.transform.GetChild(1);
        if (child1 != null) child1.gameObject.SetActive(true);
        if (child2 != null) child2.gameObject.SetActive(true);
        child2.GetComponent<BottomCardController>().bottomCardMoved += BottomNewCardCreate;
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
    }
    private void SecondSetImage()
    {
        secondCardPrefab.GetComponent<SpriteRenderer>().sprite
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
        foreach (Transform child in topCardSpawnPosition)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in bottomCardSpawnPosition)
        {
            Destroy(child.gameObject);
        }
    }
    #endregion
}
