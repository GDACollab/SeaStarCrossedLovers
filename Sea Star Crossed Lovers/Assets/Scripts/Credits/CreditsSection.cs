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

    // The position of the top of the 
    private Vector3 position;

    /**
     * Constructor
     * 
     * @Param textBoxes: an array of the 3 text objects used: header, roles, people
     * @Param postion: the initial position the text boxes are to be set to
     */
    public CreditsSection(Text[] textBoxes, Vector3 position)
    {
        this.header = textBoxes[0];
        this.roles = textBoxes[1];
        this.people = textBoxes[2];
        this.position = position;
    }

    /**
     * Adds a name-role combonation to the credits
     * 
     * @Param name: the person being credited
     * @Param role: what the person did
     * 
     * Precondidtion: the people and roles text boxes must have the same number of rows
     */
    public void addCredit(string name, string role)
    {
        people.text += name + '\n';
        roles.text += role + '\n';
    }

    /**
     * Sets the position of the 3 text boxes
     * 
     * @Param position: the position of the header text box
     * @Param margins: the distance between the text boxes, both vertical and horizontal
     */
    public void setPosition(Vector3 position, float margins)
    {
        //
    }

    // Returns the total height of the Text objects
    public float getHeight()
    {
        return 0;
    }
}
