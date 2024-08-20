using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondCardController : MonoBehaviour
{
    [SerializeField] private Transform secondCard;
    [SerializeField] private Sprite[] cardImage = null;

    void Start()
    {
        SetImage();
    }
    
    public void SetImage()
    {
        secondCard.gameObject.GetComponent<SpriteRenderer>().sprite
            = cardImage[GameManager.Instance.CardDeck[1] - 1];
    }
}
