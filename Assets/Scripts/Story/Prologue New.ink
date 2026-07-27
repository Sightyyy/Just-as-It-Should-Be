VAR Happy = false
VAR Confident = false
VAR Neutral = false
VAR Afraid = false
VAR Sad = false
VAR Depressed = false
VAR Traumatized = false

VAR nextBranch = ""

#Player
Ugh...
I'm tired...
These typing noises...

#Boss
ITSUKI!!!
Come here

#Player
Whu- wha-
Yes, Boss?

#Boss
You good for nothing piece of worker
STOP SLACKING OFF!!!

#Player
S-sorry boss
It won't happen again
I-I promise

#Boss
It's your 5th violation, so im going to give you some choices.
....
I'm hiding a number behind me
If you guessed it right, you'll be pardoned, ONCE AGAIN
If you guessed it wrong,
you're, F I R E D !
Ok then Itsuki, choose.

+[4] -> fired
+[9] ->fired
+[2] -> notFired
+[7] ->fired

==notFired==
#Boss
you really have a good intuition on what number I'm going to pick
If only you use it for the work and when presenting our project to our client
Sigh
Ok, I'm a man of a word
You get yourself a pardon once again
But as a punishment, do half of my work and prepare for presentation with Ringo Corp. tomorrow

#Player
~ Happy = true
Yes, Boss!
I'll accept the punishment for the reward you gave me

#transition
some time later

#worker
Hey, Itsuki, can i have some of your pile of work you got?
I'm kinda free so I'm gladly to lend you my hand on your worker

#Player
Sure, I'm bringing it now
-> continueInOffice

==fired==
#Boss
Ohohoho...
Tough luck, Itsuki.
You're,
F I R E D !

#Player
~ Sad = true
NO, no, no, no
B-boss, please
Give me another chance
I-I-I'll be your footrest
I-I can massage your-

#Boss
Rules are rules, Itsuki
GET OUT!!!!!

#Player
B-But-

#Boss
SECURITY!!!
Intruder alert!!!

#Player
Fine, I'm going, I'm going
~ Sad = false
~ Depressed = true
-> continueFired

==continueInOffice==
#player
Here you go, some of the work that needs to be done

#worker
Thanks. I'll help you finish it quick

#player
Thank you so much
I owe you one
~ Happy = true

some time later

#worker
Itsuki, some of the work is done

#player
Mine too
Hey, I really owe you one for this
I never expect the boss giving me a lot of work load

#worker
Well he's the CEO's cousin and always spoiled rotten in this company
He always give all his work to his own subordinate, just like you
Because of him, 7 of the company's best worker assets were moving out, being recruited by our competitor company, and gaining heavy profit

* Did the CEO do anything about it?
    He thought about it, but he reluctantly solve it
    Rumors have it that the CEO got a lot of pressure from his own family to keep the boss
    All that we and the CEO can do is just find any solutions that doesn't include removing the boss
    
* How did you survive from all of boss' pressure?
    Just the usual take, fuse, modify, reprint, send
    Nah I'm kidding
    I created my own AI that helps me do my thing
    
    #player
    Does the boss know about this?
    
    #worker
    Nah, the AI is at my personal laptop and i rarely bring it out also
    
    #player
    Ah okay
    
* Are there anyone that get the same treatment like me other than the previous one?
    It's just you, Itsuki
    
-
#player
In the end there are no way to change this company to a better future, huh...

#worker
Yeah
That's a shame though, this company was once the most influential company to the whole world
But now, because of this kind of problem, this company is now in a brink of collapse by nepotism

#player
Mhm
Anyways, it's kind of late so I'll be going now
Thank you for your help

#worker
Alright see you tomorrow

outside the office tower

#player
I'm hungry
I think I'm gonna give Chef Tan a visit
-> visitRamenShop

==continueFired==
test
-> visitRamenShop

==visitRamenShop==
test
-> DONE