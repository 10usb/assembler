# How to build a language file
The assembler by it's self doesn't have any platform specific instructions. Only the tools to create one. It has a small set of instructions, the ability to define macro's and enum's. perform some pre assembled expressions.

## Value types
In most instruction sets you want to pass arguments that can then be used by the instruction. To transforms it into the correct byte values. These values can be represented in various ways.
 - numbers
 - strings
 - symbols

 ### numbers
 These can be have various notations decimal, hexidecimal octal and binary. Decimal values can be noted positive and negative other types can only be positive. Hexidecimal number have to be prefixed with `0x` and may contain `0` to `F` for example `0xA1`. Octal number starts with a `0`  and may only contain `0` to `7`. Binary number can only contain a `0` and `1` and must be postfixed by a `b` for binary for example `00101b`. The numbers are internaly stored as a 64 bit signed integer. Therfor if the 64th bit is set with any of the notations it will be treated in the expressions as a negative number.

 ### strings
 A string value it written by placing the text between `"qoutes"` it's use is limited. It can be used as parameter for the `db` instruction to output that value as an UTF-8 string or use it in comparisons.

 ### symbols
 Symbols have no value by them self but can only be used to reference to other variables or labels. Symbol can only contain these characters `a-z A-Z 0-9 $_` and can't start with a number.  
 *NOTE: When a symbol is used in an instruction it's value will be resolved before passing on. If is can find a variable for it in the local, global constant or types scope it will use that value. Otherwise it will be treated as an reference to a label*

## Labels
Labels are the anker point in the documents that can be referenced to. The name has the same restrictions as a symbol with the difference that the `$` is not allowed. A label is defined by a name with a `:` at the end.
```
label:
    db 1
```

A label doesn't have to claim is't own line as long as it's the first thing on a line
```
label: db 1 ; Comments
```

## Instructions
- db - *write a bytes to output*
- org - *set an origin for labels*
- throw - *throw an exception*
- include - *include a other file*
- import - *import definitions*
- file - *embed the contents of a file*

### db
The db instruction is the main instruction to output a value to the output file. It accepts multiple argument. In most cases this will make the code less readable. But it's vary usefull to output a nil terminated string.
```
db "Hello world" 0
```

### org
The org instruction is to set an origin value. Lets consider you're programm starts at address 0x100 and all jumps and data references need to have this offset calculated into them. Then this can be achieved adding this instruction before anything else like
 ```
org 0x100
db label    ; Will output a byte with the value 0x101
label:
    db Hello world

```
It can also be used halfway the document, for code that will be relocation after boot

### include
The include instruction will include an other assembly file into the spot the include is called. Once it has pased the included file it will continue passing the current file. The path can be relative to the current file or an absolute file. If an include folder is passed to the command it will also search in that folder for a file to include with a matching name.
```
include "irq-handlers.inc"
```

### import
The import instruction is similar to the include but with the exception that is only allowes definitions. It's therefor safe to use as language file as it will not disalign any ouput. As with tthe include a folder can be defined where it will try to find files. But while the include only accepts string values the import will also accept symbols as the text value.
```
import CPU7849
```

### file
The file instruction is similar to the include that it will output the bytes at that spot in the file. But it will not parse the contents of that file instead it will copy it's contents to the output.
```
include "image.bmp"
```

### throw
The throw is a globaly supported instrcuctions but it's usefullness it only within macro's. To check if values of arguments are of the correct type, within range or a global counter is below a certain  value.

## Variables
To not have often used number scatterd around the code variables can be defined. A variable can be in `global`, `local` and `const` scope. Variables in the global and const scope can be accesed from any where. Local scope variables can only be accessed in the current scope as the name implies. Variables don't have to be declared with the scope in front of it. Within a macro context it will result into a local variable. When declared in the global context its default scope is global. Local variables declared in the global context can only be access from within that context and is only shared to included files not imported files.
```
global ARRAY_SIZE = 100
```

## Expressions
Sometimes raw values just won't do. You might need a smaller part or a notation using a bit shfting is more readable. Or you might need to perform a small calculation of some configuration variables.
```
db (1 << 5)
```

All expression need to have the parentness brackets and can only have a single operator. To have a sub-expression you have to define parentness brackets again.
```
db (1 << (a + 5))
```

## Macro's
A macro is a small template that can be called as if it's an instruction. When called it will evaluate its contents with its parameters defined as local variables. 
```
macro add a b {
    db 1
    db ((a & 1111b) | (b << 4))
}

add r1 r2
```

Macro's can call other macro's but only the ones defined before it. Macro's can also be refined, allowing to hide earlier defined macro's.
```
macro add a b {
    throw "Unsupported instruction"
}
```

But as a macro can call the macro's defined before it, it can call the macro it hide. This can be handy as it allowes for extending an instruction.
```
macro mov a b {
    if (a is reg16) {
        db 128 a b
    } else {
        mov a b
    }
}
```

## If elseif else
Within a macro an if elseif else construction can be added. This allows to checking variabled values before outputing bytes. Or throw errors when the values are invalid. The If expects a value. followed by a `{` on the same line. It treats alle lines following it as it's contents for when the condition is true. A value is considerd true when it's not equal to `0`. As shown in the example below the if by its self doesn't have parentness brackets for its condition part. But in most cases the condition will be an expression and will therefor have parentness brackets as in other languages.
```
if a {
    db "Hello world"
}
```

## Enumerations
Enumerations by them self don't add much compared to a constant declared variables. As the variables defined within an enum are added to the constant scope. The biggest value of an enum is that the values originated from a enum can be checked on there type in an expression. This can be done with the `is` and `is not` operator, which return 1 on true and 0 on false.

```
enum reg16 {
    ax = 0
    dx = 1
    cx = 2
    bx = 3
}

if (var is reg16) {
    ....
} else {
    throw "var is not a reg16"
}
```

In some cases you might need an macro that only accept values of a certain type to accept an other value that in that edge case is valid. For this end there also the operator `as` which type cast the value to be one of the enum without adding it to the enum.
```
(15 as reg16)
```