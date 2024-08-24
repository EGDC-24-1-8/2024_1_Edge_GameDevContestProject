using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TopCardController : MonoBehaviour
{
    [SerializeField] private Transform topCard;
    private bool isDragging = false;
    private Vector3 mouseOffset;

    public event Action<bool> TopCardMoved;

    private float topCardTopY;
    private float topCardBottomY;

    private Vector3 origin_position; //클릭하는순간 그 카드의 기본위치를 잡은다음, 내가 이제 그 위치에서 특정 값만큼 이동해야만 실행되게끔

    private void Start()
    {
        SetPosition();
    }

    private void SetPosition()
    {
        // frontCard의 BoxCollider2D에서 상단과 하단 Y 값을 가져옴
        BoxCollider2D collider = topCard.GetComponent<BoxCollider2D>();
        topCardTopY = collider.bounds.max.y;
        topCardBottomY = collider.bounds.min.y;
    }

    void OnMouseDown()
    {
        if (DialogManager.Instance.isDialogMiddlePriority)
            return;
        if (GameManager.Instance.gameState != GameManager.GameState.deal)
            return;
        if (!(GameManager.Instance.mousePointState == GameManager.MousePointState.normal))
            return;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        origin_position = topCard.position;
        Collider2D collider = topCard.GetComponent<Collider2D>();

        if (mousePos.y >= topCardBottomY && mousePos.y <= topCardTopY)
        {
            isDragging = true;
            
            // 마우스 위치와 오브젝트 위치 간의 오프셋 계산
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = Camera.main.WorldToScreenPoint(topCard.position).z;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            mouseOffset = topCard.position - worldPosition;
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
            mousePosition.z = Camera.main.WorldToScreenPoint(topCard.position).z;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            // 새로운 y 위치 계산 및 적용
            float newY = worldPosition.y + mouseOffset.y;

            if (newY < origin_position.y)
                newY = origin_position.y;
            // x, z 위치 고정, y 위치만 변경
            topCard.position = new Vector3(topCard.position.x, newY, topCard.position.z);
        }
    }

    void OnMouseUp()
    {
        if (isDragging)
        {
            if(Math.Abs(topCard.position.y - origin_position.y) > 0.7f)
            {
                isDragging = false;
                GameManager.Instance.NormalDeal();
                topCard.gameObject.SetActive(false); // frontCard를 비활성화
                DealingManager.Instance.TopSetImage();
                TopCardMoved?.Invoke(true); // 이벤트 호출
                Destroy(gameObject);
            }
            else
            {

                topCard.gameObject.transform.position = origin_position;
            }
          
        }
    }
}
