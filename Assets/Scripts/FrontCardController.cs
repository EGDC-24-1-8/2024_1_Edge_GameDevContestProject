using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FrontCardController : MonoBehaviour
{
    [SerializeField] private Transform frontCard;
    [SerializeField] private Sprite[] cardImage = null;
    [SerializeField] private GameObject secondCard;
    private bool isDragging = false;
    private Vector3 mouseOffset;

    public event Action<bool> FrontCardMoved;

    private float frontCardTopY;
    private float frontCardBottomY;

    private Vector3 origin_Position;
    //클릭하는순간 그 카드의 기본위치를 잡은다음, 내가 이제 그 위치에서 특정 값만큼 이동해야만 실행되게끔
    private void Start()
    {
        SetPosition();
        SetImage();
        secondCard.GetComponent<FrontCardCreate>().SetSecondCardImage();
    }

    private void SetPosition()
    {
        // frontCard의 BoxCollider2D에서 상단과 하단 Y 값을 가져옴
        BoxCollider2D collider = frontCard.GetComponent<BoxCollider2D>();
        frontCardTopY = collider.bounds.max.y;
        frontCardBottomY = collider.bounds.min.y;
    }

    private void SetImage()
    {
        frontCard.gameObject.GetComponent<SpriteRenderer>().sprite
            = cardImage[GameManager.Instance.CardDeck[0] % 13];
    }

    void OnMouseDown()
    {
        if (DialogSystem.Instance.isDialog)
            return;
        if (GameManager.Instance.gameState != GameManager.GameState.deal)
            return;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        origin_Position = frontCard.position;
        Collider2D collider = frontCard.GetComponent<Collider2D>();

        if (mousePos.y >= frontCardBottomY && mousePos.y <= frontCardTopY)
        {
            isDragging = true;
            
            // 마우스 위치와 오브젝트 위치 간의 오프셋 계산
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = Camera.main.WorldToScreenPoint(frontCard.position).z;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            mouseOffset = frontCard.position - worldPosition;
        }
        else
        {
            isDragging = false;
        }
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            // 마우스 위치를 월드 좌표로 변환
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = Camera.main.WorldToScreenPoint(frontCard.position).z;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            // 새로운 y 위치 계산 및 적용
            float newY = worldPosition.y + mouseOffset.y;

            // x, z 위치 고정, y 위치만 변경
            frontCard.position = new Vector3(frontCard.position.x, newY, frontCard.position.z);
        }
    }

    void OnMouseUp()
    {
        if (isDragging)
        {
            if(Math.Abs(frontCard.position.y - origin_Position.y) > 0.7f)
            {
                isDragging = false;
                frontCard.gameObject.SetActive(false); // frontCard를 비활성화
                FrontCardMoved?.Invoke(true); // 이벤트 호출
                GameManager.Instance.NormalDeal();
                SetImage();
                secondCard.GetComponent<FrontCardCreate>().SetSecondCardImage();
                Destroy(gameObject);
            }
            else
            {

                frontCard.gameObject.transform.position = origin_Position;
            }
          
        }
    }
}
