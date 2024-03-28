grammar Syntax;

program: 'program' wsc '(' wsc commands wsc ')' EOF;

commands: this=command wsc next=commands                                                            #notLastCommand
        | this=command                                                                              #lastCommand
        ;

command: this=function                                                                              #functionCommand
       | this=terms                                                                                 #termsCommand
       ;

terms: this=term wsc next=terms                                                                         #notLastTerm
     | this=term                                                                                    #lastTerm
     ;

term: this=controlStructures                                                                        #controlTerm
    | this=statement                                                                                #statementTerm
    | this=comment                                                                                  #commentTerm
    ;

value: this=use                                                                                     #useValue
     | this=read                                                                                    #readValue
     | this=flag                                                                                    #flagValue
     | this=lengthOf                                                                                #lengthOfValue
     | this=listElement                                                                             #listElementValue
     | this=number                                                                                  #numberValue
     | this=text                                                                                    #textValue
     | this=identifier                                                                              #identifierValue
     ;

lengthOf: l e n g t h o f wsc '(' wsc id=identifier wsc ')';
type: n u m b e r | t e x t | f l a g | l i s t wsc o f wsc tp=type;

identifier: id=nonKeywordName;
listElement: id=nonKeywordName wsc '(' wsc index=expression wsc ')';
nonKeywordName: character (digit | character)*;

number: ('-')? nonZeroNumber | '0';
nonZeroNumber: nonZeroDigit digit*;
digit: '0' | nonZeroDigit;
nonZeroDigit: '1' | '2' | '3' | '4' | '5' | '6' | '7' | '8' | '9' ;

flag: t r u e | f a l s e;

text: '"' (character | symbol | digit)* '"';
textWithoutNewlineOrQuotationmarks: (character | symbolWithoutNewline | digit)*;

controlStructures: this=loop wsc                                                                         #loopStructure
                 | this=if_else wsc                                                                      #ifElseStructure
                 ;

loop: r e p e a t wsc w h i l e wsc expr=expression wsc d o wsc '(' wsc trms=terms ')';

//If statements to do something based on a boolean expression, or continue with more if, optional else at the end.
if_else: r u n wsc i f wsc expr=expression wsc '(' wsc trms=terms ')' wsc                                                          #ifNoElse
       | r u n wsc i f wsc expr=expression wsc '(' wsc trms=terms ')' wsc e l s e wsc r u n wsc '(' wsc elseTrms=terms ')' wsc     #ifWithElse
       ;

function: c r e a t e wsc f u n c t i o n a l i t y wsc id=identifier wsc gives=givesArgument wsc '(' wsc trms=terms ')'                            #functionNoTakes
        | c r e a t e wsc f u n c t i o n a l i t y wsc id=identifier wsc takes=takesArgument wsc gives=givesArgument wsc '(' wsc trms=terms ')'    #functionWithTakes
        ;

takesArgument: t a k e s wsc '(' param=parameter wsc ')';

parameter: wsc tp=type wsc id=identifier wsc ',' wsc next=parameter                                 #notLastParameter
         | tp=type wsc id=identifier                                                                #lastParameter
         ;

givesArgument: g i v e s wsc tp=nothing                                                             #givesNothing
             | g i v e s wsc tp=type                                                                #givesType
             ;

nothing: n o t h i n g;

statement: this=create wsc ';' wsc                                                                  #createStatement
         | this=give wsc ';' wsc                                                                    #giveStatement
         | this=break wsc ';' wsc                                                                   #breakStatement
         | this=use wsc ';' wsc                                                                     #useStatement
         | this=print wsc ';' wsc                                                                   #printStatement
         | this=read wsc ';' wsc                                                                    #readStatement
         | this=assignment wsc ';' wsc                                                              #assignStatement
         ;

assignment: id=identifier wsc '=' wsc expr=expression;
// Keyword for making new variables.
create: c r e a t e wsc tp=type wsc id=identifier                                                   #createNoInput
      | c r e a t e wsc tp=type wsc id=identifier wsc '=' wsc expr=expression                       #createWithInput
      ;

give: g i v e wsc this=use                                                                          #useGive
    | g i v e wsc this=nothing                                                                      #nothingGive
    | g i v e wsc expr=expression                                                                   #expressionGive
    ;

break: b r e a k;

use: u s e wsc id=identifier (wsc '(' wsc ')')?                                                     #useNoInput
   | u s e wsc id=identifier wsc '(' input=useInput wsc ')'                                         #useWithInput
   ;
useInput: wsc id=identifier wsc ',' wsc next=useInput                                               #notLastInput
        | id=identifier                                                                             #lastInput
        ;

print: p r i n t wsc t o wsc s c r e e n wsc '(' wsc expr=expression wsc ')';
read: r e a d wsc u s e r wsc i n p u t wsc ('(' wsc ')')?;


expression: left=expression wsc op=operator wsc right=expression                                    #infixExpression
            | '(' expr=expression ')'                                                               #parensExpression
            | this=use                                                                              #useExpression
            | this=value                                                                            #valueExpression
            | expr=expression wsc a s wsc tp=type                                                   #convertExpression
            ;

operator: '+' | '-' | '*' | '/' | 'modulo' | '=' | '>' | '<' | '<=' | '>=' | a n d | o r | n o t;

character: 'A' | 'B' | 'C' | 'D' | 'E' | 'F' | 'G' | 'H' |
           'I' | 'J' | 'K' | 'L' | 'M' | 'N' | 'O' | 'P' |
           'Q' | 'R' | 'S' | 'T' | 'U' | 'V' | 'W' | 'X' |
           'Y' | 'Z' | 'a' | 'b' | 'c' | 'd' | 'e' | 'f' |
           'g' | 'h' | 'i' | 'j' | 'k' | 'l' | 'm' | 'n' |
           'o' | 'p' | 'q' | 'r' | 's' | 't' | 'u' | 'v' |
           'w' | 'x' | 'y' | 'z' ;

symbol: symbolWithoutNewline | '\\newline' | '\n';
symbolWithoutNewline: '[' | ']' | '{' | '}' | '(' | ')' | '<' | '>' | '\'' | '\\"' | '=' | '|' | '.' | ',' | ';' |
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
