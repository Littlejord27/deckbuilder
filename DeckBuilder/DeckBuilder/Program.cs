using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeckBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            Random r = new Random();
            Game game = new Game();
            Player p1 = new Player(r.Next(1, 30));
            int turn = 1;

            Console.WriteLine("Hello. Welcome to the DC Deck Building Game");
            Console.WriteLine("If you need an explination of the rules or inputs at any point");
            Console.WriteLine("Just type in Help");

            LBreak();


            LBreak();
            Console.WriteLine("Enter anything when ready...");
            if (Console.ReadLine().ToUpper() == "HELP")
            {
                DisplayHelp();
            }

            String resp;


            while (true)
            {
                Console.Clear();
                if (turn > 1)
                {
                    game.StartTurn();
                }
                Console.WriteLine("Turn "+turn);
                Console.WriteLine("Your Current Opponent is " + (game.SVillians[0] as SVillianCard).Name);
                if(!(game.Crisis[0] as CrisisCard).Beat)
                    Console.WriteLine("Your Current Crisis Event is " + (game.Crisis[0] as CrisisCard).Name);
                Console.WriteLine("Current Power: " + p1.Power);
                LBreak();
                LBreak();
                Console.WriteLine("Let's take a look at the line-up.");
                LBreak();
                game.DisplayLine();
                LBreak();
                Console.WriteLine("Hand:");
                p1.DisplayHand();
                LBreak();
                Console.WriteLine("Input:");
                resp = Console.ReadLine().ToUpper();
                LBreak();
                switch (resp)
                {
                    case "PLAY": p1.HandleCardRequest(game); break;
                    case "BUY": game.Spend(p1, game); break;
                    case "HAND": p1.DisplayHand(); break;
                    case "CRISIS": game.AttemptCrisis(p1, game); break;
                    case "HELP": goto case "?";
                    case "?": DisplayHelp(); break;
                    case "INFO": DisplayAll(p1, game); break;
                    case "DECK": p1.DisplayDeck(); break;
                    case "END": game.EndTurn(); p1.EndTurn(); turn++; break;

                    case "P": goto case "PLAY";
                    case "B": goto case "BUY";
                    case "C": goto case "CRISIS";
                }

            }
        }

        static void DisplayHelp()
        {
            LBreak();
            Console.Clear();
            Console.WriteLine("Play => Takes you to your screen to play cards from your hand.");
            Console.WriteLine("Buy => Takes you to your screen to Buy cards from the line up.");
            Console.WriteLine("Buy -> Kick => If you type in Kick on the Buy screen, you will buy a Kick for 3 Power.");
            Console.WriteLine("Buy -> SVil => If you type in SVil on the Buy screen, you will buy a the Super Villian.");
            Console.WriteLine("Hand => Display your current Hand.");
            Console.WriteLine("Info => Let's you take a little closer look at some cards.");
            Console.WriteLine("Crisis => View the Crisis and Attempt to Beat it.");
            Console.WriteLine("End => End your turn.");
            Console.WriteLine(" -> Back => Go up a screen.");
            Console.WriteLine();
            Console.WriteLine("Type anything to continue...");
            Console.ReadLine();
            LBreak();
        }


        static void DisplayAll(Player p, Game g)
        {
            ArrayList temp = new ArrayList();
            Console.WriteLine("Here are all the cards showing. Type the number to get info.");
            LBreak();
            LBreak();
            int counter = 1;
            foreach (Card c in p.Hand)
            {

                Console.Write("(" + counter + ")" + c.Name);

                if (counter == p.Hand.Count)
                {
                    Console.WriteLine();
                }
                else
                {
                    Console.Write(", ");
                }
                counter++;
                temp.Add(c);
            }

            int nucounter = 1;

            foreach (Card c in g.Line)
            {

                Console.Write("(" + counter + ")" + c.Name);

                if (nucounter == g.Line.Count)
                {
                    Console.WriteLine();
                }
                else
                {
                    Console.Write(", ");
                }
                nucounter++;
                counter++;
                temp.Add(c);
            }
            LBreak();
            LBreak();
            DisplayInfo(Console.ReadLine(), temp, g);
        }

        static void DisplayInfo(String resp, ArrayList arr, Game g)
        {
            int i;
            if (Int32.TryParse(resp, out i))
            {
                if (i <= arr.Count)
                {
                    LBreak();
                    LBreak();
                    g.DisplayInfo(arr[i - 1]);
                    LBreak();
                    LBreak();
                }
            }
        }

        static void LBreak()
        {
            Console.WriteLine();
        }
    }
}
