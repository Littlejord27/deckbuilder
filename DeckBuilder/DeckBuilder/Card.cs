using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Card
{
    public String Name { get; set; }
    public int Cost { get; set; }
    public String CardType { get; set; }
    public int VP { get; set; }
    public int BasePower { get; set; }
    public String Text { get; set; }
    public String Attr { get; set; }

    public Card(String name, int cost, String cardtype, int vp, int basepower, String text, String attr)
    {
        this.Name = name;
        this.Cost = cost;
        this.CardType = cardtype;
        this.VP = vp;
        this.BasePower = basepower;
        this.Text = text;
        this.Attr = attr;
    }
}

public class SVillianCard
{
    public String Name { get; set; }
    public int Cost;
    public String CardType;
    public int VP;
    public String Text;

    public SVillianCard(string name, int cost, string cardtype, int vp, string text)
    {
        this.Name = name;
        this.Cost = cost;
        this.CardType = cardtype;
        this.VP = vp;
        this.Text = text;
    }

    public void Attack()
    {
        switch (Name)
        {
            case "Ra's Al Ghul": break;
        }
    }
}

public class CrisisCard
{
    public String Name;
    public String Ongoing;
    public String Req;
    public Boolean Beat;

    public CrisisCard(String name, String ongoing, String req)
    {
        this.Ongoing = ongoing;
        this.Req = req;
        this.Name = name;
        Beat = false;

    }
}