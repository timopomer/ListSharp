#ShowDebuggingInformation: false
//demo showing how to read files

//you can directly read a STRG
STRG FileContents = READ[<here>+@"\READ.ls"]
SHOW = FileContents

//also create a STRG that contains the path
STRG FileLocation = <here>+@"\READ.ls"
STRG FileContents = READ[FileLocation]
SHOW = FileContents