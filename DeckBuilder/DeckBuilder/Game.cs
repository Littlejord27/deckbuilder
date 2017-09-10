using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Data;
using System.Text;

public class Game
{
    public ArrayList MainDeck;

    public ArrayList SVillians;

    public ArrayList Crisis;

    public ArrayList DestryoedPile = new ArrayList();

    public ArrayList Kicks = new ArrayList();
    public ArrayList Weaks = new ArrayList();

    public ArrayList Line = new ArrayList();

    private Random R = new Random();

    public int SVilMod = 0;

    public int SVilCost
    {
        get
        {
            return (SVillians[0] as SVillianCard).Cost - SVilMod;
        }
        set
        {
            SVilCost = (SVillians[0] as SVillianCard).Cost;
        }
    }

    public Game()
    {
        MainDeck = CreateMainDeck();
        SVillians = CreateSVillainStack();
        Crisis = CreateCrisisDeck();

        for (int i = 0; i < R.Next(12, 64); i++)
        {
            Shuffle(MainDeck);
        }

        CreateLine();
    }

    private ArrayList CreateMainDeck()
    {
        Random r = new Random();
        ArrayList pile = new ArrayList();
        ArrayList temp = new ArrayList();

        string path = Path.Combine(Environment.CurrentDirectory, @"cards\", "Main.JSON");
        string JSONText = System.IO.File.ReadAllText(path);

        DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(JSONText);

        DataTable tbl = dataSet.Tables["cards"];

        foreach (DataRow row in tbl.Rows)
        {
            temp.Add(row);
        }

        foreach (Object o in temp)
        {
            String name = ((DataRow)o)[0].ToString();
            int cost = Convert.ToInt32(((DataRow)o)[1].ToString());
            String ct = ((DataRow)o)[2].ToString();
            int vp = Convert.ToInt32(((DataRow)o)[3].ToString());
            int basepwr = Convert.ToInt32(((DataRow)o)[4]);
            String text = ((DataRow)o)[5].ToString();
            String attr = ((DataRow)o)[6].ToString();

            pile.Add(new Card(name, cost, ct, vp, basepwr, text, attr));
        }

        return pile;
    }

    private ArrayList CreateSVillainStack()
    {
        ArrayList pile = new ArrayList();
        ArrayList temp = new ArrayList();

        string path = Path.Combine(Environment.CurrentDirectory, @"cards\", "SVillain.JSON");
        string JSONText = System.IO.File.ReadAllText(path);

        DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(JSONText);

        DataTable tbl = dataSet.Tables["cards"];

        foreach (DataRow row in tbl.Rows)
        {
            temp.Add(row);
        }

        foreach (Object o in temp)
        {
            String name = ((DataRow)o)[0].ToString();
            int cost = Convert.ToInt32(((DataRow)o)[1].ToString());
            String ct = ((DataRow)o)[2].ToString();
            int vp = Convert.ToInt32(((DataRow)o)[3].ToString());
            String text = ((DataRow)o)[4].ToString();

            pile.Add(new SVillianCard(name, cost, ct, vp, text));
        }

        return pile;
    }

    private ArrayList CreateCrisisDeck()
    {
        ArrayList pile = new ArrayList();
        ArrayList temp = new ArrayList();

        string path = Path.Combine(Environment.CurrentDirectory, @"cards\", "crisis1.JSON");
        string JSONText = System.IO.File.ReadAllText(path);

        DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(JSONText);

        DataTable tbl = dataSet.Tables["cards"];

        foreach (DataRow row in tbl.Rows)
        {
            temp.Add(row);
        }

        foreach (Object o in temp)
        {
            String name = ((DataRow)o)[0].ToString();
            String ongoing = ((DataRow)o)[1].ToString();
            String req = ((DataRow)o)[2].ToString();

            pile.Add(new CrisisCard(name, ongoing, req));
        }

        return pile;
    }

    public ArrayList CreateStartingHand()
    {
        ArrayList pile = new ArrayList();
        for (int i = 0; i < 7; i++)
        {
            pile.Add(new Card("Punch", 0, "Starter", 0, 1, "+1 Power.", "None"));
            if (i < 3)
            {
                pile.Add(new Card("Vulnerability", 0, "Starter", 0, 0, "", "None"));
            }
        }
        return pile;
    }

    public void Destroy(Card c)
    {
        DestryoedPile.Add(c);
    }

    public void Shuffle(ArrayList a)
    {
        Random r = new Random();
        int n = a.Count;
        while (n > 1)
        {
            n--;
            int k = r.Next(n + 1);
            Object o = a[k];
            a[k] = a[n];
            a[n] = o;
        }
    }

    private void CreateLine()
    {
        for (int i = 0; i < 5; i++)
        {
            Line.Add(MainDeck[i]);
            MainDeck.RemoveAt(i);
        }
    }

    public void FillLine()
    {
        int i = 5 - Line.Count;
        if (i > 0)
        {
            Line.Add(MainDeck[0]);
            MainDeck.RemoveAt(0);
            FillLine();
        }
    }

    public void EndTurn()
    {
        FillLine();
        SVilMod = 0;
    }

    public void StartTurn()
    {

    }

    public void DisplayLine()
    {
        Console.WriteLine();
        Console.WriteLine();
        int i = 1;
        foreach (Card c in Line)
        {
            Console.Write(i + "   ");
            Console.Write(c.Name);
            Console.Write(" | " + c.CardType);
            Console.WriteLine(" | " + c.Cost);
            Console.WriteLine();
            i++;
        }
        Console.WriteLine("(SVIL) " + (SVillians[0] as SVillianCard).Name + " | " + (SVillians[0] as SVillianCard).CardType + " | " + SVilCost);
    }

    public void Spend(Player p, Game g)
    {
        Card kick = new Card("Kick", 3, "Super Power", 1, 2, "+2 Power.", "None");
        String resp = "";
        while (resp != "BACK")
        {
            Console.WriteLine();
            Console.WriteLine("Which Card would you like to buy?");
            Console.WriteLine("You currently have " + p.Power + " Power to spend.");
            DisplayLine();
            resp = Console.ReadLine().ToUpper();
            int i;
            String resp1 = resp;
            if (Int32.TryParse(resp, out i))
            {
                i--;
                if (i < Line.Count && i >= 0)
                {
                    if (CanBuy(p, Line[i] as Card))
                    {
                        if (Line[i] is Card)
                            p.Buy(Line[i] as Card, g);
                        if (Line[i] is SVillianCard)
                            p.Buy(Line[i] as SVillianCard);
                        Line.RemoveAt(i);
                    }
                    else
                    {
                        Console.WriteLine("Could not buy " + (Line[i] as Card).Name + ". Insufficient Power.");
                    }
                }
            }
            else
            {
                switch (resp)
                {
                    case "KICK":
                        if (CanBuy(p, kick))
                            p.Buy(new Card("Kick", 3, "Super Power", 1, 2, "+2 Power.", "None"), g);
                        break;
                    case "SVIL":
                        AttemptSVIL(p);
                        break;
                }
            }
        }
    }

    private Boolean CanBuy(Player p, Card c)
    {
        int cost = c.Cost;

        if (p.Power >= cost && cost != -1)
        {
            p.SpendPower(cost);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Gain(Player p)
    {
        p.Gain(Line[0] as Card, p.Hand);
        Line.RemoveAt(0);
    }

    public void DisplayInfo(Object c)
    {
        if (c is Card)
        {
            Console.WriteLine((c as Card).Name);
            Console.WriteLine((c as Card).CardType);
            Console.WriteLine((c as Card).Text);
            Console.WriteLine((c as Card).VP + " VP");
        }
    }

    public void AttemptCrisis(Player p, Game g)
    {
        Console.Clear();
        Boolean beat = false;
        String words = "";
        if ((Crisis[0] as CrisisCard).Beat)
        {
            Console.WriteLine("You already beat it.");
        }
        else if (!LineStatus())
        {
            beat = CrisisEvent.CrisisResolve(Crisis[0] as CrisisCard, p, g);
            if (beat) words = "has been defeated";
            else words = "is Still Occuring.";
            Console.WriteLine("The current Crisis Event " + words);
            (Crisis[0] as CrisisCard).Beat = beat;
        }
        else
        {
            Console.WriteLine("There is currently a Villian on the line.");
        }

        Console.WriteLine("Press any key to return...");
    }

    public void AttemptSVIL(Player p)
    {
        Console.WriteLine();
        if (!LineStatus() && (Crisis[0] as CrisisCard).Beat)
        {
            Console.WriteLine("The Current Super Villain is " + (SVillians[0] as SVillianCard).Name);
            Console.WriteLine("He cost " + SVilCost + ". You have " + p.Power + ".");
            Console.WriteLine();
            Console.WriteLine("Would you like to buy him? [Y]es or Back");
            String resp = Console.ReadLine().ToUpper();
            if (resp.Substring(0, 1).Equals("Y") && SVilCost <= p.Power)
            {
                p.SpendPower(SVilCost);
                SVillians.RemoveAt(0);
                Crisis.RemoveAt(0);
            }
        }
        else
        {
            if (!(Crisis[0] as CrisisCard).Beat)
                Console.WriteLine("Crisis Event is still occuring.");
            if (LineStatus())
                Console.WriteLine("There is a Villain in the line up.");
        }

        Console.WriteLine();
        Console.WriteLine("Type any key to return...");
        Console.ReadLine();
        Console.Clear();
    }

    public Boolean LineStatus()
    {
        bool vilInLine = false;

        foreach (Card c in Line)
        {
            if (c.CardType == "Villain")
            {
                vilInLine = true;
                break;
            }
        }

        return vilInLine;
    }
}