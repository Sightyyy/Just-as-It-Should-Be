// ===== STATS VARIABLES =====
VAR calm = 0
VAR fear = 0
VAR doubt = 0
VAR courage = 0

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
~ fear += 1
# speaker: Random A
# portrait: npc_portrait
Hey chill, I'm not a mean person y'know haha
-> continue

==calmBranch1==
~ calm += 1
-> continue

==doubtBranch1==
~ doubt += 1
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

+[I don't trust you, why should I?] -> doubtBranch2
+[Sure, prove it to me] -> courageBranch1
+[*silence] -> fearBranch2

==doubtBranch2==
~ doubt += 1
# speaker: Random A
# portrait: npc_portrait
You don't have a choice, do you :3 *horror suspense
-> continue2

==courageBranch1==
~ courage += 1
# speaker: Random A
# portrait: npc_portrait
Heh, I see you dare to trust me huh?
-> continue2

==fearBranch2==
~ fear += 1
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