#ShowDebuggingInformation: false
ROWS nums = {"1","2","3","4","5","6","7","8","9"}
STRG bestnum = GETLINE nums [nums LENGTH MINUS 2]
SHOW = bestnum

[IF bestnum IS STRG["7"]]
{
STRG outplace = STRG[<here> + "\\testrun.txt"]
OUTP = STRG[READ[outplace] + <newline> + "GETLINE"] HERE[outplace]
(exit)
}
