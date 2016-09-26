#ShowDebuggingInformation: false
ROWS chars = {"a","b","c","d","e","g"}
NUMB counter = 0
[FOREACH STRG IN chars AS character]
{
SHOW=character
NUMB counter = counter PLUS 1
}

SHOW = "we just displayed all STRGs inside of the ROWS one by one"
[IF counter ISEQUAL 6]
{
STRG outplace = STRG[<here> + "\\testrun.txt"]
OUTP = STRG[READ[outplace] + <newline> + "FOREACH"] HERE[outplace]
(exit)
}
