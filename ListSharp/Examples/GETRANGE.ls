#ShowDebuggingInformation: false
STRG m = "abcd1234"
STRG b = GETRANGE m FROM [1] TO [4]
SHOW = b

[IF b IS STRG["abcd"]]
{
STRG outplace = STRG[<here> + "\\testrun.txt"]
OUTP = STRG[READ[outplace] + <newline> + "GETRANGE"] HERE[outplace]
(exit)
}
