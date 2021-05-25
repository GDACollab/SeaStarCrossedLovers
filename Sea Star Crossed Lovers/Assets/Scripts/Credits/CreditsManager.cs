using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    // Time it will take for the credits to scroll all the way through, in seconds
    public float time;
    
    // Speed the credits will move at. I.e. the distance it will move up each frame
    private float creditsSpeed = 1;

    [Header("UI Text")]
    // Distance set between the text boxes, both vertically and horizontally
    public float textBoxMargin;
    
    public float startingY = 0;
    
    // The beginning position of the credits (X is constant at the center of the screen)
    private Vector2 startingPosition;
    
    // Text canvas for the header
    public Canvas headerCanvas;

    // The template for the header text box
    public Canvas textBoxTemplate;

    int frameCount = 0;
    int secondCount = 0;
    
    // Initializes the starting position, parses the CSV file into the Dictionary creditsMap, and writes the credits into text boxes based on the template
    void Awake()
    {
        startingPosition = new Vector2(Screen.width/2, startingY);
        //startingPosition = new Vector2(Screen.width/2, Screen.height);

        setDefaultText();

        creditsMap = new Dictionary<string, Dictionary<string, string>>();
        parseCSV();
        
        credits = new Dictionary<string, CreditsSection>();
        writeCredits();
    }

    // Sets the initial position the text box templates and the header text
    private void setDefaultText()
    {
        TextMeshProUGUI[] headerText = headerCanvas.GetComponentsInChildren<TextMeshProUGUI>();
        //Debug.Log(startingPosition.x);
        headerText[0].transform.position = new Vector2(startingPosition.x, startingPosition.y);
        headerText[1].transform.position = new Vector2(startingPosition.x, startingPosition.y - headerText[0].rectTransform.rect.height);

        // Sets a temporary creditsSection object to set up the initial position
        CreditsSection defaultCredits = new CreditsSection(textBoxTemplate.GetComponentsInChildren<TextMeshProUGUI>());
        defaultCredits.setPosition(startingPosition, textBoxMargin);
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

    // Iterates through the creditsMap Dictionary and initiates the new text boxes based on the template
    private void writeCredits()
    {
        foreach(KeyValuePair<string, Dictionary<string, string>> headerRolePair in creditsMap)
        {
            // Clones the template canvas, makes the clone visible, and sets it as the child of the gameobject this is attatched to
            Canvas textCanvas = Instantiate(textBoxTemplate);
            textCanvas.gameObject.SetActive(true);
            textCanvas.transform.parent = gameObject.transform;

            TextMeshProUGUI[] TextBoxes = textCanvas.GetComponentsInChildren<TextMeshProUGUI>();
            // Check to make sure there are 3 text boxes in the template: header, people, and roles
            if(TextBoxes.Length != 3)
            {
                Debug.LogError("Incorrect number of text boxes in copy of template");
            }
            
            // Sets the 3 text boxes to their default values
            TextBoxes[0].text = headerRolePair.Key.Split(' ')[0];
            TextBoxes[1].text = "";
            TextBoxes[2].text = "";
            // Creates a new CreditsSection object to manage the position of the 3 text boxes
            CreditsSection section = new CreditsSection(TextBoxes);
            credits.Add(headerRolePair.Key, section);

            // Adds each line of credits to the credits
            foreach(KeyValuePair<string, string> personRolePair in headerRolePair.Value)
            {
                // Only credits someone if they contributed something
                // Set to 1 as writing credits have a carriage return character
                if(personRolePair.Value.Length > 1)
                {
                    string[] roles = personRolePair.Value.Split(',');
                    for(int i = 0; i < roles.Length; i++)
                    {
                        // Removes spaces from the beginning and end
                        roles[i] = roles[i].Trim();
                        if(i == 0)
                        {
                            section.addCredit(personRolePair.Key, roles[i]);
                        }
                        else
                        {
                            section.addCredit("", roles[i]);
                        }
                    }
                }
            }

            // Disables the content size fitter for each text box after text is added
            foreach(TextMeshProUGUI textBox in TextBoxes)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(textBox.rectTransform);
                textBox.GetComponent<ContentSizeFitter>().enabled = false;
            }
        }
    }

    // Sets the initial position of all credits text boxes and calculates the speed the credits will travel at
    void Start()
    {
        float startingYPostion = startingPosition.y - (textBoxMargin * 1.5f);
        foreach(TextMeshProUGUI textBox in headerCanvas.GetComponentsInChildren<TextMeshProUGUI>())
        {
            startingYPostion -= textBox.rectTransform.rect.height;
        }
        startingYPostion -= textBoxMargin;

        Vector2 position = new Vector2(startingPosition.x, startingYPostion);
        foreach(KeyValuePair<string, CreditsSection> creditsSection in credits)
        {
            //Debug.Log(creditsSection.Key);
            CreditsSection section = creditsSection.Value;
            section.setPosition(position, textBoxMargin);
            float change = section.getHeight();
            //Debug.Log(change);
            position.y -= change + textBoxMargin;
        }

        creditsSpeed = (getCreditsHeight() + Screen.height) / (time * 120);
    }

    // Calculates the total height of the credits
    private float getCreditsHeight()
    {
        float height = 0;
        
        // Adds height of header
        foreach(TextMeshProUGUI textBox in headerCanvas.GetComponentsInChildren<TextMeshProUGUI>())
        {
            height += textBox.rectTransform.rect.height;
        }

        // Adds height of sections
        foreach(KeyValuePair<string, CreditsSection> section in credits)
        {
            height += textBoxMargin;
            height += section.Value.getHeight();
        }

        return height;
    }

    // Update is called once per frame
    // Scrolls the text up by the designated amount
    void Update()
    {
        if(headerCanvas.GetComponentsInChildren<TextMeshProUGUI>()[0].transform.position.y > getCreditsHeight() + Screen.height)
        {
            Debug.Log($"Time: {Time.fixedTime}\tFramerate: {Time.frameCount/Time.fixedTime}");
            Debug.LogError("End");
            return;
        }
        
        // Translates the header up
        foreach(TextMeshProUGUI headerTextBox in headerCanvas.GetComponentsInChildren<TextMeshProUGUI>())
        {
            headerTextBox.transform.position = new Vector2(headerTextBox.transform.position.x, headerTextBox.transform.position.y + creditsSpeed);
        }

        // Translates the credit sections up
        foreach(KeyValuePair<string, CreditsSection> section in credits)
        {
            section.Value.translateVertical(creditsSpeed);
        }
    }
}
