#ShowDebuggingInformation: True
STRG long5 = "12345"

ROWS long4 = {"1","2","3","4"}

NUMB int2 = 2

NUMB correct = 0;

[IF 4 ISOVER int2 AND 1 ISUNDER 2]
{
SHOW = "and 4 is over 2"
NUMB correct = correct PLUS 1
}


[IF long5 LENGTH ISEQUALOVER long4 LENGTH]
{
SHOW = "5 is bigger than 4 apperantly"
NUMB correct = correct PLUS 1
}


[IF 3 ISUNDER long4 LENGTH]
{
SHOW = "3 is smaller than 4 apperantly"
NUMB correct = correct PLUS 1
}


[IF STRG[long4] IS STRG[long4]]
{
SHOW = "variables are equal to themselfs!"
NUMB correct = correct PLUS 1
}


[IF STRG[long4] ISNOT STRG[long5]]
{
SHOW = "variables are not equal to others!"
NUMB correct = correct PLUS 1
}


[IF long4 CONTAINS "2"]
{
SHOW = "long4 contains 2, how convinent"
NUMB correct = correct PLUS 1
}


[IF long4 CONTAINSNOT "60"]
{
SHOW = "long4 doesnt contain 60, how convinent"
NUMB correct = correct PLUS 1
}

[IF correct ISEQUAL 7]
{
STRG outplace = STRG[<here> + "\\testrun.txt"]
OUTP = STRG[READ[outplace] + <newline> + "IF"] HERE[outplace]
(exit)
}
