
enum register {
	r0 = 0
	r1 = 1
	r2 = 2
	r3 = 3
	r4 = 4
	r5 = 5
	r6 = 6
	r7 = 7
	r8 = 8
	r9 = 9
	ra = 10
	rb = 11
	rc = 12
	rd = 13
	re = 14
	rf = 15
	
	sil	= 132
	sih	= 133
	sx	= 134
	sc	= 135
	
	dil	= 136
	dih	= 137
	dx	= 138
	dc	= 139
	
	eil	= 140
	eih	= 141
	ex = 142
	ec = 143
	
	bpl	= 144
	bph	= 145
	bx = 146
	bo = 147
	
	remi = 148
	qout = 149
}

macro instr code a b {
	if ($modifier = "+") {
		code = (code | 01000000b)
	} elseif ($modifier = "-") {
		code = (code | 10000000b)
	} elseif ($modifier = "&") {
		code = (code | 11000000b)
	}
	
	if (a is not register) {
		throw "First argument is not a register"
	}
	
	if (b is not register) {
		code = (code | 00001000b)
	}
	
	db code
	db a
	db b
}

macro mov a b {
	$& instr 0 a b
}

macro add a b {
	$& instr 00110000b a b
}
macro sub a b {
	$& instr 00110001b a b
}
macro and a b {
	$& instr 00110010b a b
}
macro or a b {
	$& instr 00110011b a b
}
macro xor a b {
	$& instr 00110100b a b
}
macro shl a b {
	$& instr 00110110b a b
}
macro shr a b {
	$& instr 00110111b a b
}

macro teq a b {
	$& instr 00100000b a b
}
macro tgt a b {
	$& instr 00100001b a b
}
macro tlt a b {
	$& instr 00100010b a b
}
macro tcp a b {
	$& instr 00100011b a b
}

macro tcc a b {
	$& instr 00100100b a b
}
macro tbe a b {
	$& instr 00100101b a b
}
macro tab a b {
	$& instr 00100110b a b
}
macro tcps a b {
	$& instr 00100111b a b
}
macro instr code a b {
	throw "Unknown instruction 'instr'"
}

enum mem16 {
	si = 1
	di = 2
	ei = 3
	bp = 4
}

macro jmp L {
	$& mov (128 as register) (L & 0xFF)
	$& mov (129 as register) ((L >> 8) & 0xFF)
}

macro mov a b {
	if (a is mem16) {
		if (b is mem16) {
			$& mov ((128 + (a << 2)) as register) ((128 + (b << 2)) as register)
			$& mov ((129 + (a << 2)) as register) ((129 + (b << 2)) as register)
		} elseif (b is not register) {
			$& mov ((128 + (a << 2)) as register) (b & 0xFF)
			$& mov ((129 + (a << 2)) as register) ((b >> 8) & 0xFF)
		} else {
			throw "Can't set a 16bit register with a none imm value"
		}
	} else {
		$& mov a b
	}
}

macro stack a {
	mov ec 15
	mov ei a
}

macro push a {
	$& mov ex a
}

macro pop a {
	$& mov a ex
}

macro call L {
	$& push ((return >> 8) & 0xFF)
	$& push (return & 0xFF)
	$& jmp L
	return:
}

macro ret {
	$& pop (128 as register)
	$& pop (129 as register)
}

macro div a b {
	$& mov remi a
	$& mov qout b
}