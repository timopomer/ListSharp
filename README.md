# ListSharp

A language made for the editing of lists and blocks of text without any programming experience needed to operate it.

* made to be intuitive and easy to understand,with a word rich English based syntax.

* made in c# and opensource for any future improvements people might want to commit.

* made with ease of installation in mind, without any special dependencies besides .net needed.

* made due to the lack of innovation and work done on the automation of such simple tasks.

### Example execution screenshot

![Alpha 0.0.1](http://puu.sh/ltg6F/e0469ab209.PNG)


## Information about the ListSharp language

### 2 variable types
* STRG : 0 dimensional string of data,no particular order: used to store raw text
* ROWS : 1 dimensional array of information: order dependant on the way it is created, manipulated by a wide selection of functions

### List of functions with explanation
#### STRG functions:

##### READ

`READ[(STRG)]`

reads location put between the 2 square brackets

example usage "READ[`<here>`\mytextfile.txt]"

#### ROWS functions:

##### ROWSPLIT

`ROWSPLIT (STRG) BY [(STRG)]`

creates ROWS variable out of a string by splitting it using the parameter between the 2 square brackets

example usage "ROWSPLIT FULLFILE BY [`<newline>`]"

##### EXTRACT

`EXTRACT COLLUM[(NUMBER)] FROM (ROWS) SPLIT BY [(STRG)]`

splits each line of a ROWS variable into multiple segments by a given parameter and extracts the collum asked for

example usage "EXTRACT COLLUM[1] FROM ALLROWS SPLIT BY [":"]"

##### COMBINE

`COMBINE[ROWS,ROWS] WITH [(STRG)]`

combines to ROWS next to each other with a certain string in between the lines

example usage "COMBINE[RIGHTSIDE,LEFTSIDE] WITH ["<>"]"

##### GETLINES

`GETLINES (ROWS) [(INT),(INT)-(INT),....];`

takes certain rows form a ROWS variable into another ROWS variable

example usage "GETLINES RIGHTSIDE [1,4-6];"

#### SPECIAL functions:

##### SHOW

`SHOW = (VAR)`

Shows the variable in the output

example usage "SHOW = ALLROWS"

##### OUTP

`OUTP = (VAR) HERE[(STRG)]`

Outputs a variable to a text file

example usage "OUTP = ALLROWS HERE[`<here>`\output.txt]"

### Special constants

##### newline

`<newline>`

this is the newline constonant that can be used for example to split a list by each row

##### here

`<here>`

this is the current directory constonant that can be used for example to save a file next to the script execution

