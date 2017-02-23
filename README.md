# Lunar
C# port of the original DEC PDP-8 game written in C-FOCAL by Jim Storer in 1969

You can learn about the original at http://www.cs.brandeis.edu/~storer/LunarLander/LunarLander.html

This only performs about %99.99999999999999999999999999999999999 like the original.  

I took pains to make the console and layout present the game much as it would have appeared on the original system, however I did not figure out how to get the exact same computational results.  As a result every now a then the displayed value for altitude or speed will be off by 1, only to be subsequently back on value in the next turn.

This is because of some differences in how computers work today vs. in 1969.

I now know, having found some original docs, that the PDP8 did cheat a bit on the matter of Order of Operations, or Hierarchy as they called it in the documentation.  

Rather then treating multiplication and division as one level in the heirarchy, it did multiplication and then division. Thus,

`16/8*2`

was NOT the same as

`(16/8)*2`

Entering T 16/8\*2 rendered the answer "1".

We humans, and later computers (and/or languages) know Parenthesis, Exponents, Multiplication and Division (left to right), Addition and Subtraction (left to right).  Or at least you should, that's the point of all those dopey math memes...  FOCAL however did Parenthesis, then Exponents, then Mutliplication, then Division, then Subtraction, then Addition.

This however only required some attention to the equations and making sure that I laid them out so they would "misbehave" in C# like they did in C-FOCAL.  Just a simple matter of adding some parenthesis.

The bigger matter is that this program requires the use of the math.sqrt funcion in C# and that introduces "float" and floating point errors.  At this moment I am utterly confused as to why any computer should have a system wherein 1.2 - 1.0 should result in 0.19999999996 and not 0.2, but it does.  In the dim dark past this wasn't an issue, 1.2 - 1.0 gave you 0.2, no ifs ands or bits...

At some point, as I learn more, I may discover how to do those calculations "long hand" but as math is not my strong suit, the built in function will have to do for now.

Enjoy, and Good Luck!
