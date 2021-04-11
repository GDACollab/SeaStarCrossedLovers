using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextboxExitCmd : MonoBehaviour, ICommandCall
{
    public IEnumerator Command(List<string> args, bool isImmediate)
    {
        // TODO ? set /remove data similar to character transitions?
        //charObj.SetData(data);
        //charObj.ChangeSprite(data.defaultSprite);

        VN_Manager manager = VN_Util.manager;
        yield return StartCoroutine(manager.textboxTransition.Co_ExitScreen(manager, manager));
    }
}
