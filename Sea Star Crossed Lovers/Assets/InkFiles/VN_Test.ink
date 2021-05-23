VAR testNumber = 1
VAR name = "Default"

// >>> SetUnityVar(testInt, {testNumber});

// >>> UpdateInkVar(name, testString);

// >>> !CharEnter(Noelani); CharEnter(Ocean); Wait(1); !CharExit(Noelani); CharExit(Ocean);

// >>> CharEnter(Noelani); CharEnter(Ocean); Wait(1); !CharExit(Noelani); CharExit(Ocean);

// >>> FadeBlack(1);
// >>> FadeBlack(0, 2);

// >>> MoveBackground(0, 100);
// >>> MoveBackground(0, 500, 3);

>>> TextboxEnter(Default);

Narrator: I am {name}. Test text text text text text text text text text text text text text text text text text text text text text text text text text text text text text text text 

>>> FadeBlack(1); MoveBackground(0, -500, 0); FadeBlack(0);

>>> PlayAudio(Concept_SFX_Block_Wood Impact); Wait(.5); !CharEnter(Noelani); 

Noelani: Hi. I'm Noelani.

>>> TextboxExit(); CharEnter(Ocean);

>>> TextboxEnter(Default, StarsDecorationSprite);

Ocean: Hi. I'm Ocean.

>>> !CharExit(Ocean) 

Noelani: Wait don't leave Ocean.

>>> Wait(2);

Noelani: Guess I'll follow.

>>> !CharExit(Noelani)