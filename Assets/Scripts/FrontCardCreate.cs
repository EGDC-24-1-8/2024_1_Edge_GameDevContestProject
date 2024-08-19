using System;
using UnityEngine;

public class FrontCardCreate : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab; 
    [SerializeField] private Transform cardSpawnPosition; 
    private void Start()
    {
        NewCardCreate(true);
    }
    
    void NewCardCreate(bool state)
    {
        GameObject newCard = Instantiate(cardPrefab, cardSpawnPosition.transform.position, Quaternion.identity, cardPrefab.transform);
        Debug.Log("newFrontCard생성");
        newCard.SetActive(true);
        newCard.GetComponent<FrontCardController>().FrontCardMoved += NewCardCreate;
    }
}