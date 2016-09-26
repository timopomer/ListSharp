#ShowDebuggingInformation: True
//demo showing how to read files

//you can directly read a STRG
STRG FileContents = READ[<here>+@"\READ.ls"]
SHOW = FileContents

STRG outplace = STRG[<here> + "\\testrun.txt"]
OUTP = STRG[READ[outplace] + <newline> + "READ"] HERE[outplace]
(exit)

