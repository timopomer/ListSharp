STRG outplace = STRG[<here> + "\\testrun.txt"]
OUTP = STRG[READ[outplace] + <newline> + "OUTP"] HERE[outplace]
(exit)

