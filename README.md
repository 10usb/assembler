# Yet another assembler
Why? Well I'm trying to build my own 8-bit CPU from discrete logic IC's and settled on a instruction set which allows modifiers to be optionally added to every instruction.

Couldn't I've done it with existing universal assemblers? Yes and no. I can define all instructions in "Flat Assembler". But the elegant syntax I'm aiming for is more like the one used in the game "SHENZHEN IO". It uses the + and - sign prefixed before the instruction to give control wether it should be executed or not. From the assemblers I know the best candidate is still "Flat Assembler" and though it used this feature for it's own templating instructions. You can't define it for your own custom instructions.

## Concept
First off to add the modifier option obviously. Also to remove the redundant comma separating the arguments. Once seens that it's redundant it can't be unseen.

To make parsing of expressions easier they have to be inside parentheses brackets.Sub-expression also need to have these parentheses brackets. Benefit of doing it like this, is that operator precedence issue is automaticly resolved.

The instructions should NOT be hardcoded into the assembler. Instead these should be defined as macro's. These in combination with enum's and simple checks should allow for defining all option.

## Basic syntax
```
enum register {
    a = 0
    b = 1
    ; ....
}

macro mov a b {
    if (b is register){
        local opcode = 00000000b
    } else {
        local opcode = 00001000b
    }

    if ($modifier == "+") {
        local opcode = (opcode | 10000000b)
    }

    if ($modifier == "-") {
        local opcode = (opcode | 01000000b)
    }

    db opcode
    db a
    db b
}

main:
    mov A 23
    tlt A (1 << 5)
    + add A 1
```

## Still needs to be added
 - accessing global variables from within macro's
 - defining enum types
 - include other files
 - direct import of other file contents as data
 - User-friendly error messages
 - Think of a better name (cli command?)

 ## Whishes
 - array types using the square brackets [] to allow passing an array of bytes and misusing it as memory addresses or indirect selectors