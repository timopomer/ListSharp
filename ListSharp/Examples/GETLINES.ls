#ShowDebuggingInformation: True

ROWS nums = {"1","2","3","4","5","6","7","8","9"}
ROWS bestnums = GETLINES nums [7 AND 1 TO nums LENGTH]
SHOW = bestnums

[IF bestnums IS ROWS[{"7","1","2","3","4","5","6","7","8","9"}]]
{
STRG outplace = STRG[<here> + "\\testrun.txt"]
OUTP = STRG[READ[outplace] + <newline> + "GETLINES"] HERE[outplace]
(exit)
}
