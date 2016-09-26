#ShowDebuggingInformation: false
ROWS combinedcharnum = {"a:1","b:2",":3"}
ROWS onlynums = EXTRACT COLLUM[2] FROM combinedcharnum SPLIT BY [":"]
SHOW = onlynums

[IF onlynums IS ROWS[{"1","2","3"}]]
{
STRG outplace = STRG[<here> + "\\testrun.txt"]
OUTP = STRG[READ[outplace] + <newline> + "EXTRACT"] HERE[outplace]
(exit)
}
