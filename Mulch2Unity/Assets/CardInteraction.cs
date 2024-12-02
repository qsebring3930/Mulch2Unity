using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardInteraction : MonoBehaviour
{
    public static CardInteraction currentlyHeldCard;
    // Track if mouse is over any card
    public static bool isMouseOver;

    public bool isFlipped = false;
    public float lastClick = 0f;

    //Time threshold for double click
    public float doubleClick = 0.25f;

    public float flipDuration = 0.25f;
    public float floatDuration = 0.25f;

    private Vector3 mousePosition;
    private Rigidbody rb;

    // Bounds of the tabletop surface
    private float minYPosition = 0f;
    private float maxYPosition = 2f;
    private float minXPosition = -12.5f;
    private float maxXPosition = 12.5f;
    private float minZPosition = -30f;
    private float maxZPosition = 32f;

    private bool isHighlighted = false;

    // Track relative positions of highlighted cards
    private Dictionary<CardInteraction, Vector3> highlightedOffsets = new Dictionary<CardInteraction, Vector3>();


    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component
    }

    public void SetHighlight(bool highlight)
    {
        isHighlighted = highlight;
    }

    private Vector3 GetMousePos()
    {
        // Get the screen-space position of the object
        return Camera.main.WorldToScreenPoint(transform.position);
    }

    /*
    void Update()
    {
        if (!isMouseOver) // Makes sure Update only handles cases not handled by OnMouseOver
        {
            if (currentlyHeldCard == null) // Makes sure Update only handles cases not handled by OnMouseDrag
            {
                if (isHighlighted) // Cases where a user can freely input keys only includes highlighted cards
                {
                    HandleKeys();
                }
            }
        }
    }
    */

    void OnMouseOver()
    {
        isMouseOver = true;
        if (currentlyHeldCard == null)
        {
            HandleKeys();
        }
    }

    void OnMouseExit()
    {
        isMouseOver = false;
    }

    void OnMouseDown()
    {
        float timeSinceLastClick = Time.time - lastClick;

        // Check for double-click
        if (timeSinceLastClick <= doubleClick)
        {
            if (isHighlighted)
            {
                foreach (CardInteraction card in FindObjectsOfType<CardInteraction>())
                {
                    if (card.isHighlighted && card != this)
                    {
                        Debug.Log("I'm flippin this other mf");
                        StartCoroutine(card.FlipCard());
                        card.isFlipped = !card.isFlipped;
                    }
                }
            }
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
            // Store offsets for all highlighted cards
            if (isHighlighted)
            {
                highlightedOffsets.Clear();
                foreach (CardInteraction card in FindObjectsOfType<CardInteraction>())
                {
                    if (card.isHighlighted && card != this)
                    {
                        highlightedOffsets[card] = card.transform.position - transform.position;
                        card.rb.isKinematic = true;
                        card.rb.detectCollisions = false;
                    }
                }
            }
        }

        lastClick = Time.time; // Update last click time
    }

    void OnMouseDrag()
    {
        // Follow the mouse position
        currentlyHeldCard = this;
        Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition - mousePosition);

        // Clamp the y position to prevent the card from going below the tabletop
        newPosition.y = Mathf.Max(newPosition.y, minYPosition);
        newPosition.y = Mathf.Min(newPosition.y, maxYPosition);
        // Clamp the x position to prevent cards from leaving the tabletop
        newPosition.x = Mathf.Max(newPosition.x, minXPosition);
        newPosition.x = Mathf.Min(newPosition.x, maxXPosition);
        // Clamp the z position to prevent cards from leaving the tabletop
        newPosition.z = Mathf.Max(newPosition.z, minZPosition);
        newPosition.z = Mathf.Min(newPosition.z, maxZPosition);

        transform.position = newPosition;

        // Move highlighted cards relative to the dragged card
        foreach (var pair in highlightedOffsets)
        {
            CardInteraction card = pair.Key;
            Vector3 offset = pair.Value;
            card.transform.position = newPosition + offset;
        }

        HandleKeys();
    }

    void OnMouseUp()
    {
        if (rb != null)
        {
            rb.isKinematic = false; // Re-enable physics when releasing
            rb.detectCollisions = true;
        }

        if (isHighlighted)
        {
            foreach (CardInteraction card in FindObjectsOfType<CardInteraction>())
            {
                if (card.isHighlighted && card != this)
                {
                    card.rb.isKinematic = false;
                    card.rb.detectCollisions = true;
                }
            }
            highlightedOffsets.Clear();
        }


        currentlyHeldCard = null;
    }

    public void HandleKeys()
    {
        if (Input.GetKeyDown(KeyCode.Q)) // Rotate left by 45 degrees
        {
            if (isHighlighted)
            {
                RotateHighlightedCards(-45f);
            } else
            {
                RotateCard(45f);
            }
        }
        else if (Input.GetKeyDown(KeyCode.E)) // Rotate right by 45 degrees
        {
            if(isHighlighted)
            {
                RotateHighlightedCards(45f);
            } else
            {
                RotateCard(-45f);
            }

        } else if (Input.GetKeyDown(KeyCode.F))
        {
            if (isHighlighted)
            {
                foreach (CardInteraction card in FindObjectsOfType<CardInteraction>())
                {
                    if (card.isHighlighted && card != this)
                    {
                        StartCoroutine(card.FlipCard());
                        card.isFlipped = !card.isFlipped;
                    }
                }
            }
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

    public void RotateHighlightedCards(float angle)
    {
        // Find all highlighted cards
        List<CardInteraction> highlightedCards = new List<CardInteraction>();
        foreach (CardInteraction card in FindObjectsOfType<CardInteraction>())
        {
            if (card.isHighlighted)
            {
                highlightedCards.Add(card);
            }
        }

        if (highlightedCards.Count == 0) return;

        // Create a temporary parent object at the center of all highlighted cards
        Vector3 pivot = Vector3.zero;
        foreach (CardInteraction card in highlightedCards)
        {
            pivot += card.transform.position;
        }
        pivot /= highlightedCards.Count; // Average position

        GameObject tempParent = new GameObject("TempParent");
        tempParent.transform.position = pivot;

        // Parent all highlighted cards to the temporary parent
        foreach (CardInteraction card in highlightedCards)
        {
            card.transform.SetParent(tempParent.transform);
        }

        // Rotate the temporary parent
        tempParent.transform.Rotate(Vector3.up, angle);

        // Unparent the cards and destroy the temporary parent
        foreach (CardInteraction card in highlightedCards)
        {
            card.transform.SetParent(null);
        }
        Destroy(tempParent);

        // Update highlightedOffsets with new relative positions
        if (highlightedCards.Contains(this)) // Ensure the current card is included
        {
            highlightedOffsets.Clear();
            foreach (CardInteraction card in highlightedCards)
            {
                if (card != this)
                {
                    highlightedOffsets[card] = card.transform.position - transform.position;
                }
            }
        }
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