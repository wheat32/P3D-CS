using Microsoft.Xna.Framework;

namespace P3D.Items;

public abstract class Berry : MedicineItem
{
    public enum Flavours
    {
        Spicy,
        Dry,
        Sweet,
        Bitter,
        Sour,
    }

    public int PhaseTime;
    public String Size = "";
    public String Firmness = "";
    public int BerryIndex;
    public int minBerries;
    public int maxBerries;

    public int Spicy = 0;
    public int Dry = 0;
    public int Sweet = 0;
    public int Bitter = 0;
    public int Sour = 0;

    public int WinterGrow = 0;
    public int SpringGrow = 3;
    public int SummerGrow = 2;
    public int FallGrow = 1;

    public int type;
    public int Power = 80;

    public String JuiceColor = "red";
    public int JuiceGroup = 1;

    public override int PokeDollarPrice { get; protected set; } = 100;
    public override int FlingDamage { get; } = 10;
    public override ItemTypes ItemType { get; } = ItemTypes.Plants;
    public override int SortValue { get; protected set; }
    public override String Description { get; protected set; } = "";
    public override String PluralName => Name + " Berries";

    protected Berry(int phaseTime, String description, String size, String firmness, int minBerries, int maxBerries)
    {
        SortValue = ID - 1999;
        PhaseTime = phaseTime;
        Size = size;
        Firmness = firmness;
        BerryIndex = ID - 2000;
        this.minBerries = minBerries;
        this.maxBerries = maxBerries;

        int x = BerryIndex * 128;
        int y = 0;
        while (x >= 512)
        {
            x -= 512;
            y += 32;
        }

        Description = description;
        _textureSource = @"Textures\Berries";
        _textureRectangle = new Rectangle(x, y, 32, 32);
    }

    public Flavours Flavour
    {
        get
        {
            Flavours returnFlavour = Flavours.Spicy;
            int highestFlavour = Spicy;
            if (Dry > highestFlavour) { highestFlavour = Dry; returnFlavour = Flavours.Dry; }
            if (Sweet > highestFlavour) { highestFlavour = Sweet; returnFlavour = Flavours.Sweet; }
            if (Bitter > highestFlavour) { highestFlavour = Bitter; returnFlavour = Flavours.Bitter; }
            if (Sour > highestFlavour) { highestFlavour = Sour; returnFlavour = Flavours.Sour; }
            return returnFlavour;
        }
    }

    public bool PokemonLikes(Pokemon p)
    {
        switch (p.Nature)
        {
            case Pokemon.Natures.Lonely:
                if (Flavour == Flavours.Spicy) return true;
                if (Flavour == Flavours.Sour) return false;
                break;
            case Pokemon.Natures.Adamant:
                if (Flavour == Flavours.Spicy) return true;
                if (Flavour == Flavours.Dry) return false;
                break;
            case Pokemon.Natures.Naughty:
                if (Flavour == Flavours.Spicy) return true;
                if (Flavour == Flavours.Bitter) return false;
                break;
            case Pokemon.Natures.Brave:
                if (Flavour == Flavours.Spicy) return true;
                if (Flavour == Flavours.Sweet) return false;
                break;
            case Pokemon.Natures.Bold:
                if (Flavour == Flavours.Sour) return true;
                if (Flavour == Flavours.Spicy) return false;
                break;
            case Pokemon.Natures.Impish:
                if (Flavour == Flavours.Sour) return true;
                if (Flavour == Flavours.Dry) return false;
                break;
            case Pokemon.Natures.Lax:
                if (Flavour == Flavours.Sour) return true;
                if (Flavour == Flavours.Bitter) return false;
                break;
            case Pokemon.Natures.Relaxed:
                if (Flavour == Flavours.Sour) return true;
                if (Flavour == Flavours.Sweet) return false;
                break;
            case Pokemon.Natures.Modest:
                if (Flavour == Flavours.Dry) return true;
                if (Flavour == Flavours.Spicy) return false;
                break;
            case Pokemon.Natures.Mild:
                if (Flavour == Flavours.Dry) return true;
                if (Flavour == Flavours.Sour) return false;
                break;
            case Pokemon.Natures.Rash:
                if (Flavour == Flavours.Dry) return true;
                if (Flavour == Flavours.Bitter) return false;
                break;
            case Pokemon.Natures.Quiet:
                if (Flavour == Flavours.Dry) return true;
                if (Flavour == Flavours.Sweet) return false;
                break;
            case Pokemon.Natures.Calm:
                if (Flavour == Flavours.Bitter) return true;
                if (Flavour == Flavours.Spicy) return false;
                break;
            case Pokemon.Natures.Gentle:
                if (Flavour == Flavours.Bitter) return true;
                if (Flavour == Flavours.Sour) return false;
                break;
            case Pokemon.Natures.Careful:
                if (Flavour == Flavours.Bitter) return true;
                if (Flavour == Flavours.Dry) return false;
                break;
            case Pokemon.Natures.Sassy:
                if (Flavour == Flavours.Bitter) return true;
                if (Flavour == Flavours.Sweet) return false;
                break;
            case Pokemon.Natures.Timid:
                if (Flavour == Flavours.Sweet) return true;
                if (Flavour == Flavours.Spicy) return false;
                break;
            case Pokemon.Natures.Hasty:
                if (Flavour == Flavours.Sweet) return true;
                if (Flavour == Flavours.Sour) return false;
                break;
            case Pokemon.Natures.Jolly:
                if (Flavour == Flavours.Sweet) return true;
                if (Flavour == Flavours.Dry) return false;
                break;
            case Pokemon.Natures.Naive:
                if (Flavour == Flavours.Sweet) return true;
                if (Flavour == Flavours.Bitter) return false;
                break;
        }
        return true;
    }
}
