# luniASM
An attempt to support bitluni designed assembly code. As far as I can tell the correct bytecode gets generated. Haven't tested yet.

The file `lsm.def` contains the definitions needed to assemble the `GameOfLife.lsm`.
Sadly some idiot (me) made poor code that requires full paths to be used. And had some unuseful errors when file wasn't found. I guess it need some work.

## How to use
```batch
Assembler.exe --import-path "~Full path to~\Examples\luniASM" --import lsm.def "~Full path to~\Examples\luniASM\GameOfLife.lsm"
```
If all goes well it should print something like
```
Written 686 bytes to I:\Programming\Assembler\Examples\luniASM\GameOfLife.o
```
If you add --verbose to the arguments is also prints the addresses of all labels and all translated symbols.

## Changes
To get the .lsm working some small changes had to be made to `GameOfLife.lsm`. Comment had to use ; instead of // and binary numbers needed to be postfixed 101010b instead of prefixed 0x101010