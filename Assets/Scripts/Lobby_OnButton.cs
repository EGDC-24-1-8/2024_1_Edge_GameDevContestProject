namespace SoundOfSlash
{
    // System
    using System;
    using System.Collections.Generic;

    // Unity
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;

    public class Lobby_OnButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image Button_Img;
        [SerializeField] private Sprite backGroundHoverImage = null;
        [SerializeField] private Sprite backGroundIdleImage = null;


        [SerializeField] private _03_Lobby lobby = null;

        public int btn_idx;

        public void Start()
        {
            Button_Img = GetComponent<Image>();
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            lobby.selectedLobbyIndex = btn_idx;
            lobby.Invalidate();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Button_Img.sprite = backGroundIdleImage;
            
            lobby.Invalidate();


            
        }
    }
}