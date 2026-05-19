=== start ===
#scene:intro
Kamu terbangun di dunia yang terasa… berbeda.

Apa yang kamu lakukan?

* (Tenang) Tarik napas dan amati sekitar
    ~ calm += 1
    #inner_light
    -> observe

* (Panik) Apa ini?! Aku harus keluar!
    ~ fear += 1
    #inner_dark
    -> panic

* (Diam) Aku tidak tahu harus bagaimana…
    ~ doubt += 1
    -> silent

* (Berani) Siapapun kamu, tunjukkan dirimu!
    ~ courage += 1
    -> confront


=== observe ===
Kamu mulai memperhatikan detail kecil di sekitarmu.
Semuanya terasa lebih… jelas.
-> END

=== panic ===
Napasmu memburu. Dunia terasa menekan.
Bayangan mulai bergerak.
-> END

=== silent ===
Keheningan menyelimuti.
Tidak ada yang berubah… tapi terasa berat.
-> END

=== confront ===
Sebuah suara menjawab dari dalam dirimu sendiri.
“Akhirnya kamu bicara.”
-> END


// ===== VARIABLES =====
VAR calm = 0
VAR fear = 0
VAR doubt = 0
VAR courage = 0