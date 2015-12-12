<img src="http://puu.sh/lT74D/d0874336f3.png" alt="ListSharp logo" height="70" >
# ListSharp Programming Language

**Welcome to ListSharp!**

ListSharp is a programming language made in its entirety in c#, with an easy to understand, word-heavy english syntax made to suit the everyday casual kind of person without any knowledge in programming.

The Main Objective of ListSharp is to enable the manipulation of big lists and or blobs of text in an orderly way without learning a programming language,like for example learn Python and maybe get experienced with RegEx JUST to seperate that list of Co-Workers from their birthdays.



### Example execution screenshot

![Alpha 0.1](http://puu.sh/lSDl6/36a222b8ca.png)


## ListSharp Documentation

* made so people without any programming experience can easily operate it.

* made to be intuitive and easy to understand,with a word rich English based syntax.

* made in c# and opensource to enable various features and ease of edit for people which what to commit to this project.

* made with ease of installation in mind, without any special dependencies besides .net needed.

* made due to the lack of innovation and work done on the automation of such simple tasks.

### 2 variable types
* STRG : 0 dimensional string of data,no particular order: used to store raw text
* ROWS : 1 dimensional array of information: order dependant on the way it is created, manipulated by a wide selection of functions

### Associates to .ls files
![File Association](http://puu.sh/lSDir/5497c7ae40.png)

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

