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
Sigh...
~ Depressed = true
I don't know what to do anymore
Is...
Is everything worth a try anymore?
...
I lost my parents
My brother won't even acknowledge me
And now...
I lost my job that I took years to be accepted
...
#Auto walk to ramen shop
#If player succeed curing the depression, force the state to neutral
#If player failed, go to Prologue_suicide.ink
...
Ah, it's Chef Tan's
I guess I'm gonna pay Chef Tan one last visit
-> visitRamenShop

==visitRamenShop==
#player
Hey, Chef Tan
The usual please

#Chef
Right away!

#check for flags on getting fired or not
#player got fired
Hey, Itsuki, you're showing your bad aura
Is there something wrong?

#player
You really can see through me, huh old man?
I guess I can't really hide it anymore

#chef
From your face, did something happened at your work?

#player
I...
I got fired

#chef
Ah, you got fired
Wait-
Fired?
Why?

#player
I was just resting my eyes after so many days taking overtime because of this one boss i got
I guess... he just didn't really like me
I did everything i can for the company and he took all the credit
But alas, i can't fight the boss
Neither my coworkers can fight him

#chef
Sigh...
In the end you got yourself in a very sorry state
I do warn you about it like 4 or 5 years ago about that company
And yet you won't listen any of it
You keep saying you're gonna be fine, nothing bad's gonna happen to you
...
Now look at you
Do you believe me now?

#player didn't get fired
Itsuki, you seems very in high spirits
Is there something happened?
Might as well share with your old pal
Haha!
-> DONE