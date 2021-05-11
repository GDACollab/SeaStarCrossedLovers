VAR testNumber = 1
VAR name = "Default"

VAR player_character = "bob"

// >>> SetUnityVar(testInt, {testNumber});

// >>> UpdateInkVar(name, testString);

// >>> !CharEnter(Noelani); CharEnter(Ocean); Wait(1); !CharExit(Noelani); CharExit(Ocean);

// >>> CharEnter(Noelani); CharEnter(Ocean); Wait(1); !CharExit(Noelani); CharExit(Ocean);

>>> TextboxEnter(Default);

Narrator: I am {name}. Test text text text text text text text text text text text text text text text text text text text text text text text text text text text text text text text 

>>> !CharEnter(Noelani);

Noelani: Hi. I'm Noelani.

>>> TextboxExit(); CharEnter(Ocean);

>>> TextboxEnter(Default, StarsDecorationSprite);

Ocean: Hi. I'm Ocean.

>>> !CharExit(Ocean) 

Noelani: Wait don't leave Ocean.

>>> Wait(2);

Noelani: Guess I'll follow.

>>> !CharExit(Noelani)