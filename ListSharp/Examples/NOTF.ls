#ShowDebuggingInformation: True
//demo how to show a notification

[FOREACH NUMB IN 1 TO 10000000 AS index]
[IF index ISEQUAL 5000000]
{
NOTF = "got to 5m"


STRG outplace = STRG[<here> + "\\testrun.txt"]
OUTP = STRG[READ[outplace] + <newline> + "NOTF"] HERE[outplace]
(exit)

}