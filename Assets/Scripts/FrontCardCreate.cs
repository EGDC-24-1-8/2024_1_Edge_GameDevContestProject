using System;
using UnityEngine;
using UnityEngine.UI;

public class FrontCardCreate : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GameObject secondCard;
    [SerializeField] private Sprite[] cardImage = null;
    [SerializeField] private Transform cardSpawnPosition;
    private void Start()
    {
        NewCardCreate(true);
    }
    
    void NewCardCreate(bool state)
    {
        GameObject newCard = Instantiate(cardPrefab, 
            cardSpawnPosition.transform.position, 
            Quaternion.identity, cardSpawnPosition.transform);
        Debug.Log("newFrontCard생성");
        newCard.SetActive(true);
        newCard.GetComponent<FrontCardController>().FrontCardMoved += NewCardCreate;
    }

    public void SetSecondCardImage()
    {
        secondCard.GetComponent<SpriteRenderer>().sprite
           = cardImage[GameManager.Instance.CardDeck[1] % 13];
    }
}