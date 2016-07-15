#ShowDebuggingInformation: true
//demo how to debug with DEBG

ROWS nums = {"1","2","3","4","5","6","7","8","9"}
NUMB badlines = 0

[FOREACH STRG IN nums AS singlenum]
[IF singlenum ISNOT "3"]
NUMB badlines = badlines+1

DEBG = badlines + " lines arent 3, heres the variable:"
DEBG = nums 