VAR Happy = false
VAR Confident = false
VAR Neutral = false
VAR Afraid = false
VAR Sad = false
VAR Depressed = false
VAR Traumatized = false

VAR nextBranch = ""

// Runtime-bound from DialogueManager via BindExternalFunction.
EXTERNAL SetFlag(flag)

// ============================================================
// PROLOGUE — "Just As It Should Be"
// Case 1: Itsuki gets fired.  Case 2: Itsuki survives the guess.
// Both roads end on the same rooftop.
// Quest flags: SetFlag("quest_<id>") on accept, SetFlag("quest_<id>_done") on complete.
// ============================================================

// ---------- BAB 0: KANTOR ----------

~ Neutral = true
# speaker: Itsuki
...hah...
...hah...
The ceiling is spinning again.
I've been staring at this screen for...
...what day is it again?
These typing noises...
Every key sounds like a nail.

# speaker: Boss
ITSUKI!!!
Get over here. NOW.

# speaker: Itsuki
W-whu—
Y-yes, Boss. Right away. I'm sorry.

# speaker: Boss
Look at you.
Eyebags down to your chin. Shirt from yesterday.
You good-for-nothing piece of worker.
STOP SLACKING OFF!!!

# speaker: Itsuki
I-I'm sorry...
It won't happen again. I promise.
I... I only closed my eyes for a second.
Just a second...

# speaker: Boss
A second?
A SECOND?!
That's what you said last week.
And the week before that.
This is your fifth violation, Itsuki.

# speaker: Boss
So. I'll be generous.
I'm hiding a number behind my back.
Guess it right — you're pardoned. Once again.
Guess it wrong...
you're, F I R E D !
Now choose.

