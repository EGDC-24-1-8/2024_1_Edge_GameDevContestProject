using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSpawner : MonoBehaviour
{
    public GameObject cardPrefab;  // 카드 프리팹을 연결합니다.
    public float spawnInterval = 1.0f;  // 카드가 생성되는 간격
    public float fallSpeed = 200.0f;  // 카드가 떨어지는 속도
    public float rotationSpeed = 100.0f;  // 카드가 회전하는 속도
    public RectTransform canvasRect;  // 캔버스 RectTransform을 연결합니다.

    void Start()
    {
        StartCoroutine(SpawnCards());
    }

    IEnumerator SpawnCards()
    {
        yield return new WaitForSeconds(spawnInterval);
        while (true)
        {
            SpawnCard();
            yield return new WaitForSeconds(5f);
        }
    }

    void SpawnCard()
    {
        // 카드가 생성될 위치를 캔버스 상단에서 결정합니다.
        float spawnX = Random.Range(0f, canvasRect.rect.width);  // 캔버스 너비 내에서 무작위 X 좌표
        Vector3 spawnPosition = new Vector3(spawnX, canvasRect.rect.height, 0);  // 캔버스 상단에서 생성
        

        GameObject card = Instantiate(cardPrefab, spawnPosition, Quaternion.identity, canvasRect);  // 카드 생성
        card.GetComponent<CardFalling>().Initialize(fallSpeed, rotationSpeed);  // 속도와 회전 설정
    }
}