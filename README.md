# NiohDecryptor
File Decyptor for Nioh 1 and 2  
the PC version of both Nioh and Nioh 2 encrypt most of their model files making them impossible to view or edit.
This program can decrypt files from both games using their default keys or decrypt other files with a custom key.  

###### About the Encyption
Nioh 1 and 2 both use a tweaked version of XOR encryption, if the byte is 0x0 or the resulting byte would equal 0x0 then the byte is ignored otherwise it's XOR based on the given key. Both games also use a 32 byte header to tell the game if the file in question is encrypted or not, nioh 1 is all 0x0 bytes while nioh 2 uses a unique string. Because of this it is unnecessary to re-encrypt files as without the header the game not try to decrypt them. This header will be trimmed by default when decrypting.

### Options
| Arugment | Type | Info | 
| -------- | ---- | ---- |
| k, key | string | custom XOR Key used for decryption |
| nioh1 | bool | Uses the default XOR key for Nioh 1 for decryption |
| nioh2 | bool | Uses the default XOR key for Nioh 2 for decryption |
| t, NoTrim | bool | Disables trimming the first 32 bytes |
| c, NoCheck | bool | Bypasses encyption header check and decypts file regardless |
| i, infile | string | Input File to decrypt |

### Credits
[Joschuka](https://github.com/Joschuka) - Help with finding the XOR key and setting up decryption