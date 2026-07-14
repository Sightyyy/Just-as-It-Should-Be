// ===== STATE VARIABLES =====
VAR Happy = false
VAR Confident = false
VAR Neutral = false
VAR Afraid = false
VAR Sad = false
VAR Depressed = false
VAR Traumatized = false

// ===== BRANCHING VARIABLES =====
VAR nextBranch = ""

# speaker: Random A
# portrait: npc_portrait
Hey there!
Are you from around here too?

+[N-no, where am I?] -> fearBranch1
+[Unfortunately I'm not, where am I?] -> calmBranch1
+[Who are you?] -> doubtBranch1

==fearBranch1==
~ Afraid = true
# speaker: Random A
# portrait: npc_portrait
Hey chill, I'm not a mean person y'know haha
-> continue

==calmBranch1==
~ Neutral = true
-> continue

==doubtBranch1==
~ Sad = true
# speaker: Random A
# portrait: npc_portrait
My identity doesn't really matter lmao
-> continue

==continue==
# speaker: Random A
# portrait: npc_portrait
We're currently at a place called "In Between"
I know it sounds confusing to you, I can guarantee you I was once lost too when I arrived here for the first time.
Trust me, and you'll be fine, okay?

+[Sure, prove it to me] -> courageBranch1
+[*silence] -> fearBranch2

==courageBranch1==
~ Confident = true
# speaker: Random A
# portrait: npc_portrait
Heh, I see you dare to trust me huh?
-> continue2

==fearBranch2==
~ Afraid = true
# speaker: Random A
# portrait: npc_portrait
You look pale
-> continue2

==continue2==
# speaker: Random A
# portrait: npc_portrait
~ nextBranch = "chat_another_npc"
Just follow me, and you'll be fine
Cause if you don't... I won't want to imagine your fate

-> END
