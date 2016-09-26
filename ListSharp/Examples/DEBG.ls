#ShowDebuggingInformation: True
//demo how to debug with DEBG

ROWS nums = {"1","2","3","4","5","6","7","8","9"}
NUMB badlines = 0

[FOREACH STRG IN nums AS singlenum]
[IF singlenum ISNOT "3"]
NUMB badlines = badlines PLUS 1

DEBG = badlines

STRG outplace = STRG[<here> + "\\testrun.txt"]
OUTP = STRG[READ[outplace] + <newline> + "DEBG"] HERE[outplace]
(exit)

