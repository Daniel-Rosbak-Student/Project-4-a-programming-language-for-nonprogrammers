#include <stdio.h> 
char* evaluateRPS(char* p1,char* p2)
{
if(p1 == p2){
return "draw";
}
if(p1 == "stone" && p2 == "paper" || p1 == "paper" && p2 == "scissors" || p1 == "scissors" && p2 == "stone"){
return "Player 2 wins";
}else{
return "Player 1 wins";
}
}
int main(){
char* player1 = "stone";
char* player2 = "paper";

printf(evaluateRPS(player1,player2););
return 0;
}