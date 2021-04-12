using UnityEngine;
using UnityEngine.UI;
using Ink.Runtime;

public class VN_UIFactory : MonoBehaviour
{
	// UI Prefabs
	[SerializeField]
	[Tooltip("Used for VN name text")]
	private Text nameTextPrefab = null;
	[SerializeField]
	[Tooltip("Used for VN content text")]
	private Text contentTextPrefab = null;
	[SerializeField]
	[Tooltip("Used for VN choice buttons")]
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
		Text contentText = Instantiate(contentTextPrefab);
		contentText.text = text;
		contentText.transform.SetParent(_manager.TextCanvas.transform, false);
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
		Text nameText = Instantiate(nameTextPrefab);
		nameText.transform.SetParent(_manager.NameCanvas.transform, false);
		// Update NameText
		if (name == "Narrator")
		{
			nameText.text = "";
			_manager.contentTextObj.fontStyle = FontStyle.Italic;
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
		choice.transform.SetParent(_manager.ButtonCanvas.transform, false);

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
			_manager.activeLoader.QuickFadeOutLoad("TowerLevel1");
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
