#ShowDebuggingInformation: false
//demo showing how to use select
ROWS nums = {"1", "12", "123", "1234","12345"}
ROWS chars = { "1","ab","abc","abcd","abcde","abcdef"}

ROWS s = SELECT FROM nums WHERE[EVERY STRG ISNOT "123"]  //{ ""1", "12", "1234","12345""}
ROWS s = SELECT FROM nums WHERE[EVERY STRG IS ANY STRG IN chars]  //{ "1"}
ROWS s = SELECT FROM nums WHERE[EVERY STRG ISNOT ANY STRG IN chars]  //{ "12", "123", "1234","12345"}
ROWS s = SELECT FROM nums WHERE[EVERY STRG CONTAINS "4"]  //{ "1234","12345"}
ROWS s = SELECT FROM nums WHERE[EVERY STRG CONTAINSNOT "4"]  //{ "1","12","123"}
ROWS s = SELECT FROM chars WHERE[EVERY STRG CONTAINSNOT EVERY STRG IN nums]  //{"ab","abc","abcd","abcde","abcdef"}
ROWS s = SELECT FROM chars WHERE[EVERY STRG LENGTH ISOVER EVERY STRG LENGTH IN nums]  //{"ab","abc","abcd","abcde","abcdef"}
ROWS s = SELECT FROM nums WHERE[EVERY STRG LENGTH ISUNDER 4]  //{ "1","12","123"}
ROWS s = SELECT FROM nums WHERE[EVERY STRG LENGTH ISOVER 2]  //{ "123","1234","12345"}
ROWS s = SELECT FROM nums WHERE[EVERY STRG LENGTH ISEQUAL "123" LENGTH]  //{ "123"}

SHOW = s
//move the show=s around to see what each line does