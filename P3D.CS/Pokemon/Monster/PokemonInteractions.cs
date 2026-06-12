using Microsoft.Xna.Framework;
using P3D.Items;

namespace P3D;

public class PokemonInteractions
{
    // Friendship tier thresholds
    private const int FRIENDSHIP_HATE_MAX = 50;
    private const int FRIENDSHIP_NEUTRAL_MAX = 120;
    private const int FRIENDSHIP_LIKES_MAX = 200;
    private const int FRIENDSHIP_LOYAL_MAX = 245;

    // Reaction selection probabilities
    private const int HP_LOW_REACTION_THRESHOLD = 15;
    private const int LOW_HP_REACTION_CHANCE = 40;
    private const int SPECIAL_REACTION_CHANCE = 60;
    private const int LIKE_REACTION_CHANCE = 60;
    private const int LOYAL_REACTION_CHANCE = 60;
    private const int LOYAL_LIKE_CHANCE_MAX = 75;
    private const int LOVE_LOYAL_REACTION_CHANCE = 55;
    private const int LOVE_LOYAL_CHANCE_MAX = 65;
    private const int LOVE_LIKE_CHANCE_MAX = 75;

    // Stat thresholds for conditional reactions
    private const int STAT_FAST_SPEED = 100;
    private const int STAT_HIGH_ATTACK = 100;
    private const int STAT_HIGH_DEFENSE = 100;
    private const int STAT_HIGH_SP_ATTACK = 100;
    private const int STAT_HIGH_LEVEL_LOYAL = 30;
    private const int STAT_LOW_LEVEL_CAVE = 20;

    // Pokémon size thresholds for love reactions
    private const float HEIGHT_SMALL = 0.7f;
    private const float HEIGHT_MEDIUM = 1.6f;

    // Entity proximity radii
    private const float PROXIMITY_WATER = 5.0f;
    private const float PROXIMITY_NPC = 4.0f;
    private const float PROXIMITY_TRAINER = 3.0f;
    private const float PROXIMITY_GRASS = 5.0f;
    private const float PROXIMITY_ITEM = 5.0f;
    private const float PROXIMITY_ICE = 2.0f;
    private const float PROXIMITY_LOAMY_SOIL = 5.0f;

    // MessageBulb position offsets used in action script strings
    private const float NOTIFICATION_HEIGHT_OFFSET = 0.7f;
    private const int CAMERA_NOTIFICATION_Y = 1;

    // Follower pickup — friendship and chance
    private const int PICKUP_FRIENDSHIP_DIVISOR = 270;
    private const int PICKUP_FEATHER_OUTSIDE_CHANCE = 90;
    private const int PICKUP_ROCK_CAVE_CHANCE = 90;
    private const int PICKUP_ICE_NEVERMELTICE_CHANCE = 20;
    private const int PICKUP_LOAMY_BERRY_CHANCE = 50;
    private const int PICKUP_GRASS_CHANCE = 50;
    private const int PICKUP_WATER_CHANCE = 50;
    private const int PICKUP_CAVE_WATER_CHANCE = 65;

    // Item IDs — pickup system
    private const int ITEM_STICKY_FEATHER = 261;
    private const int ITEM_STICKY_ROCK = 262;
    private const int ITEM_NEVERMELTICE = 107;
    private const int ITEM_ASPEAR_BERRY = 2004;
    private const int ITEM_LEAF_STONE = 34;
    private const int ITEM_ENERGY_ROOT = 122;
    private const int ITEM_REVIVAL_HERB = 124;
    private const int ITEM_HEAL_POWDER = 123;
    private const int ITEM_WATER_STONE = 24;
    private const int ITEM_PEARL = 110;
    private const int ITEM_BIG_PEARL = 190;
    private const int ITEM_HEART_SCALE = 111;
    private const int ITEM_THUNDERSTONE = 22;
    private const int ITEM_FIRE_STONE = 23;
    private const int ITEM_HARD_STONE = 125;
    private const int ITEM_EVERSTONE = 112;
    private const int ITEM_MOON_STONE = 8;
    private const int ITEM_SUN_STONE = 169;
    private const int ITEM_GOLD_LEAF = 75;
    private const int ITEM_SILVER_LEAF = 60;
    private const int ITEM_LEFTOVERS = 146;
    private const int ITEM_LAVACOOKIE = 7;
    private const int ITEM_MAX_POTION = 15;
    private const int ITEM_HYPER_POTION = 16;
    private const int ITEM_SUPER_POTION = 17;
    private const int ITEM_POTION = 18;
    private const int ITEM_ETHER = 63;
    private const int ITEM_ELIXIR = 65;
    private const int ITEM_QUICK_CLAW = 73;

    // Item ID ranges
    private const int ITEM_WINGS_FIRST = 254;
    private const int ITEM_STICKY_FEATHER_EXCLUSIVE = 261;
    private const int ITEM_WINGS_ALL_EXCLUSIVE = 262;
    private const int ITEM_BERRY_FIRST = 2000;
    private const int ITEM_BERRY_BASIC_EXCLUSIVE = 2011;
    private const int ITEM_BERRY_ALL_EXCLUSIVE = 2064;

    // Badge thresholds for indoor potion-tier pickup
    private const int BADGE_SUPER_POTION_MIN = 2;
    private const int BADGE_HYPER_POTION_MIN = 5;
    private const int BADGE_MAX_POTION_MIN = 8;

    // Outdoor pickup probability thresholds (cumulative %)
    private const int OUTDOOR_GRASS_LEAFSTONE_CHANCE = 10;
    private const int OUTDOOR_GRASS_ENERGYROOT_CHANCE = 60;
    private const int OUTDOOR_GRASS_REVIVALHERB_CHANCE = 75;
    private const int OUTDOOR_WATER_WATERSTONE_CHANCE = 10;
    private const int OUTDOOR_WATER_PEARL_CHANCE = 50;
    private const int OUTDOOR_WATER_BIGPEARL_CHANCE = 60;
    private const int OUTDOOR_GENERAL_BERRY_CHANCE = 45;
    private const int OUTDOOR_GENERAL_WINGS_CHANCE = 90;
    private const int OUTDOOR_GENERAL_GOLDLEAF_CHANCE = 92;

    // Indoor pickup probability thresholds
    private const int INDOOR_LEFTOVERS_CHANCE = 5;
    private const int INDOOR_LAVACOOKIE_CHANCE = 25;
    private const int INDOOR_POTION_CHANCE = 45;
    private const int INDOOR_ETHER_CHANCE = 65;
    private const int INDOOR_ELIXIR_CHANCE = 85;
    private const int INDOOR_QUICKCLAW_CHANCE = 93;
    private const int INDOOR_GOLDLEAF_CHANCE = 95;

    // Cave pickup probability thresholds
    private const int CAVE_WATER_WATERSTONE_CHANCE = 30;
    private const int CAVE_WATER_PEARL_CHANCE = 70;
    private const int CAVE_WATER_BIGPEARL_CHANCE = 80;
    private const int CAVE_GENERAL_FIRESTONE_CHANCE = 10;
    private const int CAVE_GENERAL_THUNDERSTONE_CHANCE = 20;
    private const int CAVE_GENERAL_PEARL_CHANCE = 50;
    private const int CAVE_GENERAL_HARDSTONE_CHANCE = 60;
    private const int CAVE_GENERAL_EVERSTONE_CHANCE = 70;
    private const int CAVE_GENERAL_BERRY_CHANCE = 90;
    private const int CAVE_ROCK_THUNDERSTONE_CHANCE = 20;
    private const int CAVE_ROCK_FIRESTONE_CHANCE = 40;
    private const int CAVE_ROCK_WATERSTONE_CHANCE = 60;
    private const int CAVE_ROCK_LEAFSTONE_CHANCE = 80;
    private const int CAVE_ROCK_MOONSTONE_CHANCE = 90;

    public enum FriendshipLevels
    {
        Hate,
        Neutral,
        Likes,
        Loyal,
        Love
    }

    private static readonly List<ReactionContainer> _specialReactionList = [];
    private static String _pickupIndividualValue = String.Empty;
    private static String _pickupItemId = "-1";

    // -------------------------------------------------------------------------
    // Public API
    // -------------------------------------------------------------------------

