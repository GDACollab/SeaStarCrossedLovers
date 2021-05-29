>>> TextboxEnter(Default);

>>> !CharEnter(Noelani, left); CharEnter(Ocean, right);

Ocean: Great work Noelani, you’re almost to the point where I had to stop when I tried to climb. #ocean_happy_MeganChau 
Ocean: Unfortunately, I have to return to the island now. I should warn you: as you continue to build this tower, you're going to encounter more resistance than just waves. #ocean_longing_MeganChau

Ocean: The winds were another obstacle I encountered when I attempted to reach my Moon. It’s about to get more difficult, but just remember why you’re doing this. 

Ocean: Good Luck! I believe in you! #ocean_happy_MeganChau 
>>> !CharExit(Ocean)

Noelani: Why am I doing this?... I’m not totally sure. I want to see Astra, but it’s also more than that… It just feels like something I need to do. It’s important to me… I guess I sound like Astra right now: #Noelani_pensive

>>> TextboxExit();

>>> FadeBlack(1); Wait(1); !CharExit(Noelani); MoveBackground(0, 600, 0); FadeBlack(0.2); 
// [fade to black for one second]
// [island/ ground background]

>>> TextboxEnter(Default);

>>> !CharEnter(Noelani, left); CharEnter(Astra, right);

Noelani: Hey, Astra… I’ve been wondering… #Noelani_pensive
Noelani:Why do you even want to be a Bright star?
Astra: I’m not totally sure. It just feels like something I need to do. It’s important to me… I want to be able to live up to the best I can be. Even if the work is hard, it’s fulfilling when I make progress. #AstraQuestioning_GriffinConway

Noelani: Is such constant hard work really the best way to go about, though? It’s good to be hard working, but if you push yourself too far then you really don't get anywhere.
Astra:... #AstraSad_GriffinConway
Noelani: And it sounds like being a Bright star just means you need to work even harder? What's the reward here? Work really hard and then double the work? That sounds so exhausting 
Noelani: I might not be the best person to ask about hard work though. I can only find the focus to work hard when I’m really into something. The other nymphs even call me Happy Go Lucky Noelani. 
Noelani: But, enough about what I think. Besides working to be bright, what other things do you love to do? Don’t you have any other passions?#Noelani_happy
Astra: … Umm… Define “Love”?#AstraQuestioning_GriffinConway
Noelani: Really?#Noelani_pensive
Astra: … yes.#AstraSurprised_GriffinConway
Noelani: Okay, uhh, something that sparks joy--you could do it for hours and never get tired of it.#Noelani_determined
Astra: Hmm… I love… learning new things? Moon’s so knowledgeable about things down on earth… and I like listening to her talk about stuff?#AstraQuestioning_GriffinConway
Noelani: Great! Kinda studious, but still, sounds cool! Anything else? #Noelani_happy
Astra: … I love hanging out with you.#AstraHappy_GriffinConway
Noelani: H-huh? What do you mean?#Noelani_pensive
Noelani: AAAAAAA I hope she can’t see me blushing #text_italics
Astra: You're really easy to talk to, and sweet to people you don’t even know. You tell fun jokes, and even explain them to me when I don't get them. You’re really passionate about stuff, but also really chill too.#AstraSurprised_GriffinConway
Astra: you're an amazing person, Noelani. #AstraHappy_GriffinConway
>>> TextboxExit();
>>> FadeBlack(1); CharExit(Astra); Wait(1); MoveBackground(0, 100, 0); FadeBlack(0);
>>> TextboxEnter(Default);
Noelani: *sigh* It’s time to be the person Astra believes I am. #Noelani_determined