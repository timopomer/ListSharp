#ShowDebuggingInformation: True
#DownloadMaxTries: 5

STRG testscripts = GETBETWEEN <here> [""] AND ["Testing"] + "Examples"
OUTP = "" HERE[STRG[testscripts+"\\testrun.txt"]]

ROWS files = <c#System.IO.Directory.GetFiles(testscripts) c#>
<c#System.IO.File.Delete(testscripts+"\\testrun.txt"); c#>
(hide)
[FOREACH STRG IN files AS file]
{
OPEN = HERE[file]
}

SHOW = "Test completed!\nif this passed no windows but this one should be open"