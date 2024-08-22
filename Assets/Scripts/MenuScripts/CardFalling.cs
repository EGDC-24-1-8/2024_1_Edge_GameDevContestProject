using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardFalling : MonoBehaviour
{
    private float fallSpeed;
    private float rotationSpeed;

    public void Initialize(float fallSpeed, float rotationSpeed)
    {
        this.fallSpeed = fallSpeed;
        this.rotationSpeed = rotationSpeed;
    }

    void Update()
    {
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime, Space.World);
        
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        
        if (transform.position.y < -Screen.height)
        {
            Destroy(gameObject);
        }
    }
}
