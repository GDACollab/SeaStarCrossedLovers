using System.Collections;
using System.Collections.Generic;

public interface ICmdCall
{
    void Construct(VN_Manager manager, ICmdFrame cmdFrame, ICmdPart cmdPart);

    IEnumerator Command(List<string> args, bool isImmediate);
}
