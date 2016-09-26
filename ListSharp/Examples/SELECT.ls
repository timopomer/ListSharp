#ShowDebuggingInformation: True
//demo showing how to use select
ROWS nums = {"1", "12", "123", "1234","12345"}
ROWS chars = { "1","ab","abc","abcd","abcde","abcdef"}

NUMB counter = 0

ROWS s = SELECT FROM nums WHERE[ISNOT "123"] 
[IF s IS ROWS[{"1", "12", "1234","12345"}]]
NUMB counter = counter PLUS 1

SHOW = s

ROWS s = SELECT FROM nums WHERE[IS ANY STRG IN chars] 
[IF s IS ROWS[{"1"}]]
NUMB counter = counter PLUS 1

SHOW = s

ROWS s = SELECT FROM nums WHERE[ISNOT ANY STRG IN chars]
[IF s IS ROWS[{"12", "123", "1234","12345"}]]
NUMB counter = counter PLUS 1

SHOW = s

ROWS s = SELECT FROM nums WHERE[CONTAINS "4"]
[IF s IS ROWS[{"1234","12345"}]]
NUMB counter = counter PLUS 1

SHOW = s

ROWS s = SELECT FROM nums WHERE[CONTAINSNOT "4"] 
[IF s IS ROWS[{"1","12","123"}]]
NUMB counter = counter PLUS 1

SHOW = s

ROWS s = SELECT FROM chars WHERE[CONTAINSNOT EVERY STRG IN nums]
[IF s IS ROWS[{"ab","abc","abcd","abcde","abcdef"}]]
NUMB counter = counter PLUS 1

SHOW = s

ROWS s = SELECT FROM chars WHERE[LENGTH ISOVER EVERY STRG LENGTH IN nums]
[IF s IS ROWS[{"ab","abc","abcd","abcde","abcdef"}]]
NUMB counter = counter PLUS 1

SHOW = s

ROWS s = SELECT FROM nums WHERE[LENGTH ISUNDER 4]
[IF s IS ROWS[{"1","12","123"}]]
NUMB counter = counter PLUS 1

SHOW = s

ROWS s = SELECT FROM nums WHERE[LENGTH ISOVER 2]
[IF s IS ROWS[{"123","1234","12345"}]]
NUMB counter = counter PLUS 1

SHOW = s

ROWS s = SELECT FROM nums WHERE[LENGTH ISEQUAL "123" LENGTH]
[IF s IS ROWS[{"123"}]]
NUMB counter = counter PLUS 1

SHOW = s


[IF counter ISEQUAL 9]
{
STRG outplace = STRG[<here> + "\\testrun.txt"]
OUTP = STRG[READ[outplace] + <newline> + "SELECT"] HERE[outplace]
(exit)
}

