using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Handles the text management of 1 section of the credits
public class CreditsSection : Object
{
    // The name of this credits section
    private Text header;

    // What the people being credited did.
    private Text roles;

    // The people being credited
    private Text people;

    /**
     * Constructor
     * 
     * @Param textBoxes: an array of the 3 text objects used: header, roles, people
     * @Param postion: the initial position the text boxes are to be set to
     */
    public CreditsSection(Text[] textBoxes)
    {
        this.header = textBoxes[0];
        this.roles = textBoxes[1];
        this.people = textBoxes[2];
    }

    /**
     * Adds a name-role combonation to the credits
     * 
     * @Param name: the person being credited
     * @Param role: what the person did
     * 
     * Precondition: the people and roles text boxes must have the same number of rows
     * Precondition: the Content Size Fitter for both text boxes must be enabled
     */
    public void addCredit(string name, string role)
    {
        people.text += name + '\n';
        roles.text += role + '\n';
    }

    /**
     * Sets the position of the 3 text boxes
     * Also forces their transforms to update for more accurate positioning
     * 
     * @Param position: the top center of the header text box
     * @Param margins: the distance between the text boxes, both vertical and horizontal
     */
    public void setPosition(Vector3 position, float margins)
    {
        setTextPosition(header, new Vector3(position.x, position.y, position.z));
        setTextPosition(roles, new Vector3(position.x + (margins/2), position.y - header.rectTransform.rect.height - margins, position.z));
        setTextPosition(people, new Vector3(position.x - (margins/2), position.y - header.rectTransform.rect.height - margins, position.z));
    }

    /**
     * Sets the position of the provided text box
     * 
     * @Param text: the text box in question
     * @Param position: the position the text box will be set to
     */
    private void setTextPosition(Text text, Vector3 position)
    {
        text.transform.position = new Vector3(position.x, position.y, position.z);
    }

    // Returns the height of the 3 text boxes by subtracting the y position of the bottom from the y position of the top
    // Note: people and roles should be the lowest and identical, so this only gets the bottom of roles
    public float getHeight()
    {
        return header.transform.position.y - (roles.transform.position.y - roles.rectTransform.rect.height);
    }
}
