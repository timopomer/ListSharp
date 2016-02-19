#ShowDebuggingInformation: false
//demo showing how to split a STRG or a ROWS variable by something
STRG teststring = "1:2a3:4"
ROWS testrows = ROWSPLIT teststring BY [":"]
SHOW = testrows
ROWS testrows = ROWSPLIT testrows BY ["a"]
SHOW = testrows

SHOW = "at first we split by : and get it into 3 ROWS where one contains an 'a'"
SHOW = "then we split it again and get 4 rows with each one of the with an according number and the 'a' is now missing"

