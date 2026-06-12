using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace P3D
{
    // TODO Phase 3: full Nature port
    public static class Nature
    {
        public static float GetMultiplier(Pokemon.Natures nature, String statName)
        {
            return 1.0f;
        }
    }

    // TODO Phase 3: full PokemonForms port
    public static class PokemonForms
    {
        public static String GetAnimationName(Pokemon p) => p.Number.ToString();
        public static String GetFormName(Pokemon p) => "";
        public static String GetInitialAdditionalData(Pokemon p) => "";
        public static String GetTypeAdditionFromItem(Pokemon p) => "";
        public static String GetFrontBackSpriteFileSuffix(Pokemon p) => "";
        public static String GetOverworldAddition(Pokemon p) => "";
        public static String GetMenuImagePosition(Pokemon p) => "";
        public static Vector2 GetMenuImagePositionVec(Pokemon p) => Vector2.Zero;
        public static Size GetMenuImageSize(Pokemon p) => new Size(32, 32);
        public static String GetSheetName(Pokemon p) => "Sheet";
        public static String GetCrySuffix(Pokemon p) => "";
        public static String GetPokemonDataFileName(int number, String additionalData) => number.ToString();
        public static String GetPokemonDataFileName(int number, String additionalData, bool fullPath) => number.ToString();
        public static String GetPokemonDataFile(int number, String additionalData) => "";
        public static String GetOverworldSpriteName(Pokemon p, bool shiny) => p.Number.ToString();
        public static String[]? GetAdditionalDataForms(int number) => null;
        public static String GetAdditionalValueFromDataFile(String dataFileName) => "";
        public static String GetFormDataInParty(Pokemon p) => String.Empty;
        public static void Initialize() { }
    }

    // TODO Phase 3: full PokedexEntry port
    public class PokedexEntry
    {
        public String text = String.Empty;
        public float height;
        public float weight;
        public String category = String.Empty;
        public String color = String.Empty;
        public String species = String.Empty;
        public String Text { get => text; set => text = value; }
        public float Height { get => height; set => height = value; }
        public float Weight { get => weight; set => weight = value; }
        public String Color { get => color; set => color = value; }
        public String Species { get => species; set => species = value; }
    }

    // Sound stub for Pokemon
    public static partial class SoundManager
    {
        public static void PlayPokemonCry(int number, String crySuffix = "") { }
        public static void PlayPokemonCry(int number, float pitch, float pan, String crySuffix = "") { }
        public static void PlayPokemonCry(int number, float pitch, float pan, float volume, String crySuffix = "") { }
    }

    // EggCreator stub
    public static class EggCreator
    {
        public static Texture2D? CreateEggSprite(Pokemon p, Texture2D baseSprite, Texture2D template) => baseSprite;
    }

    // GameModeManager extensions needed by Pokemon
    public static partial class GameModeManager
    {
        public static String GetGameRuleValue(String rule, String defaultValue) => defaultValue;
        public static void CreateGameModesFolder() { }
        public static void CreateKolbenMode() { }
        public static String GetPokemonDataFilePath(String filename) =>
            Path.Combine(GameController.GamePath, "Content", "Pokemon", filename);
        public static String GetContentFilePath(String contentFile) =>
            Path.Combine(GameController.GamePath, "Content", contentFile);
    }

    public static class GameModeItemLoader
    {
        public static void Load() { }
    }

    // BattleSystem stubs needed by Pokemon
    namespace BattleSystem
    {
        public static class GameModeElementLoader
        {
            public static Element GetElementByName(String name) =>
                new Element(Element.Types.Normal);

            public static Element GetElementByID(int id) =>
                new Element(Element.Types.Normal);

            // Overload for item-type enums passed as argument
            public static Element GetElementByID(Object id) =>
                new Element(Element.Types.Normal);

            public static void Load() { }
        }

        public static partial class GameModeAttackLoader
        {
            public static void Load() { }
        }
    }

    // ScriptVersion2.ScriptCommander.Parse is now in
    // World/ActionScript/V2/ScriptStubs.cs (full partial class).

    // TODO Phase 5: remove once battle system is fully ported (these are
    // PascalCase aliases for the camelCase public fields on Pokemon, needed
    // because VB is case-insensitive and the move files access them via
    // PascalCase while our C# fields follow AGENTS.md camelCase convention).
    public partial class Pokemon
    {
        // Battle-stat modifier aliases
        public int StatAttack { get => statAttack; set => statAttack = value; }
        public int StatDefense { get => statDefense; set => statDefense = value; }
        public int StatSpAttack { get => statSpAttack; set => statSpAttack = value; }
        public int StatSpDefense { get => statSpDefense; set => statSpDefense = value; }
        public int StatSpeed { get => statSpeed; set => statSpeed = value; }
        public int Evasion { get => evasion; set => evasion = value; }
        // Battle accuracy stat on Pokemon (distinct from Attack.Accuracy property)
        public int Accuracy { get => accuracy; set => accuracy = value; }

        // Base-stat aliases
        public int BaseAttack => baseAttack;
        public int BaseDefense => baseDefense;
        public int BaseSpAttack => baseSpAttack;
        public int BaseSpDefense => baseSpDefense;
        public int BaseSpeed => baseSpeed;

        // Collection aliases
        public List<BattleSystem.Attack> Attacks => attacks;

        // Other field aliases
        public String? AbilitySlot { get => abilitySlot; set => abilitySlot = value; }

    }
}
