using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICmdFrame
{
    IEnumerator CmdFrame(List<string> args, ICmdPart part, bool isImmediate);

    void Construct(VN_Manager manager);
}
