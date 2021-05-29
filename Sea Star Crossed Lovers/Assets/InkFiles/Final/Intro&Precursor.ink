>>> TextboxEnter(Default);
>>> CharEnter(Noelani, left);

Noelani: I have always loved swimming in the sea at night… the stars are reflected in the water when the waves are calm and the sky is clear.#text_italics

Noelani: I’ve always thought that stars were beautiful up in the sky, but... #text_italics

// [meteor sound effect, followed by splash sound effect]
>>> TextboxExit();
>>> PlayAudio(SFX_Level_MeteorObstacle); Wait(6); PlayAudio(Block_SFX_Splash_1)
>>> TextboxEnter(Default);

Noelani: I never expected to see one up close. #text_italics

Noelani: What--Hey, are you okay?! #Noelani_sad

>>> FadeBlack(1)

Narrator: Several Days later…#text_italics
// >>> MoveBackground(0, 600, 0);
>>> FadeBlack(0)

Noelani: Hey Ocean, any change? #Noelani_happy

>>> !CharEnter(Ocean, right)

Ocean: Morning Noelani, and No, still asleep. #ocean_concerned_MeganChau

Noelani: She’s been asleep for several days! Is she in a coma? She seems a bit more sparkly than yesterday, do you think that's a good sign? Her feet look less burned too! What happened to her to make her so tired? What can I do to help? #Noelani_pensive

Ocean: Poor girl probably just needs rest. She was so dull when you first brought her in, I could barely tell she was a Star at first glance…#ocean_sad_MeganChau

>>> CharExit(Noelani); 

>>> !CharEnter(Astra, left);

Astra: Urrrgh… #AstraSad_GriffinConway

Astra: AAAAHHH!!! What?! Where-- Where am I?! What Happened?! #AstraSurprised_GriffinConway

>>> CharEnter(Ocean, right);

Ocean: Hey, hey, easy there! You’re on an island down on earth. You seemed to have burnt out, fallen down to earth, and splashed down onto the beach. Noelani here saw you splash down, and brought you here to rest.#ocean_shocked_MeganChau

>>> !CharExit(Ocean); CharExit(Astra)

>>> !CharEnter(Noelani, left); CharEnter(Astra, right);

Noelani: Hi there! I’m Noelani, nice to meet you! #Noelani_happy

Astra: H-hello. My name is Astra… Thank you for saving me. #AstraSurprised_GriffinConway

Noelani: Of course! I’m glad you’re alright! You definitely look brighter than you did when I found you--hope you’re feeling better!

Astra: … Yeah, I do feel a bit better. #AstraHappy_GriffinConway

>>> CharExit(Noelani); !CharEnter(Ocean, left);

Astra: Speaking of being bright, what is the quickest way to get back to space? I have a lot of work to get back to in space if I want to be a Bright Star. #AstraQuestioning_GriffinConway

Ocean: Ambitious goal you got there! Good for you! To answer your question, the moon can come get you on the next full moon to help bring you back to space. #ocean_happy_MeganChau

Narrator: Her voice perked up when she mentioned the moon. The ocean and the moon have been dating for centuries now, it's not surprising that they’d be excited to see each other. #text_italics

Astra: Okay… How long will it take for her to come down here? a few days?

Ocean: … Unfortunately, the full moon just passed. You’ll have to wait a month. #ocean_concerned_MeganChau

Astra: A MONTH?!  I’M GOING TO BE SO BEHIND ON ALL MY WORK!!! I CAN'T WORK ON BEING A BRIGHTER STAR DOWN ON THE GROUND!!! WHAT AM I GONNA DO?! WHAT DO I EVEN DO IF I’M NOT WORKING??? #AstraSad_GriffinConway

>>> CharExit(Ocean); !CharEnter(Noelani, left);

Noelani: Whoa whoa slow down take a breath! Come on, in … and out...  #Noelani_sad

Astra: Huu…  ahh… Huu… ahh… Thank you… 

Noelani: I know it sucks being stuck in a new place without warning… and I’m sorry you’re stuck here… but on the bright side, maybe think of it like a vacation? I could even show you around! #Noelani_determined

Noelani: There’s lots of stuff to do! I can even show you some beautiful quiet spots if you just want to chill. 

Noelani: That sound okay to you?#Noelani_happy

Astra: … Okay. #AstraSurprised_GriffinConway

>>> FadeBlack(1)

Noelani: and so began the most fun month I’ve had in years. Showing Astra all the cool things on Earth! Like crabs, and flowers, and seagulls. She’s so curious about everything. Though I found one of my new favorite things to do was make her smile. #Noelani_happy

Noelani: Every so often, she’d get real quiet and look forlorn and far away… she said she was worried about losing all the progress she’s made in being a Bright star. Sounds like it really takes a lot.  #Noelani_sad

Noelani: I didn’t really know how to help ease her worries. I may not be the smartest nymph in the sea, but I could listen to her, or at least try to take her mind off being stuck here-- be it with pretty flowers or bad jokes.  #Noelani_determined

Noelani: During those days of finding flowers and chasing seagulls, my feelings for Astra grew into something more. #Noelani_pensive

Noelani: And suddenly the month was up, and Moon came to take Astra back to space.  #Noelani_sad

// >>> MoveBackground(0, 100, 0);
>>> FadeBlack(0);

Noelani: I want Astra to know my feelings before she leaves, so now’s the time to say it! Just take a deep breath and… #Noelani_determined #text_italics

Noelani: Astra! Before you go, I…  #Noelani_sad

Noelani: I want to tell you something!  #Noelani_determined

Astra: … Yes? #AstraQuestioning_GriffinConway

