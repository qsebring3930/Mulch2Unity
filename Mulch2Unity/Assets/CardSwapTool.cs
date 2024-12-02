using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSwapTool : MonoBehaviour
{
    private CardInteraction firstCard;
    private LineRenderer lineRenderer;

    private bool isToolActive = false;
    private Vector3 mouseWorldPosition;

    void Start()
    {
        // Add a LineRenderer to visualize the rope
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        lineRenderer.material.color = Color.blue;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.enabled = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // Right-click pressed
        {
            ActivateTool();
        }

        if (isToolActive)
        {
            UpdateRope();
        }

        if (Input.GetMouseButtonUp(1)) // Right-click released
        {
            DeactivateTool();
        }
    }

    private void ActivateTool()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            firstCard = hit.collider.GetComponent<CardInteraction>();
            if (firstCard != null)
            {
                isToolActive = true;
                lineRenderer.enabled = true;
                lineRenderer.SetPosition(0, firstCard.transform.position); // Start of the rope
            }
        }
    }

    private void UpdateRope()
    {
        // Update the mouse's world position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            mouseWorldPosition = hit.point;
        }
        else
        {
            mouseWorldPosition = ray.origin + ray.direction * 10f; // Fallback to a point in the distance
        }

        // Update the LineRenderer
        lineRenderer.SetPosition(1, mouseWorldPosition); // End of the rope
    }

    private void DeactivateTool()
    {
        if (!isToolActive) return;

        lineRenderer.enabled = false;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            CardInteraction secondCard = hit.collider.GetComponent<CardInteraction>();
            if (secondCard != null && secondCard != firstCard)
            {
                SwapCards(firstCard, secondCard);
            }
        }

        isToolActive = false;
        firstCard = null;
    }

    private void SwapCards(CardInteraction card1, CardInteraction card2)
    {
        Vector3 tempPosition = card1.transform.position;
        card1.transform.position = card2.transform.position;
        card2.transform.position = tempPosition;
    }
}
