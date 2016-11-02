#ShowDebuggingInformation: True
ROWS a = {"a","b","c"}
ROWS b = {"1","2","3"}
ROWS c = COMBINE[a,b] WITH [":"]
SHOW = c

//SHOW = EXTRACT COLLUM[INPUT["Give a num"] AS NUMB] FROM c SPLIT BY [":"]

//SHOW = GETLINES c [2 TO 3 AND 1 AND 2 TO c LENGTH PLUS 2]