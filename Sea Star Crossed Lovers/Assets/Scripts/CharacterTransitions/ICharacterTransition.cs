using System.Collections;

public interface ICharacterTransition
{
    IEnumerator Co_EnterScreen();

    IEnumerator Co_ExitScreen();

    void Construct(VN_Character character);
}