using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

public class Player
{
    public ArrayList Deck;
    public ArrayList DiscardPile = new ArrayList();
    public ArrayList Hand = new ArrayList();

    //Cards you played to be moved to the discard pile
    private ArrayList HoldingPile = new ArrayList();

    //Cards you gained that will be moved to the discard pile
    public ArrayList GainedCards = new ArrayList();

    //Cards destroyed by you
    public ArrayList Destroyed = new ArrayList();

    public int Power { get; set; }

    public bool Safe { get; set; }

    private bool CanSave = false;

    public Player(int i)
    {
        Safe = false;
        Deck = CreateStarterDeck();
        for (int j = 0; j < i; j++)
            Shuffle(Deck);
        Draw(5);
    }

    public void Draw(int drawAmount)
    {
        int amt = 0;
        int diff = 0;
        if (drawAmount > Deck.Count)
        {
            amt = Deck.Count;
            diff = drawAmount - Deck.Count;
        }
        else
        {
            amt = drawAmount;
        }
        for (int i = 1; i <= amt; i++)
        {
            //Move the card from your deck to your hand
            //Then remove the row from the datatable
            Hand.Add(Deck[0]);
            Deck.RemoveAt(0);
        }
        if (diff > 0)
        {
            ShuffleDiscard();
            Draw(diff);
        }
    }

    private void ShuffleDiscard()
    {
        Random r = new Random();
        Deck.Clear();
        foreach (Object c in DiscardPile)
            Deck.Add(c);
        DiscardPile.Clear();
        for (int i = 3; i < r.Next(Deck.Count); i++)
        {
            Shuffle(Deck);
        }
    }

    public void Gain(Card c, ArrayList pile)
    {
        pile.Add(c);
    }

    public void Buy(Card c, Game g)
    {
        if (c.CardType != "Villain")
            GainedCards.Add(c);
        else
            g.DestryoedPile.Add(c);
    }

