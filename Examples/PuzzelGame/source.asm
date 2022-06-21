import R322

const vidx = (160 as register)
const vidy = (161 as register)
const vid = (162 as register)

const keyp = (164 as register)
const keyd = (165 as register)

const map_addres = 0x8000

stack 0x8080

load_next:
	call load_map
	call draw_map

readkey:
	tgt keyp 0
-	jmp readkey
	mov r0 keyd
	teq r0 53
+	jmp load_next
	call move
	teq ra re
+	jmp load_next
	call check
-	jmp readkey
	add rf 1
	jmp load_next

hang:
jmp hang

check:
	mov r0 0
	mov r1 1
	mov sc 9
	mov si map_addres
	
check_tile:
	mov r2 sx
	teq r2 010b
+	mov r1 0
	teq r2 100b
+	mov r1 0

	add r0 1
	tlt r0 64
+	jmp check_tile

	teq r1 1
	ret

move:
	teq r0 69
+	mov r1 -1
+	mov r2 0
	teq r0 70
+	mov r1 0
+	mov r2 -1
	teq r0 71
+	mov r1 1
+	mov r2 0
	teq r0 72
+	mov r1 0
+	mov r2 1
	
	mov r3 rc
	mov r4 rd
	add r3 r1
	add r4 r2
	; Make sure we don't move out the map
	tlt r3 8
-	ret
	tlt r4 8
-	ret
	
	; Translate tile next
	mov bp map_addres
	mov bo r4
	shl bo 3
	or bo r3
	
	; Can walk throug walls
	teq bx 1
+	ret

	mov r5 bx
	and r5 11b
	teq r5 10b
-	jmp move_user

	; Calculate the place the box need to move to
	mov r5 r3
	mov r6 r4
	add r5 r1
	add r6 r2
	
	; Make sure we don't move out the map
	tlt r5 8
-	ret
	tlt r6 8
-	ret

	mov bo r6
	shl bo 3
	or bo r5
	
	mov r7 bx
	and r7 11b
	; Can't move a box into something else
	teq r7 0
-	ret
	
	; Set box
	mov bo r6
	shl bo 3
	or bo r5
	and bx 100b
	or bx 10b
	
	push r0
	push r1
	mov r0 r5
	mov r1 r6
	call update_map
	pop r1
	pop r0

move_user:
	; Remove used
	mov bo rd
	shl bo 3
	or bo rc
	and bx 100b
	
	push r0
	push r1
	mov r0 rc
	mov r1 rd
	call update_map
	pop r1
	pop r0
	
	; Set user
	mov bo r4
	shl bo 3
	or bo r3
	and bx 100b
	or bx 11b
	
	push r0
	push r1
	mov r0 r3
	mov r1 r4
	call update_map
	pop r1
	pop r0
	
	mov rc r3
	mov rd r4
	
	; Increment the used steps
	add re 1
	
	mov vidy 5
	div re 10
	mov r0 remi
	add r0 48
	mov vidx 4
	mov vid r0
	
	div qout 10
	mov r0 remi
	add r0 48
	mov vidx 3
	mov vid r0
	
	mov r0 qout
	add r0 48
	mov vidx 2
	mov vid r0
	
	ret

update_map:
	push bpl
	push bph
	mov vidx 7
	add vidx r0
	mov vidy 1
	add vidy r1
	
	mov bp map_addres
	mov bo r1
	shl bo 3
	or bo r0
	
	mov r0 bx
	mov bp charmap
	mov bo r0
	mov vid bx

	pop bph
	pop bpl
	ret


load_map:
	mov r0 0
	mov re 0
	mov si (maps + 3)
	
next_map:
	teq r0 rf
+	jmp found_map
	add sil 69
&	add sih 1
	add r0 1
	jmp next_map
found_map:
	
	mov sc 9
	mov dc 9
	mov di map_addres
	mov ra sx
	mov rb sx

	mov bp translate
	mov r0 0
	mov r1 0
read_map:
	mov r2 sx
	mov bo r2
	mov dx bx
	teq r2 3	; is current square a player
+	mov rc r1
+	mov rd r0
	add r1 1
	
	tlt r1 8
+	jmp read_map
	mov r1 0
	add r0 1
	tlt r0 8
+	jmp read_map
	ret

draw_map:

	mov vidy 1
	mov vidx 1
	mov si limit_text
	mov r0 0
copy_limit:
	mov vid sx
	add r0 1
	tlt r0 5
+	jmp copy_limit

	mov vidy 2
	div ra 10
	mov r0 remi
	add r0 48
	mov vidx 4
	mov vid r0
	
	div qout 10
	mov r0 remi
	add r0 48
	mov vidx 3
	mov vid r0
	
	mov r0 qout
	add r0 48
	mov vidx 2
	mov vid r0

	mov vidy 4
	mov vidx 1
	mov si steps_text
	mov sc 9
	mov r0 0
copy_steps:
	mov vid sx
	add r0 1
	tlt r0 5
+	jmp copy_steps

	mov vidy 5
	mov vidx 2
	mov vid 48
	mov vid 48
	mov vid 48

	mov vidy 1
	mov r0 0
	mov sc 9
	mov si map_addres
	mov bp charmap
	
draw_line:
	mov r1 0
	mov vidx 7

draw_char:
	mov bo sx
	mov vid bx
	add r1 1
	tlt r1 8
+	jmp draw_char
	
	add r0 1
	tlt r0 8
+	add vidy 1
+	jmp draw_line
	
	ret

; empty	= 0
; wall	= 1
; box	= 2
; unit	= 3
; dot	= 4
translate:
	db 1 2 4 3 0 6 7
charmap:
	db 32 219 176 1 42 3 178 2
	
limit_text: db "limit"
steps_text: db "steps"

maps:
	file "maps/stage01.map"
	file "maps/stage02.map"
	file "maps/stage03.map"
	file "maps/stage04.map"
	file "maps/stage05.map"
	; ..........
	; ..........
	file "maps/stage55.map"