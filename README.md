<img src="http://yoram.de/listsharp.svg" alt="ListSharp logo" height="120" >

# ListSharp Programming Language

**Welcome to ListSharp!**

ListSharp is a programming language made in its entirety in c#, with an easy to understand, word-heavy english syntax made to suit the everyday casual kind of person without any knowledge in programming.

The Main Objective of ListSharp is to enable the manipulation of big lists and or blobs of text in an orderly fashion without learning a general use programming language, like for example learn Python and maybe get experienced with RegEx just to save a list of products on a webpage to a file.

### Example execution screenshot

![Alpha 0.1](http://puu.sh/lSDl6/36a222b8ca.png)


## ListSharp Documentation

### With what things in mind ListSharp was made

* made so people without any programming experience can easily operate it.
* made to be intuitive and easy to understand, with a word rich English based syntax.
* made in c# and opensource to enable various features and ease of edit for people which what to commit to this project.
* made with ease of installation in mind, without any special dependencies besides .net needed.
* made due to the lack of innovation and work done on the automation of such simple tasks.

### How ListSharp is different

* ListSharp does not need the initializatiion of variables. you can start using all variables you want to as if they exist and assign them values to your liking
* ListSharp is an expandable platform of useful functions for the manipulation of text. want to remove all entries in a rows variable that contains a certain string? a single function will do that for you, no need to study the code or understand how its done.
* ListSharp does not need editing programs to code in, notpad will do!
* ListSharp is extremely easy to understand the logic behind, because it has the logic of a 6 year old. writing ListSharp code is like explaining a computer what to do.
* ListSharp functions are logically named, you will appreciate the the word rich syntax when it all comes together.

### 2 variable types
* STRG : 0 dimensional string of data, used to store raw text
* ROWS : 1 dimensional array of strings

### Associates to .ls files
![File Association](http://puu.sh/lSDir/5497c7ae40.png)

### Follows c# standarts for the creation/addition of variables
```
STRG MYSTRG = "I love"
ROWS MYLIST = {"My Listsharp",@"<here>"}
STRG MYSTRGS = MYSTRG+" you"
```
yet innovates where as you dont need to declare a variable to use it, it exists the moment you try to access it

for example
```
ROWS c = ADD["a","b"] TO c
```
c exists when you try to add 2 STRG's to it, even tho its declaraition should in theory only happen after you did that -> all variable names exist as empty placeholders before their use
