#ShowDebuggingInformation: true


//replace STRG
STRG characters = "abcdefghijklmnopqrstuv1111"
STRG characters = REPLACE["1111","wxyz"] IN characters
SHOW = "characters: " + characters


//replace ROWS
ROWS nums = {"a","2","3","4","5","6","7","8","9","a0","aa","a2","a3","a4","a5","a6","a7","a8","a9"}
ROWS nums = REPLACE["a","1"] IN nums
SHOW = "nums: "
SHOW = nums


//getbetween STRG
STRG characters = "1111abcd1111"
STRG characters = GETBETWEEN characters ["1111"] AND ["1111"]
SHOW = "characters: " + characters


//getbetween ROWS
ROWS nums = {"a1a","a2a","a3a","a4a"}
ROWS nums = GETBETWEEN nums ["a"] AND ["a"]
SHOW = "nums: "
SHOW = nums


//getrange STRG
STRG characters = "1111abcd1111"
STRG characters = GETRANGE characters FROM [5] TO [8]
SHOW = "characters: " + characters



//getrange ROWS
ROWS nums = {"a1a","a2a","a3a","a4a"}
ROWS nums = GETRANGE nums FROM [2] TO [3]
SHOW = "nums: "
SHOW = nums


//read
STRG fullcode = READ[<here> + "\\test_features.ls"]
SHOW = "full code: " + fullcode


//download
STRG html = DOWNLOAD[@"https://github.com/timopomer/ListSharp/wiki/List-of-functions-by-type"]
SHOW = "full html: " + html


//rowsplit
ROWS allrows = ROWSPLIT html BY [<newline>]
SHOW = "all html rows: "
SHOW = allrows


//select
ROWS filtered = SELECT FROM allrows WHERE[EVERY STRG CONTAINS "a href"]
ROWS filtered = SELECT FROM filtered WHERE[EVERY STRG CONTAINS "<td>"]
SHOW = "all filtered html rows: "
SHOW = filtered


//extract
ROWS extracted = EXTRACT COLLUM[2] FROM filtered SPLIT BY ["<td>"]
SHOW = "all extracted html rows: "
SHOW = extracted


//getlines
ROWS gotten = GETLINES filtered [1 TO 13]
SHOW = "all gotten html rows: "
SHOW = gotten



//getbetween to get commands
ROWS between = GETBETWEEN gotten ["\">"] AND ["</a>"]
SHOW = between


//add
ROWS added = ADD[between,"A COMMAND I WANT"] TO added
SHOW = added


//combine
ROWS emptyrows = {""}
ROWS combined = COMBINE[emptyrows,added] WITH ["COMMAND: "]
SHOW = "combined rows: "
SHOW = combined


//outp
OUTP = combined HERE[<here>+"//commands.txt"]



SHOW = "If this compiled,were in luck! \n Everything should be golden - this displays all functions"