    public void Buy(SVillianCard c)
    {
        GainedCards.Add(c);
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

    public ArrayList CreateStarterDeck()
    {
        ArrayList pile = new ArrayList();
        pile.Clear();
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

    public void Discard(Card c)
    {
        Hand.Remove(c);
        DiscardPile.Add(c);
    }

    public void DisplayHand()
    {
        int counter = 1;
        if (Hand.Count > 0)
        {
            foreach (Card c in Hand)
            {

                Console.Write("(" + counter + ")" + c.Name);

                if (counter == Hand.Count)
                {
                    Console.WriteLine();
                }
                else
                {
                    Console.Write(", ");
                }
                counter++;
            }
        }
    }

    public void DisplayDiscard()
    {
        int counter = 1;
        if (Hand.Count > 0)
        {
            foreach (Card c in DiscardPile)
            {

                Console.Write("(" + counter + ")" + c.Name);

                if (counter == DiscardPile.Count)
                {
                    Console.WriteLine();
                }
                else
                {
                    Console.Write(", ");
                }
                counter++;
            }
        }
    }

    public void SpendPower(int amt)
    {
        Power -= amt;
    }

    public void HandleCardRequest(Game game)
    {
        Console.WriteLine();
        String resp = "";
        while (resp != "BACK")
        {
            Console.WriteLine();
            Console.WriteLine("Please type in the number of your next card.");
            Console.WriteLine("Current Power: " + Power);
            DisplayHand();
            resp = Console.ReadLine().ToUpper();
            int i;
            if (Int32.TryParse(resp, out i))
            {
                i--;
                if (i < Hand.Count && i >= 0)
                {
                    HoldingPile.Add(Hand[i]);
                    if (Hand[i] is Card)
                        Play(Hand[i] as Card, game);
                    Hand.RemoveAt(i);
                }
            }
        }
    }

    public void DisplayHistory()
    {
        int counter = 1;
        foreach (Card c in HoldingPile)
        {

            Console.Write("(" + counter + ")" + c.Name);

            if (counter == Hand.Count)
            {
                Console.WriteLine();
            }
            else
            {
                Console.Write(", ");
            }
            counter++;
        }
    }

    private void Play(Card c, Game g)
    {
        char delim1 = ',';
        char delim2 = ' ';
        int dec = -1;
        String[] arr = c.Attr.Split(delim1);
        for (int i = 0; i < arr.Length; i++)
        {
            String[] temp = arr[i].Split(delim2);
            switch (temp[0])
            {
                case "Special":
                    break;
                case "Draw":
                    if (Int32.TryParse(temp[1], out dec))
                        Draw(dec);
                    break;
                case "SVilain":
                    if (Int32.TryParse(temp[1], out dec))
                        ReduceVillian(dec, g);
                    break;
                case "Rez":
                    if (Int32.TryParse(temp[2], out dec))
                        ReturnCard(temp[1], dec);
                    break;
                case "May-Destroy-Both":
                    DestroyBoth(g);
                    break;
                case "Save1":
                    CanSave = true;
                    break;
                case "Scry":
                    if (Int32.TryParse(temp[1], out dec))
                        Scry(dec, temp[2], g);
                    break;
                case "GainLine":
                    if (Int32.TryParse(temp[1], out dec))
                        GainLine(dec, g);
                    break;
                default:
                    break;
            }
        }
        Power += c.BasePower;
    }

    private void GainLine(int cost, Game g)
    {
        g.DisplayLine();
        Console.WriteLine("Please select the card you wish to gain. The cost max is " + cost);

        String resp = Console.ReadLine();
        int dec = -1;

        if (Int32.TryParse(resp, out dec))
        {
            dec--;
            if (dec > 0 && dec < g.Line.Count)
            {
                GainedCards.Add(g.Line[dec]);
                g.Line.RemoveAt(dec);
            }
        }
        else
        {
            Console.WriteLine("Couldn't gained that one.");
        }

    }

    private void Scry(int amt, String reward, Game g)
    {
        if (Deck.Count == 0)
            ShuffleDiscard();
        Card c = (Deck[0] as Card);
        Console.WriteLine("Your top card is " + c.Name + " with a cost of " + c.Cost);
        if (c.Cost >= amt)
        {
            switch (reward)
            {
                case "Power1":
                    Power += 1; break;
                case "May-Destroy":
                    Console.WriteLine("Would you like to destroy " + c.Name + " -- [Y]es or [N]o.");
                    if (Console.ReadLine().ToUpper().Equals("Y"))
                    {
                        g.DestryoedPile.Add(Deck[0]);
                        Deck.RemoveAt(0);
                    }
                    break;
            }
        }
    }

    private void SaveCard()
    {
        int counter = 1;
        int index = 1;

        foreach (Card c in GainedCards)
        {
            Console.Write("(" + counter + ")" + c.Name);

            if (counter == GainedCards.Count)
            {
                Console.WriteLine();
            }
            else
            {
                Console.Write(", ");
            }
            counter++;
        }

        if (Int32.TryParse(Console.ReadLine(), out counter))
        {
            for (int i = 0; i < GainedCards.Count; i++)
            {
                if (index == counter && counter > 0)
                {
                    Deck.Insert(0, GainedCards[i]);
                    GainedCards.RemoveAt(i);
                }
                index++;
            }
        }
        else
        {
            SaveCard();
        }
    }

    private void ReturnCard(String cardtype, int limitcost)
    {
        int counter = 1;
        int index = 1;
        ArrayList temp = new ArrayList();

        foreach (Card c in DiscardPile)
        {
            if (c.CardType == cardtype)
            {
                Console.Write("(" + counter + ")" + c.Name);

                if (counter == Hand.Count)
                {
                    Console.WriteLine();
                }
                else
                {
                    Console.Write(", ");
                }
                temp.Add(c);
                counter++;
            }
        }

        if (temp.Count > 0)
        {
            String resp = Console.ReadLine().ToUpper();
            String resp1 = resp;
            if (Int32.TryParse(resp, out counter))
            {
                for (int i = 0; i < DiscardPile.Count; i++)
                {
                    if ((DiscardPile[i] as Card).CardType == cardtype)
                    {
                        if (index == counter && counter > 0)
                        {
                            Hand.Add(DiscardPile[i]);
                            DiscardPile.RemoveAt(i);
                        }
                        index++;
                    }
                }
            }
            else
            {
                switch (resp1)
                {
                    case "BACK": Hand.Add(HoldingPile[0]); break;
                    default: ReturnCard(cardtype, limitcost); break;
                }
            }
        }
        else
        {
            Hand.Add(HoldingPile[HoldingPile.Count-1]);
            Console.WriteLine("No Heroes in your Discard Pile.");
        }
    }

    private void ReduceVillian(int mod, Game g)
    {
        g.SVilMod = mod;
    }

    private void DestroyBoth(Game g)
    {
        ArrayList temp = new ArrayList();
        Console.WriteLine("Here are all the cards in your hand and discard pile. Type the number to destroy.");
        int counter = 1;
        foreach (Card c in Hand)
        {
            Console.Write("H(" + counter + ")" + c.Name);

            if (counter == Hand.Count)
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

        foreach (Card c in DiscardPile)
        {

            Console.Write("D(" + counter + ")" + c.Name);

            if (nucounter == DiscardPile.Count)
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

        String resp = Console.ReadLine();

        int i = -1;
        if (Int32.TryParse(resp, out i))
        {
            i--;
            if (i < temp.Count && i > 0)
            {
                //Card to be deleted is in the hand
                if (i < Hand.Count)
                {
                    g.DestryoedPile.Add(Hand[i]);
                    Hand.RemoveAt(i);
                }
                else //Card is in the discard
                {
                    g.DestryoedPile.Add(temp[i]); //counts for it being out 
                    DiscardPile.Remove(temp[i]);
                }

            }
        }
    }

    private void Destroy(String resp, ArrayList arr)
    {
        int i;
        if (Int32.TryParse(resp, out i))
        {
            i--;
            if (i < arr.Count && i > 0)
            {
                Destroyed.Add(i);
            }
        }
    }

    public void Play(SVillianCard c, Game g)
    {

    }

    public void EndTurn()
    {

        Console.WriteLine("Select a card to save.");
        if (CanSave && HoldingPile.Count > 0)
            SaveCard();

        foreach (Object c in HoldingPile)
        {
            DiscardPile.Add(c);
        }
        HoldingPile.Clear();

        foreach (Object c in GainedCards)
        {
            DiscardPile.Add(c);
        }
        GainedCards.Clear();

        foreach (Object c in Hand)
        {
            DiscardPile.Add(c);
        }
        Hand.Clear();
       
        Draw(5);
        Power = 0;
    }

    public void DisplayDeck()
    {
        Console.Clear();
        foreach(Card c in Deck)
        {
            Console.WriteLine(c.Name);
        }
        Console.ReadLine();
    }
}