LIST Time = (dusk), (noon), (midnight)
~ Time = noon

Narrator: Here come CharacterA and CharacterB

>>> Enter.CharacterA.CharacterB

CharacterA: "Nope I'm outta here."

>>> Exit.CharacterA, Enter.CharacterA

CharacterA: Just kidding.
CharacterB: ...

-> intro

=== intro ===
It is {Time} now.
CharacterA: Hi, I'm CharacterA. Blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah. #default

CharacterB: Hi, I'm CharacterB. #default
* [Reach to shake hands.] #happy
~ Time = dusk
Narrator: CharacterB shakes your hand.
CharacterB: Nice to meet you. #happy
* "Nice to meet you CharacterB." #happy
~ Time = midnight
CharacterB: Nice to meet you too. #surprised
- CharacterA: Cool, bye. #surprised
Narrator: It is {Time} now.

>>> Exit.CharacterA

-> END