# Lunar
C# port of the original DEC PDP-8 game written in FOCAL by Jim Storer in 1969

You can learn about the original at http://www.cs.brandeis.edu/~storer/LunarLander/LunarLander.html

This is buggy ATM, I'm not getting the same results as the original for given inputs. ~~Unfortunately I do not know what order of operations were followed in the PDP-8 FOCAL system.~~

I now know, having found some original docs, that the PDP8 did cheat a bit on the matter of Order of Operations, or Hierarchy as they called it in the documentation.  

Rather then treating multiplication and division as one level in the heirarchy, it did multiplication and then division. Thus,

`16/8*2`

was NOT the same as

`(16/8)*2`

Entering T 16/8\*2 rendered the answer "1".

We humans, and later computers (and/or languages) know Parenthesis, Exponents, Multiplication and Division (left to right), Addition and Subtraction (left to right).  Or at least you should, that's the point of all those dopey math memes...  FOCAL however did Parenthesis, then Exponents, then Mutliplication, then Division, then Subtraction, then Addition.