    public static String GetScriptString(Pokemon p, Vector3 cPosition, int facing)
    {
        if (_pickupItemId.Equals("-1") == false)
        {
            if (_pickupIndividualValue.Equals(p.IndividualValue))
            {
                return GenerateItemReaction(p, cPosition, facing);
            }
            else
            {
                _pickupItemId = "-1";
                _pickupIndividualValue = String.Empty;
            }
        }

        ReactionContainer reaction = GetReaction(p);

        Vector2 newPosition = new Vector2(0, 1);

        String s = $"version=2{Environment.NewLine}" +
                   $"@pokemon.cry({PokemonForms.GetPokemonDataFileName(p.Number, p.AdditionalData, true)}){Environment.NewLine}";

        if (Screen.Camera is OverworldCamera oc && oc.ThirdPerson == false)
        {
            if (reaction.hasNotification == true)
            {
                s += $"@camera.activatethirdperson{Environment.NewLine}" +
                     $"@camera.setposition({newPosition.X},{CAMERA_NOTIFICATION_Y},{newPosition.Y}){Environment.NewLine}";

                s += $"@entity.showmessagebulb({(int)reaction.GetNotification()}|{cPosition.X}|{cPosition.Y + NOTIFICATION_HEIGHT_OFFSET}|{cPosition.Z}){Environment.NewLine}";

                s += $"@camera.deactivatethirdperson{Environment.NewLine}";
            }
            s += $"@text.show({reaction.GetMessage(p)}){Environment.NewLine}";
        }
        else
        {
            float preYaw = Screen.Camera != null ? Screen.Camera.Yaw : 0f;
            if (reaction.hasNotification == true)
            {
                if (Screen.Camera is OverworldCamera owCam)
                {
                    s += $"@camera.setyaw({owCam.GetAimYawFromDirection(Screen.Camera.GetPlayerFacingDirection())}){Environment.NewLine}";
                }
                s += $"@camera.setposition({newPosition.X},{CAMERA_NOTIFICATION_Y},{newPosition.Y}){Environment.NewLine}";
                s += $"@entity.showmessagebulb({(int)reaction.GetNotification()}|{cPosition.X}|{cPosition.Y + NOTIFICATION_HEIGHT_OFFSET}|{cPosition.Z}){Environment.NewLine}";

                s += $"@camera.deactivatethirdperson{Environment.NewLine}";
            }
            s += $"@text.show({reaction.GetMessage(p)}){Environment.NewLine}";
            s += $"@camera.activatethirdperson{Environment.NewLine}";
            s += $"@camera.reset{Environment.NewLine}";
            s += $"@camera.setyaw({preYaw}){Environment.NewLine}";
        }
        s += ":end";

        return s;
    }

    public static void Load()
    {
        _specialReactionList.Clear();

        String path = GameModeManager.GetContentFilePath("Data\\interactions.dat");
        Security.FileValidation.CheckFileValid(path, false, "PokemonInteractions.cs");

        String[] data = File.ReadAllLines(path);

        foreach (String line in data)
        {
            if (line.StartsWith("{") == true && line.EndsWith("}") == true)
            {
                if (line.CountSeperators("|") >= 8)
                {
                    ReactionContainer r = new ReactionContainer(line);
                    _specialReactionList.Add(r);
                }
            }
        }
    }

