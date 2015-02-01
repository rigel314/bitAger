# bitAger
C# project to pull bitpacked values out of a file.

### Usage
`bitAger file -d descfile [-n]`<br />
pass in a file and a descriptor file and interpret values from `file` using the description in `descfile`<br />
`-n` means no GUI (there is currently not a working GUI)<br />
`-d` provides a descriptor file

#### Descriptor file example
uint width 16 B<br />
sint height 14 B<br />
spare spare1 2 B<br />
uint size 16 L

### Current supported functionality
- unsigned ints < 65bits
 - just hexdumps bigger ones
- signed ints < 65bits
 - just hexdumps bigger ones
- spare type to skip bits
- little and big endianness
- mono compatible

### Future support
- floating point numbers
 - with customizable exponent and mantissa
- fixed point numbers
 - with customizable 
