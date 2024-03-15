/*
Hvad betyder tegn?
? = 0 eller 1 occurence
* = 0 eller flere occurences
+ = 1 eller flere occurences
| = or
*/

grammar Syntax;

program: 'program' wsc '(' wsc (statement | function | comment | controlStructures)* wsc ')' EOF;

value: use | flag | identifier | mathExpression | textExpression | lengthOf | type_convert | listElement;
lengthOf: l e n g t h o f wsc '(' wsc identifier wsc ')';
type_convert: (identifier | use) wsc a s wsc type;
listElement: identifier wsc '(' wsc nonZeroNumber wsc ')';

type: n u m b e r | t e x t | f l a g;
identifier: character (digit | character)*;

number: ('-')? nonZeroNumber | '0';
nonZeroNumber: nonZeroDigit digit*;
digit: '0' | nonZeroDigit;
nonZeroDigit: '1' | '2' | '3' | '4' | '5' | '6' | '7' | '8' | '9' ;

flag: t r u e | f a l s e;

text: '"' (character | symbol | digit)* '"';
textWithoutNewlineOrQuotationmarks: (character | symbolWitoutNewline | digit)*;



controlStructures: (loop | if_else) wsc;
loop: r e p e a t wsc w h i l e wsc booleanExpression wsc d o '(' wsc (statement | controlStructures)* wsc ')';
//If true/false do something, or continue with more if, optional else at the end.
if_else: i f wsc booleanExpression wsc d o wsc '(' wsc (statement | controlStructures)* ')' (wsc e l s e wsc '(' wsc (statement | controlStructures)* ')')?;

function: c r e a t e wsc f u n c t i o n a l i t y wsc identifier wsc takesArgument? wsc givesArgument? '(' wsc (statement | controlStructures)* wsc ')' wsc;
takesArgument: t a k e s wsc '(' wsc parameter (wsc',' wsc parameter)* wsc ')';
givesArgument: g i v e s wsc type;
parameter: type wsc identifier;
nothing: n o t h i n g;



statement: (assignment | create | return | break | use | write | read) wsc ';' wsc;

assignment: identifier wsc '=' wsc value | textAssign | mathAssign;
mathAssign: identifier wsc assignOp wsc mathExpression;
textAssign: identifier wsc textOperator wsc textExpression;

assignOp: '=' | mathematicalOperator;

// Keyword for making new variables.
create: c r e a t e wsc (n u m b e r wsc identifier ( wsc '=' wsc mathExpression)? | t e x t wsc identifier ( wsc '=' wsc textExpression)? |
        f l a g wsc identifier ( wsc '=' wsc flag)? | l i s t wsc o f wsc type wsc identifier wsc ('(' wsc (value (wsc ',' wsc value)*)? wsc ')')?);

return: r e t u r n s wsc (identifier | use | nothing)?;
break: b r e a k;
use: u s e wsc identifier wsc ('(' wsc (identifier (wsc ',' wsc identifier)* wsc)? ')')?;
write: w r i t e wsc '(' wsc textExpression wsc ')';
read: r e a d wsc ('(' wsc ')')?;



mathExpression: identifier | listElement | number | mathExpression wsc mathematicalOperator wsc mathExpression | '(' wsc mathExpression wsc mathematicalOperator wsc mathExpression wsc ')' | type_convert;
textExpression: identifier | listElement | text | textExpression wsc textOperator wsc textExpression | '(' wsc textExpression wsc textOperator wsc textExpression wsc ')' | type_convert;
booleanExpression: identifier | listElement | flag | booleanExpression wsc booleanOperator wsc booleanExpression | '(' wsc booleanExpression wsc booleanOperator wsc booleanExpression wsc ')' | type_convert;

mathematicalOperator: '+' | '-' | '*' | '/' | 'modulo';
textOperator: '+';
booleanOperator: '=' | '>' | '<' | '<=' | '>=' | a n d | o r;



character: 'A' | 'B' | 'C' | 'D' | 'E' | 'F' | 'G' | 'H' |
           'I' | 'J' | 'K' | 'L' | 'M' | 'N' | 'O' | 'P' |
           'Q' | 'R' | 'S' | 'T' | 'U' | 'V' | 'W' | 'X' |
           'Y' | 'Z' | 'a' | 'b' | 'c' | 'd' | 'e' | 'f' |
           'g' | 'h' | 'i' | 'j' | 'k' | 'l' | 'm' | 'n' |
           'o' | 'p' | 'q' | 'r' | 's' | 't' | 'u' | 'v' |
           'w' | 'x' | 'y' | 'z' ;
symbol: symbolWitoutNewline | '\\newline' | '\n';
symbolWitoutNewline: '[' | ']' | '{' | '}' | '(' | ')' | '<' | '>' | '\'' | '\\"' | '=' | '|' | '.' | ',' | ';' |
        '-' | '+' | '*' | '?' | '\\enter' | '\\tab' | '\t' | '\r' | '\\carriageReturn'|
        '\f' | '\\formfeed' | '\\backspace' | '\b' | '@' | '!' | '&' | '/' | ':' | '?' | '#' | '$' | '¤' |
        '%' | '´' | '`' | '~' | '^' | '¨' | '_' | '½' | '§' | ' ';



//wsc stands for WhiteSpace Character
wsc: (' ' | '\\newline' | '\n' | '\\tab' | '\t')*;

// Lets you add comment text with # prefix.
comment: '#' textWithoutNewlineOrQuotationmarks wsc;

//allows for keywords to be non-case-sensitive. All letters are set to be both able to be both upper- and lower case.
a: 'a' | 'A';
b: 'b' | 'B';
c: 'c' | 'C';
d: 'd' | 'D';
e: 'e' | 'E';
f: 'f' | 'F';
g: 'g' | 'G';
h: 'h' | 'H';
i: 'i' | 'I';
j: 'j' | 'J';
k: 'k' | 'K';
l: 'l' | 'L';
m: 'm' | 'M';
n: 'n' | 'N';
o: 'o' | 'O';
p: 'p' | 'P';
q: 'q' | 'Q';
r: 'r' | 'R';
s: 's' | 'S';
t: 't' | 'T';
u: 'u' | 'U';
v: 'v' | 'V';
w: 'w' | 'W';
x: 'x' | 'X';
y: 'y' | 'Y';
z: 'z' | 'Z';