using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D;

public static class Badge
{
    public enum HMMoves
    {
        Surf,
        Cut,
        Strength,
        Flash,
        Fly,
        Whirlpool,
        Waterfall,
        Ride,
        Dive,
        RockClimb,
    }

    private class BadgeDeclaration
    {
        public int ID;
        public String Name = "";
        public int LevelCap = -1;
        public List<HMMoves> HMs = [];
        public Rectangle TextureRectangle = new Rectangle(0, 0, 50, 50);
        public String TexturePath = @"GUI\Badges";
        public String Region = "Johto";

        public BadgeDeclaration(String inputData)
        {
            String[] data = inputData.Split('|');

            ID = int.Parse(data[0]);
            Name = data[1];

            if (data.Length > 2)
            {
                for (int i = 2; i <= data.Length - 1; i++)
                {
                    String argName = data[i].Remove(data[i].IndexOf('='));
                    String argData = data[i].Remove(0, data[i].IndexOf('=') + 1);

                    switch (argName.ToLower())
                    {
                        case "level":
                            LevelCap = int.Parse(argData);
                            break;

                        case "hm":
                            String[] hms = argData.Split(',');
                            foreach (String hm in hms)
                            {
                                switch (hm.ToLower())
                                {
                                    case "surf":
                                        HMs.Add(HMMoves.Surf);
                                        break;
                                    case "cut":
                                        HMs.Add(HMMoves.Cut);
                                        break;
                                    case "strength":
                                        HMs.Add(HMMoves.Strength);
                                        break;
                                    case "flash":
                                        HMs.Add(HMMoves.Flash);
                                        break;
                                    case "fly":
                                        HMs.Add(HMMoves.Fly);
                                        break;
                                    case "whirlpool":
                                        HMs.Add(HMMoves.Whirlpool);
                                        break;
                                    case "waterfall":
                                        HMs.Add(HMMoves.Waterfall);
                                        break;
                                    case "ride":
                                        HMs.Add(HMMoves.Ride);
                                        break;
                                    case "dive":
                                        HMs.Add(HMMoves.Dive);
                                        break;
                                    case "rockclimb":
                                        HMs.Add(HMMoves.RockClimb);
                                        break;
                                }
                            }
                            break;

                        case "texture":
                            String[] texData = argData.Split(',');
                            TexturePath = texData[0];
                            TextureRectangle = new Rectangle(
                                int.Parse(texData[1]),
                                int.Parse(texData[2]),
                                int.Parse(texData[3]),
                                int.Parse(texData[4]));
                            break;

                        case "region":
                            Region = argData;
                            break;
                    }
                }
            }
        }
    }

    private static List<BadgeDeclaration> _badges = [];

    public static void Load()
    {
        _badges.Clear();

        String file = GameModeManager.GetContentFilePath("Data\\badges.dat");
        Security.FileValidation.CheckFileValid(file, false, "Badge.cs");
        String[] data = System.IO.File.ReadAllLines(file);
        foreach (String line in data)
        {
            if (line.Contains("|") == true && StringHelper.IsNumeric(line.GetSplit(0, "|")) == true)
            {
                _badges.Add(new BadgeDeclaration(line));
            }
        }
    }

    public static String GetBadgeName(int id)
    {
        foreach (BadgeDeclaration b in _badges)
        {
            if (b.ID == id)
            {
                String tokenKey = "badge_" + b.ID.ToString();
                if (Localization.TokenExists(tokenKey) == true)
                {
                    if (GameModeManager.ActiveGameMode != null &&
                        GameModeManager.ActiveGameMode.IsDefaultGamemode == false)
                    {
                        if (Localization.LocalizationTokens[tokenKey].IsGameModeToken == false)
                        {
                            return b.Name;
                        }
                        else
                        {
                            return Localization.GetString(tokenKey);
                        }
                    }
                    else
                    {
                        return Localization.GetString(tokenKey);
                    }
                }
                else
                {
                    return b.Name;
                }
            }
        }
        return "Plain";
    }

    public static Texture2D GetBadgeTexture(int id)
    {
        foreach (BadgeDeclaration b in _badges)
        {
            if (b.ID == id)
            {
                return TextureManager.GetTexture(b.TexturePath, b.TextureRectangle, "");
            }
        }
        return TextureManager.GetTexture(@"GUI\Badges", new Rectangle(0, 0, 50, 50), "");
    }

    public static int GetLevelCap()
    {
        List<int> trainerBadges = Core.Player.Badges;
        int highestCap = 10;
        foreach (BadgeDeclaration b in _badges)
        {
            if (b.LevelCap > highestCap && trainerBadges.Contains(b.ID) == true)
            {
                highestCap = b.LevelCap;
            }
        }
        return highestCap;
    }

    public static bool CanUseHMMove(HMMoves hm)
    {
        List<int> trainerBadges = Core.Player.Badges;
        foreach (BadgeDeclaration b in _badges)
        {
            if ((b.HMs.Contains(hm) == true && trainerBadges.Contains(b.ID) == true) || b.ID == 0)
            {
                return true;
            }
        }
        return false;
    }

    public static String GetRegion(int index)
    {
        List<String> regions = [];
        foreach (BadgeDeclaration b in _badges)
        {
            if (regions.Any(m => m.ToLowerInvariant().Equals(b.Region.ToLowerInvariant())) == false)
            {
                regions.Add(b.Region);
            }
        }
        if (regions.Count - 1 >= index)
        {
            return regions[index];
        }
        return "Johto";
    }

    public static int GetBadgesCount(String region)
    {
        int c = 0;
        foreach (BadgeDeclaration b in _badges)
        {
            if (b.Region.ToLower().Equals(region.ToLower()) == true)
            {
                c += 1;
            }
        }
        return c;
    }

    public static int GetRegionCount()
    {
        List<String> regions = [];
        foreach (BadgeDeclaration b in _badges)
        {
            if (regions.Any(m => m.ToLowerInvariant().Equals(b.Region.ToLowerInvariant())) == false)
            {
                regions.Add(b.Region);
            }
        }
        return regions.Count;
    }

    public static int GetBadgeID(String region, int index)
    {
        List<BadgeDeclaration> cBadges = [];
        foreach (BadgeDeclaration b in _badges)
        {
            if (b.Region.ToLower().Equals(region.ToLower()) == true)
            {
                cBadges.Add(b);
            }
        }
        if (cBadges.Count - 1 >= index)
        {
            return cBadges[index].ID;
        }
        return 1;
    }

    public static bool PlayerHasBadge(int badgeID)
    {
        return Core.Player.Badges.Contains(badgeID);
    }
}