# speaker: Itsuki
(A number...?)
(My hands won't stop shaking...)
(If I lose this job... then what?)
(Mom... Dad... you always said if I worked hard, everything would be okay...)
(I worked hard. I worked so hard.)
...okay. Okay.
I've got this.

// MECHANIC: 4 choice buttons. Only [2] survives.
+[4] -> fired
+[9] -> fired
+[2] -> notFired
+[7] -> fired

// ---------- BAB 1a: CASE 2 — HARI TERAKHIR YANG TERLIHAT NORMAL ----------

== notFired ==

# speaker: Boss
...
Two.
Hah.
You really do have a good intuition for what I'm thinking.
If only you used that brain for actual work.
For the presentation. For our client.
Such a waste.

# speaker: Boss
Fine. A man of my word.
You're pardoned. Once again.
BUT. As punishment —
you do half of MY work.
And you prepare the Ringo Corp. presentation. Tomorrow.
Alone.

# speaker: Itsuki
~ Afraid = true
Y-yes, Boss!
Half of your work. The presentation. Alone. Tomorrow.
...thank you. Thank you so much.
(I hate how grateful I sound.)
(I hate it.)

# speaker: Boss
Quit bowing and get moving.

// --- [QUEST] give_work ---
~ SetFlag("quest_give_work")

# speaker: Worker
Hey, Itsuki.
Whoa, hey— you're white as paper.
Let me guess. He piled his work on you again?

# speaker: Itsuki
...
Half of it. And the Ringo Corp presentation.
By tomorrow.

# speaker: Worker
That psycho.
Look, I'm actually free this afternoon.
Give me some of that pile. I'll help you carry it.

# speaker: Itsuki
You'd... really do that?
For me?

# speaker: Worker
Don't make it weird. Just hand it over.
// MECHANIC: 1 real-time minute to bring the documents to the coworker.
// If the timer runs out -> SetFlag("flag_hesitant") before this knot ends.

# speaker: Itsuki
Here. This stack, and these folders.
Careful with the blue ones, they go to accounting.

# speaker: Worker
Got it.
...Hey. Itsuki.
Whatever happens tomorrow — you did your best. Okay?

# speaker: Itsuki
~ Happy = true
...Yeah.
Thanks. I owe you one.

// --- [QUEST] give_work selesai ---
~ SetFlag("quest_give_work_done")

// --- [QUEST] ramen_store ---
~ SetFlag("quest_ramen_store")

# speaker: Itsuki
(Ugh, my stomach.)
(I skipped breakfast. And yesterday's lunch.)
(Chef Tan's place is on the way home.)
Just this once. I deserve one warm bowl.
-> case2_ramen

== case2_ramen ==

# speaker: Chef Tan
Itsuki! Haven't seen you in days.
Sit, sit. The usual is already on the stove.

# speaker: Itsuki
...You started cooking before I even ordered?

# speaker: Chef Tan
Kid, you order the same thing every single time.
Even the stray cats outside have it memorized.

# speaker: Itsuki
~ Happy = true
...Heh. Fair enough.
Eat well, everyone...

# speaker: Chef Tan
Hm? You say something?

# speaker: Itsuki
Nothing, old man. Nothing at all.

// --- [QUEST] ramen_store selesai ---
~ SetFlag("quest_ramen_store_done")

-> case2_rest

== case2_rest ==

// --- [QUEST] rest ---
~ SetFlag("quest_rest")

# speaker: Itsuki
Home.
Finally.
Shoes off. Lights on.
...The fridge is empty except for expired milk.
Of course it is.

# speaker: Itsuki
One missed call.
...Takumi?
No. No, I can't deal with that right now.
I'll call him back tomorrow.
When things are better.
When I'm better.

# speaker: Itsuki
(The ceiling of this apartment always looks the same.)
(A blank white nothing.)
(Just like my calendar.)
(Just like me.)
// MECHANIC: sleep interaction. If Itsuki sleeps past 1:00 AM -> SetFlag("flag_night_owl")
// and apply the "slowness until 12PM" debuff the next morning.

# speaker: Itsuki
...
Tomorrow.
I'll fix everything tomorrow.
...Goodnight, Mom. Goodnight, Dad.

// --- [QUEST] rest selesai ---
~ SetFlag("quest_rest_done")

// --- [QUEST] checkin ---
~ SetFlag("quest_checkin")

# speaker: Itsuki
~ Afraid = true
...oh no.
Oh no, no, no.
9:14.
// MECHANIC: arrival after 9:00 AM is always counted as late.

# speaker: Itsuki
(I ran. I actually ran. People were staring.)
(My chest hurts.)
(Just one more day. Just get through one more day.)

// --- [QUEST] checkin selesai (late) ---
~ SetFlag("flag_late_checkin")
~ SetFlag("quest_checkin_done")

# speaker: Boss
Well, well.
9:14, Itsuki.
Do you think this company runs on your personal schedule?
The Ringo Corp people will be here in an hour.
If you embarrass me today, don't bother coming back.

# speaker: Itsuki
Y-yes, Boss. It won't happen again. I promise.

# speaker: Boss
...
Why do you always say that like it means something?

// --- [QUEST] do_work ---
~ SetFlag("quest_do_work")
// MECHANIC: simon-says minigame using arrow keys and space bar.
// If the player fails -> Boss scolds Itsuki again (SetFlag("flag_scolded_work")).

# speaker: Itsuki
~ Neutral = true
Alright.
Focus.
Left. Up. Space. Right.
One task at a time.
Like Dad taught me when I was little...
"Slow is fine, Itsuki. As long as you don't stop."

// --- [QUEST] do_work selesai ---
~ SetFlag("quest_do_work_done")

// --- [QUEST] brother_call ---
~ SetFlag("quest_brother_call")

# speaker: Itsuki
Phone's ringing again.
Takumi...
...why now?
What do I even say to you?
"Hey, little brother, my life is a dumpster fire, how's yours?"
You made it pretty clear you don't want a brother like me.

# speaker: Itsuki
...
I'll call him back after lunch.
I will. I promise I will.
...Who am I lying to.
// MECHANIC: optional interaction — player can choose to return the call here.
// If ignored -> SetFlag("flag_ignored_takumi").

// --- [QUEST] brother_call selesai ---
~ SetFlag("quest_brother_call_done")

// --- [QUEST] lunch_ramen ---
~ SetFlag("quest_lunch_ramen")

# speaker: Itsuki
(Chef Tan's. My lungs remember the steam.)
(Just half an hour where nobody screams at me.)

# speaker: Chef Tan
Itsuki! The usual?
...Hold on. Sit down. Look at me.
When was the last time you slept? Really slept?

# speaker: Itsuki
Ha... you sound like my mom.

# speaker: Chef Tan
Somebody has to.
Eat first. Then you can dodge my questions all you want.

# speaker: Itsuki
~ Happy = true
...Thanks, old man.
For the record...
...you make the best ramen in this whole rotten city.

# speaker: Chef Tan
Hah! Flattery gets you extra chashu.
Go on. Before your boss invents a new way to ruin your day.

// --- [QUEST] lunch_ramen selesai ---
~ SetFlag("quest_lunch_ramen_done")

// --- [QUEST] back_office ---
~ SetFlag("quest_back_office")
// MECHANIC: real-time deadline — be back at the office before 13:00.

# speaker: Itsuki
Check, please.
...and Chef Tan?
If... if someone keeps going even when it hurts.
Even when nobody cares.
Does that mean something?

# speaker: Chef Tan
...
It means everything, kid.
Why do you ask?

# speaker: Itsuki
No reason.
See you around.

// --- [QUEST] back_office selesai ---
~ SetFlag("quest_back_office_done")

// ---------- TITIK BALIK CASE 2: PRESENTASI GAGAL ----------

~ Sad = true
# speaker: Itsuki
The conference room is so bright.
Everyone's watching.
Ringo Corp. The boss. My coworker.
Okay. Okay, breathe.
"Good afternoon, we are here today to—"

# speaker: Boss
Itsuki.
What. Is. This.
Slide 14. LOOK at slide 14.
That is last quarter's data. In MY presentation.

# speaker: Itsuki
I— no, that—
Boss, I compiled everything myself, the numbers were—
Someone must have swapped—

# speaker: Boss
Are you accusing someone?
Are you accusing ME?
In front of OUR CLIENT?

# speaker: Itsuki
N-no! I just—

# speaker: Boss
Enough.
Security. Escort him out.
And Itsuki — don't even think about asking for a reference.

# speaker: Itsuki
...but I...
(the room is spinning again)
(it's happening again)
(I did everything right this time...)
(I DID EVERYTHING RIGHT—)

// --- Case 2 now collapses into Case 1's road ---
-> case1_goHome

// ---------- BAB 1b: CASE 1 — SEMUANYA RUNTUH SEKALIGUS ----------

== fired ==

# speaker: Boss
Ohohoho...
Tough luck, Itsuki.
You're,
F I R E D !

# speaker: Itsuki
~ Sad = true
NO. No, no, no...
B-boss, please.
Give me another chance.
I'll be your footrest. I can massage your—
I can sleep here! I won't close my eyes ever again, I swear—

# speaker: Boss
Rules are rules, Itsuki.
GET OUT!!!

# speaker: Itsuki
B-but my rent—
I took YEARS to get accepted here—
Please—

# speaker: Boss
SECURITY!!!
Intruder alert!!!

# speaker: Itsuki
...
Fine.
I'm going. I'm going.
~ Sad = false
~ Depressed = true

-> case1_goHome

== case1_goHome ==

// --- [QUEST] go_home ---
~ SetFlag("quest_go_home")
// MECHANIC: 2 real-time minutes of watch time to get home.
// If the player fails -> SetFlag("flag_death_cutscene") and jump straight to -> rooftop.

# speaker: Itsuki
(The streets are the same streets I walk every day.)
(Same vending machine. Same stray cat. Same cracked pavement.)
(Nothing has changed.)
(And yet everything is over.)
// MECHANIC: auto-walk toward home. The player keeps control but the clock is ticking.

-> case1_ramen

== case1_ramen ==

# speaker: Itsuki
~ Depressed = true
I lost my parents.
My brother won't even acknowledge me.
And now...
I lost the job it took me years to get.
...
Is anything still worth a try?

# speaker: Itsuki
...
Ah.
Chef Tan's.
I guess I'll pay him one last visit.

# speaker: Itsuki
Hey, Chef Tan.
The usual, please.

# speaker: Chef Tan
Right away—
...
Kid. Sit down.
You're wearing your bad aura all over your face.
Talk to your old man.

# speaker: Itsuki
...I got fired.

# speaker: Chef Tan
Ah. You got—
Wait. FIRED? Why?!

# speaker: Itsuki
I closed my eyes for a second.
After weeks of overtime because of my boss.
I did everything for that company.
He took all the credit.
And I couldn't fight him. None of us could.

# speaker: Chef Tan
...
Sigh.
I warned you. Four, five years ago.
That company eats people, I said.
"You watch yourself, Itsuki," I said.
And you kept telling me you were fine.
Nothing bad would happen to you.
...Now look at you.
Do you believe me now?

# speaker: Itsuki
...
Yeah.
I believe you.

# speaker: Chef Tan
Kid. Listen to me.
A job is a job. It's not your life.
You're still breathing. You hear me?
As long as you're breathing, the story isn't over.
Come by tomorrow. I'll teach you the soup recipe.
You can start over. People do it all the time.

# speaker: Itsuki
~ Happy = true
...
Heh.
You'd really do that? For a failure like me?

# speaker: Chef Tan
Hah! For Itsuki who always pays his bill and tips too much?
Any day of the week.

# speaker: Itsuki
...
Then... see you tomorrow, Chef Tan.

# speaker: Chef Tan
That's the spirit! Tomorrow, kid!

# speaker: Itsuki
~ Happy = false
~ Depressed = true
...(I'm sorry, old man.)
(There's no tomorrow for me.)

// --- [QUEST] another_day ---
~ SetFlag("quest_another_day")

# speaker: Itsuki
Start another day.
That's what normal people do, right?
Wake up. Wash up. Pretend.
I can pretend for one more evening.

// --- [QUEST] another_day selesai ---
~ SetFlag("quest_another_day_done")

-> case1_goHome_done

== case1_goHome_done ==

# speaker: Itsuki
(Home.)
(This tiny room.)
(The only thing waiting for me is the hum of the fridge.)
// NOTE: this knot replaces the failed-timer branch when the player made it home in time.

-> rooftop

// ---------- BAB 2: KONVERGEN — MALAM TERAKHIR DI ATAP ----------

== rooftop ==

// --- [QUEST] rooftop ---
~ SetFlag("quest_rooftop")

# speaker: Itsuki
The elevator's dead again.
Of course.
Emergency stairs, then.
One flight.
Two flights.
My legs remember this. Overtime taught them well.
// MECHANIC: player walks the emergency stairway to the roof. Keep player control.

# speaker: Itsuki
...The door's open.
The roof.

# speaker: Itsuki
The city is so loud from down there.
Up here...
...it's just wind.

// --- [QUEST] rooftop selesai ---
~ SetFlag("quest_rooftop_done")

// --- [QUEST] last_words ---
~ SetFlag("quest_last_words")

# speaker: Itsuki
Mom. Dad.
You always told me —
work hard, be kind, and everything will turn out fine.
I worked hard.
I was kind.
So why...
why am I standing here?

# speaker: Itsuki
Takumi...
My little brother.
The last time you called me "big brother" was at the funeral.
After that, you looked at me like I was a stranger.
Like I was the reason they were gone.
...
Maybe I was.
Maybe if I had come home earlier that night.
Maybe if I had picked up the phone—
// MECHANIC: optional phone interaction. "Delivered." Never read.

# speaker: Itsuki
Chef Tan.
You were the only one who ever asked if I had slept.
You offered me a tomorrow.
I'm sorry.
I won't be there for the soup recipe.

# speaker: Itsuki
Six years of overtime.
And the only thing in my mailbox is a layoff letter.
The only thing on my phone is a missed call.
The only thing in my future is...
...this.
Just this.

# speaker: Itsuki
Everything was supposed to be just as it should be.
Work hard. Be kind. Get rewarded.
Who wrote that rule?
Who wrote it...
and why did I believe it until the very end?

# speaker: Itsuki
...
It's quiet.
No keyboards. No boss. No alarm clocks.
No tomorrow.
For the first time in six years...
it's quiet.

// --- Pilihan terakhir kecil untuk pemain ---
+[Look at the city one last time] -> rooftop_city
+[Hold your parents' watch] -> rooftop_watch

== rooftop_city ==

# speaker: Itsuki
All those lights.
Every window is somebody's whole world.
None of them know I'm up here.
None of them will remember me by morning.
...
That's okay.
I was tired of being a stranger anyway.
-> rooftop_end

== rooftop_watch ==

# speaker: Itsuki
Dad's old watch.
It stopped the day he did.
Tick... tick... tick...
...Silence.
I guess we finally match, Dad.
-> rooftop_end

== rooftop_end ==

# speaker: Itsuki
~ Traumatized = true
For the first time,
the wind doesn't feel cold.
It feels...
...like hands.
Like someone saying,
"You did well. You can rest now."

# speaker: Itsuki
...
Goodbye.
I'm sorry.
And thank you...
for every bowl of ramen.
For every small kindness.
For trying.

// --- Screen fades to black. No details. No sound. ---

~ SetFlag("quest_last_words_done")
~ SetFlag("prologue_ended")
~ nextBranch = "inner_self"

-> END
