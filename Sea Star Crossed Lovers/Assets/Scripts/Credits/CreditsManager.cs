using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Manages the content and movement of the credits based on the input CSV file
public class CreditsManager : MonoBehaviour
{
    // The CSV file inputted as credits.
    // Format: Row = Name, Col = Credits section, Row + Col = Role
    public TextAsset creditsText;

    // Index of the column of the CSV file that distinguishes the the credits of Gremlin Gardens and Sea Star Crossed Lovers
    public int minColIndex = 7;

    // Dictionary containing the map of the credits. Called using column (credits section), row (name/role)
    private Dictionary<string, Dictionary<string, string>> creditsMap;

    // Dictionary of all sections of the credits, indexed by header
    private Dictionary<string, CreditsSection> credits;

    [Header("UI Text")]
    // Distance set between the text boxes, both vertically and horizontally
    public float textBoxMargin;
    // The beginning position of the credits (below bottom of screen)
    public Vector3 startingPosition;
    // The template for the header text box
    public Canvas textBoxTemplate;
    
    // Parses the CSV file into the Dictionary creditsMap
    void Awake()
    {
        setTemplate();

        creditsMap = new Dictionary<string, Dictionary<string, string>>();
        parseCSV();
        
        credits = new Dictionary<string, CreditsSection>();
        writeCredits();
    }

    // Sets the initial position the text box templates
    private void setTemplate()
    {

    }

    // Parses the CSV file into a dictionary
    private void parseCSV()
    {
        string[] rows = creditsText.text.Split('\n');
        string[] colHeaders = rows[0].Split(',');
        // Initializes the columns of the dictionary
        for(int colIndex = minColIndex; colIndex < colHeaders.Length; colIndex++)
        {
            creditsMap.Add(colHeaders[colIndex], new Dictionary<string, string>());
        }

        // Initializes the rows of the dictionary and assigns values to each coordinate.
        for(int rowIndex = 1; rowIndex < rows.Length; rowIndex++)
        {
            // Name of the person being credited
            // Is the first value assigned, so begins as null
            string name = null;
            // Local variables facilitate running the loop
            bool currentlyQuoted = false;
            string currentValue = "";
            int colIndex = 0;
            foreach(char character in rows[rowIndex])
            {
                if(character == ',' && !currentlyQuoted)
                {
                    // Store currentValue
                    if(name == null)
                    {
                        name = currentValue;
                        //Debug.Log(name);
                    }
                    else if(colIndex >= minColIndex) // Check to only store information from the Sea Star Crossed Lovers credits
                    {
                        //Debug.Log($"Name: {name}, Header: {colHeaders[colIndex]}, Value: {currentValue}");
                        creditsMap[colHeaders[colIndex]].Add(name, currentValue);
                    }
                    currentValue = "";
                    colIndex++;
                }
                else if(character == '"')
                {
                    // Allows/prevents the current value from being stored depending on whether or not the character is quoted
                    // Quoted entries are stored as one value
                    currentlyQuoted = !currentlyQuoted;
                }
                else
                {
                    // If no above conditions were met, add the current character to the current value
                    currentValue += character;
                }
            }
            // Stores last value in dictionary
            // Debug.Log($"Name: {name}, Header: {colHeaders[colIndex]}, Value: {currentValue}");
            creditsMap[colHeaders[colIndex]].Add(name, currentValue);
        }
    }

    // Iterates through the creditsMap Dictionary and initiates 
    private void writeCredits()
    {
        foreach(KeyValuePair<string, Dictionary<string, string>> headerRolePair in creditsMap)
        {
            Canvas textCanvas = Instantiate(textBoxTemplate);
            textCanvas.gameObject.SetActive(true);
            Text[] TextBoxes = textCanvas.GetComponentsInChildren<Text>();
            // Check to make sure there are 3 text boxes in the template: header, people, and roles
            if(TextBoxes.Length != 3)
            {
                Debug.LogError("Incorrect number of text boxes in copy of template");
            }
            
            // Sets the 3 text boxes to their default values
            TextBoxes[0].text = headerRolePair.Key;
            TextBoxes[1].text = "";
            TextBoxes[2].text = "";
            // Creates a new CreditsSection object to manage the position of the 3 text boxes
            CreditsSection section = new CreditsSection(TextBoxes);
            credits.Add(headerRolePair.Key, section);

            // Adds each line of credits to the credits
            foreach(KeyValuePair<string, string> personRolePair in headerRolePair.Value)
            {
                // Only credits someone if they contributed something
                if(personRolePair.Value.Length > 1)
                {
                    section.addCredit(personRolePair.Key, personRolePair.Value);
                }
            }

            // Disables the content size fitter for each text box after text is added
            foreach(Text textBox in TextBoxes)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(textBox.rectTransform);
                textBox.GetComponent<ContentSizeFitter>().enabled = false;
            }
        }
    }

    void Start()
    {
        Vector3 position = new Vector3(startingPosition.x, startingPosition.y, startingPosition.z);
        foreach(KeyValuePair<string, CreditsSection> creditsSection in credits)
        {
            //Debug.Log(creditsSection.Key);
            CreditsSection section = creditsSection.Value;
            section.setPosition(position, textBoxMargin);
            float change = section.getHeight();
            //Debug.Log(change);
            position.y -= change;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //
    }
}
