using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public GameObject[] cardPrefabs;
    public List<GameObject> deck = new List<GameObject>();
    public GameObject walkthroughParser;
    public GameObject[] numberCards;

    // Start is called before the first frame update
    void Start()
    {
        if (WalkthroughParser.walkthroughName == "None") {
            CreateDeck();
            ShuffleDeck();
            if (numberCards != null)
            {
            foreach (GameObject numbercard in numberCards)
                {
                    numbercard.SetActive(true);
                }
            }
        }
    }

    public void CreateDeck()
    {
        for (int i = 0; i < cardPrefabs.Length; i++)
        {
            Vector3 cardPos = new Vector3(transform.position.x, 0.025f + i * 0.025f, transform.position.z);
            Quaternion cardRot = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 180);
            GameObject card = Instantiate(cardPrefabs[i], cardPos, cardRot);
            Rigidbody rb = card.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }

            card.name = "Card (" + i + ")";
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
        for (int i = deck.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            Vector3 tempPosition = deck[i].transform.position;
            deck[i].transform.position = deck[randomIndex].transform.position;
            deck[randomIndex].transform.position = tempPosition;

            Rigidbody rb = deck[i].GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
            }
        }
    }
}