using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class UpdateInkVarCmd : MonoBehaviour, ICommandCall
{
    public IEnumerator Command(List<string> args)
    {
        Story Story = VN_Util.manager.Story;
        VN_SharedVariables sharedVariables = VN_Util.manager.sharedVariables;

        var newVal = sharedVariables.GetVariableValue(args[1]);
        Story.variablesState[args[0]] = newVal;
        yield break;
    }
}
