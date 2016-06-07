#ShowDebuggingInformation: false
ROWS chars = {"a","b","c","d","e","g"}

[FOREACH STRG IN chars AS character]
{
SHOW=character
}

SHOW = "we just displayed all STRGs inside of the ROWS one by one"