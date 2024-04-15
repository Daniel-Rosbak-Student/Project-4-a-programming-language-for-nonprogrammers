package program;
import java.util.List;
public class Program{
public static String evaluateRPS(String p1,String p2)
{
if(p1 == p2){
return "draw";
};
if(p1 == "stone" && p2 == "paper" || p1 == "paper" && p2 == "scissors" || p1 == "scissors" && p2 == "stone"){
return "Player 2 wins";
}else{
return "Player 1 wins";
}
}
public static void main(){
String player1 = "stone";;
String player2 = "paper";;
;
System.out.println(evaluateRPS(player1,player2));
}
}