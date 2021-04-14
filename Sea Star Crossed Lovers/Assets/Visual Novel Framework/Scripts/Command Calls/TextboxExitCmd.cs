using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextboxExitCmd : MonoBehaviour, ICommandCall
{
    public IEnumerator Command(List<string> args)
    {
        VN_Manager manager = VN_Util.manager;
        TextboxData data = manager.textboxManager.data;

        yield return StartCoroutine(data.textboxTransition.Co_ExitScreen(manager, manager));
        manager.activeState = VN_Manager.ActiveState.hidden;
        manager.textboxManager.SetDefaultData();
    }
}
