using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using System.IO;

public class WalkthroughParser : MonoBehaviour
{
    public static string walkthroughName = "None";
    public GameObject[] prefabs;

    void Awake()
    {
        Debug.Log("I'm Awake");
        string walkthroughFolder = Path.Combine(Application.dataPath, "Walkthroughs");
        string filePath = Path.Combine(walkthroughFolder, walkthroughName);
        if (File.Exists(filePath))
        {
            // Read the file content
            string fileContent = File.ReadAllText(filePath);

            // Parse the content for commands
            ParseCommands(fileContent);
        }
    }

    private void ParseCommands(string fileContent)
    {
        // Use regular expression to match placeCard commands, e.g., %placeCard(6)%
        string pattern = @"%placeCard\((\d+)\)%"; // Regex pattern to capture card number
        MatchCollection matches = Regex.Matches(fileContent, pattern);

        foreach (Match match in matches)
        {
            // Extract card number from the match
            string objectID = match.Groups[1].Value;

            // Call the method to place the card on the table
            PlaceCardOnTable(objectID);
        }
    }

    private void PlaceCardOnTable(string cardId)
    {
        foreach (GameObject prefab in prefabs)
        {
            Match match = Regex.Match(prefab.name, @"\((\d+)\)");
            if (match.Groups[1].Value == cardId)
            {
                Debug.Log("Placing card:" + match.Groups[1].Value);
                Vector3 cardPos = new Vector3(0, 0, 0);
                GameObject card = Instantiate(prefab, cardPos, Quaternion.Euler(0,-90,0));
            }
        }
    }


}
