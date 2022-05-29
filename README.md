# Yet an another assembler
Why? Well I'm trying to build my own 8-bit CPU from discrete logic IC's and settled
on a instruction set which requires modifiers to be optionally added for every instruction.

Couldn't I've done it with existing universal assemblers? Yes and no. I can define all
instructions in "Flat Assembler". But the elegant syntax I'm aiming for is the one more like 
the one used in the game "SHENZHEN IO". It uses a + and - sign prefixed before the instruction.
This gives control wether it should be executed. The best candidate is "Flat Assembler" and
though it used this feature for it's own templating instructions. You can't define it for your
own custom defined ones.

## Concept
At first to add the modifier option obviously. Also to remove the redundant comma separating 
the arguments. To make parsing easier expressions need to be inside parentheses brackets. 
Sub-expression need to have these parentheses brackets aswell. Benefit of this is that operator
precedence issue is automaticly resolved.

The instruction should NOT be hardcoded into the assembler. Instead these should be defined as
macro's. These in combination with enum's and simple checks should allow for defining all option.

## Basic syntax
```asm
enum register {
    a = 0
    b = 1
    ; ....
}

macro mov a b {
    if (b is register){
        local opcode = 00000000b
    }else{
        local opcode = 00001000b
    }

    if($modifier == "+"){
        local opcode = (opcode | 10000000b)
    }

    if($modifier == "-"){
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
 - if/elseif/else processing
 - defining enum types
 - include other files
 - direct import of other file contents as data
 - User-friendly error messages
 - Think of a better name