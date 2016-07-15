#ShowDebuggingInformation: true
STRG long5 = "12345"

ROWS long4 = {"1","2","3","4"}

NUMB int2 = 2

[IF long5 LENGTH ISOVER long4 LENGTH]
SHOW = "5 is bigger than 4 apperantly"

[IF 4 ISOVER int2]
SHOW = "and 4 is over 2"

[IF long4 IS long4]
SHOW = "variables are equal to themselfs!"

[IF long4 CONTAINS "2"]
SHOW = "long4 contains 2, how convinent"