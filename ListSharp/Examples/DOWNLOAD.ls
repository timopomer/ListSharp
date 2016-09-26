#ShowDebuggingInformation: false
STRG url = @"http://github.com"
STRG html = DOWNLOAD[url]
SHOW = html


STRG outplace = STRG[<here> + "\\testrun.txt"]
OUTP = STRG[READ[outplace] + <newline> + "DOWNLOAD"] HERE[outplace]
(exit)

