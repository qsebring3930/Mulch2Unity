using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardInteraction : MonoBehaviour
{
    public static CardInteraction currentlyHeldCard;

    public bool isFlipped = false;
    public float lastClick = 0f;
    public float doubleClick = 0.25f;

    public float flipDuration = 0.25f;
    public float floatDuration = 0.25f;

    private Vector3 mousePosition;
    private Rigidbody rb;

    // Set the minimum allowed y position (tabletop level)
    public float minYPosition = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component
    }

    private Vector3 GetMousePos()
    {
        // Get the screen-space position of the object
        return Camera.main.WorldToScreenPoint(transform.position);
    }

    void OnMouseOver()
    {
        if (currentlyHeldCard == null)
        {
            HandleKeys();
        }
    }

    void OnMouseDown()
    {
        float timeSinceLastClick = Time.time - lastClick;

        // Check for double-click
        if (timeSinceLastClick <= doubleClick)
        {
            StartCoroutine(FlipCard());
            isFlipped = !isFlipped;
        }
        else
        {
            mousePosition = Input.mousePosition - GetMousePos(); // Store the offset for dragging
            if (rb != null)
            {
                rb.isKinematic = true; // Disable physics while holding
                rb.detectCollisions = false;
            }
        }

        lastClick = Time.time; // Update last click time
    }

    void OnMouseDrag()
    {
        Debug.Log("card is being held: " + gameObject.name);
        // Follow the mouse position
        currentlyHeldCard = this;
        Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition - mousePosition);

        // Clamp the y position to prevent the card from going below the tabletop
        newPosition.y = Mathf.Max(newPosition.y, minYPosition);

        transform.position = newPosition;

        HandleKeys();
    }

    void OnMouseUp()
    {
        if (rb != null)
        {
            rb.isKinematic = false; // Re-enable physics when releasing
            rb.detectCollisions = true;
        }
        currentlyHeldCard = null;
    }

    public void HandleKeys()
    {
        if (Input.GetKeyDown(KeyCode.Q)) // Rotate left by 45 degrees
        {
            RotateCard(45f);
        }
        else if (Input.GetKeyDown(KeyCode.E)) // Rotate right by 45 degrees
        {
            RotateCard(-45f);
            
        } else if (Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(FlipCard());
            isFlipped = !isFlipped;
        }
    }

    public void RotateCard(float angle)
    {
        if (isFlipped)
        {
            angle = -angle;
        }
        transform.Rotate(Vector3.up, angle);
    }

    public IEnumerator FlipCard()
    {
        rb.isKinematic = true;
        rb.detectCollisions = false;

        Vector3 currentPos = transform.position; // Store the current position of the card
        Vector3 finPos = currentPos + new Vector3(0, 1.5f, 0);

        Quaternion initRot = transform.rotation;
        Quaternion finRot = transform.rotation * Quaternion.Euler(0f, 0f, 180f);
        float elapsedTime = 0f;

        // Float up
        while (elapsedTime < floatDuration)
        {
            transform.position = Vector3.Lerp(currentPos, finPos, elapsedTime / floatDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = finPos;

        elapsedTime = 0f;

        // Flip the card
        while (elapsedTime < flipDuration)
        {
            transform.rotation = Quaternion.Lerp(initRot, finRot, elapsedTime / flipDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = finRot;
        if (currentlyHeldCard != this)
        {
            rb.isKinematic = false;
            rb.detectCollisions = true;
        }
    }
}