using System.Collections;
using System.Collections.Generic;

public interface ICommandCall
{
    IEnumerator Command(List<string> args);

    void Construct(VN_Manager manager);
}
