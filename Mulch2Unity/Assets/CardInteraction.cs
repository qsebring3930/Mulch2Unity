using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardInteraction : MonoBehaviour
{
    public bool isFlipped = false;
    public float lastClick = 0f;
    public float doubleClick = 0.25f;

    public float flipDuration = 0.25f;
    public float floatDuration = 0.25f;

    private Camera mainCamera;
    private bool isBeingHeld = false;
    private bool mouseHasMoved = false; // Track if the mouse has moved
    private Rigidbody rb;

    private Vector3 initialPosition; // Store the initial position of the card

    void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component
        initialPosition = transform.position; // Store the initial position
    }

    void OnMouseDown()
    {
        float timeSinceLastClick = Time.time - lastClick;

        // Check for double-click
        if (timeSinceLastClick <= doubleClick)
        {
            isFlipped = !isFlipped;

            if (isFlipped)
            {
                StartCoroutine(FlipCard(Vector3.forward, 0));
            }
            else
            {
                StartCoroutine(FlipCard(Vector3.forward, 180));
            }
        }
        else
        {
            isBeingHeld = true; // Set the flag to indicate the card is being held
            mouseHasMoved = false; // Reset the mouse movement flag
            if (rb != null)
            {
                rb.isKinematic = true; // Disable physics while holding
            }
        }

        lastClick = Time.time; // Update last click time
    }

    void OnMouseUp()
    {
        if (isBeingHeld)
        {
            isBeingHeld = false; // Reset the holding state
            if (rb != null)
            {
                rb.isKinematic = false; // Re-enable physics when dropping
            }
        }
    }

    void Update()
    {
        if (isBeingHeld)
        {
            // Check if the mouse has moved
            if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
            {
                mouseHasMoved = true; // Set flag to true when the mouse moves
                FollowMouse(); // Follow the mouse
            }
        }
    }

    private void FollowMouse()
    {
        Vector3 currentPos = transform.position;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, currentPos + Vector3.up); // Set the plane slightly above the initial position
        float hitDistance;

        if (plane.Raycast(ray, out hitDistance))
        {
            Vector3 hitPoint = ray.GetPoint(hitDistance);
            hitPoint.y = currentPos.y; // Keep the Y position of the card unchanged
            transform.position = hitPoint; // Update the card's position to the mouse position
        }
    }

    public IEnumerator FlipCard(Vector3 axis, float angle)
    {
        Vector3 currentPos = transform.position; // Store the current position of the card
        Vector3 finPos = currentPos + new Vector3(0, 1.5f, 0);

        Quaternion initRot = transform.rotation;
        Quaternion finRot = Quaternion.Euler(axis * angle);
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
    }
}