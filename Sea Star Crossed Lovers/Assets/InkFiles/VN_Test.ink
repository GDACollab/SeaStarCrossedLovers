>>> !CharEnter(Noelani); CharEnter(Ocean); Wait(1); CharExit(Noelani); CharExit(Ocean)

>>> TextboxEnter();

Narrator: I am the narrator.

>>> !CharEnter(Noelani)

Noelani: Hi. I'm Noelani.

>>> TextboxExit(); CharEnter(Ocean);

>>> TextboxEnter();

Ocean: Hi. I'm Ocean

>>> !CharExit(Ocean) 

Noelani: Wait don't leave Ocean.

>>> !CharExit(Noelani); TextboxExit();