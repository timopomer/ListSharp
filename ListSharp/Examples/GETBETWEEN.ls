#ShowDebuggingInformation: false
STRG m = "abcd1234"
STRG b = GETBETWEEN m ["b"] AND ["3"]
SHOW = b

[IF b IS STRG["cd12"]]
{
STRG outplace = STRG[<here> + "\\testrun.txt"]
OUTP = STRG[READ[outplace] + <newline> + "GETBETWEEN"] HERE[outplace]
(exit)
}