    public static void CheckForRandomPickup()
    {
        if (Screen.Level != null && Screen.Level.ShowOverworldPokemon == true &&
            bool.Parse(GameModeManager.GetGameRuleValue("ShowFollowPokemon", "1")) == true &&
            bool.Parse(GameModeManager.GetGameRuleValue("RandomFollowItemPickup", "1")) == true)
        {
            if (Core.Player.Pokemons.Count > 0 && Screen.Level.Surfing == false &&
                Screen.Level.Riding == false && Screen.Level.ShowOverworldPokemon == true &&
                Core.Player.GetWalkPokemon() != null)
            {
                if (Core.Player.GetWalkPokemon()!.Status == Pokemon.StatusProblems.None)
                {
                    if (_pickupIndividualValue.Equals(Core.Player.GetWalkPokemon()!.IndividualValue) == false)
                    {
                        _pickupItemId = "-1";
                    }

                    if (Core.Random.Next(0, PICKUP_FRIENDSHIP_DIVISOR) < Core.Player.GetWalkPokemon()!.Friendship)
                    {
                        int newItemId = -1;

                        if (IsOutside() == true)
                        {
                            Pokemon walkPokemon = Core.Player.GetWalkPokemon()!;
                            if (walkPokemon.Item != null && walkPokemon.Item.IsGameModeItem == false &&
                                walkPokemon.Item.ID == ITEM_STICKY_FEATHER &&
                                Core.Random.Next(0, 100) < PICKUP_FEATHER_OUTSIDE_CHANCE)
                            {
                                newItemId = Core.Random.Next(ITEM_WINGS_FIRST, ITEM_STICKY_FEATHER_EXCLUSIVE);
                            }
                            else
                            {
                                if (IceAround() == true)
                                {
                                    if (Core.Random.Next(0, 100) < PICKUP_ICE_NEVERMELTICE_CHANCE)
                                    {
                                        newItemId = ITEM_NEVERMELTICE;
                                    }
                                    else
                                    {
                                        newItemId = ITEM_ASPEAR_BERRY;
                                    }
                                }
                                else
                                {
                                    if (LoamySoilAround() == true && Core.Random.Next(0, 100) < PICKUP_LOAMY_BERRY_CHANCE)
                                    {
                                        newItemId = Core.Random.Next(ITEM_BERRY_FIRST, ITEM_BERRY_ALL_EXCLUSIVE);
                                    }
                                    else
                                    {
                                        if (GrassAround() == true && Core.Random.Next(0, 100) < PICKUP_GRASS_CHANCE)
                                        {
                                            int r = Core.Random.Next(0, 100);
                                            if (r < OUTDOOR_GRASS_LEAFSTONE_CHANCE)
                                            {
                                                newItemId = ITEM_LEAF_STONE;
                                            }
                                            else if (r >= OUTDOOR_GRASS_LEAFSTONE_CHANCE && r < OUTDOOR_GRASS_ENERGYROOT_CHANCE)
                                            {
                                                newItemId = ITEM_ENERGY_ROOT;
                                            }
                                            else if (r >= OUTDOOR_GRASS_ENERGYROOT_CHANCE && r < OUTDOOR_GRASS_REVIVALHERB_CHANCE)
                                            {
                                                newItemId = ITEM_REVIVAL_HERB;
                                            }
                                            else
                                            {
                                                newItemId = ITEM_HEAL_POWDER;
                                            }
                                        }
                                        else
                                        {
                                            if (WaterAround() == true && Core.Random.Next(0, 100) < PICKUP_WATER_CHANCE)
                                            {
                                                int r = Core.Random.Next(0, 100);
                                                if (r < OUTDOOR_WATER_WATERSTONE_CHANCE)
                                                {
                                                    newItemId = ITEM_WATER_STONE;
                                                }
                                                else if (r >= OUTDOOR_WATER_WATERSTONE_CHANCE && r < OUTDOOR_WATER_PEARL_CHANCE)
                                                {
                                                    newItemId = ITEM_PEARL;
                                                }
                                                else if (r >= OUTDOOR_WATER_PEARL_CHANCE && r < OUTDOOR_WATER_BIGPEARL_CHANCE)
                                                {
                                                    newItemId = ITEM_BIG_PEARL;
                                                }
                                                else
                                                {
                                                    newItemId = ITEM_HEART_SCALE;
                                                }
                                            }
                                            else
                                            {
                                                int r = Core.Random.Next(0, 100);
                                                if (r < OUTDOOR_GENERAL_BERRY_CHANCE)
                                                {
                                                    newItemId = Core.Random.Next(ITEM_BERRY_FIRST, ITEM_BERRY_BASIC_EXCLUSIVE);
                                                }
                                                else if (r >= OUTDOOR_GENERAL_BERRY_CHANCE && r < OUTDOOR_GENERAL_WINGS_CHANCE)
                                                {
                                                    newItemId = Core.Random.Next(ITEM_WINGS_FIRST, ITEM_WINGS_ALL_EXCLUSIVE);
                                                }
                                                else if (r >= OUTDOOR_GENERAL_WINGS_CHANCE && r < OUTDOOR_GENERAL_GOLDLEAF_CHANCE)
                                                {
                                                    newItemId = ITEM_GOLD_LEAF;
                                                }
                                                else
                                                {
                                                    newItemId = ITEM_SILVER_LEAF;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (IsInside() == true)
                        {
                            int r = Core.Random.Next(0, 100);
                            if (r < INDOOR_LEFTOVERS_CHANCE)
                            {
                                newItemId = ITEM_LEFTOVERS;
                            }
                            else if (r >= INDOOR_LEFTOVERS_CHANCE && r < INDOOR_LAVACOOKIE_CHANCE)
                            {
                                newItemId = ITEM_LAVACOOKIE;
                            }
                            else if (r >= INDOOR_LAVACOOKIE_CHANCE && r < INDOOR_POTION_CHANCE)
                            {
                                int b = Core.Player.Badges.Count;
                                if (b <= BADGE_SUPER_POTION_MIN - 1)
                                {
                                    newItemId = ITEM_POTION;
                                }
                                else if (b >= BADGE_SUPER_POTION_MIN && b <= BADGE_HYPER_POTION_MIN - 1)
                                {
                                    newItemId = ITEM_SUPER_POTION;
                                }
                                else if (b >= BADGE_HYPER_POTION_MIN && b <= BADGE_MAX_POTION_MIN - 1)
                                {
                                    newItemId = ITEM_HYPER_POTION;
                                }
                                else
                                {
                                    newItemId = ITEM_MAX_POTION;
                                }
                            }
                            else if (r >= INDOOR_POTION_CHANCE && r < INDOOR_ETHER_CHANCE)
                            {
                                newItemId = ITEM_ETHER;
                            }
                            else if (r >= INDOOR_ETHER_CHANCE && r < INDOOR_ELIXIR_CHANCE)
                            {
                                newItemId = ITEM_ELIXIR;
                            }
                            else if (r >= INDOOR_ELIXIR_CHANCE && r < INDOOR_QUICKCLAW_CHANCE)
                            {
                                newItemId = ITEM_QUICK_CLAW;
                            }
                            else if (r >= INDOOR_QUICKCLAW_CHANCE && r < INDOOR_GOLDLEAF_CHANCE)
                            {
                                newItemId = ITEM_GOLD_LEAF;
                            }
                            else
                            {
                                newItemId = ITEM_SILVER_LEAF;
                            }
                        }
                        else if (IsCave() == true)
                        {
                            Pokemon walkPokemon = Core.Player.GetWalkPokemon()!;
                            if (walkPokemon.Item != null && walkPokemon.Item.IsGameModeItem == false &&
                                walkPokemon.Item.ID == ITEM_STICKY_ROCK &&
                                Core.Random.Next(0, 100) < PICKUP_ROCK_CAVE_CHANCE)
                            {
                                int r1 = Core.Random.Next(0, 100);
                                if (r1 < CAVE_ROCK_THUNDERSTONE_CHANCE)
                                {
                                    newItemId = ITEM_THUNDERSTONE;
                                }
                                else if (r1 >= CAVE_ROCK_THUNDERSTONE_CHANCE && r1 < CAVE_ROCK_FIRESTONE_CHANCE)
                                {
                                    newItemId = ITEM_FIRE_STONE;
                                }
                                else if (r1 >= CAVE_ROCK_FIRESTONE_CHANCE && r1 < CAVE_ROCK_WATERSTONE_CHANCE)
                                {
                                    newItemId = ITEM_WATER_STONE;
                                }
                                else if (r1 >= CAVE_ROCK_WATERSTONE_CHANCE && r1 < CAVE_ROCK_LEAFSTONE_CHANCE)
                                {
                                    newItemId = ITEM_LEAF_STONE;
                                }
                                else if (r1 >= CAVE_ROCK_LEAFSTONE_CHANCE && r1 < CAVE_ROCK_MOONSTONE_CHANCE)
                                {
                                    newItemId = ITEM_MOON_STONE;
                                }
                                else
                                {
                                    newItemId = ITEM_SUN_STONE;
                                }
                            }
                            else
                            {
                                if (WaterAround() == true && Core.Random.Next(0, 100) < PICKUP_CAVE_WATER_CHANCE)
                                {
                                    int r = Core.Random.Next(0, 100);
                                    if (r < CAVE_WATER_WATERSTONE_CHANCE)
                                    {
                                        newItemId = ITEM_WATER_STONE;
                                    }
                                    else if (r >= CAVE_WATER_WATERSTONE_CHANCE && r < CAVE_WATER_PEARL_CHANCE)
                                    {
                                        newItemId = ITEM_PEARL;
                                    }
                                    else if (r >= CAVE_WATER_PEARL_CHANCE && r < CAVE_WATER_BIGPEARL_CHANCE)
                                    {
                                        newItemId = ITEM_BIG_PEARL;
                                    }
                                    else
                                    {
                                        newItemId = ITEM_HEART_SCALE;
                                    }
                                }
                                else
                                {
                                    int r = Core.Random.Next(0, 100);
                                    if (r < CAVE_GENERAL_FIRESTONE_CHANCE)
                                    {
                                        newItemId = ITEM_FIRE_STONE;
                                    }
                                    else if (r >= CAVE_GENERAL_FIRESTONE_CHANCE && r < CAVE_GENERAL_THUNDERSTONE_CHANCE)
                                    {
                                        newItemId = ITEM_THUNDERSTONE;
                                    }
                                    else if (r >= CAVE_GENERAL_THUNDERSTONE_CHANCE && r < CAVE_GENERAL_PEARL_CHANCE)
                                    {
                                        newItemId = ITEM_PEARL;
                                    }
                                    else if (r >= CAVE_GENERAL_PEARL_CHANCE && r < CAVE_GENERAL_HARDSTONE_CHANCE)
                                    {
                                        newItemId = ITEM_HARD_STONE;
                                    }
                                    else if (r >= CAVE_GENERAL_HARDSTONE_CHANCE && r < CAVE_GENERAL_EVERSTONE_CHANCE)
                                    {
                                        newItemId = ITEM_EVERSTONE;
                                    }
                                    else if (r >= CAVE_GENERAL_EVERSTONE_CHANCE && r < CAVE_GENERAL_BERRY_CHANCE)
                                    {
                                        newItemId = Core.Random.Next(ITEM_BERRY_FIRST, ITEM_BERRY_BASIC_EXCLUSIVE);
                                    }
                                    else
                                    {
                                        newItemId = ITEM_STICKY_ROCK;
                                    }
                                }
                            }
                        }

                        if (newItemId > -1)
                        {
                            Item? foundItem = Item.GetItemByID(newItemId.ToString());
                            if (foundItem != null)
                            {
                                Logger.Debug($"Pokémon picks up item ({foundItem.Name})");
                            }
                            _pickupItemId = newItemId.ToString();
                            _pickupIndividualValue = Core.Player.GetWalkPokemon()!.IndividualValue;
                            SoundManager.PlaySound("pickup");
                        }
                    }
                }
            }
            else
            {
                _pickupItemId = "-1";
                _pickupIndividualValue = String.Empty;
            }
        }
    }

    // -------------------------------------------------------------------------
    // Private — reaction generation
    // -------------------------------------------------------------------------

    private static String GenerateItemReaction(Pokemon p, Vector3 cPosition, int facing)
    {
        String message = Localization.GetString("FollowerInteraction_HeldItem_Question",
            "It looks like your Pokémon~holds on to something.*Do you want to~take it?");

        Vector2 newPosition = new Vector2(0, 1);

        Item? item = Item.GetItemByID(_pickupItemId);

        float preYaw = Screen.Camera != null ? Screen.Camera.Yaw : 0f;

        String s = $"version=2{Environment.NewLine}" +
                   $"@pokemon.cry({PokemonForms.GetPokemonDataFileName(p.Number, p.AdditionalData)}){Environment.NewLine}";

        if (Screen.Camera is OverworldCamera oc && oc.ThirdPerson == false)
        {
            s += $"@camera.activatethirdperson{Environment.NewLine}" +
                 $"@camera.setposition({newPosition.X},{CAMERA_NOTIFICATION_Y},{newPosition.Y}){Environment.NewLine}";

            s += $"@entity.showmessagebulb({(int)MessageBulb.NotificationTypes.Question}|{cPosition.X}|{cPosition.Y + NOTIFICATION_HEIGHT_OFFSET}|{cPosition.Z}){Environment.NewLine}";

            s += $"@camera.deactivatethirdperson{Environment.NewLine}";
            s += BuildItemChoiceScript(message, item);
        }
        else
        {
            if (Screen.Camera is OverworldCamera owCam)
            {
                s += $"@camera.setyaw({owCam.GetAimYawFromDirection(Screen.Camera.GetPlayerFacingDirection())}){Environment.NewLine}";
            }
            s += $"@camera.setposition({newPosition.X},{CAMERA_NOTIFICATION_Y},{newPosition.Y}){Environment.NewLine}";
            s += $"@entity.showmessagebulb({(int)MessageBulb.NotificationTypes.Question}|{cPosition.X}|{cPosition.Y + NOTIFICATION_HEIGHT_OFFSET}|{cPosition.Z}){Environment.NewLine}";

            s += $"@camera.deactivatethirdperson{Environment.NewLine}";
            s += BuildItemChoiceScript(message, item);
            s += $"@camera.activatethirdperson{Environment.NewLine}";
            s += $"@camera.reset{Environment.NewLine}";
            s += $"@camera.setyaw({preYaw}){Environment.NewLine}";
        }
        s += ":end";

        _pickupItemId = "-1";
        _pickupIndividualValue = String.Empty;

        return s;
    }

    private static String BuildItemChoiceScript(String message, Item item)
    {
        String yesText = Localization.GetString("FollowerInteraction_HeldItem_Answer_Yes",
            "Your Pokémon handed over~the [ITEM]!").Replace("[ITEM]", item.OneLineName());
        String noText = Localization.GetString("FollowerInteraction_HeldItem_Answer_No",
            "Your Pokémon kept~the item happily.");

        return $"@text.show({message}){Environment.NewLine}" +
               $"@options.show(<system.token(global_yes)>,<system.token(global_no)>){Environment.NewLine}" +
               $":when:<system.token(global_yes)>{Environment.NewLine}" +
               $"@text.show({yesText}){Environment.NewLine}" +
               $"@item.give({_pickupItemId},1){Environment.NewLine}" +
               $"@item.messagegive({_pickupItemId},1){Environment.NewLine}" +
               $":when:<system.token(global_no)>{Environment.NewLine}" +
               $"@text.show({noText}){Environment.NewLine}" +
               $"@pokemon.addfriendship(0,10){Environment.NewLine}" +
               $":endwhen{Environment.NewLine}";
    }

    private static ReactionContainer GetReaction(Pokemon p)
    {
        FriendshipLevels friendshipLevel = GetFriendshipLevel(p);
        ReactionContainer? reaction = null;

        if (p.Status != Pokemon.StatusProblems.None)
        {
            reaction = GetStatusConditionReaction(p);
        }

        if (reaction == null && ((float)p.HP / p.MaxHP) * 100f <= HP_LOW_REACTION_THRESHOLD)
        {
            if (Core.Random.Next(0, 100) < LOW_HP_REACTION_CHANCE)
            {
                reaction = GetLowHPReaction(p);
            }
        }

        if (reaction == null)
        {
            if (Core.Random.Next(0, 100) < SPECIAL_REACTION_CHANCE)
            {
                reaction = GetSpecialReaction(p);
            }
        }

        if (reaction == null)
        {
            int r = Core.Random.Next(0, 100);
            switch (friendshipLevel)
            {
                case FriendshipLevels.Hate:
                    reaction = GetHateReaction(p);
                    break;

                case FriendshipLevels.Neutral:
                    reaction = GetNeutralReaction(p);
                    break;

                case FriendshipLevels.Likes:
                    if (r < LIKE_REACTION_CHANCE)
                    {
                        reaction = GetLikeReaction(p);
                    }
                    else
                    {
                        reaction = GetNeutralReaction(p);
                    }
                    break;

                case FriendshipLevels.Loyal:
                    if (r < LOYAL_REACTION_CHANCE)
                    {
                        reaction = GetLoyalReaction(p);
                    }
                    else if (r >= LOYAL_REACTION_CHANCE && r < LOYAL_LIKE_CHANCE_MAX)
                    {
                        reaction = GetLikeReaction(p);
                    }
                    else
                    {
                        reaction = GetNeutralReaction(p);
                    }
                    break;

                case FriendshipLevels.Love:
                    if (r < LOVE_LOYAL_REACTION_CHANCE)
                    {
                        reaction = GetLoveReaction(p);
                    }
                    else if (r >= LOVE_LOYAL_REACTION_CHANCE && r < LOVE_LOYAL_CHANCE_MAX)
                    {
                        reaction = GetLoyalReaction(p);
                    }
                    else if (r >= LOVE_LOYAL_CHANCE_MAX && r < LOVE_LIKE_CHANCE_MAX)
                    {
                        reaction = GetLikeReaction(p);
                    }
                    else
                    {
                        reaction = GetNeutralReaction(p);
                    }
                    break;
            }
        }

        return reaction!;
    }

    private static FriendshipLevels GetFriendshipLevel(Pokemon p)
    {
        int f = p.Friendship;
        if (f <= FRIENDSHIP_HATE_MAX) return FriendshipLevels.Hate;
        if (f > FRIENDSHIP_HATE_MAX && f <= FRIENDSHIP_NEUTRAL_MAX) return FriendshipLevels.Neutral;
        if (f > FRIENDSHIP_NEUTRAL_MAX && f <= FRIENDSHIP_LIKES_MAX) return FriendshipLevels.Likes;
        if (f > FRIENDSHIP_LIKES_MAX && f <= FRIENDSHIP_LOYAL_MAX) return FriendshipLevels.Loyal;
        return FriendshipLevels.Love;
    }

    private static ReactionContainer GetStatusConditionReaction(Pokemon p)
    {
        switch (p.Status)
        {
            case Pokemon.StatusProblems.BadPoison:
            case Pokemon.StatusProblems.Poison:
                return new ReactionContainer(
                    Localization.GetString("FollowerInteraction_StatusEffect_Poison",
                        "<name> is shivering~with the effects of being~poisoned."),
                    MessageBulb.NotificationTypes.Poisoned);

            case Pokemon.StatusProblems.Burn:
                return new ReactionContainer(
                    Localization.GetString("FollowerInteraction_StatusEffect_Burn",
                        "<name>'s burn~looks painful!"),
                    MessageBulb.NotificationTypes.Poisoned);

            case Pokemon.StatusProblems.Freeze:
                switch (Core.Random.Next(0, 2))
                {
                    case 0:
                        return new ReactionContainer(
                            Localization.GetString("FollowerInteraction_StatusEffect_Freeze1",
                                "<name> seems very cold!"),
                            MessageBulb.NotificationTypes.Poisoned);
                    case 1:
                        return new ReactionContainer(
                            Localization.GetString("FollowerInteraction_StatusEffect_Freeze2",
                                ".....Your Pokémon seems~a little cold."),
                            MessageBulb.NotificationTypes.Poisoned);
                }
                break;

            case Pokemon.StatusProblems.Paralyzed:
                return new ReactionContainer(
                    Localization.GetString("FollowerInteraction_StatusEffect_Paralyzed",
                        "<name> is trying~very hard to keep~up with you..."),
                    MessageBulb.NotificationTypes.Poisoned);

            case Pokemon.StatusProblems.Sleep:
                switch (Core.Random.Next(0, 3))
                {
                    case 0:
                        return new ReactionContainer(
                            Localization.GetString("FollowerInteraction_StatusEffect_Sleep1",
                                "<name> seems~a little tired."),
                            MessageBulb.NotificationTypes.Poisoned);
                    case 1:
                        return new ReactionContainer(
                            Localization.GetString("FollowerInteraction_StatusEffect_Sleep2",
                                "<name> is somehow~fighting off sleep..."),
                            MessageBulb.NotificationTypes.Poisoned);
                    case 2:
                        return new ReactionContainer(
                            Localization.GetString("FollowerInteraction_StatusEffect_Sleep3",
                                "<name> yawned~very loudly!"),
                            MessageBulb.NotificationTypes.Poisoned);
                }
                break;
        }

        return new ReactionContainer(
            Localization.GetString("FollowerInteraction_StatusEffect_Other",
                "<name> is trying~very hard to keep~up with you..."),
            MessageBulb.NotificationTypes.Poisoned);
    }

    private static ReactionContainer GetLowHPReaction(Pokemon p)
    {
        switch (Core.Random.Next(0, 2))
        {
            case 0:
                return new ReactionContainer(
                    Localization.GetString("FollowerInteraction_LowHP1",
                        "<name> is going~to fall down!"),
                    MessageBulb.NotificationTypes.Exclamation);
            case 1:
                return new ReactionContainer(
                    Localization.GetString("FollowerInteraction_LowHP2",
                        "<name> seems to~be about to fall over!"),
                    MessageBulb.NotificationTypes.Exclamation);
        }
        return new ReactionContainer(
            Localization.GetString("FollowerInteraction_LowHP2",
                "<name> seems to~be about to fall over!"),
            MessageBulb.NotificationTypes.Exclamation);
    }

    private static ReactionContainer? GetSpecialReaction(Pokemon p)
    {
        List<ReactionContainer> matching = [];

        foreach (ReactionContainer spReaction in _specialReactionList)
        {
            if (spReaction.Match(p) == true)
            {
                matching.Add(spReaction);
            }
        }

        if (matching.Count > 0)
        {
            List<int> chances = [];
            foreach (ReactionContainer r in matching)
            {
                chances.Add(r.probability);
            }

            int index = Extensions.GetRandomChance(chances);
            return matching[index];
        }

        return null;
    }

    private static ReactionContainer GetHateReaction(Pokemon p)
    {
        ReactionContainer? r = null;
        while (r == null)
        {
            switch (Core.Random.Next(0, 17))
            {
                case 0:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Hate1", "<name> is doing~it's best to keep up~with you."), MessageBulb.NotificationTypes.Unhappy);
                    break;
                case 1:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Hate2", "<name> is somehow~forcing itself to keep going."), MessageBulb.NotificationTypes.Unsure);
                    break;
                case 2:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Hate3", "<name> is staring~patiantly at nothing at all."), MessageBulb.NotificationTypes.Unsure);
                    break;
                case 3:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Hate4", "<name> is staring~intently into the distance."), MessageBulb.NotificationTypes.Waiting);
                    break;
                case 4:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Hate5", "<name> is dizzy..."), MessageBulb.NotificationTypes.Unhappy);
                    break;
                case 5:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Hate6", "<name> is stepping~on your feet!"), MessageBulb.NotificationTypes.Waiting);
                    break;
                case 6:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Hate7", "<name> seems~unhappy somehow..."), MessageBulb.NotificationTypes.Unhappy);
                    break;
                case 7:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Hate8", "<name> is making~an unhappy face."), MessageBulb.NotificationTypes.Unhappy);
                    break;
                case 8:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Hate9", "<name> seems~uneasy and is poking~<player.name>."), MessageBulb.NotificationTypes.Unsure);
                    break;
                case 9:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Hate10", "<name> is making~a face like its angry!"), MessageBulb.NotificationTypes.Angry);
                    break;
                case 10:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Hate11", "<name> seems to be~angry for some reason."), MessageBulb.NotificationTypes.Angry);
                    break;
                case 11:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Hate12", "Your Pokémon turned to face~the other way, showing a~defiant expression."), MessageBulb.NotificationTypes.Unsure);
                    break;
                case 12:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Hate13", "<name> is looking~down steadily..."), MessageBulb.NotificationTypes.Waiting);
                    break;
                case 13:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Hate14", "Your Pokémon is staring~intently at nothing..."), MessageBulb.NotificationTypes.Waiting);
                    break;
                case 14:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Hate15", "Your Pokémon turned to~face the other way,~showing a defiant expression."), MessageBulb.NotificationTypes.Unhappy);
                    break;
                case 15:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Hate16", "<name> seems~a bit nervous..."), MessageBulb.NotificationTypes.Unsure);
                    break;
                case 16:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Hate17", "Your Pokémon stumbled~and nearly fell!"), MessageBulb.NotificationTypes.Exclamation);
                    break;
            }
        }
        return r;
    }

    private static ReactionContainer GetNeutralReaction(Pokemon p)
    {
        ReactionContainer? r = null;
        while (r == null)
        {
            switch (Core.Random.Next(0, 53))
            {
                case 0:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral1", "<name> is happy~but shy."), MessageBulb.NotificationTypes.Friendly);
                    break;
                case 1:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral2", "<name> puts in~extra effort."), MessageBulb.NotificationTypes.Friendly);
                    break;
                case 2:
                    if (IsOutside() == true)
                    {
                        r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral3", "<name> is smelling~the scents of the~surrounding air."), MessageBulb.NotificationTypes.Friendly);
                    }
                    break;
                case 3:
                    if (IsOutside() == true)
                    {
                        r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral4", "Your Pokémon has caught~the scent of smoke."), MessageBulb.NotificationTypes.Friendly);
                    }
                    break;
                case 4:
                    if (NPCAround() == true)
                    {
                        r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral5", "<name> greeted everyone!"), MessageBulb.NotificationTypes.CatFace);
                    }
                    break;
                case 5:
                    if (IsOutside() == true)
                    {
                        r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral6", "<name> is wandering~around and listening~to the different sounds."), MessageBulb.NotificationTypes.Note);
                    }
                    break;
                case 6:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral7", "<name> looks very~interested!"), MessageBulb.NotificationTypes.Friendly);
                    break;
                case 7:
                    if (IsOutside() == true)
                    {
                        r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral8", "<name> is steadily~poking at the ground."), MessageBulb.NotificationTypes.Waiting);
                    }
                    break;
                case 8:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral9", "Your Pokémon is looking~around restlessly."), MessageBulb.NotificationTypes.Note);
                    break;
                case 9:
                    if (IsOutside() == true)
                    {
                        r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral10", "<name> seems dazzled~after seeing the sky."), MessageBulb.NotificationTypes.Waiting);
                    }
                    break;
                case 10:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral11", "<name> is gazing~around restlessly!"), MessageBulb.NotificationTypes.Waiting);
                    break;
                case 11:
                    if (TrainerAround() == true)
                    {
                        r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral12", "<name> let out~a battle cry!"), MessageBulb.NotificationTypes.Shouting);
                    }
                    break;
                case 12:
                    if (TrainerAround() == true && p.IsType((int)Element.Types.Fire) == true)
                    {
                        r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral13", "<name> is vigorously~breathing fire!"), MessageBulb.NotificationTypes.Exclamation);
                    }
                    break;
                case 13:
                    if (TrainerAround() == true || NPCAround() == true)
                    {
                        r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral14", "<name> is on~the lookout!"), MessageBulb.NotificationTypes.Exclamation);
                    }
                    break;
                case 14:
                    if (TrainerAround() == true)
                    {
                        r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral15", "<name> roared!"), MessageBulb.NotificationTypes.Exclamation);
                    }
                    break;
                case 15:
                    if (TrainerAround() == true)
                    {
                        r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral16", "<name> let out a roar!"), MessageBulb.NotificationTypes.Exclamation);
                    }
                    break;
                case 16:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral17", "<name> is surveying~the area..."), MessageBulb.NotificationTypes.Waiting);
                    break;
                case 17:
                    if (IsInside() == true)
                    {
                        r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral18", "<name> is sniffing~at the floor."), MessageBulb.NotificationTypes.Question);
                    }
                    break;
                case 18:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral19", "<name> is peering~down."), MessageBulb.NotificationTypes.Question);
                    break;
                case 19:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral20", "<name> seems~to be wandering around."), MessageBulb.NotificationTypes.Note);
                    break;
                case 20:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral21", "<name> is looking~around absentmindedly."), MessageBulb.NotificationTypes.Waiting);
                    break;
                case 21:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral22", "<name> is relaxing~comfortably."), MessageBulb.NotificationTypes.Friendly);
                    break;
                case 22:
                    if (IsInside() == true)
                    {
                        r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral23", "<name> is sniffing~at the floor."), MessageBulb.NotificationTypes.Waiting);
                    }
                    break;
                case 23:
                    if (IsOutside() == true)
                    {
                        r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral24", "<name> seems to~relax as it hears the~sound of rustling leaves..."), MessageBulb.NotificationTypes.Friendly);
                    }
                    break;
                case 24:
                    if (IsOutside() == true)
                    {
                        r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral25", "<name> seems to~be listening to the~sound of rustling leaves..."), MessageBulb.NotificationTypes.Friendly);
                    }
                    break;
                case 25:
                    if (WaterAround() == true)
                    {
                        r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral26", "Your Pokémon is playing around~and splashing in the water!"), MessageBulb.NotificationTypes.Happy);
                    }
                    break;
                case 26:
                    if (IsOutside() == true)
                    {
                        r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral27", "<name> is looking~up at the sky."), MessageBulb.NotificationTypes.Waiting);
                    }
                    break;
                case 27:
                    if (IsOutside() == true && World.GetTime() == World.DayTimes.Night &&
                        World.GetCurrentRegionWeather() == World.Weathers.Clear)
                    {
                        r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral28", "Your Pokémon is happily~gazing at the beautiful,~starry sky!"), MessageBulb.NotificationTypes.Waiting);
                    }
                    break;
                case 28:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral29", "<name> seems to be~enjoying this a little bit!"), MessageBulb.NotificationTypes.Note);
                    break;
                case 29:
                    if (IsInside() == true)
                    {
                        r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral30", "<name> is looking~up at the ceiling."), MessageBulb.NotificationTypes.Note);
                    }
                    break;
                case 30:
                    if (IsOutside() == true && World.GetTime() == World.DayTimes.Night)
                    {
                        r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral31", "Your Pokémon is staring~spellbound at the night sky!"), MessageBulb.NotificationTypes.Friendly);
                    }
                    break;
                case 31:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral32", "<name> is in~danger of falling over!"), MessageBulb.NotificationTypes.Exclamation);
                    break;
                case 32:
                    if (String.IsNullOrEmpty(p.NickName) == false)
                    {
                        r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral33", "<name> doesn't~seem to be used to its~own name yet."), MessageBulb.NotificationTypes.Exclamation);
                    }
                    break;
                case 33:
                    if (IsInside() == true)
                    {
                        r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral34", "<name> slipped~on the floor and seems~likely to fall!"), MessageBulb.NotificationTypes.Exclamation);
                    }
                    break;
                case 34:
                    if (TrainerAround() == true || GrassAround() == true)
                    {
                        r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral35", "<name> feels something~and is howling!"), MessageBulb.NotificationTypes.Exclamation);
                    }
                    break;
                case 35:
                    if (p.HP == p.MaxHP && p.Status == Pokemon.StatusProblems.None)
                    {
                        r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral36", "<name> seems~refreshed!"), MessageBulb.NotificationTypes.Friendly);
                    }
                    break;
                case 36:
                    if (p.HP == p.MaxHP && p.Status == Pokemon.StatusProblems.None)
                    {
                        r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral37", "<name> feels~refreshed."), MessageBulb.NotificationTypes.Friendly);
                    }
                    break;
                case 37:
                    if (ItemAround() == true)
                    {
                        r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral38", "<name> seems to~have found something!"), MessageBulb.NotificationTypes.Exclamation);
                    }
                    break;
                case 38:
                    if (TrainerAround() == true || GrassAround() == true)
                    {
                        r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral39", "<name> suddenly~turned around and~started barking!"), MessageBulb.NotificationTypes.Exclamation);
                    }
                    break;
                case 39:
                    if (TrainerAround() == true || GrassAround() == true)
                    {
                        r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral40", "<name> suddenly~turned around!"), MessageBulb.NotificationTypes.Exclamation);
                    }
                    break;
                case 40:
                    if (IsOutside() == true)
                    {
                        r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral41", "<name> looked up~at the sky and shouted loudly!"), MessageBulb.NotificationTypes.Exclamation);
                    }
                    break;
                case 41:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral42", "Your Pokémon was surprised~that you suddenly spoke to it!"), MessageBulb.NotificationTypes.Exclamation);
                    break;
                case 42:
                    if (p.Item != null)
                    {
                        r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral43", "<name> almost forgot~it was holding~that <item>!"), MessageBulb.NotificationTypes.Question);
                    }
                    break;
                case 43:
                    if (IceAround() == true)
                    {
                        r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral44", "Oh!~It's slipping and came~over here for support."), MessageBulb.NotificationTypes.Exclamation);
                    }
                    break;
                case 44:
                    if (IceAround() == true)
                    {
                        r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral45", "Your Pokémon almost slipped~and fell over!"), MessageBulb.NotificationTypes.Exclamation);
                    }
                    break;
                case 45:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral46", "<name> sensed something~strange and was surprised!"), MessageBulb.NotificationTypes.Question);
                    break;
                case 46:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral47", "Your Pokémon is looking~around restlessly for~something."), MessageBulb.NotificationTypes.Question);
                    break;
                case 47:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral48", "Your Pokémon wasn't watching~where it was going and~ran into you!"), MessageBulb.NotificationTypes.Friendly);
                    break;
                case 48:
                    if (ItemAround() == true)
                    {
                        r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral49", "Sniff, sniff!~Is there something nearby?"), MessageBulb.NotificationTypes.Question);
                    }
                    break;
                case 49:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral50", "<name> is wandering~around and searching~for something."), MessageBulb.NotificationTypes.Question);
                    break;
                case 50:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral51", "<name> is sniffing~at <player.name>."), MessageBulb.NotificationTypes.Friendly);
                    break;
                case 51:
                    if (IsOutside() == true && World.GetCurrentRegionWeather() == World.Weathers.Rain && GrassAround() == true)
                    {
                        r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral52", "<name> is taking~shelter in the grass from~the rain!"), MessageBulb.NotificationTypes.Waiting);
                    }
                    break;
                case 52:
                    if (IsOutside() == true && World.GetCurrentRegionWeather() == World.Weathers.Rain && GrassAround() == true)
                    {
                        r = new ReactionContainer(Localization.GetString("FollowerInteraction_Neutral53", "<name> is splashing~around in the wet grass!"), MessageBulb.NotificationTypes.Note);
                    }
                    break;
            }
        }
        return r;
    }

    private static ReactionContainer GetLikeReaction(Pokemon p)
    {
        ReactionContainer? r = null;
        while (r == null)
        {
            switch (Core.Random.Next(0, 28))
            {
                case 0:
                    if (IsOutside() == true && World.GetCurrentRegionWeather() == World.Weathers.Clear)
                    {
                        r = new ReactionContainer(Localization.GetString("FollowerInteraction_Like1", "Your Pokémon seems happy~about the great weather!"), MessageBulb.NotificationTypes.Happy);
                    }
                    break;
                case 1:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Like2", "<name> is coming along~happily."), MessageBulb.NotificationTypes.Friendly);
                    break;
                case 2:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Like3", "<name> is composed!"), MessageBulb.NotificationTypes.Friendly);
                    break;
                case 3:
                    if (p.HP == p.MaxHP)
                    {
                        r = new ReactionContainer(Localization.GetString("FollowerInteraction_Like4", "<name> is glowing~with health!"), MessageBulb.NotificationTypes.Note);
                    }
                    break;
                case 4:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Like5", "<name> looks~very happy!"), MessageBulb.NotificationTypes.Happy);
                    break;
                case 5:
                    if (p.HP == p.MaxHP)
                    {
                        r = new ReactionContainer(Localization.GetString("FollowerInteraction_Like6", "<name> is full~of life!"), MessageBulb.NotificationTypes.Note);
                    }
                    break;
                case 6:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Like7", "<name> is very~eager!"), MessageBulb.NotificationTypes.Friendly);
                    break;
                case 7:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Like8", "<name> gives you~a happy look and a smile!"), MessageBulb.NotificationTypes.Happy);
                    break;
                case 8:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Like9", "<name> seems very~happy to see you!"), MessageBulb.NotificationTypes.Friendly);
                    break;
                case 9:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Like10", "<name> faced this~way and grinned!"), MessageBulb.NotificationTypes.CatFace);
                    break;
                case 10:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Like11", "<name> spun around~in a circle!"), MessageBulb.NotificationTypes.Note);
                    break;
                case 11:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Like12", "<name> is looking~this way and smiling."), MessageBulb.NotificationTypes.Friendly);
                    break;
                case 12:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Like13", "<name> is very~eager..."), MessageBulb.NotificationTypes.Waiting);
                    break;
                case 13:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Like14", "<name> is focusing~its attention on you!"), MessageBulb.NotificationTypes.Exclamation);
                    break;
                case 14:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Like15", "<name> focused~with a sharp gaze!"), MessageBulb.NotificationTypes.Exclamation);
                    break;
                case 15:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Like16", "<name> is looking~at <player.name>'s footprints."), MessageBulb.NotificationTypes.Question);
                    break;
                case 16:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Like17", "<name> is staring~straight into <player.name>'s~eyes."), MessageBulb.NotificationTypes.Friendly);
                    break;
                case 17:
                    if (p.baseSpeed >= STAT_FAST_SPEED)
                    {
                        r = new ReactionContainer(Localization.GetString("FollowerInteraction_Like18", "<name> is showing off~its agility!"), MessageBulb.NotificationTypes.Note);
                    }
                    break;
                case 18:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Like19", "<name> is moving~around happily!"), MessageBulb.NotificationTypes.Note);
                    break;
                case 19:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Like20", "<name> is steadily~keeping up with you!"), MessageBulb.NotificationTypes.Friendly);
                    break;
                case 20:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Like21", "<name> seems to~want to play with~<player.name>!"), MessageBulb.NotificationTypes.Note);
                    break;
                case 21:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Like22", "<name> is singing~and humming."), MessageBulb.NotificationTypes.Note);
                    break;
                case 22:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Like23", "<name> is playfully~nibbling at the ground."), MessageBulb.NotificationTypes.Friendly);
                    break;
                case 23:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Like24", "<name> is nipping~at your feet!"), MessageBulb.NotificationTypes.Note);
                    break;
                case 24:
                    if (p.baseAttack >= STAT_HIGH_ATTACK || p.baseSpAttack >= STAT_HIGH_SP_ATTACK)
                    {
                        r = new ReactionContainer(Localization.GetString("FollowerInteraction_Like25", "<name> is working~hard to show off~its mighty power!"), MessageBulb.NotificationTypes.Exclamation);
                    }
                    break;
                case 25:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Like26", "<name> is cheerful!"), MessageBulb.NotificationTypes.Note);
                    break;
                case 26:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Like27", "<name> bumped~into <player.name>!"), MessageBulb.NotificationTypes.Exclamation);
                    break;
                case 27:
                    if (IsCave() == true && p.Level < STAT_LOW_LEVEL_CAVE)
                    {
                        r = new ReactionContainer(Localization.GetString("FollowerInteraction_Like28", "<name> is scared~and snuggled up~to <player.name>!"), MessageBulb.NotificationTypes.Exclamation);
                    }
                    break;
            }
        }
        return r;
    }

    private static ReactionContainer GetLoyalReaction(Pokemon p)
    {
        ReactionContainer? r = null;
        while (r == null)
        {
            switch (Core.Random.Next(0, 21))
            {
                case 0:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Loyal1", "<name> began poking~you in the stomach!"), MessageBulb.NotificationTypes.CatFace);
                    break;
                case 1:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Loyal2", "<name> seems to be~feeling great about~walking with you!"), MessageBulb.NotificationTypes.Heart);
                    break;
                case 2:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Loyal3", "<name> is still~feeling great!"), MessageBulb.NotificationTypes.Happy);
                    break;
                case 3:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Loyal4", "<name> is poking~at your belly."), MessageBulb.NotificationTypes.Heart);
                    break;
                case 4:
                    if (p.Level > STAT_HIGH_LEVEL_LOYAL)
                    {
                        r = new ReactionContainer(Localization.GetString("FollowerInteraction_Loyal5", "<name> looks like~it wants to lead!"), MessageBulb.NotificationTypes.Note);
                    }
                    break;
                case 5:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Loyal6", "<name> seems to be~very happy!"), MessageBulb.NotificationTypes.Happy);
                    break;
                case 6:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Loyal7", "<name> nodded slowly."), MessageBulb.NotificationTypes.Friendly);
                    break;
                case 7:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Loyal8", "<name> gave you~a sunny look!"), MessageBulb.NotificationTypes.Happy);
                    break;
                case 8:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Loyal9", "<name> is very~composed and sure of itself!"), MessageBulb.NotificationTypes.Waiting);
                    break;
                case 9:
                    if (p.baseDefense >= STAT_HIGH_DEFENSE)
                    {
                        r = new ReactionContainer(Localization.GetString("FollowerInteraction_Loyal10", "<name> is~standing guard!"), MessageBulb.NotificationTypes.Exclamation);
                    }
                    break;
                case 10:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Loyal11", "<name> danced a~wonderful dance!"), MessageBulb.NotificationTypes.Note);
                    break;
                case 11:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Loyal12", "<name> is staring~steadfastly at~<player.name>'s face."), MessageBulb.NotificationTypes.Waiting);
                    break;
                case 12:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Loyal13", "<name> is staring~intently at~<player.name>'s face."), MessageBulb.NotificationTypes.Waiting);
                    break;
                case 13:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Loyal14", "<name> is concentrating."), MessageBulb.NotificationTypes.Unsure);
                    break;
                case 14:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Loyal15", "<name> faced this~way and nodded."), MessageBulb.NotificationTypes.Friendly);
                    break;
                case 15:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Loyal16", "<name> suddenly~started walking closer!"), MessageBulb.NotificationTypes.Heart);
                    break;
                case 16:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Loyal17", "Woah!*<name> is suddenly~playful!"), MessageBulb.NotificationTypes.Heart);
                    break;
                case 17:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Loyal18", "<name> blushes."), MessageBulb.NotificationTypes.Happy);
                    break;
                case 18:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Loyal19", "Woah!*<name> suddenly started~dancing in happiness!"), MessageBulb.NotificationTypes.Note);
                    break;
                case 19:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Loyal20", "<name> is happily~skipping about."), MessageBulb.NotificationTypes.Note);
                    break;
                case 20:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Loyal21", "Woah!*<name> suddenly~danced in happiness!"), MessageBulb.NotificationTypes.Note);
                    break;
            }
        }
        return r;
    }

    private static ReactionContainer GetLoveReaction(Pokemon p)
    {
        ReactionContainer? r = null;
        while (r == null)
        {
            switch (Core.Random.Next(0, 13))
            {
                case 0:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Love1", "<name> is jumping~for joy!"), MessageBulb.NotificationTypes.Heart);
                    break;
                case 1:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Love2", "Your Pokémon stretched out~its body and is relaxing."), MessageBulb.NotificationTypes.Happy);
                    break;
                case 2:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Love3", "<name> is happily~cuddling up to you!"), MessageBulb.NotificationTypes.Heart);
                    break;
                case 3:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Love4", "<name> is so happy~that it can't stand still!"), MessageBulb.NotificationTypes.Heart);
                    break;
                case 4:
                    if ((p.pokedexEntry?.height ?? 0f) <= HEIGHT_MEDIUM)
                    {
                        r = new ReactionContainer(Localization.GetString("FollowerInteraction_Love5", "<name> happily~cuddled up to you!"), MessageBulb.NotificationTypes.Heart);
                    }
                    break;
                case 5:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Love6", "<name>'s cheeks are~becoming rosy!"), MessageBulb.NotificationTypes.Heart);
                    break;
                case 6:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Love7", "Woah!*<name> suddenly~hugged you!"), MessageBulb.NotificationTypes.Heart);
                    break;
                case 7:
                    if ((p.pokedexEntry?.height ?? 0f) <= HEIGHT_SMALL)
                    {
                        r = new ReactionContainer(Localization.GetString("FollowerInteraction_Love8", "<name> is rubbing~against your legs!"), MessageBulb.NotificationTypes.Heart);
                    }
                    break;
                case 8:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Love9", "Ah!~<name> cuddles you!"), MessageBulb.NotificationTypes.Heart);
                    break;
                case 9:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Love10", "<name> is regarding~you with adoration!"), MessageBulb.NotificationTypes.Heart);
                    break;
                case 10:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Love11", "<name> got~closer to <player.name>!"), MessageBulb.NotificationTypes.Heart);
                    break;
                case 11:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Love12", "<name> is keeping~close to your feet."), MessageBulb.NotificationTypes.Heart);
                    break;
                case 12:
                    r = new ReactionContainer(Localization.GetString("FollowerInteraction_Love13", "<name> is jumping~around in a carefree way!"), MessageBulb.NotificationTypes.Note);
                    break;
            }
        }
        return r;
    }

    // -------------------------------------------------------------------------
    // Private — environment checks
    // -------------------------------------------------------------------------

    private static bool IsOutside()
    {
        if (Screen.Level == null) return false;
        if (Screen.Level.IsOutside == true) return true;
        if (Screen.Level.CanFly == true && Screen.Level.CanDig == false && Screen.Level.CanTeleport == true) return true;
        return false;
    }

    private static bool IsCave()
    {
        if (Screen.Level == null) return false;
        return Screen.Level.CanFly == false && Screen.Level.CanDig == true && Screen.Level.CanTeleport == false;
    }

    private static bool IsInside()
    {
        if (Screen.Level == null) return false;
        return Screen.Level.CanFly == false && Screen.Level.CanDig == false && Screen.Level.CanTeleport == false;
    }

    private static bool WaterAround()
    {
        if (Screen.Level == null) return false;
        foreach (Entity e in Screen.Level.Entities)
        {
            if (e.EntityID.ToLower().Equals("water") && Vector3.Distance(e.Position, Screen.Camera?.Position ?? Vector3.Zero) <= PROXIMITY_WATER)
            {
                return true;
            }
        }
        return false;
    }

    private static bool NPCAround()
    {
        if (Screen.Level == null) return false;
        foreach (Entity e in Screen.Level.Entities)
        {
            if (e.EntityID.ToLower().Equals("npc") && Vector3.Distance(e.Position, Screen.Camera?.Position ?? Vector3.Zero) <= PROXIMITY_NPC)
            {
                return true;
            }
        }
        return false;
    }

    private static bool TrainerAround()
    {
        if (Screen.Level == null) return false;
        foreach (Entity e in Screen.Level.Entities)
        {
            if (e.EntityID.ToLower().Equals("npc") && e is NPC npc && npc.IsTrainer == true)
            {
                if (Vector3.Distance(e.Position, Screen.Camera?.Position ?? Vector3.Zero) <= PROXIMITY_TRAINER)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private static bool GrassAround()
    {
        if (Screen.Level == null) return false;
        foreach (Entity e in Screen.Level.Entities)
        {
            if (e.EntityID.ToLower().Equals("grass") && Vector3.Distance(e.Position, Screen.Camera?.Position ?? Vector3.Zero) <= PROXIMITY_GRASS)
            {
                return true;
            }
        }
        return false;
    }

    private static bool ItemAround()
    {
        if (Screen.Level == null) return false;
        foreach (Entity e in Screen.Level.Entities)
        {
            if (e.EntityID.ToLower().Equals("itemobject") && Vector3.Distance(e.Position, Screen.Camera?.Position ?? Vector3.Zero) <= PROXIMITY_ITEM)
            {
                return true;
            }
        }
        return false;
    }

    private static bool IceAround()
    {
        if (Screen.Level == null) return false;
        foreach (Entity e in Screen.Level.Entities)
        {
            if (e.EntityID.ToLower().Equals("floor") && e is Floor floor && floor.IsIce == true)
            {
                if (Vector3.Distance(e.Position, Screen.Camera?.Position ?? Vector3.Zero) <= PROXIMITY_ICE)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private static bool LoamySoilAround()
    {
        if (Screen.Level == null) return false;
        foreach (Entity e in Screen.Level.Entities)
        {
            if (e.EntityID.ToLower().Equals("loamysoil") && Vector3.Distance(e.Position, Screen.Camera?.Position ?? Vector3.Zero) <= PROXIMITY_LOAMY_SOIL)
            {
                return true;
            }
        }
        return false;
    }

    // -------------------------------------------------------------------------
    // Private nested class — ReactionContainer
    // -------------------------------------------------------------------------

    private class ReactionContainer
    {
        public String message;
        public MessageBulb.NotificationTypes notification = MessageBulb.NotificationTypes.AFK;
        public String emojiString = String.Empty;
        public bool hasNotification = true;
        public List<String> mapFiles = [];
        public List<String> pokemonIds = [];
        public List<String> excludeIds = [];
        public int daytime = -1;
        public int weather = -1;
        public int season = -1;
        public List<int> elementTypes = [];
        public int probability = 100;

        public ReactionContainer(String dataLine)
        {
            dataLine = dataLine.Remove(dataLine.Length - 1, 1).Remove(0, 1);
            String[] dataParts = dataLine.Split('|');

            mapFiles = [.. dataParts[0].Split(',')];

            if (dataParts[1].Equals("-1") == false)
            {
                foreach (String pokePart in dataParts[1].Split(','))
                {
                    String part = pokePart;
                    List<String> lReference = pokemonIds;
                    if (part.StartsWith("!") == true)
                    {
                        part = part.Remove(0, 1);
                        lReference = excludeIds;
                    }
                    if (lReference.Contains(part) == false)
                    {
                        lReference.Add(part);
                    }
                }
            }

            if (dataParts[2].Equals("-1") == false)
            {
                daytime = int.Parse(dataParts[2]).Clamp(0, 3);
            }
            if (dataParts[3].Equals("-1") == false)
            {
                weather = int.Parse(dataParts[3]).Clamp(0, 9);
            }
            if (dataParts[4].Equals("-1") == false)
            {
                season = int.Parse(dataParts[4]).Clamp(0, 3);
            }

            if (dataParts[5].Equals("-1") == false)
            {
                foreach (String typePart in dataParts[5].Split(','))
                {
                    elementTypes.Add((int)BattleSystem.GameModeElementLoader.GetElementByName(typePart).Type);
                }
            }

            probability = int.Parse(dataParts[6]);

            if (dataParts[7].Equals("-1"))
            {
                hasNotification = false;
            }
            else
            {
                hasNotification = true;
                String emojiText = dataParts[7]
                    .Replace(">:(", "shouting")
                    .Replace("<3", "heart")
                    .Replace(":(", "unhappy")
                    .Replace(":)", "friendly")
                    .Replace(";)", "wink")
                    .Replace("/:(", "angry");
                emojiString = emojiText;
                if (emojiString.Contains("<") == false && emojiString.Contains(">") == false)
                {
                    notification = ConvertEmoji(ScriptVersion2.ScriptCommander.Parse(emojiText).ToString() ?? "");
                }
            }

            message = dataParts[8];
        }

        public ReactionContainer(String msg, MessageBulb.NotificationTypes notif)
        {
            message = msg;
            notification = notif;
        }

        private MessageBulb.NotificationTypes ConvertEmoji(String s)
        {
            switch (s.ToLower())
            {
                case "...":
                    return MessageBulb.NotificationTypes.Waiting;
                case "!":
                    return MessageBulb.NotificationTypes.Exclamation;
                case ">:(":
                case "shouting":
                    return MessageBulb.NotificationTypes.Shouting;
                case "?":
                    return MessageBulb.NotificationTypes.Question;
                case "note":
                    return MessageBulb.NotificationTypes.Note;
                case "<3":
                case "heart":
                    return MessageBulb.NotificationTypes.Heart;
                case ":(":
                case "unhappy":
                    return MessageBulb.NotificationTypes.Unhappy;
                case "ball":
                    return MessageBulb.NotificationTypes.Battle;
                case ":d":
                    return MessageBulb.NotificationTypes.Happy;
                case ":)":
                case "friendly":
                    return MessageBulb.NotificationTypes.Friendly;
                case "bad":
                    return MessageBulb.NotificationTypes.Poisoned;
                case ";)":
                case "wink":
                    return MessageBulb.NotificationTypes.Wink;
                case "afk":
                    return MessageBulb.NotificationTypes.AFK;
                case "/:(":
                case "angry":
                    return MessageBulb.NotificationTypes.Angry;
                case ":3":
                    return MessageBulb.NotificationTypes.CatFace;
                case ":/":
                    return MessageBulb.NotificationTypes.Unsure;
                default:
                    return MessageBulb.NotificationTypes.Waiting;
            }
        }

        public bool Match(Pokemon p)
        {
            if (mapFiles.Count > 0)
            {
                if (Screen.Level == null || mapFiles.Contains(Screen.Level.LevelFile.ToLowerInvariant()) == false)
                {
                    return false;
                }
            }

            if (pokemonIds.Count > 0)
            {
                String dexId = p.Number.ToString();
                if (String.IsNullOrEmpty(p.AdditionalData) == false)
                {
                    dexId = PokemonForms.GetPokemonDataFileName(p.Number, p.AdditionalData, true);
                }
                if (pokemonIds.Contains(dexId) == false)
                {
                    return false;
                }
            }

            if (excludeIds.Count > 0)
            {
                String dexId = p.Number.ToString();
                if (String.IsNullOrEmpty(p.AdditionalData) == false)
                {
                    dexId = PokemonForms.GetPokemonDataFileName(p.Number, p.AdditionalData, true);
                }
                if (excludeIds.Contains(dexId) == true)
                {
                    return false;
                }
            }

            if (daytime > -1)
            {
                if (daytime != (int)World.GetTime()) return false;
            }

            if (weather > -1)
            {
                if (weather != (int)World.GetCurrentRegionWeather()) return false;
            }

            if (season > -1)
            {
                if (season != (int)World.CurrentSeason) return false;
            }

            if (elementTypes.Count > 0)
            {
                foreach (int t in elementTypes)
                {
                    if (p.IsType(t) == false) return false;
                }
            }

            return true;
        }

        public String GetMessage(Pokemon p)
        {
            String textMessage = (ScriptVersion2.ScriptCommander.Parse(
                message.Replace("<name>", p.GetDisplayName())).ToString() ?? "")
                .Replace("[POKEMONNAME]", p.GetDisplayName());
            if (p.Item != null)
            {
                textMessage = textMessage
                    .Replace("<item>", p.Item.OneLineName())
                    .Replace("[ITEM]", p.Item.OneLineName());
            }
            return textMessage;
        }

        public MessageBulb.NotificationTypes GetNotification()
        {
            if (String.IsNullOrEmpty(emojiString) == false &&
                emojiString.Contains("<") == true &&
                emojiString.Contains(">") == true)
            {
                String resolved = (ScriptVersion2.ScriptCommander.Parse(emojiString).ToString() ?? "")
                    .Replace(">:(", "shouting")
                    .Replace("<3", "heart")
                    .Replace(":(", "unhappy")
                    .Replace(":)", "friendly")
                    .Replace(";)", "wink")
                    .Replace("/:(", "angry");
                return ConvertEmoji(resolved);
            }
            return notification;
        }
    }
}