Noelani: I…

Noelani: Should I really do this now? Right before she leaves? She said she’s got work to get back to …#text_italics #Noelani_pensive

Noelani: I would just be a distraction to her, if I told her now. I can tell her some other time.#text_italics  #Noelani_sad

Noelani: … I’ll see you later. #Noelani_sad

Astra: Of course you will! I may not be a really Bright star yet, but you should still be able to see me if you look up! I live next to Moon, so just look around there! #AstraHappy_GriffinConway

Astra: … I’ll come visit with Moon whenever we both can find the time to come down here, okay? #AstraSurprised_GriffinConway

Noelani: ...Okay.

Noelani: Bye Astra.

Astra: Bye Noelani. #AstraSad_GriffinConway

>>> !CharExit(Astra);

Noelani: I watched as she rose back into the sky. #text_italics

>>> FadeBlack(1); Wait(3); FadeBlack(0);

>>> CharEnter(Ocean, right);

Ocean: Noelani… are you doing alright? #ocean_concerned_MeganChau

Noelani: What do you mean? #Noelani_pensive

Noelani: I’m doing good. #Noelani_happy

Noelani: No worries at all.

Ocean: Noelani... you're not fooling anyone with that act. #ocean_sad_MeganChau

Ocean: If you want to talk about it, I'm here for you. #ocean_concerned_MeganChau

Ocean: But if you don't want to now, I'll be on my way. #ocean_longing_MeganChau

Noelani: Wait! #Noelani_sad

Noelani: umm... I... I'm not really... doing well.

Noelani: I haven't been.. for a while.. since..

Ocean: Since Astra returned to space? #ocean_concerned_MeganChau

Noelani: Yeah... that...

Ocean: It was about two months ago that she fell into the sea and you found her, right? #ocean_longing_MeganChau

Noelani: Yeah, I was just minding my own business when I saw a bright trail crash into the water not that far from me. #Noelani_pensive

Noelani: Since I was the only nymph around, I figured I'd check on it, you know?  #Noelani_determined

Ocean: You sure it wasn't your curiosity telling you? #ocean_shocked_MeganChau

Noelani: Okay, it was a mix of both. But curiosity called a little louder. #Noelani_happy

Noelani: Anyway when I got close, I saw Astra floating asleep in the waves. #Noelani_pensive

Noelani: She looked so ethereal and beautiful but when I tried waking her up she just kept saying

Noelani: -I'll be fine.. I'll be okay...- #text_italics

Ocean: She was quite the talker! Both when she was asleep and after she woke up. #ocean_longing_MeganChau

Noelani: Yeah! It was fun to learn about what she does up there and to see her learn about stuff here on earth for the first time! #Noelani_happy

Noelani: But then she had to go... #Noelani_sad

Ocean: Noelani, she has responsibilities to space just like you do to the sea. #ocean_sad_MeganChau

Noelani: I get that... I just didn't want that time to end....

Noelani: Everything seemed so much more exciting with her around. 

Noelani: She could find joy in the smallest things, even if we weren’t really doing anything at all… 

Noelani: I don’t know how you and the Moon manage it.

Noelani: How do you deal with the distance?  

Noelani: You guys can see each other once a month, but sometimes you’re both so busy you don’t even get that!

 
Ocean: Lots of questions, huh? #ocean_longing_MeganChau

Ocean: Unfortunately, you and I — among many others — are bound to this planet. #ocean_concerned_MeganChau

Ocean: And despite how close the moon and stars may seem, the distance from here to where they reside is immense. 

Ocean: Even with all of my power, getting to the heavens would probably take a few days. 

Ocean: If I took that much time off, I’d be neglecting my duties to Earth. #ocean_sad_MeganChau

Ocean: My Moon is much more powerful than I am, and even she has to take part of the month to store energy to make the trip down here, and to recover when she gets back. #ocean_longing_MeganChau

Ocean: As much as it sucks to only see each other face to face every so often, that’s as much as we can manage. 

Ocean: So I’m happy with what we have, even if I do feel lonely sometimes. #ocean_longing_MeganChau
 
Noelani: Wait… it’d “probably” take a few days? You mean you’ve never tried? #Noelani_pensive
 
Ocean: I took a day off to try, and only got a little more than half of the way there #ocean_concerned_MeganChau

Ocean: The water was reaching for me the whole time, slowly washing away my spire as I built it. 

Ocean: I had to spend most of the next day recovering from using so much energy. #ocean_sad_MeganChau
 
Noelani: What if I tried? 

Noelani: I’m a nymph so the extent of my duties is not as much as yours.

Noelani: Wrangling unruly waves during a storm, guiding lost creatures back to their proper place, and tending to the reef...

Noelani: I'm sure the other nymphs could cover me for a while! #Noelani_determined

Noelani: There’s so many of us so there’s usually not much to do anyways! #Noelani_happy
 
Ocean: The sea will still reach for you and tear down your attempts. #ocean_shocked_MeganChau

Ocean: I can't void the flow of water either. So the most I could do is calm the water slightly when you start, or allow you a short moment of rest. 

Noelani: Simple! 
 
Noelani: I’ll just have to keep building faster than the waves can break it down! #Noelani_determined
 
Ocean: Noelani, building such a tower would push the limits of your powers.
 
Noelani: But if I try I have the chance to see her again. 

Noelani: And I'd rather try and fail than live with the uncertainty. #Noelani_sad
 
Ocean: *sigh* You’ve already set your mind to it, huh? #ocean_longing_MeganChau

Ocean: Well then, give it your all. #ocean_happy_MeganChau 
