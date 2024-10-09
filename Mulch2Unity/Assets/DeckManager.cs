using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public GameObject[] cardPrefabs;
    public List<GameObject> deck = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        CreateDeck();
        ShuffleDeck();
    }

    public void CreateDeck()
    {
        for (int i = 0; i < cardPrefabs.Length; i++)
        {
            Vector3 cardPos = new Vector3(0, i * 0.02f, 0);
            GameObject card = Instantiate(cardPrefabs[i], cardPos, Quaternion.identity);
            Rigidbody rb = card.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }
            card.GetComponent<CardInteraction>().FlipCard(Vector3.forward, 180);

            card.name = "Acorn Card (" + i + ")";
            card.transform.SetParent(this.transform);

            deck.Add(card);
        }

        foreach (GameObject card in deck)
        {
            Rigidbody rb = card.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
            }
        }
    }

    public void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
                GameObject temp = deck[i];
                int randomIndex = Random.Range(0, deck.Count);
                deck[i] = deck[randomIndex];
                deck[randomIndex] = temp;
        }
    }
}
