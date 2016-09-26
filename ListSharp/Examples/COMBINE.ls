#ShowDebuggingInformation: false
ROWS a = {"a","b","c"}
ROWS b = {"1","2","3"}
ROWS c = COMBINE[a,b] WITH [" character number:"]
SHOW = c

[IF c IS ROWS["a character number:1","b character number:2","c character number:3"]]
{
STRG outplace = STRG[<here> + "\\testrun.txt"]
OUTP = STRG[READ[outplace] + <newline> + "COMBINE"] HERE[outplace]
(exit)
}
