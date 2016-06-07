#ShowDebuggingInformation: false
ROWS a = {"a","b","c"}
ROWS b = {"1","2","3"}
ROWS c = COMBINE[a,b] WITH [" character number:"]
SHOW = c