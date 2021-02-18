using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICmdPart
{
    IEnumerator CmdPart(VN_Character charObj, CharacterData characterData);
}
