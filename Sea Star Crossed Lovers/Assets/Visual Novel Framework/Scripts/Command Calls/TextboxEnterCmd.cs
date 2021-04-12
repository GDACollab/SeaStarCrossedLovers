using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextboxEnterCmd : MonoBehaviour, ICommandCall
{
    public IEnumerator Command(List<string> args, bool isImmediate)
    {
        // TODO ? set /remove data similar to character transitions?
        //charObj.SetData(data);
        //charObj.ChangeSprite(data.defaultSprite);

        VN_Manager manager = VN_Util.manager;

        TextboxData data = VN_Util.FindTextboxData(args[0]);
        Sprite decor = VN_Util.FindTextboxCornerDecor(data, args[1]);

        manager.textboxManager.SetTextboxData(data, decor);

        if (isImmediate)
        {
            StartCoroutine(manager.textboxTransition.Co_EnterScreen(manager, this));
        }
        else
        {
            yield return StartCoroutine(manager.textboxTransition.Co_EnterScreen(manager, this));
        }
    }
}
