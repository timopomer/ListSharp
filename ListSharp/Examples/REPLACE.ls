#ShowDebuggingInformation: True
//demo showing how to replace a STRG by another in a STRG/ROWS
STRG teststring = "Hey my name is c#"
STRG mystring = REPLACE ["c#"] WITH ["ListSharp"] IN teststring
SHOW = mystring

[IF mystring IS STRG["Hey my name is ListSharp"]]
{
STRG outplace = STRG[<here> + "\\testrun.txt"]
OUTP = STRG[READ[outplace] + <newline> + "REPLACE"] HERE[outplace]
(exit)
}
