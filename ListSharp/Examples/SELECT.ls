#ShowDebuggingInformation: True
//demo showing how to use select
ROWS nums = {"1", "12", "123", "1234","12345"}
ROWS chars = { "1","ab","abc","abcd","abcde","abcdef"}

NUMB errorvalue = 0

ROWS s = SELECT FROM nums WHERE[ISNOT "123"] 
//{"1", "12", "1234","12345"}
//[IF s IS ROWS[{"1", "12", "1234","12345"}]]
SHOW = s

ROWS s = SELECT FROM nums WHERE[IS ANY STRG IN chars] 
//{"1"}

SHOW = s

ROWS s = SELECT FROM nums WHERE[ISNOT ANY STRG IN chars] 
//{"12", "123", "1234","12345"}

SHOW = s

ROWS s = SELECT FROM nums WHERE[CONTAINS "4"]
//{"1234","12345"}

SHOW = s

ROWS s = SELECT FROM nums WHERE[CONTAINSNOT "4"] 
//{"1","12","123"}

SHOW = s

ROWS s = SELECT FROM chars WHERE[CONTAINSNOT EVERY STRG IN nums] 
//{"ab","abc","abcd","abcde","abcdef"}

SHOW = s

ROWS s = SELECT FROM chars WHERE[LENGTH ISOVER EVERY STRG LENGTH IN nums]
//{"ab","abc","abcd","abcde","abcdef"}

SHOW = s

ROWS s = SELECT FROM nums WHERE[LENGTH ISUNDER 4]
//{"1","12","123"}

SHOW = s

ROWS s = SELECT FROM nums WHERE[LENGTH ISOVER 2] 
//{"123","1234","12345"}

SHOW = s

ROWS s = SELECT FROM nums WHERE[LENGTH ISEQUAL "123" LENGTH] 
//{"123"}

SHOW = s
