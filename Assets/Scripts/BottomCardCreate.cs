using System;
using UnityEngine;

public class BottomCardCreate : MonoBehaviour
{
    [SerializeField] private GameObject bottomCard; 
    [SerializeField] private GameObject cardPrefab; 
    [SerializeField] private Transform cardSpawnPosition; 
    private bool trigger;

    private void Start()
    {
        NewCardCreate(true);
    }
    
    void NewCardCreate(bool state)
    {
        GameObject newCard = Instantiate(cardPrefab, cardSpawnPosition.transform.position, Quaternion.identity, cardSpawnPosition.transform);
        //newCard.transform.parent = cardPrefab.transform;
        Debug.Log("newCard생성");
        
        newCard.SetActive(true);
        Transform child1 = newCard.transform.GetChild(0);
        Transform child2 = newCard.transform.GetChild(1);
        if (child1 != null) child1.gameObject.SetActive(true);
        if (child2 != null) child2.gameObject.SetActive(true);
        child2.GetComponent<CardController>().cardMoved += NewCardCreate;
    }
}
