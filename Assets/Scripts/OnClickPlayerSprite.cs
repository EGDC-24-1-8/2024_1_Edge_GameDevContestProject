using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnClickPlayerSprite : MonoBehaviour
{
    bool isOver = false;
    [SerializeField] private int playerIdx = 0;
    void Update()
    {
        if (isOver && Input.GetMouseButtonDown(0))
        {
            GameManager.Instance.OnMouseClickPlayer(playerIdx);
        }
    }

    void OnMouseOver()
    {
        GameManager.Instance.ChangeHoverCursor();
        isOver = true;
    }

    void OnMouseExit()
    {
        GameManager.Instance.ChangeNormalCursor();
        isOver = false;
    }
}
