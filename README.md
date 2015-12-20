<img src="http://puu.sh/lT74D/d0874336f3.png" alt="ListSharp logo" height="70" >
# ListSharp Programming Language

**Welcome to ListSharp!**

ListSharp is a programming language made in its entirety in c#, with an easy to understand, word-heavy english syntax made to suit the everyday casual kind of person without any knowledge in programming.

The Main Objective of ListSharp is to enable the manipulation of big lists and or blobs of text in an orderly way without learning a programming language,like for example learn Python and maybe get experienced with RegEx JUST to seperate that list of Co-Workers from their birthdays.


### Example execution screenshot

![Alpha 0.1](http://puu.sh/lSDl6/36a222b8ca.png)


## ListSharp Documentation

### With what things in mind ListSharp was made

* made so people without any programming experience can easily operate it.
* made to be intuitive and easy to understand,with a word rich English based syntax.
* made in c# and opensource to enable various features and ease of edit for people which what to commit to this project.
* made with ease of installation in mind, without any special dependencies besides .net needed.
* made due to the lack of innovation and work done on the automation of such simple tasks.

### How ListSharp is different

* ListSharp does not need the initializatiion of variables. you can start using all variables you want to as if they exist and assign them values to your liking
* ListSharp is an expandable platform of useful functions for the manipulation of text. want to remove all entries in a rows variable that contains a certain string? a single function will do that for you, no need to study the code or understand how its done.. its like magic.
* ListSharp does not need editing programs to code in, notpad will do!
* ListSharp is extremely easy to understand the logic behind, because it has the logic of a 6 year old. writing ListSharp code is like explaining a computer what to do.
* ListSharp functions are logically named, you will  appreciate the the word rich syntax when it all comes together.

### 2 variable types
* STRG : 0 dimensional string of data,no particular order: used to store raw text
* ROWS : 1 dimensional array of information: order dependant on the way it is created, manipulated by a wide selection of functions

### Associates to .ls files
![File Association](http://puu.sh/lSDir/5497c7ae40.png)

### Follows coding standarts for the creation of variables
`STRG MYSTRG = "I love"`

`ROWS MYLIST = {"My Listsharp",@"<here>"}`

### List of functions with explanation

#### STRG & ROWS functions:

##### ROWSPLIT

`ROWSPLIT (STRG) BY [(VAR)]`

creates ROWS variable out of a string or a row by splitting it using the parameter between the 2 square brackets


###### *example usage:*
```
ROWS AllLines = ROWSPLIT FileContents BY ["<newline>"]
ROWS DotSplit = ROWSPLIT AllLines BY ["."]
```
___

##### REPLACE

`REPLACE[(STRG),(STRG)]`

repalces a certain string by another string in a rows/strg variable

###### *example usage:*
```
SRTG Mylang = REPLACE["python","ListSharp"]
ROWS Mylangs = REPLACE["python","ListSharp"]
```
___


#### STRG functions:

##### READ

`READ[(STRG)]`

reads location put between the 2 square brackets

###### *example usage:*
```
SRTG FileContents = READ[<here>\mytextfile.txt]
```
___

#### ROWS functions:

##### EXTRACT

`EXTRACT COLLUM[(INT)] FROM (ROWS) SPLIT BY [(STRG)]`

splits each line of a ROWS variable into multiple segments by a given parameter and extracts the collum asked for

###### *example usage:*
```
ROWS LeftSide = EXTRACT COLLUM[1] FROM ALLROWS SPLIT BY [":"]
```
___

##### COMBINE

`COMBINE[ROWS,ROWS] WITH [(STRG)]`

combines two ROWS next to each other with a certain string in between the lines

###### *example usage:*
```
ROWS BothSides = COMBINE[RIGHTSIDE,LEFTSIDE] WITH ["<>"]
```
___

##### GETLINES

`GETLINES (ROWS) [(INT),(INT)-(INT),....]`

takes certain rows form a ROWS variable into another ROWS variable

###### *example usage:*
```
ROWS BestLines = GETLINES RIGHTSIDE [1,4-6]
```
___

#### SPECIAL functions:

##### SHOW

`SHOW = (VAR)`

Shows a variable in the output

###### *example usage:*
```
SHOW = ALLROWS
SHOW = "We can write what we want here"
```
___

##### OUTP

`OUTP = (VAR) HERE[(STRG)]`

Outputs a variable to a text file

###### *example usage:*
```
OUTP = ALLROWS HERE[<here>\output.txt]
```
___

### Special constants

##### newline

`<newline>`

this is the newline constonant that can be used for example to split a list by each row
___
##### here

`<here>`

this is the current directory constonant that can be used for example to save a file next to the script execution
___
### Launching arguments

##### #ShowDebuggingInformation

```
#ShowDebuggingInformation: false
#ShowDebuggingInformation: true
```

Enables or disables the showing of debugging information in the output of the console like the interperted c# code
