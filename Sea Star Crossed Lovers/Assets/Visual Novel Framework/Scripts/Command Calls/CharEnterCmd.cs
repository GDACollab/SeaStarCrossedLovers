using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharEnterCmd : MonoBehaviour, ICommandCall
{
	public IEnumerator Command(List<string> args)
	{
        CharacterData data = VN_Util.FindCharacterData(args[0]);

        string screenPosition = args[1];
        if(screenPosition == "left")
        {
            data.scenePosition = CharacterData.ScenePosition.left;
        }
        else if (screenPosition == "right")
        {
            data.scenePosition = CharacterData.ScenePosition.right;
        }
        else
        {
            Debug.LogError("screenPosition error: " + screenPosition);
        }

        VN_Character charObj = VN_Util.FindEmptyCharObj(data);

        charObj.SetData(data);
        charObj.ChangeSprite(data.defaultSprite);

        yield return StartCoroutine(charObj.data.transition.Co_EnterScreen(charObj, charObj));
    }
}
