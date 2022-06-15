# Yet another assembler
Why? Well I'm trying to build my own 8-bit CPU from discrete logic IC's and settled on a instruction set which allows modifiers to be optionally added to every instruction.

Couldn't I've done it with existing universal assemblers? Yes and no. I can define all instructions in "Flat Assembler". But the elegant syntax I'm aiming for is more like the one used in the game "SHENZHEN IO". It uses the + and - sign prefixed before the instruction to give control wether it should be executed or not. From the assemblers I know the best candidate is still "Flat Assembler" and though it used this feature for it's own templating instructions. You can't define it for your own custom instructions.

## Concept
First off to add the modifier option obviously. Also to remove the redundant comma separating the arguments. Once seens that it's redundant it can't be unseen.

To make parsing of expressions easier they have to be inside parentheses brackets. For now sub-expression also need to have these parentheses brackets. Benefit of doing it like this, is that operator precedence issue is automaticly resolved. And the assembler doesn't have to become very complicated.

Templating, the instructions should NOT be hardcoded into the assembler. Instead these should be defined with macro's. These in combination with enum's and simple checks should allow for most things.

## Basic syntax
```
import "other.file"

enum register {
    a = 0
    b = 1
    ; ....
}

; globally accessible and can't be changed
const NUM_DRIVES 4

; global variable, prefixing with global is optional
counter = 0

; can only be used in code local to the global code
local varX = "Hello world"

macro mov a b {
    if (counter > 100){
        throw "Can't move more then 100 times"
    }

    ; To set a global variable from within a macro is has to be prefixed
    global counter = counter + 1

    if (b is register){
        opcode = 00000000b
    } else {
        opcode = 00001000b
    }

    if ($modifier == "+") {
        opcode = (opcode | 10000000b)
    } elseif ($modifier == "-") {
        opcode = (opcode | 01000000b)
    }

    db opcode
    db a
    db b
}

main:
    mov r0 23
    tlt r0 (1 << 5)
+   add r0 1

image:
    file "binary.file"
```

## Still needs to be added
 - import/including other files from within an import context
 - Think of a better name (cli command?)
 - Wiki pages

 ## Whishes
 - array types using the square brackets [] to allow passing an array of bytes and misusing it as memory addresses or indirect selectors
 - build-in $ variables for offset and origin values
 - import/including other files from within a macro