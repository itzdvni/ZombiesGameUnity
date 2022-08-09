using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    private RectTransform reticle;
    public float restingSize;
    public float maxSize;
    public float speed;
    public float currentSize;
    private bool moving, jumping, sprinting, crouching, sliding, forward, crouchstarted;

    void Start()
    {
        reticle = GetComponent<RectTransform>();
        reticle.sizeDelta = new Vector2(100, 100);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) moving = true; else moving = false;
        sliding = GameObject.Find("Player").GetComponent<Movimiento>().sliding;
        jumping = GameObject.Find("Player").GetComponent<Movimiento>().grounded;
        sprinting = GameObject.Find("Player").GetComponent<Movimiento>().sprinting;
        forward = GameObject.Find("Player").GetComponent<Movimiento>().forward;
        crouchstarted = GameObject.Find("Player").GetComponent<Movimiento>().crouchstarted;
        crouching = GameObject.Find("Player").GetComponent<Movimiento>().crouching;
        if (!jumping && !crouching)
        {
            maxSize = 300;
            currentSize = Mathf.Lerp(currentSize, maxSize, Time.deltaTime * speed);
        }
        else if (sprinting && !crouching && forward)
        {
            maxSize = 250;
            currentSize = Mathf.Lerp(currentSize, maxSize, Time.deltaTime * speed);
        }
        else if (crouching && !sliding && moving)
        {
            maxSize = 120;
            currentSize = Mathf.Lerp(currentSize, maxSize, Time.deltaTime * speed);
        }
        else if (crouching && !sliding && !moving)
        {
            maxSize = 75;
            currentSize = Mathf.Lerp(currentSize, maxSize, Time.deltaTime * speed);
        }
        else if (sliding)
        {
            maxSize = 250;
            currentSize = Mathf.Lerp(currentSize, maxSize, Time.deltaTime * speed);
        }
        else if (moving && !crouching)
        {
            maxSize = 200;
            currentSize = Mathf.Lerp(currentSize, maxSize, Time.deltaTime * speed);
        }
        else
        {
            currentSize = Mathf.Lerp(currentSize, restingSize, Time.deltaTime * speed);
        }

        reticle.sizeDelta = new Vector2(currentSize, currentSize);

    }
}
