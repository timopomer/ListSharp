#ShowDebuggingInformation: false
ROWS combinedcharnum = {"a:1","b:2",":3"}
ROWS onlynums = EXTRACT COLLUM[2] FROM combinedcharnum SPLIT BY [":"]
SHOW = onlynums