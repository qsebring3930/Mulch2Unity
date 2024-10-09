using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardInteraction : MonoBehaviour
{
    public Rigidbody rb;

    public bool isFlipped = false;
    public float lastClick = 0f;
    public float doubleClick = 0.25f;

    public float flipDuration = 0.25f;
    public float floatDuration = .25f;
    public Vector3 cardPos;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cardPos = transform.position;
    }


    // Update is called once per frame
    void OnMouseDown()
    {
        float timeSinceLastClick = Time.time - lastClick;

        if (timeSinceLastClick <= doubleClick)
        {
            isFlipped = !isFlipped;

            if (isFlipped)
            {
                StartCoroutine(FlipCard(Vector3.forward, 180));
            }
            else
            {
                StartCoroutine(FlipCard(Vector3.forward, 0));
            }

        }

        lastClick = Time.time;

    }

    public IEnumerator FlipCard(Vector3 axis, float angle)
    {
        Vector3 finPos = cardPos + new Vector3(0, .5f, 0);


        Quaternion initRot = transform.rotation;
        Quaternion finRot = Quaternion.Euler(axis * angle);
        float elapsedTime = 0f;

        if (rb != null)
        {
            rb.isKinematic = true;
        }

        while (elapsedTime < floatDuration)
        {
            transform.position = Vector3.Lerp(cardPos, finPos, elapsedTime / floatDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = finPos;

        elapsedTime = 0f;

        while (elapsedTime < flipDuration)
        {
            transform.rotation = Quaternion.Lerp(initRot, finRot, elapsedTime / flipDuration);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.rotation = finRot;

        if (rb != null)
        {
            rb.isKinematic = false;
        }
    }
    
}
