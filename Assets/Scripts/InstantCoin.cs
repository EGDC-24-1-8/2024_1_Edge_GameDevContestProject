using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantCoin : MonoBehaviour
{

    private void Awake()
    {
    }
    void destroy()
    {
        Destroy(this.gameObject);
    }
}
