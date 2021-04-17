using UnityEngine;
using UnityEngine.UI;
using Ink.Runtime;

public class VN_UIFactory : MonoBehaviour
{
	/* TODO ? Find a way to make TextCanavs and other references
	 * be public to only VN_Manager.
	*/

	// Textbox canvas objects
	public Canvas TextCanvas = null;
	public Canvas ButtonCanvas = null;
	public Canvas NameCanvas = null;
	public Canvas TextboxCanvas = null;

	// Textbox text objects
	// Text object that displays text content
	[HideInInspector] public Text contentTextObj = null;
	// Text object that displays speaker name
	[HideInInspector] public Text nameTextObj = null;

	// UI Prefabs
	[SerializeField]
	[Tooltip("Used for VN text context")]
	private Text textPrefab = null;
	[SerializeField]
	[Tooltip("Used for VN buttons")]
	private Button buttonPrefab = null;

	private VN_Manager _manager;

	public void Construct(VN_Manager VN_Manager)
    {
		_manager = VN_Manager;
	}

	/**
	 * Creates a Text object of the content of a story
	 * 
	 * @param text: the content of the story
	 * @return: the Text object of the content
	 */
	public Text CreateContentView(string text)
	{
		Text contentText = Instantiate(textPrefab);
		contentText.text = text;
		contentText.transform.SetParent(TextCanvas.transform, false);
		return contentText;
	}

	/**
	 * Creates a Text object of the name of a character
	 * 
	 * @param name: the name of the character
	 * @return: the Text object of the the character's name
	 */
	public Text CreateNameTextView(string name)
	{
		Text nameText = Instantiate(textPrefab);
		nameText.transform.SetParent(NameCanvas.transform, false);
		// Update NameText
		if (name == "Narrator")
		{
			nameText.text = "";
			contentTextObj.fontStyle = FontStyle.Italic;
		}
		else
		{
			nameText.text = name;
		}

		return nameText;
	}

	/**
	 * Creates a buttons for a choice in a story
	 * 
	 * @param text: the text and choice for the button
	 * @return: the Button object for the choice
	 */
	public Button CreateChoiceView(string text)
	{
		// Creates the button from a prefab
		Button choice = Instantiate(buttonPrefab) as Button;
		choice.transform.SetParent(ButtonCanvas.transform, false);

		// Gets the text from the button prefab
		Text choiceText = choice.GetComponentInChildren<Text>();
		choiceText.text = text;

		return choice;
	}

	// Creates a button to restart the story and resets the story if the button is clicked
	public void CreateRestartStoryButton()
	{
		//Very quick hack.
		//Hopefully you'll replace this with actual data, but here's a quick hack to get you back to the start screen:
		Button choice = CreateChoiceView("End story");
		choice.onClick.AddListener(delegate
		{
			_manager.activeLoader.QuickFadeOutLoad("Wave&BlockPlacement");
		});
	}

	// Creates a button to start the story
	public void CreateStartStoryButton()
	{
		Button choice = CreateChoiceView("Start story");
		choice.onClick.AddListener(delegate
		{
			_manager.StartStory();
		});
	}

	// Creates all buttons correlating to a choice
	public void CreateAllChoiceButtons()
	{
		// Display all the choices, if there are any!
		if (_manager.story.currentChoices.Count > 0)
		{
			for (int i = 0; i < _manager.story.currentChoices.Count; i++)
			{
				Choice choice = _manager.story.currentChoices[i];
				Button button = CreateChoiceView(choice.text.Trim());
				// Tell the button what to do when we press it
				button.onClick.AddListener(delegate
				{
					_manager.OnClickChoiceButton(choice);
				});
			}
		}
		// If there are no choices on this line of text
		// And add a button to continue
		else if (_manager.story.canContinue)
		{
			Button button = CreateChoiceView("Continue");
			button.onClick.AddListener(delegate {
				_manager.RefreshView();
			});
		}
		// If there is no more content, prompt to restart
		else
		{
			CreateRestartStoryButton();
		}
	}
}
