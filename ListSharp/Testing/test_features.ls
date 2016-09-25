#ShowDebuggingInformation: True
#DownloadMaxTries: 5

STRG testscripts = GETBETWEEN <here> [""] AND ["Testing"] + "Examples"
OUTP = "" HERE[STRG[testscripts+"\\testrun.txt"]]
SHOW = testscripts
ROWS files = <c#System.IO.Directory.GetFiles(testscripts) c#>
(hide)
[FOREACH STRG IN files AS file]
{
//OPEN = HERE[file]
}

