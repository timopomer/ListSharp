#ShowDebuggingInformation: false


//part1

STRG MYSTRG = "<here>"
ROWS MYLIST = {@MYSTRG,"1234:abcd"}
ROWS MYLIST = REPLACE["\\",":"]
SHOW = "Im going to show you some tricks"
SHOW = MYLIST
ROWS MYLIST = ROWSPLIT MYLIST BY [":"]
SHOW = MYLIST

//part2

STRG ThisCodePath = "<here>\test_features.ls"
STRG ThisCode = READ[ThisCodePath]
SHOW = ThisCode
ROWS SplitCode = ROWSPLIT ThisCode BY [<newline>]
SHOW = SplitCode
ROWS RIGHTSIDE = EXTRACT COLLUM[1] FROM SplitCode SPLIT BY ["="]
ROWS LEFTSIDE = EXTRACT COLLUM[2] FROM SplitCode SPLIT BY ["="]
SHOW = RIGHTSIDE
SHOW = LEFTSIDE
ROWS OGCODE = COMBINE[RIGHTSIDE,LEFTSIDE] WITH ["="]
SHOW = OGCODE

//part3

ROWS BestLines = GETLINES OGCODE [2,4-6]
SHOW = BestLines
OUTP = BestLines HERE["<here>\output.txt"]

SHOW = "Everything should be done now"
