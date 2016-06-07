<img src="http://yoram.de/listsharp.svg" alt="ListSharp logo" height="120" >

# ListSharp Programming Language

**Welcome to ListSharp!**

ListSharp is a programming language made in its entirety in c#, with an easy to understand, word-heavy english syntax made to suit the average joe without previous and or limited knowledge in programming.

The Main Objective of ListSharp is to enable the manipulation of big lists and or blobs of text in an orderly fashion without learning a general use programming language, like for example learn Python and maybe get experienced with RegEx just to save a list of products on a webpage to a file.

ListSharp comes pre equipped with powerful algorithmic tools information extraction and pre existing easy to understand concepts for  for web scraping and automation. 

### Example execution screenshot of console

![Beta](http://puu.sh/pkwVK/89d09e42ed.png)


## ListSharp Documentation

### With what things in mind ListSharp was made

* made so people without any programming experience can easily operate it.
* made to be intuitive and easy to understand, with a word rich English based syntax.
* made in c# and opensource to enable various features and ease of edit for people which what to commit to this project.
* made with ease of installation in mind, without any special dependencies besides .net needed.
* made due to the lack of easy to master languages for automation.

### How ListSharp is different

* ListSharp does not need the initializatiion of variables. you can start using all variables you want to as if they exist and assign them values to your liking
* ListSharp is an expandable platform of useful functions for the manipulation of text. most manipulation algorithems are single liners without the need to study the code or understand how its done you can get everything up and running in minutes.
* ListSharp does not need editing programs to code in, while notpad will do an editor is on the way
* ListSharp is extremely simplistic, writing ListSharp code is like explaining the computer what you want just like in your head.
* ListSharp functions are logically named, you will appreciate the the word rich syntax when you read it after some time.

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
