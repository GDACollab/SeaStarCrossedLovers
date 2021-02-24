using System.Collections;
using System.Collections.Generic;

public interface ICmdCall
{
    IEnumerator Command(List<string> args);

    void Construct(VN_Manager manager, ICmdFrame cmdFrame, ICmdPart cmdPart);
}
