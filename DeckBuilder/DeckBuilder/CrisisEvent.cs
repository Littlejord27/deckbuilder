using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class CrisisEvent
{

    public static bool CrisisResolve(CrisisCard c, Player p, Game g)
    {
        switch (c.Name)
        {
            case "Arkham Breakout": return ArkhamBreakout(p);
            case "Wave of Terror": return WaveOfTerror(p);
            case "Final Countdown": break;
            case "Electromagnetic Pulse": return ElectromagneticPulse(p, g);
            case "Identity Crisis": break;
            case "Collapsing Parallel Worlds": break;
            case "Alternate Reality": return AlternateReality(p, g);
            case "Kryptonite Meteor": return KryptoniteMeteor(p, g);
            case "World Domination": break;
            case "Rise of the Rot": break;
            case "Legion of Doom": break;
            case "Untouchable Villain": break;
        }

        return false;
    }

    private static bool ArkhamBreakout(Player p)
    {
        Console.WriteLine("Discard Any Card to defeat");
        p.DisplayHand();
        String resp = Console.ReadLine();
        int i = -1;
        if (Int32.TryParse(resp, out i))
        {
            if (i <= p.Hand.Count && i > 0)
            {
                i--;
                p.Discard(p.Hand[i] as Card);
                return true;
            }
        }
        return false;
    }
    
    private static bool WaveOfTerror(Player p)
    {
        Random r = new Random();
        String resp = "";

        while (resp != "BACK" && p.Hand.Count > 0)
        {
            Console.WriteLine("Discard a random card from your hand. If it costs 1 or greater, you beat this crisis event. Type [Y]es to attempt.");
            resp = Console.ReadLine().ToUpper();
            if (resp.Substring(0, 1).Equals("Y"))
            {
                int rand = r.Next(p.Hand.Count);
                Card c = p.Hand[rand] as Card;
                Console.WriteLine("You have discarded " + c.Name + ". It costs " + c.Cost + " power.");
                p.Hand.RemoveAt(rand);
                if(c.Cost > 0)
                {
                    Console.WriteLine("You have beat the Crisis. Type in anything to go back to the main play area.");
                    Console.ReadLine();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        return false;
    }

    private static bool ElectromagneticPulse(Player p, Game g)
    {
        String resp = "";
        while (resp != "BACK")
        {
            Console.WriteLine("You must destroy an Equipment in your hand.");

            p.DisplayHand();

            resp = Console.ReadLine().ToUpper();

            int i = -1;
            if (Int32.TryParse(resp, out i))
            {
                if (i <= p.Hand.Count && i > 0)
                {
                    i--;
                    if ((p.Hand[i] as Card).CardType == "Equipment")
                    {
                        g.DestryoedPile.Add(p.Hand[i]);
                        p.Hand.RemoveAt(i);
                    }
                }
            }
        }

        return false;
    }

    private static bool AlternateReality(Player p, Game g)
    {
        ArrayList destroyed = new ArrayList();
        int cost = 0;
        String resp = "";
        while (cost < 12 && resp != "BACK")
        {
            Console.WriteLine("You must destroy 12(6) cost worth of Heroes in your discard pile.");
            Console.WriteLine("You currently have "+cost+" cost destroyed.");
            Console.WriteLine("Typing in 'Back' will undo any destroying.");

            p.DisplayDiscard();

            resp = Console.ReadLine().ToUpper();

            int i = -1;
            if (Int32.TryParse(resp, out i))
            {
                if (i <= p.DiscardPile.Count && i > 0)
                {
                    i--;
                    cost += (p.DiscardPile[i] as Card).Cost;
                    destroyed.Add(p.DiscardPile[i]);
                    p.DiscardPile.RemoveAt(i);
                }
            }
        }

        //They destroyed 12 costs. Move them to g.DestroyedPile.
        if (cost >= 12) {
            foreach(Card c in destroyed)
                g.DestryoedPile.Add(c);
            return true;
        }
        //They did not destroy 12. Add them back to the discard pile.
        else
        {
            foreach (Card c in destroyed)
                p.DiscardPile.Add(c);
            return false;
        }
    }

    private static bool KryptoniteMeteor(Player p, Game g)
    {
        String resp = "";
        while (resp != "BACK")
        {
            Console.WriteLine("You must destroy an Equipment in your hand.");

            p.DisplayHand();

            resp = Console.ReadLine().ToUpper();

            int i = -1;
            if (Int32.TryParse(resp, out i))
            {
                if (i <= p.Hand.Count && i > 0)
                {
                    i--;
                    if ((p.Hand[i] as Card).CardType == "Super Power")
                    {
                        g.DestryoedPile.Add(p.Hand[i]);
                        p.Hand.RemoveAt(i);
                    }

                }
            }
        }

        return false;
    }

}

/**
private static bool FinalCountdown(Player p)
{

}

private static bool WorldDomination(Player p)
{

}

private static bool RiseOfTheRot(Player p)
{

}

private static bool LegionOfDoom(Player p)
{

}

private static bool UntouchableVillain(Player p)
{

}

private static bool IdentityCrisis(Player p)
{

}

private static bool CollapsingParallelWorlds(Player p)
{

}
*/
