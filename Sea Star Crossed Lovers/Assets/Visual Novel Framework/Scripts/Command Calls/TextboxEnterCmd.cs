﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextboxEnterCmd : MonoBehaviour, ICommandCall
{
    public IEnumerator Command(List<string> args)
    {
        if (args.Count != 2)
        {
            Debug.LogError("Argument number error: " + this);
            yield break;
        }
        VN_Manager manager = VN_Util.manager;

        TextboxData data = VN_Util.FindTextboxData(args[0]);
        Sprite decor = VN_Util.FindTextboxCornerDecor(data, args[1]);

        yield return StartCoroutine(manager.textboxManager.ShowTextbox(data, decor));
    }
}
