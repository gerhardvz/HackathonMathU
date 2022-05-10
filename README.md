# HackathonMathU
Math Silimarity Indexing

To run. Put MathML text into a file with .ml extention. In coding point to file to load and get hash of the math ML. Use Element.parseMathMLFile method to load file.
Please to parse MathML file the computer must be connected to the internet to download the MathML DTD file.

Hash 4 - Hashses the whole tree including numbers and identifiers
Hash 5 - Hashses the only the operators to match similar structure

Both hashes are capable of identifying math formulas that are swapped over an = sign and a + sign.
