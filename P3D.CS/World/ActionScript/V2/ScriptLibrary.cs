using System.Diagnostics;
using System.Linq;

namespace P3D.ScriptVersion2
{
    public static class ScriptLibrary
    {
        private static List<ScriptCommand> _scripts = [];

        // ---- Inner classes ----

        private sealed class ScriptArgument
        {
            public enum ArgumentTypes
            {
                Str,
                Int,
                Sng,
                ItemCollection,
                Bool,
                Rec,
                IntArr,
                StrArr,
                SngArr,
                BoolArr,
                PokemonData,
                Arr
            }

            public ArgumentTypes ArgumentType { get; }
            public String Name { get; } = String.Empty;
            public bool IsOptional { get; }
            public String DefaultValue { get; } = String.Empty;
            public String[] ValidArguments { get; } = [];

            public ScriptArgument(String name, ArgumentTypes argumentType)
            {
                Name = name;
                ArgumentType = argumentType;
            }

            public ScriptArgument(String name, ArgumentTypes argumentType, bool isOptional, String defaultValue = "")
            {
                Name = name;
                ArgumentType = argumentType;
                IsOptional = isOptional;
                DefaultValue = defaultValue;
            }

            public ScriptArgument(String name, ArgumentTypes argumentType, String[] validArguments, bool isOptional = false, String defaultValue = "")
            {
                Name = name;
                ArgumentType = argumentType;
                ValidArguments = validArguments;
                IsOptional = isOptional;
                DefaultValue = defaultValue;
            }

            public override String ToString()
            {
                if (IsOptional == true)
                {
                    String s = $"{ArgumentType.ToString().ToLower()} {Name}";
                    if (DefaultValue.Equals("") == false)
                    {
                        s += $"=\"{DefaultValue}\"";
                    }
                    return $"[{s}]";
                }
                else
                {
                    return $"{ArgumentType.ToString().ToLower()} {Name}";
                }
            }
        }

        private sealed class ScriptCommand
        {
            private String _description = String.Empty;
            private String _returnType = String.Empty;
            private List<ScriptArgument> _arguments = [];

            public String MainClass { get; }
            public String SubClass { get; }
            public String ArgumentSeparator { get; }
            public bool IsConstruct { get; }

            // Simple command: no args, no return type
            public ScriptCommand(String mainClass, String subClass, String description)
                : this(mainClass, subClass, "", [], description, ",", false)
            {
            }

            // Has argument list, no return type
            public ScriptCommand(String mainClass, String subClass, List<ScriptArgument> arguments, String description, String argumentSeparator = ",", bool isConstruct = false)
                : this(mainClass, subClass, "", arguments, description, argumentSeparator, isConstruct)
            {
            }

            // Has return type, no argument list
            public ScriptCommand(String mainClass, String subClass, String returnType, String description, String argumentSeparator = ",", bool isConstruct = false)
                : this(mainClass, subClass, returnType, [], description, argumentSeparator, isConstruct)
            {
            }

            // Canonical constructor
            public ScriptCommand(String mainClass, String subClass, String returnType, List<ScriptArgument> arguments, String description, String argumentSeparator = ",", bool isConstruct = false)
            {
                MainClass = mainClass;
                SubClass = subClass;
                ArgumentSeparator = argumentSeparator;
                IsConstruct = isConstruct;
                _arguments = arguments;
                _description = description;
                _returnType = returnType;
            }

            public bool MatchesClass(String mainClass, String subClass)
            {
                if (MainClass.Equals(mainClass, StringComparison.OrdinalIgnoreCase) == true &&
                    SubClass.Equals(subClass, StringComparison.OrdinalIgnoreCase) == true)
                {
                    return true;
                }
                return false;
            }

            public override String ToString()
            {
                String args = String.Empty;
                foreach (ScriptArgument arg in _arguments)
                {
                    if (args.Equals("") == false)
                    {
                        args += ArgumentSeparator;
                    }
                    args += arg.ToString();
                }

                String des = String.Empty;
                if (_description.Equals("") == false)
                {
                    des = " " + _description;
                }

                if (IsConstruct == true)
                {
                    String c = $"<{MainClass}.{SubClass}({args})>{des}";
                    if (_returnType.Equals("") == false)
                    {
                        c = $"({_returnType.ToLower()}) {c}";
                    }
                    return c;
                }
                else
                {
                    return $"@{MainClass}.{SubClass}({args}){des}";
                }
            }
        }

        private static void r(ScriptCommand s)
        {
            _scripts.Add(s);
        }

        // ---- Public API ----

        /// <summary>
        /// Call this at the initialize phase of the game. Fills the library with the script content.
        /// </summary>
        public static void InitializeLibrary()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            _scripts.Clear();

            DoFileSystem();
            DoRadio();
            DoPokedex();
            DoMath();
            DoRival();
            DoDaycare();
            DoPokemon();
            DoOverworldPokemon();
            DoNPC();
            DoPlayer();
            DoEnvironment();
            DoSystem();
            DoStorage();
            DoRegister();
            DoScript();
            DoScreen();
            DoChat();
            DoInventory();
            DoItem();
            DoPhone();
            DoEntity();
            DoLevel();
            DoBattle();
            DoMusic();
            DoSound();
            DoText();
            DoOptions();
            DoCamera();
            DoTitle();

            _scripts = _scripts.OrderBy(s => s.MainClass + "." + (s.IsConstruct == true ? "0" : "1") + s.SubClass).ToList();
            sw.Stop();
            Logger.Log(Logger.LogTypes.Debug, $"Initialized script library in {sw.ElapsedMilliseconds} milliseconds with {_scripts.Count} entries.");
        }

        // ---- DoXxx populate methods ----

        private static void DoFileSystem()
        {
            // Constructs:
            r(new ScriptCommand("FileSystem", "PathSplit", "str",
                [
                    new ScriptArgument("Index", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("Path", ScriptArgument.ArgumentTypes.Str)
                ],
                "Returns the part of the path that is at the position of Index.", ",", true));
            r(new ScriptCommand("FileSystem", "PathSplitCount", "int",
                [new ScriptArgument("Path", ScriptArgument.ArgumentTypes.Str)],
                "Returns the amount of parts in the given path.", ",", true));
            r(new ScriptCommand("FileSystem", "PathUp", "str",
                [new ScriptArgument("Path", ScriptArgument.ArgumentTypes.Str)],
                "Returns the parent path to the given path if possible.", ",", true));
        }

        private static void DoTitle()
        {
            // Commands:
            r(new ScriptCommand("Title", "Add",
                [
                    new ScriptArgument("Text", ScriptArgument.ArgumentTypes.Str, true, "Sample Text"),
                    new ScriptArgument("Delay", ScriptArgument.ArgumentTypes.Sng, true, "20.0"),
                    new ScriptArgument("ColorR", ScriptArgument.ArgumentTypes.Int, true, "255"),
                    new ScriptArgument("ColorG", ScriptArgument.ArgumentTypes.Int, true, "255"),
                    new ScriptArgument("ColorB", ScriptArgument.ArgumentTypes.Int, true, "255"),
                    new ScriptArgument("Scale", ScriptArgument.ArgumentTypes.Sng, true, "10.0"),
                    new ScriptArgument("IsCentered", ScriptArgument.ArgumentTypes.Bool, true, "true"),
                    new ScriptArgument("X", ScriptArgument.ArgumentTypes.Sng, true, "0.0"),
                    new ScriptArgument("Y", ScriptArgument.ArgumentTypes.Sng, true, "0.0")
                ],
                "Adds a new title for the game to display during gameplay."));
            r(new ScriptCommand("Title", "Clear", "Clears all titles that are currently being displayed."));
        }

        private static void DoCamera()
        {
            // Commands:
            r(new ScriptCommand("Camera", "ActivateThirdPerson",
                [new ScriptArgument("UpdateCamera", ScriptArgument.ArgumentTypes.Bool, true, "True")],
                "Activates the third person camera."));
            r(new ScriptCommand("Camera", "DeactivateThirdPerson",
                [new ScriptArgument("UpdateCamera", ScriptArgument.ArgumentTypes.Bool, true, "True")],
                "Deactivates the third person camera."));
            r(new ScriptCommand("Camera", "ToggleThirdPerson",
                [new ScriptArgument("UpdateCamera", ScriptArgument.ArgumentTypes.Bool, true, "True")],
                "Sets the camera to the opposite of the current perspective mode (first person or third person)."));
            r(new ScriptCommand("Camera", "SetThirdPerson",
                [
                    new ScriptArgument("PerspectiveMode", ScriptArgument.ArgumentTypes.Bool),
                    new ScriptArgument("UpdateCamera", ScriptArgument.ArgumentTypes.Bool, true, "True")
                ],
                "Sets the camera to the desired perspective mode."));
            r(new ScriptCommand("Camera", "Fix", "Fixes the camera to the current position."));
            r(new ScriptCommand("Camera", "Defix", "Defixes the camera so that it clips behind the player again."));
            r(new ScriptCommand("Camera", "ToggleFix", "Sets the fix state of the camera to the opposite of the current state."));
            r(new ScriptCommand("Camera", "Set",
                [
                    new ScriptArgument("X", ScriptArgument.ArgumentTypes.Sng),
                    new ScriptArgument("Y", ScriptArgument.ArgumentTypes.Sng),
                    new ScriptArgument("Z", ScriptArgument.ArgumentTypes.Sng),
                    new ScriptArgument("Yaw", ScriptArgument.ArgumentTypes.Sng),
                    new ScriptArgument("Pitch", ScriptArgument.ArgumentTypes.Sng)
                ],
                "Changes the position and rotation of the camera."));
            r(new ScriptCommand("Camera", "SetPitch",
                [new ScriptArgument("Pitch", ScriptArgument.ArgumentTypes.Sng)],
                "Changes the Pitch (vertical) rotation of the camera."));
            r(new ScriptCommand("Camera", "SetYaw",
                [new ScriptArgument("Yaw", ScriptArgument.ArgumentTypes.Sng)],
                "Changes the Yaw (horizontal) rotation of the camera."));
            r(new ScriptCommand("Camera", "SetPosition",
                [
                    new ScriptArgument("X", ScriptArgument.ArgumentTypes.Sng),
                    new ScriptArgument("Y", ScriptArgument.ArgumentTypes.Sng),
                    new ScriptArgument("Z", ScriptArgument.ArgumentTypes.Sng)
                ],
                "Changes the position of the camera."));
            r(new ScriptCommand("Camera", "SetX",
                [new ScriptArgument("X", ScriptArgument.ArgumentTypes.Sng)],
                "Changes the X-coordinate of the camera (left/right)."));
            r(new ScriptCommand("Camera", "SetY",
                [new ScriptArgument("Y", ScriptArgument.ArgumentTypes.Sng)],
                "Changes the Y-coordinate of the camera (up/down)."));
            r(new ScriptCommand("Camera", "SetZ",
                [new ScriptArgument("Z", ScriptArgument.ArgumentTypes.Sng)],
                "Changes the Z-coordinate of the camera (forward/backward)."));
            r(new ScriptCommand("Camera", "SetFocus",
                [
                    new ScriptArgument("FocusType", ScriptArgument.ArgumentTypes.Str, ["Player", "NPC", "Entity"]),
                    new ScriptArgument("FocusID", ScriptArgument.ArgumentTypes.Int)
                ],
                "Focuses the camera on an object (with the given ID for NPCs & Entities)."));
            r(new ScriptCommand("Camera", "SetFocusType",
                [new ScriptArgument("FocusType", ScriptArgument.ArgumentTypes.Str, ["Player", "NPC", "Entity"])],
                "Sets the focus type for the camera."));
            r(new ScriptCommand("Camera", "SetFocusID",
                [new ScriptArgument("FocusID", ScriptArgument.ArgumentTypes.Int)],
                "Sets the ID of the focus target for the camera."));
            r(new ScriptCommand("Camera", "SetToPlayerFacing", "Sets the Yaw rotation of the camera to the direction in which the player is facing."));
            r(new ScriptCommand("Camera", "Reset", "Resets the camera to its default location and rotation."));
            r(new ScriptCommand("Camera", "Update", "Updates the camera. Used for things like fading the screen during a script or when a property of the camera is changed."));

            // Constructs:
            r(new ScriptCommand("Camera", "IsFixed", "bool", "Returns if the camera is fixed to a specific position.", ",", true));
            r(new ScriptCommand("Camera", "X", "sng", "Returns the current X position of the camera.", ",", true));
            r(new ScriptCommand("Camera", "Y", "sng", "Returns the current Y position of the camera.", ",", true));
            r(new ScriptCommand("Camera", "Z", "sng", "Returns the current Z position of the camera.", ",", true));
            r(new ScriptCommand("Camera", "Pitch", "sng", "Returns the current Pitch (vertical) rotation of the camera.", ",", true));
            r(new ScriptCommand("Camera", "Yaw", "sng", "Returns the current Yaw (horizontal) rotation of the camera.", ",", true));
            r(new ScriptCommand("Camera", "ThirdPerson", "bool", "Returns if the camera is in third person mode.", ",", true));
        }

        private static void DoOptions()
        {
            // Commands:
            r(new ScriptCommand("Options", "Show",
                [
                    new ScriptArgument("Options", ScriptArgument.ArgumentTypes.StrArr),
                    new ScriptArgument("Flag", ScriptArgument.ArgumentTypes.Str, ["[TEXT=FALSE]"], true, "")
                ],
                "Displays a choose box with the given options."));
            r(new ScriptCommand("Options", "SetCancelIndex",
                [new ScriptArgument("Index", ScriptArgument.ArgumentTypes.Int)],
                "Sets the cancel index of the next choose box. This index gets chosen when the player presses a back key."));
        }

        private static void DoText()
        {
            // Commands:
            r(new ScriptCommand("text", "show",
                [new ScriptArgument("text", ScriptArgument.ArgumentTypes.Str)],
                "Displays a textbox with the given text."));
            r(new ScriptCommand("text", "setfont",
                [new ScriptArgument("font", ScriptArgument.ArgumentTypes.Str)],
                "Changes the font of the textbox. All fonts from loaded ContentPacks, GameModes and the standard game can be loaded."));
            r(new ScriptCommand("text", "notification",
                [
                    new ScriptArgument("message", ScriptArgument.ArgumentTypes.Str),
                    new ScriptArgument("delay", ScriptArgument.ArgumentTypes.Int, true, "500"),
                    new ScriptArgument("backgroundindex", ScriptArgument.ArgumentTypes.Int, true, "0"),
                    new ScriptArgument("iconindex", ScriptArgument.ArgumentTypes.Int, true, "0"),
                    new ScriptArgument("soundeffect", ScriptArgument.ArgumentTypes.Str, true),
                    new ScriptArgument("scriptfile", ScriptArgument.ArgumentTypes.Str, true)
                ],
                "Displays a textbox with the given text."));
            r(new ScriptCommand("text", "debug",
                [new ScriptArgument("text", ScriptArgument.ArgumentTypes.Str)],
                "Prints the \"text\" argument to the immediate window console."));
            r(new ScriptCommand("text", "log",
                [new ScriptArgument("text", ScriptArgument.ArgumentTypes.Str)],
                "Logs the \"text\" argument into the log.dat file."));
            r(new ScriptCommand("text", "color",
                [new ScriptArgument("colorName", ScriptArgument.ArgumentTypes.Str, ["playercolor", "defaultcolor"])],
                "Changes the font color to a preset. You can also use a VB.NET compatible color in the \"KnownColor\" enum instead."));
            r(new ScriptCommand("text", "color",
                [
                    new ScriptArgument("Red", ScriptArgument.ArgumentTypes.Str),
                    new ScriptArgument("Green", ScriptArgument.ArgumentTypes.Str),
                    new ScriptArgument("Blue", ScriptArgument.ArgumentTypes.Str)
                ],
                "Changes the font color to the specified RGB values."));
        }

        private static void DoSound()
        {
            // Commands:
            r(new ScriptCommand("sound", "play",
                [
                    new ScriptArgument("soundFile", ScriptArgument.ArgumentTypes.Str),
                    new ScriptArgument("stopBackgroundMusic", ScriptArgument.ArgumentTypes.Bool, true, "false")
                ],
                "Plays a sound."));
            r(new ScriptCommand("sound", "playadvanced",
                [
                    new ScriptArgument("soundFile", ScriptArgument.ArgumentTypes.Str),
                    new ScriptArgument("stopBackgroundMusic", ScriptArgument.ArgumentTypes.Bool),
                    new ScriptArgument("pitch", ScriptArgument.ArgumentTypes.Sng),
                    new ScriptArgument("pan", ScriptArgument.ArgumentTypes.Sng),
                    new ScriptArgument("volume", ScriptArgument.ArgumentTypes.Sng)
                ],
                "Plays a sound with advanced parameters."));
        }

        private static void DoMusic()
        {
            // Commands:
            r(new ScriptCommand("Music", "Play",
                [
                    new ScriptArgument("MusicFile", ScriptArgument.ArgumentTypes.Str),
                    new ScriptArgument("LoopSong", ScriptArgument.ArgumentTypes.Bool, true, "true"),
                    new ScriptArgument("FadeIntoSong", ScriptArgument.ArgumentTypes.Bool, true, "false")
                ],
                "Stops the currently playing music and plays a new one until the song is changed by another command or a new map."));
            r(new ScriptCommand("Music", "ForcePlay",
                [
                    new ScriptArgument("MusicFile", ScriptArgument.ArgumentTypes.Str),
                    new ScriptArgument("LoopSong", ScriptArgument.ArgumentTypes.Bool, true, "true"),
                    new ScriptArgument("FadeIntoSong", ScriptArgument.ArgumentTypes.Bool, true, "false")
                ],
                "Stops the currently playing music and plays a new one, which can't be changed until the music is unforced again with @Music.Unforce."));
            r(new ScriptCommand("Music", "Unforce", "Allows warps and surfing/riding etc. to change the music again."));
            r(new ScriptCommand("Music", "SetMusicLoop",
                [new ScriptArgument("MusicFile", ScriptArgument.ArgumentTypes.Str)],
                "Sets the \"MusicLoop\" Tag of the current map to a new one until the song is changed by another command or a new map. This doesn't reset the playback position to the start like @Music.Play does."));
            r(new ScriptCommand("Music", "Stop", "Stops the currently playing song (and plays silence)."));
            r(new ScriptCommand("Music", "Mute", "Silences the music playback (but keeps it playing while silent)."));
            r(new ScriptCommand("Music", "Unmute", "Reverts the volume of the music playback back to what it was before."));
            r(new ScriptCommand("Music", "Pause", "Pauses the music playback (and keeps track of when it was paused, so it can be resumed later)."));
            r(new ScriptCommand("Music", "Resume", "Resumes the music playback starting from when it was paused."));
        }

        private static void DoBattle()
        {
            // Commands:
            r(new ScriptCommand("Battle", "StartTrainer",
                [new ScriptArgument("TrainerFilePath", ScriptArgument.ArgumentTypes.Str)],
                "Initializes a trainer interaction and checks the register if the player has already beaten that trainer."));
            r(new ScriptCommand("Battle", "Trainer",
                [new ScriptArgument("TrainerFilePath", ScriptArgument.ArgumentTypes.Str)],
                "Initializes a trainer battle without displaying an intro message or checking the register."));
            r(new ScriptCommand("Battle", "Wild",
                [
                    new ScriptArgument("PokemonData", ScriptArgument.ArgumentTypes.PokemonData),
                    new ScriptArgument("IntroMusic", ScriptArgument.ArgumentTypes.Str, true, "")
                ],
                "Initializes a wild battle against the given Pokemon."));
            r(new ScriptCommand("Battle", "Wild",
                [
                    new ScriptArgument("PokemonID", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("Level", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("Shiny", ScriptArgument.ArgumentTypes.Int, true, "-1"),
                    new ScriptArgument("IntroMusic", ScriptArgument.ArgumentTypes.Str, true, ""),
                    new ScriptArgument("IntroType", ScriptArgument.ArgumentTypes.Int, true, "0-10"),
                    new ScriptArgument("Gender", ScriptArgument.ArgumentTypes.Int, true, "")
                ],
                "Initializes a wild battle against the given Pokemon."));
            r(new ScriptCommand("Battle", "SetVar",
                [
                    new ScriptArgument("VarName", ScriptArgument.ArgumentTypes.Str, ["CanRun", "CanAlwaysRun", "CanCatch", "CanBlackout", "CanReceiveExp", "CanUseItems", "CanGainLoseMoney", "FrontierTrainer", "DiveBattle", "InverseBattle, CustomBattleMusic, HiddenAbilityChance"]),
                    new ScriptArgument("VarValue", ScriptArgument.ArgumentTypes.Str)
                ],
                "Changes the given battle variable to the given value."));
            r(new ScriptCommand("Battle", "ResetVars", "Resets the battle variables to their default value."));

            // Constructs:
            r(new ScriptCommand("Battle", "DefeatMessage", "str",
                [new ScriptArgument("TrainerFilePath", ScriptArgument.ArgumentTypes.Str)],
                "Returns the defeat message of the trainer loaded from the given \"TrainerFilePath\".", ",", true));
            r(new ScriptCommand("Battle", "IntroMessage", "str",
                [new ScriptArgument("TrainerFilePath", ScriptArgument.ArgumentTypes.Str)],
                "Returns the intro message of the trainer loaded from the given \"TrainerFilePath\".", ",", true));
            r(new ScriptCommand("Battle", "OutroMessage", "str",
                [new ScriptArgument("TrainerFilePath", ScriptArgument.ArgumentTypes.Str)],
                "Returns the outro message of the trainer loaded from the given \"TrainerFilePath\".", ",", true));
            r(new ScriptCommand("Battle", "Won", "bool", "Returns \"true\" if the player won the last battle. Returns \"false\" otherwise.", ",", true));
            r(new ScriptCommand("Battle", "Caught", "bool", "Returns \"true\" if the player caught the Pokemon in the last battle. Returns \"false\" otherwise.", ",", true));
            r(new ScriptCommand("Battle", "TrainerName", "str",
                [new ScriptArgument("TrainerIndex", ScriptArgument.ArgumentTypes.Int, true, "0")],
                "Returns the name of one trainer that you're currently battling. Can be 0 or 1 for double battles.", ",", true));
            r(new ScriptCommand("Battle", "PokemonName", "str",
                [new ScriptArgument("OwnOrOppPokemon", ScriptArgument.ArgumentTypes.Int, true, "true")],
                "Returns the name of a Pokemon currently in battle. True = Your Pokemon, False = Opponent's Pokemon.", ",", true));
            r(new ScriptCommand("Battle", "PokemonID", "str",
                [new ScriptArgument("OwnOrOppPokemon", ScriptArgument.ArgumentTypes.Int, true, "true")],
                "Returns the Pokemon ID (including form suffix) of a Pokemon currently in battle. True = Your Pokemon, False = Opponent's Pokemon.", ",", true));
            r(new ScriptCommand("Battle", "PokemonItem", "str",
                [new ScriptArgument("OwnOrOppPokemon", ScriptArgument.ArgumentTypes.Int, true, "true")],
                "Returns the Item ID of a Pokemon currently in battle. True = Your Pokemon, False = Opponent's Pokemon.", ",", true));
        }

        private static void DoLevel()
        {
            // Commands:
            r(new ScriptCommand("level", "wait",
                [new ScriptArgument("ticks", ScriptArgument.ArgumentTypes.Int)],
                "Makes the level idle for the duration of the given ticks."));
            r(new ScriptCommand("level", "update", "Updates the level and all entities once."));
            r(new ScriptCommand("level", "waitforevents", "Makes the level idle until every NPC movement is done."));
            r(new ScriptCommand("level", "waitforsave", "Makes the level idle until the current saving of a GameJolt save is done."));
            r(new ScriptCommand("level", "reload", "Reloads the current map."));
            r(new ScriptCommand("level", "setsafari",
                [new ScriptArgument("safari", ScriptArgument.ArgumentTypes.Bool)],
                "Sets if the current map is a Safari Zone (influences battle style)."));
            r(new ScriptCommand("level", "setridetype",
                [new ScriptArgument("rideType", ScriptArgument.ArgumentTypes.Int, ["0-3"])],
                "Sets the Ride Type of the current map. (0 = Depends on CanDig and CanFly tags, 1 = Can ride, 2 = Can not ride, 3 = Can't stop riding once started)"));

            // Constructs:
            r(new ScriptCommand("level", "mapfile", "str", "Returns the mapfile of the currently loaded map.", ",", true));
            r(new ScriptCommand("level", "levelfile", "str", "Returns the mapfile of the currently loaded map.", ",", true));
            r(new ScriptCommand("level", "filename", "str", "Returns only the name of the current map file, without path and extension.", ",", true));
            r(new ScriptCommand("level", "riding", "bool", "Returns if the player is Riding a Pokemon right now.", ",", true));
            r(new ScriptCommand("level", "surfing", "bool", "Returns if the player is Surfing on a Pokemon right now.", ",", true));
            r(new ScriptCommand("level", "musicloop", "str", "Returns only the name of the current played song, without path and extension.", ",", true));
            r(new ScriptCommand("level", "daytime", "int", "Returns the DayTime of the current map.", ",", true));
            r(new ScriptCommand("level", "environmenttype", "int", "Returns the EnvironmentType of the current map.", ",", true));
            r(new ScriptCommand("level", "loadoffsetmaps", "bool", "Returns if OffsetMaps are being loaded (based on the Offset Map Quality option in the Options Menu).", ",", true));
        }

        private static void DoEntity()
        {
            // Commands:
            r(new ScriptCommand("Entity", "ShowMessageBulb",
                [
                    new ScriptArgument("BulbID", ScriptArgument.ArgumentTypes.Int, ["0-15"]),
                    new ScriptArgument("X", ScriptArgument.ArgumentTypes.Sng),
                    new ScriptArgument("Y", ScriptArgument.ArgumentTypes.Sng),
                    new ScriptArgument("Z", ScriptArgument.ArgumentTypes.Sng)
                ],
                "Displays the given Message Bulb at the given position.", "|"));
            r(new ScriptCommand("Entity", "Warp",
                [
                    new ScriptArgument("EntityID", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("X", ScriptArgument.ArgumentTypes.Sng),
                    new ScriptArgument("Y", ScriptArgument.ArgumentTypes.Sng),
                    new ScriptArgument("Z", ScriptArgument.ArgumentTypes.Sng)
                ],
                "Warps the given entity to the given position on the map."));
            r(new ScriptCommand("Entity", "AddToPosition",
                [
                    new ScriptArgument("EntityID", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("X", ScriptArgument.ArgumentTypes.Sng),
                    new ScriptArgument("Y", ScriptArgument.ArgumentTypes.Sng),
                    new ScriptArgument("Z", ScriptArgument.ArgumentTypes.Sng)
                ],
                "Adds the given coordinates to the position of the given entity."));
            r(new ScriptCommand("Entity", "Remove",
                [new ScriptArgument("EntityID", ScriptArgument.ArgumentTypes.Int)],
                "Removes the given entity from the map (when it updates) until the map is loaded again."));
            r(new ScriptCommand("Entity", "SetID",
                [
                    new ScriptArgument("OldID", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("NewID", ScriptArgument.ArgumentTypes.Int)
                ],
                "Changes the ID of the given entity."));
            r(new ScriptCommand("Entity", "SetScale",
                [
                    new ScriptArgument("EntityID", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("xScale", ScriptArgument.ArgumentTypes.Sng),
                    new ScriptArgument("yScale", ScriptArgument.ArgumentTypes.Sng),
                    new ScriptArgument("zScale", ScriptArgument.ArgumentTypes.Sng)
                ],
                "Changes the Scale property of the given entity."));
            r(new ScriptCommand("Entity", "SetOpacity",
                [
                    new ScriptArgument("EntityID", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("Opacity", ScriptArgument.ArgumentTypes.Int)
                ],
                "Changes the Opacity (transparency) property of the given entity to the given value (in %)."));
            r(new ScriptCommand("Entity", "SetVisible",
                [
                    new ScriptArgument("EntityID", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("Visible", ScriptArgument.ArgumentTypes.Bool)
                ],
                "Changes whether the entity is visible or not."));
            r(new ScriptCommand("Entity", "SetAdditionalValue",
                [
                    new ScriptArgument("EntityID", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("AdditionalValue", ScriptArgument.ArgumentTypes.Str)
                ],
                "Sets the AdditionalValue property of the given entity."));
            r(new ScriptCommand("Entity", "SetAction",
                [
                    new ScriptArgument("EntityID", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("Action", ScriptArgument.ArgumentTypes.Str)
                ],
                "Sets the Action property of the given entity."));
            r(new ScriptCommand("Entity", "SetCollision",
                [
                    new ScriptArgument("EntityID", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("Collision", ScriptArgument.ArgumentTypes.Bool)
                ],
                "Sets the Collision property of the given entity."));
            r(new ScriptCommand("Entity", "SetTexture",
                [
                    new ScriptArgument("EntityID", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("TextureIndex", ScriptArgument.ArgumentTypes.Str),
                    new ScriptArgument("TextureName", ScriptArgument.ArgumentTypes.Str),
                    new ScriptArgument("RectangleX", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("RectangleY", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("RectangleWidth", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("RectangleHeight", ScriptArgument.ArgumentTypes.Int)
                ],
                "Sets the texture in the selected entity's texture array. Example: @Entity.SetTexture(0,0,Routes,112,64,16,32)"));
            r(new ScriptCommand("Entity", "SetModelPath",
                [
                    new ScriptArgument("EntityID", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("ModelPath", ScriptArgument.ArgumentTypes.Str)
                ],
                "Sets the ModelPath property of the given entity."));

            // Constructs:
            r(new ScriptCommand("Entity", "Visible", "bool",
                [new ScriptArgument("EntityID", ScriptArgument.ArgumentTypes.Int)],
                "Returns the Visible property of the given entity.", ",", true));
            r(new ScriptCommand("Entity", "Opacity", "int",
                [new ScriptArgument("EntityID", ScriptArgument.ArgumentTypes.Int)],
                "Returns the Opacity property of the given entity.", ",", true));
            r(new ScriptCommand("Entity", "Position", "sngArr",
                [new ScriptArgument("EntityID", ScriptArgument.ArgumentTypes.Int)],
                "Returns the Position of the given entity in the pattern \"x,y,z\".", ",", true));
            r(new ScriptCommand("Entity", "PositionX", "sng",
                [new ScriptArgument("EntityID", ScriptArgument.ArgumentTypes.Int)],
                "Returns the X Position of the given entity.", ",", true));
            r(new ScriptCommand("Entity", "PositionY", "sng",
                [new ScriptArgument("EntityID", ScriptArgument.ArgumentTypes.Int)],
                "Returns the Y Position of the given entity.", ",", true));
            r(new ScriptCommand("Entity", "PositionZ", "sng",
                [new ScriptArgument("EntityID", ScriptArgument.ArgumentTypes.Int)],
                "Returns the Z Position of the given entity.", ",", true));
            r(new ScriptCommand("Entity", "Scale", "sngArr",
                [new ScriptArgument("EntityID", ScriptArgument.ArgumentTypes.Int)],
                "Returns the Scale property of the given entity in the pattern \"x,y,z\".", ",", true));
            r(new ScriptCommand("Entity", "Rotation", "sngArr",
                [new ScriptArgument("EntityID", ScriptArgument.ArgumentTypes.Int)],
                "Returns the Rotation property of the given entity in the pattern \"x,y,z\".", ",", true));
            r(new ScriptCommand("Entity", "AdditionalValue", "str",
                [new ScriptArgument("EntityID", ScriptArgument.ArgumentTypes.Int)],
                "Returns the AdditionalValue property of the given entity.", ",", true));
            r(new ScriptCommand("Entity", "Collision", "bool",
                [new ScriptArgument("EntityID", ScriptArgument.ArgumentTypes.Int)],
                "Returns the Collision property of the given entity.", ",", true));
        }

        private static void DoPhone()
        {
            // Constructs:
            r(new ScriptCommand("phone", "callflag", "str", "Returns if the Pokegear is calling or is being called. Values: \"calling\", \"receiving\"", ",", true));
            r(new ScriptCommand("phone", "got", "bool", "Returns if the player got the Pokegear.", ",", true));
        }

        private static void DoItem()
        {
            // Commands:
            r(new ScriptCommand("Item", "Give",
                [
                    new ScriptArgument("ItemID", ScriptArgument.ArgumentTypes.Str),
                    new ScriptArgument("Amount", ScriptArgument.ArgumentTypes.Int, true, "1")
                ],
                "Adds the given amount of items to the player's inventory."));
            r(new ScriptCommand("Item", "Remove",
                [
                    new ScriptArgument("ItemID", ScriptArgument.ArgumentTypes.Str),
                    new ScriptArgument("Amount", ScriptArgument.ArgumentTypes.Int, true, "1"),
                    new ScriptArgument("ShowMessage", ScriptArgument.ArgumentTypes.Bool, true, "true")
                ],
                "Removes the given amount of items from the player's inventory. Displays a message afterwards, if \"showMessage\" is true."));
            r(new ScriptCommand("Item", "ClearItem",
                [new ScriptArgument("ItemID", ScriptArgument.ArgumentTypes.Str, true, "")],
                "Clears all items with the given ID from the player's inventory. Clears the whole inventory if ItemID is empty."));
            r(new ScriptCommand("Item", "MessageGive",
                [
                    new ScriptArgument("ItemID", ScriptArgument.ArgumentTypes.Str),
                    new ScriptArgument("Amount", ScriptArgument.ArgumentTypes.Int, true, "1")
                ],
                "Displays a message for getting the specified amount of items."));
            r(new ScriptCommand("Item", "Repel",
                [new ScriptArgument("RepelItemID", ScriptArgument.ArgumentTypes.Int, ["20", "42", "43"])],
                "Adds the steps of the Repel to the Repel steps of the player."));
            r(new ScriptCommand("Item", "Use",
                [new ScriptArgument("ItemID", ScriptArgument.ArgumentTypes.Str)],
                "Uses the specified item if the player has it."));
            r(new ScriptCommand("Item", "Select",
                [
                    new ScriptArgument("AllowedPages", ScriptArgument.ArgumentTypes.Str, true, "-1"),
                    new ScriptArgument("AllowedItems", ScriptArgument.ArgumentTypes.Str, true, "-1")
                ],
                "Opens an item select screen with only the specified item type pages (separated with \";\", e.g. \"0;1;2\" or \"standard;medicine;plants\") and possible item IDs (single items separated with \";\", or with a \"-\" if you want a range, e.g. \"2000-2066\")."));
        }

        private static void DoInventory()
        {
            // Constructs:
            r(new ScriptCommand("inventory", "countitem", "int",
                [new ScriptArgument("itemID", ScriptArgument.ArgumentTypes.Int)],
                "Returns the amount of the Item with the given ID in the player's inventory.", ",", true));
            r(new ScriptCommand("inventory", "countitems", "int", "Counts all items in the player's inventory.", ",", true));
            r(new ScriptCommand("inventory", "name", "str",
                [new ScriptArgument("itemID", ScriptArgument.ArgumentTypes.Int)],
                "Returns the name of an Item by its ItemID.", ",", true));
            r(new ScriptCommand("inventory", "id", "int",
                [new ScriptArgument("itemName", ScriptArgument.ArgumentTypes.Str)],
                "Returns the ID of an Item by its Name.", ",", true));
            r(new ScriptCommand("inventory", "selected", "str", "Returns the item ID of the item selected with @item.select.", ",", true));
        }

        private static void DoChat()
        {
            // Commands:
            r(new ScriptCommand("Chat", "Clear", "Clears the chat."));
        }

        private static void DoScreen()
        {
            // Commands:
            r(new ScriptCommand("screen", "storagesystem", "Opens the storage system."));
            r(new ScriptCommand("screen", "apricornkurt", "Opens the Apricorn Screen."));
            r(new ScriptCommand("screen", "trade",
                [
                    new ScriptArgument("tradeItems", ScriptArgument.ArgumentTypes.ItemCollection),
                    new ScriptArgument("canBuy", ScriptArgument.ArgumentTypes.Bool),
                    new ScriptArgument("canSell", ScriptArgument.ArgumentTypes.Bool),
                    new ScriptArgument("Currency", ScriptArgument.ArgumentTypes.Str, ["P", "BP", "C"], true, "P"),
                    new ScriptArgument("shopIdentifier", ScriptArgument.ArgumentTypes.Str, [], true, "")
                ],
                "Opens a new trade screen with the given items in stock. tradeItems: {itemID|amount|price}{...}..., amount and price are default for -1. Currency defaults to \"P\" and shopIdentifier is optional.", ","));
            r(new ScriptCommand("screen", "townmap",
                [new ScriptArgument("regionList", ScriptArgument.ArgumentTypes.StrArr)],
                "Opens the map screen with the given regions."));
            r(new ScriptCommand("screen", "donation", "Opens the donation screen."));
            r(new ScriptCommand("screen", "blackout", "Opens the blackout screen and warps the player back to the last rest place."));
            r(new ScriptCommand("screen", "fadein",
                [new ScriptArgument("fadeSpeed", ScriptArgument.ArgumentTypes.Int, true, "5")],
                "Fades the screen back in."));
            r(new ScriptCommand("screen", "fadeout",
                [new ScriptArgument("fadeSpeed", ScriptArgument.ArgumentTypes.Int, true, "5")],
                "Fades the screen to black."));
            r(new ScriptCommand("screen", "fadeoutcolor",
                [new ScriptArgument("color", ScriptArgument.ArgumentTypes.IntArr, ["0-255"], true, "0,0,0")],
                "Sets the color of the screen fade."));
            r(new ScriptCommand("screen", "setfade",
                [new ScriptArgument("alpha", ScriptArgument.ArgumentTypes.Int, ["0-255"])],
                "Sets the alpha value of the screen fade."));
            r(new ScriptCommand("screen", "Credits",
                [
                    new ScriptArgument("Ending", ScriptArgument.ArgumentTypes.Str, true, "Johto"),
                    new ScriptArgument("CanBeSkipped", ScriptArgument.ArgumentTypes.Bool, true, "false")
                ],
                "Displays the credits scene (optionally for a different set of maps/credits, and optionally skippable)."));
            r(new ScriptCommand("screen", "halloffame",
                [new ScriptArgument("displayEntryIndex", ScriptArgument.ArgumentTypes.Int, true, "")],
                "Displays the Hall of Fame. If the argument \"displayEntryIndex\" is not empty, it displays only that entry."));
            r(new ScriptCommand("screen", "teachmoves",
                [
                    new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("moveIDs", ScriptArgument.ArgumentTypes.IntArr, true, "")
                ],
                "Displays a move learn screen. If the argument \"moveIDs\" is left empty, it defaults to the Pokemon's tutor moves."));
            r(new ScriptCommand("screen", "mailsystem", "Opens the PC Inbox screen."));
            r(new ScriptCommand("screen", "pvp", "Opens the PvP lobby screen (not finished yet, don't use)."));
            r(new ScriptCommand("screen", "input",
                [
                    new ScriptArgument("defaultName", ScriptArgument.ArgumentTypes.Str, true, ""),
                    new ScriptArgument("inputMode", ScriptArgument.ArgumentTypes.Str, ["0-2", "name", "numbers", "text"], true, "0"),
                    new ScriptArgument("currentText", ScriptArgument.ArgumentTypes.Str, true, ""),
                    new ScriptArgument("maxChars", ScriptArgument.ArgumentTypes.Int, true, "14")
                ],
                "Displays the Input screen. The input can be retrieved with <system.lastinput>."));
            r(new ScriptCommand("screen", "mysteryevent", "Opens the Mystery Event screen."));
            r(new ScriptCommand("screen", "showPokemon",
                [
                    new ScriptArgument("PokemonID", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("Shiny", ScriptArgument.ArgumentTypes.Bool),
                    new ScriptArgument("Front", ScriptArgument.ArgumentTypes.Bool)
                ],
                "Displays a box and an image of the specified Pokemon."));
            r(new ScriptCommand("screen", "showimage",
                [
                    new ScriptArgument("Texture", ScriptArgument.ArgumentTypes.Str),
                    new ScriptArgument("SoundEffect", ScriptArgument.ArgumentTypes.Str, true, ""),
                    new ScriptArgument("X", ScriptArgument.ArgumentTypes.Int, true),
                    new ScriptArgument("Y", ScriptArgument.ArgumentTypes.Int, true),
                    new ScriptArgument("Width", ScriptArgument.ArgumentTypes.Int, true),
                    new ScriptArgument("Height", ScriptArgument.ArgumentTypes.Int, true)
                ],
                "Displays a box and (part of a) texture image."));
            r(new ScriptCommand("screen", "showmessagebox",
                [
                    new ScriptArgument("Message", ScriptArgument.ArgumentTypes.Str),
                    new ScriptArgument("BackgroundColor", ScriptArgument.ArgumentTypes.IntArr, ["0-255"], true),
                    new ScriptArgument("FontColor", ScriptArgument.ArgumentTypes.IntArr, ["0-255"], true),
                    new ScriptArgument("BorderColor", ScriptArgument.ArgumentTypes.IntArr, ["0-255"], true)
                ],
                "Displays a dynamically sized customizable message box in the center of the screen."));
            r(new ScriptCommand("screen", "secretbase", "Opens the Secret Base screen."));
            r(new ScriptCommand("screen", "skinselection", "Opens the Player Skin selection screen."));
            r(new ScriptCommand("screen", "voltorbflip", "Opens the Voltorb Flip minigame screen."));

            // Constructs:
            r(new ScriptCommand("screen", "selectedskin", "str", "Returns the texture name of the skin selected by using @Screen.SkinSelection", ",", true));
            r(new ScriptCommand("screen", "selectedname", "str", "Returns the default name assigned to the skin selected by using @Screen.SkinSelection", ",", true));
            r(new ScriptCommand("screen", "selectedgender", "str", "Returns the default gender assigned to the skin selected by using @Screen.SkinSelection", ",", true));
        }

        private static void DoScript()
        {
            // Commands:
            r(new ScriptCommand("script", "start",
                [new ScriptArgument("scriptFile", ScriptArgument.ArgumentTypes.Str)],
                "Starts a script with the given filename (without file extension)."));
            r(new ScriptCommand("script", "text",
                [new ScriptArgument("text", ScriptArgument.ArgumentTypes.Str)],
                "Starts a script with a simple text to display."));
            r(new ScriptCommand("script", "run",
                [new ScriptArgument("scriptContent", ScriptArgument.ArgumentTypes.Str)],
                "Runs script content. New lines are represented with \"^\"."));
            r(new ScriptCommand("script", "delay",
                [
                    new ScriptArgument("delayID", ScriptArgument.ArgumentTypes.Str),
                    new ScriptArgument("scriptPath", ScriptArgument.ArgumentTypes.Str),
                    new ScriptArgument("delayType", ScriptArgument.ArgumentTypes.Str, ["steps", "itemcount"]),
                    new ScriptArgument("valueArguments", ScriptArgument.ArgumentTypes.StrArr)
                ],
                "Executes a script file after something happened (like having moved a certain amount of steps)."));
            r(new ScriptCommand("script", "cleardelay",
                [new ScriptArgument("delayID", ScriptArgument.ArgumentTypes.Str)],
                "Removes the register with the specified identifier (delayID) created with @script.delay, preventing the script from being executed."));

            // Constructs:
            r(new ScriptCommand("script", "delay", "str,int",
                [
                    new ScriptArgument("delayID", ScriptArgument.ArgumentTypes.Str),
                    new ScriptArgument("returnType", ScriptArgument.ArgumentTypes.Str, ["type", "script", "value"])
                ],
                "Returns the \"type\", \"scriptpath\" or \"value\" of what will trigger the script, like the number of steps. Returns \"false\" if no delay is registered.", ",", true));
        }

        private static void DoRegister()
        {
            // Commands:
            r(new ScriptCommand("register", "register",
                [new ScriptArgument("name", ScriptArgument.ArgumentTypes.Str)],
                "Registers a new register with the given name."));
            r(new ScriptCommand("register", "register",
                [
                    new ScriptArgument("name", ScriptArgument.ArgumentTypes.Str),
                    new ScriptArgument("type", ScriptArgument.ArgumentTypes.Str, ["str", "int", "sng", "bool"]),
                    new ScriptArgument("value", ScriptArgument.ArgumentTypes.Str)
                ],
                "Registers a new register with the given name, type and value."));
            r(new ScriptCommand("register", "unregister",
                [new ScriptArgument("name", ScriptArgument.ArgumentTypes.Str)],
                "Unregisters a register with the given name."));
            r(new ScriptCommand("register", "unregister",
                [
                    new ScriptArgument("name", ScriptArgument.ArgumentTypes.Str),
                    new ScriptArgument("type", ScriptArgument.ArgumentTypes.Str, ["str", "int", "sng", "bool"])
                ],
                "Unregisters a register with the given name and type that has a value."));
            r(new ScriptCommand("register", "registertime",
                [
                    new ScriptArgument("name", ScriptArgument.ArgumentTypes.Str),
                    new ScriptArgument("time", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("timeFormat", ScriptArgument.ArgumentTypes.Str, ["days", "hours", "minutes", "seconds", "years", "weeks", "months", "dayofweek"])
                ],
                "Registers a time based register."));
            r(new ScriptCommand("register", "change",
                [
                    new ScriptArgument("name", ScriptArgument.ArgumentTypes.Str),
                    new ScriptArgument("value", ScriptArgument.ArgumentTypes.Str)
                ],
                "Changes the specified register's stored value to a new value."));

            // Constructs:
            r(new ScriptCommand("register", "registered", "bool",
                [new ScriptArgument("name", ScriptArgument.ArgumentTypes.Str)],
                "Checks if a register with the given name is registered.", ",", true));
            r(new ScriptCommand("register", "count", "int", "Counts all registers.", ",", true));
            r(new ScriptCommand("register", "type", "str",
                [new ScriptArgument("name", ScriptArgument.ArgumentTypes.Str)],
                "Returns the type of a register with the given name.", ",", true));
            r(new ScriptCommand("register", "value", "str,int,bool,sng",
                [new ScriptArgument("name", ScriptArgument.ArgumentTypes.Str)],
                "Returns the value of a register with the given name as its type.", ",", true));
        }

        private static void DoStorage()
        {
            // Commands:
            r(new ScriptCommand("storage", "set",
                [
                    new ScriptArgument("type", ScriptArgument.ArgumentTypes.Str, ["Pokemon", "item", "string", "integer", "boolean", "single", "str", "int", "bool", "sng"]),
                    new ScriptArgument("name", ScriptArgument.ArgumentTypes.Str),
                    new ScriptArgument("value", ScriptArgument.ArgumentTypes.Str)
                ],
                "Creates or overwrites a storage with the given name and type."));
            r(new ScriptCommand("storage", "clear", "Clears all storage items."));
            r(new ScriptCommand("storage", "update",
                [
                    new ScriptArgument("type", ScriptArgument.ArgumentTypes.Str, ["Pokemon", "item", "string", "integer", "boolean", "single", "str", "int", "bool", "sng"]),
                    new ScriptArgument("name", ScriptArgument.ArgumentTypes.Str),
                    new ScriptArgument("operation", ScriptArgument.ArgumentTypes.Str, ["add", "substract", "multiply", "divide"]),
                    new ScriptArgument("value", ScriptArgument.ArgumentTypes.Str)
                ],
                "Updates the value in a storage with the given name and type."));

            // Constructs:
            r(new ScriptCommand("storage", "get", "str",
                [
                    new ScriptArgument("type", ScriptArgument.ArgumentTypes.Str, ["Pokemon", "item", "string", "integer", "boolean", "single", "str", "int", "bool", "sng"]),
                    new ScriptArgument("name", ScriptArgument.ArgumentTypes.Str)
                ],
                "Returns the value for the storage with the type \"type\" and name \"name\".", ",", true));
            r(new ScriptCommand("storage", "count", "int",
                [new ScriptArgument("type", ScriptArgument.ArgumentTypes.Str, ["Pokemon", "item", "string", "integer", "boolean", "single", "str", "int", "bool", "sng"])],
                "Returns the amount of items in the storage for a specific type.", ",", true));
        }

        private static void DoSystem()
        {
            // Commands:
            r(new ScriptCommand("system", "endnewgame",
                [
                    new ScriptArgument("Map", ScriptArgument.ArgumentTypes.Str),
                    new ScriptArgument("X", ScriptArgument.ArgumentTypes.Sng, true, "~"),
                    new ScriptArgument("Y", ScriptArgument.ArgumentTypes.Sng, true, "~"),
                    new ScriptArgument("Z", ScriptArgument.ArgumentTypes.Sng, true, "~"),
                    new ScriptArgument("Rotation", ScriptArgument.ArgumentTypes.Int, ["0-3"], true, "0")
                ],
                "Ends the 3D new game intro screen and creates a save file."));
            r(new ScriptCommand("system", "replacetextures",
                [new ScriptArgument("textureReplacementFile", ScriptArgument.ArgumentTypes.Str)],
                "Loads the given texture replacement file (without file extension)."));

            // Constructs:
            r(new ScriptCommand("system", "random", "int",
                [
                    new ScriptArgument("min", ScriptArgument.ArgumentTypes.Int, true, "1"),
                    new ScriptArgument("max", ScriptArgument.ArgumentTypes.Int, true, "2")
                ],
                "Generates a random number between min and max, inclusive.", ",", true));
            r(new ScriptCommand("system", "chooserandom", "str",
                [new ScriptArgument("StringsToChooseFrom", ScriptArgument.ArgumentTypes.Str)],
                "From the given arguments separated by commas, a random argument is chosen and returned. You can specify a range of numbers by separating two numbers with a dash (e.g. 1-151).", ",", true));
            r(new ScriptCommand("system", "unixtimestamp", "int", "Returns the UNIX timestamp for the current computer time.", ",", true));
            r(new ScriptCommand("system", "dayofyear", "int", "Returns the day of the year (Outdated, use <environment.dayofyear> instead).", ",", true));
            r(new ScriptCommand("system", "year", "int", "Returns the current year (Outdated, use <environment.year> instead).", ",", true));
            r(new ScriptCommand("system", "booltoint", "int",
                [new ScriptArgument("bool", ScriptArgument.ArgumentTypes.Bool)],
                "Converts a boolean into an integer (Outdated, use <math.int> instead).", ",", true));
            r(new ScriptCommand("system", "startswith", "bool",
                [
                    new ScriptArgument("DoesThisStartWith", ScriptArgument.ArgumentTypes.Str),
                    new ScriptArgument("StringsToCheck", ScriptArgument.ArgumentTypes.Str)
                ],
                "Returns true if the argument \"DoesThisStartWith\" starts with one of given strings to check (separated by ;).", ",", true));
            r(new ScriptCommand("system", "contains", "bool",
                [
                    new ScriptArgument("DoesThisContain", ScriptArgument.ArgumentTypes.Str),
                    new ScriptArgument("StringsToCheck", ScriptArgument.ArgumentTypes.Str)
                ],
                "Returns true if the argument \"DoesThisContain\" contains one of given strings to check (separated by ;).", ",", true));
            r(new ScriptCommand("system", "calcint", "int",
                [new ScriptArgument("expression", ScriptArgument.ArgumentTypes.Str)],
                "Converts the expression to an integer (Outdated, use <math.int> instead).", ",", true));
            r(new ScriptCommand("system", "int", "int",
                [new ScriptArgument("expression", ScriptArgument.ArgumentTypes.Str)],
                "Converts the expression to an integer (Outdated, use <math.int> instead).", ",", true));
            r(new ScriptCommand("system", "calcsng", "sng",
                [new ScriptArgument("expression", ScriptArgument.ArgumentTypes.Str)],
                "Converts the expression to a single (Outdated, use <math.sng> instead).", ",", true));
            r(new ScriptCommand("system", "sng", "sng",
                [new ScriptArgument("expression", ScriptArgument.ArgumentTypes.Str)],
                "Converts the expression to a single (Outdated, use <math.sng> instead).", ",", true));
            r(new ScriptCommand("system", "sort", "str",
                [
                    new ScriptArgument("sortMode", ScriptArgument.ArgumentTypes.Str, ["ascending", "descending"]),
                    new ScriptArgument("returnIndex", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("list", ScriptArgument.ArgumentTypes.Arr)
                ],
                "Sorts the list after sortmode and returns the item at the given index.", ",", true));
            r(new ScriptCommand("system", "scripttrigger", "string", "Returns what triggered the current script (NPCInSight, NPCInteract, ScriptBlockWalkOn, ScriptBlockInteract, Notification, PhoneReceive, PhoneCall, StartScript, ScriptCommand, StrengthTrigger, MapScript, ChatCommand).", ",", true));
            r(new ScriptCommand("system", "isinsightscript", "bool", "Returns if the running script was triggered by the inSight function of an NPC.", ",", true));
            r(new ScriptCommand("system", "lastinput", "str", "Returns the last input received from the input screen (@screen.input).", ",", true));
            r(new ScriptCommand("system", "return", "str", "Returns the value set with the \":return\" switch.", ",", true));
            r(new ScriptCommand("system", "isint", "bool",
                [new ScriptArgument("expression", ScriptArgument.ArgumentTypes.Str)],
                "Checks if the expression is an integer (Outdated, use <math.isint> instead).", ",", true));
            r(new ScriptCommand("system", "issng", "bool",
                [new ScriptArgument("expression", ScriptArgument.ArgumentTypes.Str)],
                "Checks if the expression is a single (Outdated, use <math.issng> instead).", ",", true));
            r(new ScriptCommand("system", "chrw", "str",
                [new ScriptArgument("charCodes", ScriptArgument.ArgumentTypes.IntArr)],
                "Converts Unicode CharCodes into a string.", ",", true));
            r(new ScriptCommand("system", "scriptlevel", "int", "Returns the current script level (call depth).", ",", true));
            r(new ScriptCommand("system", "language", "str", "Returns the current game language suffix.", ",", true));
            r(new ScriptCommand("system", "fileexists", "bool",
                [new ScriptArgument("filePath", ScriptArgument.ArgumentTypes.Str)],
                "Returns if the specified file (including extension) exists (relative to the GameMode's ContentPath).", ",", true));
        }

        private static void DoEnvironment()
        {
            // Commands:
            r(new ScriptCommand("Environment", "SetWeather",
                [new ScriptArgument("weatherType", ScriptArgument.ArgumentTypes.Int)],
                "Changes the weather type of the current map."));
            r(new ScriptCommand("Environment", "SetRegionWeather",
                [new ScriptArgument("weatherID", ScriptArgument.ArgumentTypes.Int)],
                "Changes the weather of the current region."));
            r(new ScriptCommand("Environment", "ResetRegionWeather", "Resets the weather to be based on the current season."));
            r(new ScriptCommand("Environment", "SetSeason",
                [new ScriptArgument("seasonID", ScriptArgument.ArgumentTypes.Int)],
                "Changes the season. Use -1 as the argument to change back to the default season."));
            r(new ScriptCommand("Environment", "SetCanFly",
                [new ScriptArgument("canfly", ScriptArgument.ArgumentTypes.Bool)],
                "Sets the \"CanFly\" parameter of the current map."));
            r(new ScriptCommand("Environment", "SetCanDig",
                [new ScriptArgument("candig", ScriptArgument.ArgumentTypes.Bool)],
                "Sets the \"CanDig\" parameter of the current map."));
            r(new ScriptCommand("Environment", "SetCanTeleport",
                [new ScriptArgument("canteleport", ScriptArgument.ArgumentTypes.Bool)],
                "Sets the \"CanTeleport\" parameter of the current map."));
            r(new ScriptCommand("Environment", "SetWildPokemonGrass",
                [new ScriptArgument("canencounter", ScriptArgument.ArgumentTypes.Bool)],
                "Sets the \"WildPokemonGrass\" parameter of the current map."));
            r(new ScriptCommand("Environment", "SetWildPokemonWater",
                [new ScriptArgument("canencounter", ScriptArgument.ArgumentTypes.Bool)],
                "Sets the \"WildPokemonWater\" parameter of the current map."));
            r(new ScriptCommand("Environment", "SetWildPokemonEverywhere",
                [new ScriptArgument("canencounter", ScriptArgument.ArgumentTypes.Bool)],
                "Sets the \"WildPokemonFloor\" parameter of the current map."));
            r(new ScriptCommand("Environment", "SetIsDark",
                [new ScriptArgument("isDark", ScriptArgument.ArgumentTypes.Bool)],
                "Sets the \"IsDark\" parameter of the current map."));
            r(new ScriptCommand("Environment", "SetRenderDistance",
                [new ScriptArgument("distance", ScriptArgument.ArgumentTypes.Str, ["0-4", "tiny", "small", "normal", "far", "extreme"])],
                "Sets the render distance."));
            r(new ScriptCommand("Environment", "ToggleDarkness", "Toggles the \"IsDark\" parameter of the current map."));
            r(new ScriptCommand("Environment", "SetDayTime",
                [new ScriptArgument("daytime", ScriptArgument.ArgumentTypes.Int)],
                "Sets the daytime to use for the Outside EnvironmentType (0). Can be 1-4, any other number resets to the default daytime."));
            r(new ScriptCommand("Environment", "SetEnvironmentType",
                [new ScriptArgument("environmenttype", ScriptArgument.ArgumentTypes.Int)],
                "Sets the \"EnvironmentType\" parameter of the map, which also changes the sky texture and sometimes adds particles. Value can be 0-5."));

            // Constructs:
            r(new ScriptCommand("environment", "daytime", "str", "Returns the current DayTime of the game.", ",", true));
            r(new ScriptCommand("environment", "daytimeid", "int", "Returns the current DayTimeID of the game.", ",", true));
            r(new ScriptCommand("environment", "season", "str", "Returns the current Season of the game.", ",", true));
            r(new ScriptCommand("environment", "seasonid", "int", "Returns the current SeasonID of the game.", ",", true));
            r(new ScriptCommand("environment", "day", "str", "Returns the current day of the week.", ",", true));
            r(new ScriptCommand("environment", "dayofyear", "int", "Returns the current day of the year.", ",", true));
            r(new ScriptCommand("environment", "dayinformation", "str", "Returns the current day of the week and DayTime of the game.", ",", true));
            r(new ScriptCommand("environment", "week", "str", "Returns the current week of the year.", ",", true));
            r(new ScriptCommand("environment", "hour", "str", "Returns the current hour in 24-hour time.", ",", true));
            r(new ScriptCommand("environment", "year", "str", "Returns the current year.", ",", true));
            r(new ScriptCommand("environment", "weather", "str", "Returns the Weather of the current map.", ",", true));
            r(new ScriptCommand("environment", "mapweather", "str", "Returns the Weather of the current map.", ",", true));
            r(new ScriptCommand("environment", "currentmapweather", "str", "Returns the Weather of the current map.", ",", true));
            r(new ScriptCommand("environment", "weatherid", "int", "Returns the WeatherID of the current map.", ",", true));
            r(new ScriptCommand("environment", "mapweatherid", "int", "Returns the WeatherID of the current map.", ",", true));
            r(new ScriptCommand("environment", "currentmapweatherid", "int", "Returns the WeatherID of the current map.", ",", true));
            r(new ScriptCommand("environment", "regionweather", "str", "Returns the Weather of the current region.", ",", true));
            r(new ScriptCommand("environment", "regionweatherid", "str", "Returns the WeatherID of the current region.", ",", true));
            r(new ScriptCommand("environment", "canfly", "bool", "Returns the \"CanFly\" parameter of the current map.", ",", true));
            r(new ScriptCommand("environment", "candig", "bool", "Returns the \"CanDig\" parameter of the current map.", ",", true));
            r(new ScriptCommand("environment", "canteleport", "bool", "Returns the \"CanTeleport\" parameter of the current map.", ",", true));
            r(new ScriptCommand("environment", "wildpokemongrass", "bool", "Returns the \"WildPokemonGrass\" parameter of the current map.", ",", true));
            r(new ScriptCommand("environment", "wildpokemonwater", "bool", "Returns the \"WildPokemonWater\" parameter of the current map.", ",", true));
            r(new ScriptCommand("environment", "wildpokemoneverywhere", "bool", "Returns the \"WildPokemonEverywhere\" parameter of the current map.", ",", true));
            r(new ScriptCommand("environment", "isdark", "bool", "Returns the \"IsDark\" parameter of the current map.", ",", true));
            r(new ScriptCommand("environment", "graphicstyle", "bool", "Returns if the Graphics option is set to fancy (true) or fast (false).", ",", true));
        }

        private static void DoPlayer()
        {
            // Commands:
            r(new ScriptCommand("Player", "ReceivePokedex", "Makes the Pokedex accessible for the player."));
            r(new ScriptCommand("Player", "ReceivePokegear", "Makes the Pokegear accessible for the player."));
            r(new ScriptCommand("Player", "RenameRival", "Opens the rival rename screen."));
            r(new ScriptCommand("Player", "WearSkin",
                [new ScriptArgument("Skin", ScriptArgument.ArgumentTypes.Str)],
                "Changes the player skin temporarily."));
            r(new ScriptCommand("Player", "SetSkin",
                [new ScriptArgument("Skin", ScriptArgument.ArgumentTypes.Str)],
                "Changes the player skin permanently."));
            r(new ScriptCommand("Player", "Move",
                [new ScriptArgument("Steps", ScriptArgument.ArgumentTypes.Sng)],
                "Starts the player movement."));
            r(new ScriptCommand("Player", "MoveAsync",
                [new ScriptArgument("Steps", ScriptArgument.ArgumentTypes.Str)],
                "Starts the async player movement."));
            r(new ScriptCommand("Player", "Turn",
                [new ScriptArgument("Turns", ScriptArgument.ArgumentTypes.Int)],
                "Adds to the direction the player faces and starts the turning."));
            r(new ScriptCommand("Player", "TurnAsync",
                [new ScriptArgument("Turns", ScriptArgument.ArgumentTypes.Int)],
                "Adds to the direction the player faces and starts the async turning."));
            r(new ScriptCommand("Player", "TurnTo",
                [new ScriptArgument("Facing", ScriptArgument.ArgumentTypes.Int)],
                "Changes the direction the player faces and starts the turning."));
            r(new ScriptCommand("Player", "TurnToAsync",
                [new ScriptArgument("Facing", ScriptArgument.ArgumentTypes.Int)],
                "Changes the direction the player faces and starts the async turning."));
            r(new ScriptCommand("Player", "Warp",
                [
                    new ScriptArgument("MapPath", ScriptArgument.ArgumentTypes.Str, true, "Current map."),
                    new ScriptArgument("xPos", ScriptArgument.ArgumentTypes.Sng, true, "~"),
                    new ScriptArgument("yPos", ScriptArgument.ArgumentTypes.Sng, true, "~"),
                    new ScriptArgument("zPos", ScriptArgument.ArgumentTypes.Sng, true, "~"),
                    new ScriptArgument("Rotations", ScriptArgument.ArgumentTypes.Int, ["0-3"], true, "0"),
                    new ScriptArgument("WarpSound", ScriptArgument.ArgumentTypes.Int, ["0-3"], true, "0")
                ],
                "Warps the player to a new location on a new map and changes the facing afterwards. To get relative coordinates, enter a \"~\"."));
            r(new ScriptCommand("Player", "StopMovement", "Stops the player movement."));
            r(new ScriptCommand("Player", "AddMoney",
                [new ScriptArgument("Amount", ScriptArgument.ArgumentTypes.Int)],
                "Adds the given amount to the player's money."));
            r(new ScriptCommand("Player", "RemoveMoney",
                [new ScriptArgument("Amount", ScriptArgument.ArgumentTypes.Int)],
                "Removes the given amount from the player's money."));
            r(new ScriptCommand("Player", "AddCoins",
                [new ScriptArgument("Amount", ScriptArgument.ArgumentTypes.Int)],
                "Adds the given amount to the player's coins."));
            r(new ScriptCommand("Player", "RemoveCoins",
                [new ScriptArgument("Amount", ScriptArgument.ArgumentTypes.Int)],
                "Removes the given amount from the player's coins."));
            r(new ScriptCommand("Player", "SetSpeed",
                [new ScriptArgument("Speed", ScriptArgument.ArgumentTypes.Sng)],
                "Sets the movement speed of the player. The default is \"1\"."));
            r(new ScriptCommand("Player", "ResetSpeed", "Resets the movement speed of the player to the default speed, which is \"1\"."));
            r(new ScriptCommand("Player", "SetMovement",
                [
                    new ScriptArgument("xDir", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("yDir", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("zDir", ScriptArgument.ArgumentTypes.Int)
                ],
                "Sets the direction the player will move next regardless of facing."));
            r(new ScriptCommand("Player", "ResetMovement", "Resets the player movement to the default movement directions."));
            r(new ScriptCommand("Player", "PreventMovement", "Makes the player unable to move, while still keeping control over the menu, interactions etc."));
            r(new ScriptCommand("Player", "AllowMovement", "Gives the player back their ability to move after using @Player.PreventMovement."));
            r(new ScriptCommand("Player", "GetBadge",
                [new ScriptArgument("BadgeID", ScriptArgument.ArgumentTypes.Int)],
                "Adds the given Badge to the player's Badges and displays a message."));
            r(new ScriptCommand("Player", "RemoveBadge",
                [new ScriptArgument("BadgeID", ScriptArgument.ArgumentTypes.Int)],
                "Removes the given Badge from the player's Badges."));
            r(new ScriptCommand("Player", "AddBadge",
                [new ScriptArgument("BadgeID", ScriptArgument.ArgumentTypes.Int)],
                "Adds the given Badge to the player's Badges."));
            r(new ScriptCommand("Player", "AddFrontierEmblem",
                [
                    new ScriptArgument("FrontierEmblemID", ScriptArgument.ArgumentTypes.Str),
                    new ScriptArgument("SilverOrGold", ScriptArgument.ArgumentTypes.Bool, true, "1")
                ],
                "Adds a frontier emblem (silver or gold) to the player's emblems. Second argument can be 0 = silver, 1 = gold"));
            r(new ScriptCommand("Player", "RemoveFrontierEmblem",
                [
                    new ScriptArgument("FrontierEmblemID", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("SilverOrGold", ScriptArgument.ArgumentTypes.Bool, true, "")
                ],
                "Removes a frontier emblem from the player's emblems. Second argument can be 0 = silver, 1 = gold. Without it, both silver and gold emblems will be removed."));
            r(new ScriptCommand("Player", "AchieveEmblem",
                [new ScriptArgument("EmblemName", ScriptArgument.ArgumentTypes.Str)],
                "Makes the player achieve an emblem (GameJolt only)."));
            r(new ScriptCommand("Player", "AddBP",
                [new ScriptArgument("Amount", ScriptArgument.ArgumentTypes.Int)],
                "Adds the given amount to the player's Battle Points."));
            r(new ScriptCommand("Player", "RemoveBP",
                [new ScriptArgument("Amount", ScriptArgument.ArgumentTypes.Int)],
                "Removes the given amount from the player's Battle Points."));
            r(new ScriptCommand("Player", "ShowRod",
                [new ScriptArgument("RodID", ScriptArgument.ArgumentTypes.Int, ["0-2"])],
                "Displays a Fishing Rod on the screen."));
            r(new ScriptCommand("Player", "HideRod", "Hides the Fishing Rod."));
            r(new ScriptCommand("Player", "Save", "Saves the game."));
            r(new ScriptCommand("Player", "SetRivalName",
                [new ScriptArgument("Name", ScriptArgument.ArgumentTypes.Str)],
                "Sets the rival's name."));
            r(new ScriptCommand("Player", "SetRivalSkin",
                [new ScriptArgument("Skin", ScriptArgument.ArgumentTypes.Str)],
                "Sets the rival's skin."));
            r(new ScriptCommand("Player", "SetGender",
                [new ScriptArgument("Gender", ScriptArgument.ArgumentTypes.Str, ["0-2, Male, Female, Other"])],
                "Sets the player's gender."));
            r(new ScriptCommand("Player", "SetOpacity",
                [new ScriptArgument("Opacity", ScriptArgument.ArgumentTypes.Sng)],
                "Sets the player entity's opacity."));
            r(new ScriptCommand("Player", "SetDifficulty",
                [new ScriptArgument("DifficultyLevel", ScriptArgument.ArgumentTypes.Int, ["0-2"])],
                "Sets the difficulty level for the player."));
            r(new ScriptCommand("Player", "QuitGame",
                [new ScriptArgument("DoFade", ScriptArgument.ArgumentTypes.Bool, true, "")],
                "Quits the game and goes back to the Main Menu (with optionally a fade out and in)."));
            r(new ScriptCommand("Player", "DoWalkAnimation",
                [new ScriptArgument("WalkAnimation", ScriptArgument.ArgumentTypes.Bool)],
                "Enables or disables the player's walking animation when walking or during a @player.move command."));
            r(new ScriptCommand("Player", "RemoveItemData",
                [
                    new ScriptArgument("LevelPath", ScriptArgument.ArgumentTypes.Str),
                    new ScriptArgument("itemIndex", ScriptArgument.ArgumentTypes.Str)
                ],
                "Makes the specified item index of the specified map spawn again after it has been found."));

            // Constructs:
            r(new ScriptCommand("Player", "Position", "sngarr",
                [new ScriptArgument("Coordinate", ScriptArgument.ArgumentTypes.StrArr, ["x", "y", "z"], true, "")],
                "Returns the position of the player. The normal coordinate combination is \"X,Y,Z\".", ",", true));
            r(new ScriptCommand("Player", "HasBadge", "bool",
                [new ScriptArgument("BadgeID", ScriptArgument.ArgumentTypes.Int)],
                "Returns if the player owns a specific Badge.", ",", true));
            r(new ScriptCommand("Player", "HasFrontierEmblem", "bool",
                [
                    new ScriptArgument("FrontierEmblemID", ScriptArgument.ArgumentTypes.Str),
                    new ScriptArgument("SilverOrGold", ScriptArgument.ArgumentTypes.Bool, true, "")
                ],
                "Returns if the player owns a specific frontier emblem. Without the second argument, returns true if the player owns either silver or gold.", ",", true));
            r(new ScriptCommand("Player", "Skin", "str", "Returns the current skin the player wears.", ",", true));
            r(new ScriptCommand("Player", "Velocity", "sng", "Returns the player's velocity (Steps until the player movement ends).", ",", true));
            r(new ScriptCommand("Player", "Speed", "sng", "Returns the player's movement speed (Divided by 0.04F).", ",", true));
            r(new ScriptCommand("Player", "IsRunning", "bool", "Returns if the player is currently running.", ",", true));
            r(new ScriptCommand("Player", "IsMoving", "bool", "Returns if the player is currently moving.", ",", true));
            r(new ScriptCommand("Player", "Facing", "int", "Returns the direction the player is facing.", ",", true));
            r(new ScriptCommand("Player", "Compass", "str", "Returns \"north\", \"east\", \"south\" or \"west\" depending on the direction the player is facing.", ",", true));
            r(new ScriptCommand("Player", "Money", "int", "Returns the player's money.", ",", true));
            r(new ScriptCommand("Player", "Coins", "int", "Returns the player's coins.", ",", true));
            r(new ScriptCommand("Player", "Name", "str", "Returns the player's name", ",", true));
            r(new ScriptCommand("Player", "Gender", "str", "Returns the player's gender (Male, Female, Other)", ",", true));
            r(new ScriptCommand("Player", "Bp", "int", "Returns the amount of Battle Points the player owns.", ",", true));
            r(new ScriptCommand("Player", "Badges", "int", "Returns the amount of Badges the player owns", ",", true));
            r(new ScriptCommand("Player", "ThirdPerson", "bool", "Returns if the game is currently played in third person.", ",", true));
            r(new ScriptCommand("Player", "Rival", "str", "Returns the rival's name.", ",", true));
            r(new ScriptCommand("Player", "RivalName", "str", "Returns the rival's name.", ",", true));
            r(new ScriptCommand("Player", "Ot", "str", "Returns the player's Original Trainer value.", ",", true));
            r(new ScriptCommand("Player", "GameJoltID", "str", "Returns the player's GameJolt ID.", ",", true));
            r(new ScriptCommand("Player", "HasPokedex", "bool", "Returns if the player received the Pokedex.", ",", true));
            r(new ScriptCommand("Player", "HasPokegear", "bool", "Returns if the player received the Pokegear.", ",", true));
        }

        private static void DoNPC()
        {
            // Commands:
            r(new ScriptCommand("NPC", "Remove",
                [new ScriptArgument("ID", ScriptArgument.ArgumentTypes.Int)],
                "Removes the selected NPC from the map."));
            r(new ScriptCommand("NPC", "Position",
                [
                    new ScriptArgument("ID", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("xPos", ScriptArgument.ArgumentTypes.Sng),
                    new ScriptArgument("yPos", ScriptArgument.ArgumentTypes.Sng),
                    new ScriptArgument("zPos", ScriptArgument.ArgumentTypes.Sng)
                ],
                "Moves the selected NPC to a different place on the map. To get relative coordinates, enter a \"~\"."));
            r(new ScriptCommand("NPC", "Warp",
                [
                    new ScriptArgument("ID", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("xPos", ScriptArgument.ArgumentTypes.Sng),
                    new ScriptArgument("yPos", ScriptArgument.ArgumentTypes.Sng),
                    new ScriptArgument("zPos", ScriptArgument.ArgumentTypes.Sng)
                ],
                "Moves the selected NPC to a different place on the map. To get relative coordinates, enter a \"~\"."));
            r(new ScriptCommand("NPC", "AddToPosition",
                [
                    new ScriptArgument("ID", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("xPos", ScriptArgument.ArgumentTypes.Sng),
                    new ScriptArgument("yPos", ScriptArgument.ArgumentTypes.Sng),
                    new ScriptArgument("zPos", ScriptArgument.ArgumentTypes.Sng)
                ],
                "Adds the given coordinates to the position of the given NPC. To get relative coordinates, enter a \"~\"."));
            r(new ScriptCommand("NPC", "Register",
                [new ScriptArgument("RegisterData", ScriptArgument.ArgumentTypes.Str)],
                "Registers NPC data. Format: {MapFile|ID|Action(\"position\",\"remove\")|addition)"));
            r(new ScriptCommand("NPC", "Unregister",
                [new ScriptArgument("RegisterData", ScriptArgument.ArgumentTypes.Str)],
                "Unregisters NPC data. Format: {MapFile|ID|Action(\"position\",\"remove\")|addition)"));
            r(new ScriptCommand("NPC", "WearSkin",
                [
                    new ScriptArgument("ID", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("Skin", ScriptArgument.ArgumentTypes.Str)
                ],
                "Sets the skin of the selected NPC."));
            r(new ScriptCommand("NPC", "Move",
                [
                    new ScriptArgument("ID", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("Steps", ScriptArgument.ArgumentTypes.Int)
                ],
                "Starts NPC movement of the selected NPC."));
            r(new ScriptCommand("NPC", "SetMoveY",
                [
                    new ScriptArgument("ID", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("Distance", ScriptArgument.ArgumentTypes.Sng)
                ],
                "Sets the distance that the selected NPC should move in the Y direction."));
            r(new ScriptCommand("NPC", "MoveAsync",
                [
                    new ScriptArgument("ID", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("Steps", ScriptArgument.ArgumentTypes.Int)
                ],
                "Starts async NPC movement of the selected NPC."));
            r(new ScriptCommand("NPC", "Turn",
                [
                    new ScriptArgument("ID", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("Facing", ScriptArgument.ArgumentTypes.Int)
                ],
                "Sets the face direction of the selected NPC."));
            r(new ScriptCommand("NPC", "Spawn",
                [
                    new ScriptArgument("xPos", ScriptArgument.ArgumentTypes.Sng),
                    new ScriptArgument("yPos", ScriptArgument.ArgumentTypes.Sng),
                    new ScriptArgument("zPos", ScriptArgument.ArgumentTypes.Sng),
                    new ScriptArgument("ActionValue", ScriptArgument.ArgumentTypes.Int, true, "0"),
                    new ScriptArgument("AdditionalValue", ScriptArgument.ArgumentTypes.Str, true, ""),
                    new ScriptArgument("TextureID", ScriptArgument.ArgumentTypes.Str, true, "0"),
                    new ScriptArgument("AnimateIdle", ScriptArgument.ArgumentTypes.Bool, true, "false"),
                    new ScriptArgument("Rotation", ScriptArgument.ArgumentTypes.Int, true, "0"),
                    new ScriptArgument("Name", ScriptArgument.ArgumentTypes.Str, true, ""),
                    new ScriptArgument("ID", ScriptArgument.ArgumentTypes.Int, true, "0"),
                    new ScriptArgument("Movement", ScriptArgument.ArgumentTypes.Str, ["Pokeball", "Still", "Looking", "FacePlayer", "Walk", "Straight", "Turning"], true, "Still")
                ],
                "Spawns a new NPC with the given conditions."));
            r(new ScriptCommand("NPC", "SetSpeed",
                [
                    new ScriptArgument("ID", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("Speed", ScriptArgument.ArgumentTypes.Sng)
                ],
                "Sets the speed of an NPC. The default is \"1\"."));
            r(new ScriptCommand("NPC", "SetScale",
                [
                    new ScriptArgument("ID", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("xScale", ScriptArgument.ArgumentTypes.Sng),
                    new ScriptArgument("yScale", ScriptArgument.ArgumentTypes.Sng),
                    new ScriptArgument("zScale", ScriptArgument.ArgumentTypes.Sng)
                ],
                "Changes the Scale property of the selected NPC."));
            r(new ScriptCommand("NPC", "SetAnimateIdle",
                [
                    new ScriptArgument("ID", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("AnimateIdle", ScriptArgument.ArgumentTypes.Bool)
                ],
                "Sets the AnimateIdle property of the selected NPC."));
            r(new ScriptCommand("NPC", "SetMovement",
                [
                    new ScriptArgument("ID", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("Movement", ScriptArgument.ArgumentTypes.Str, ["still", "faceplayer", "walk", "straight", "turning", "pokeball"])
                ],
                "Sets the Movement property of the selected NPC."));
            r(new ScriptCommand("NPC", "SetAdditionalValue",
                [
                    new ScriptArgument("ID", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("AdditionalValue", ScriptArgument.ArgumentTypes.Str)
                ],
                "Sets the AdditionalValue property of the given NPC."));
            r(new ScriptCommand("NPC", "SetAction",
                [
                    new ScriptArgument("ID", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("Action", ScriptArgument.ArgumentTypes.Str)
                ],
                "Sets the Action property of the given NPC."));

            // Constructs:
            r(new ScriptCommand("NPC", "Position", "sngArr",
                [new ScriptArgument("ID", ScriptArgument.ArgumentTypes.Int)],
                "Returns the position of the selected NPC.", ",", true));
            r(new ScriptCommand("NPC", "Exists", "bool",
                [new ScriptArgument("ID", ScriptArgument.ArgumentTypes.Int)],
                "Returns if an NPC with the given ID exists on the map.", ",", true));
            r(new ScriptCommand("NPC", "IsMoving", "bool",
                [new ScriptArgument("ID", ScriptArgument.ArgumentTypes.Int)],
                "Returns if the selected NPC is moving.", ",", true));
            r(new ScriptCommand("NPC", "Moved", "sng",
                [new ScriptArgument("ID", ScriptArgument.ArgumentTypes.Int)],
                "Returns the amount of steps the selected NPC still has to move.", ",", true));
            r(new ScriptCommand("NPC", "Skin", "str",
                [new ScriptArgument("ID", ScriptArgument.ArgumentTypes.Int)],
                "Returns the skin of the selected NPC.", ",", true));
            r(new ScriptCommand("NPC", "Facing", "int",
                [new ScriptArgument("ID", ScriptArgument.ArgumentTypes.Int)],
                "Returns the direction the selected NPC is facing.", ",", true));
            r(new ScriptCommand("NPC", "ID", "int",
                [new ScriptArgument("ID", ScriptArgument.ArgumentTypes.Int)],
                "Returns the NPC ID for the selected NPC.", ",", true));
            r(new ScriptCommand("NPC", "Name", "str",
                [new ScriptArgument("ID", ScriptArgument.ArgumentTypes.Int)],
                "Returns the name of the selected NPC.", ",", true));
            r(new ScriptCommand("NPC", "Action", "str",
                [new ScriptArgument("ID", ScriptArgument.ArgumentTypes.Int)],
                "Returns the action value of the selected NPC.", ",", true));
            r(new ScriptCommand("NPC", "AdditionalValue", "int",
                [new ScriptArgument("ID", ScriptArgument.ArgumentTypes.Int)],
                "Returns the additional value of the selected NPC.", ",", true));
            r(new ScriptCommand("NPC", "Movement", "str",
                [new ScriptArgument("ID", ScriptArgument.ArgumentTypes.Int)],
                "Returns the movement type of the selected NPC.", ",", true));
            r(new ScriptCommand("NPC", "HasMoveRectangles", "bool",
                [new ScriptArgument("ID", ScriptArgument.ArgumentTypes.Int)],
                "Returns if the selected NPC has any movement rectangles.", ",", true));
            r(new ScriptCommand("NPC", "TrainerTexture", "str",
                [new ScriptArgument("TrainerFilePath", ScriptArgument.ArgumentTypes.Str)],
                "Returns the texture name of the given trainer. Trainer file starts at the \"Scripts\\Trainer\\\" path and must not have the \".trainer\" extension.", ",", true));
        }

        private static void DoRadio()
        {
            // Commands:
            r(new ScriptCommand("radio", "allowchannel",
                [new ScriptArgument("channel", ScriptArgument.ArgumentTypes.Sng)],
                "Allows a Radio channel on the map."));
            r(new ScriptCommand("radio", "blockchannel",
                [new ScriptArgument("channel", ScriptArgument.ArgumentTypes.Sng)],
                "Blocks a Radio channel on the map."));

            // Constructs:
            r(new ScriptCommand("radio", "currentchannel", "str", "Returns the name of the channel that is currently playing.", "", true));
        }

        private static void DoPokedex()
        {
            // Commands:
            r(new ScriptCommand("pokedex", "setautodetect",
                [new ScriptArgument("autodetect", ScriptArgument.ArgumentTypes.Bool)],
                "Sets if the Pokedex registers seen Pokemon in wild or trainer battles."));
            r(new ScriptCommand("pokedex", "changeentry",
                [
                    new ScriptArgument("ID", ScriptArgument.ArgumentTypes.Str),
                    new ScriptArgument("Type", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("ForceChange", ScriptArgument.ArgumentTypes.Bool, true, "false")
                ],
                "Changes a Pokedex Entry."));

            // Constructs:
            r(new ScriptCommand("pokedex", "caught", "int", "Returns the amount of Pokemon registered as caught by the player.", "", true));
            r(new ScriptCommand("pokedex", "seen", "int", "Returns the amount of Pokemon registered as seen by the player.", "", true));
            r(new ScriptCommand("pokedex", "shiny", "int", "Returns the amount of Pokemon registered as Shiny by the player.", "", true));
            r(new ScriptCommand("pokedex", "dexcaught", "int",
                [new ScriptArgument("dexIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns the amount of Pokemon registered as caught by the player for a specific Pokedex.", "", true));
            r(new ScriptCommand("pokedex", "dexseen", "int",
                [new ScriptArgument("dexIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns the amount of Pokemon registered as seen by the player for a specific Pokedex.", "", true));
            r(new ScriptCommand("pokedex", "getheight", "sng",
                [new ScriptArgument("ID", ScriptArgument.ArgumentTypes.Str)],
                "Returns the height of the Pokemon.", "", true));
            r(new ScriptCommand("pokedex", "getweight", "sng",
                [new ScriptArgument("ID", ScriptArgument.ArgumentTypes.Str)],
                "Returns the weight of the Pokemon.", "", true));
            r(new ScriptCommand("pokedex", "getentry", "str",
                [new ScriptArgument("ID", ScriptArgument.ArgumentTypes.Str)],
                "Returns the description of the Pokemon.", "", true));
            r(new ScriptCommand("pokedex", "getspecies", "str",
                [new ScriptArgument("ID", ScriptArgument.ArgumentTypes.Str)],
                "Returns the species of the Pokemon.", "", true));
            r(new ScriptCommand("pokedex", "getname", "str",
                [new ScriptArgument("ID", ScriptArgument.ArgumentTypes.Str)],
                "Returns the name of the Pokemon.", "", true));
            r(new ScriptCommand("pokedex", "getability", "int",
                [
                    new ScriptArgument("ID", ScriptArgument.ArgumentTypes.Str),
                    new ScriptArgument("requestType", ScriptArgument.ArgumentTypes.Int)
                ],
                "Returns one of the abilities of the Pokemon based on the requestType. 0 = random 1st or 2nd, 1 = first ability, 2 = second ability, 3 = hidden ability.", "", true));
            r(new ScriptCommand("pokedex", "Pokemoncaught", "bool",
                [new ScriptArgument("ID", ScriptArgument.ArgumentTypes.Str)],
                "Returns if the specified Pokemon has been caught.", "", true));
            r(new ScriptCommand("pokedex", "Pokemonseen", "bool",
                [new ScriptArgument("ID", ScriptArgument.ArgumentTypes.Str)],
                "Returns if the specified Pokemon has been seen.", "", true));
        }

        private static void DoMath()
        {
            // Constructs:
            r(new ScriptCommand("math", "int", "int",
                [new ScriptArgument("expression", ScriptArgument.ArgumentTypes.Str)],
                "Converts the argument to an integer.", "", true));
            r(new ScriptCommand("math", "sng", "sng",
                [new ScriptArgument("expression", ScriptArgument.ArgumentTypes.Str)],
                "Converts the argument to a single.", "", true));
            r(new ScriptCommand("math", "abs", "sng",
                [new ScriptArgument("expression", ScriptArgument.ArgumentTypes.Sng)],
                "Returns the absolute value of a number.", "", true));
            r(new ScriptCommand("math", "ceiling", "int",
                [new ScriptArgument("expression", ScriptArgument.ArgumentTypes.Sng)],
                "Rounds the value up to the next integer.", "", true));
            r(new ScriptCommand("math", "floor", "int",
                [new ScriptArgument("expression", ScriptArgument.ArgumentTypes.Sng)],
                "Rounds the value down to the next integer.", "", true));
            r(new ScriptCommand("math", "isint", "bool",
                [new ScriptArgument("expression", ScriptArgument.ArgumentTypes.Str)],
                "Checks if the expression is an integer.", "", true));
            r(new ScriptCommand("math", "issng", "bool",
                [new ScriptArgument("expression", ScriptArgument.ArgumentTypes.Str)],
                "Checks if the expression is a single.", "", true));
            r(new ScriptCommand("math", "clamp", "sng",
                [
                    new ScriptArgument("number", ScriptArgument.ArgumentTypes.Sng),
                    new ScriptArgument("min", ScriptArgument.ArgumentTypes.Sng),
                    new ScriptArgument("max", ScriptArgument.ArgumentTypes.Sng)
                ],
                "Clamps a number.", "", true));
            r(new ScriptCommand("math", "rollover", "sng",
                [
                    new ScriptArgument("number", ScriptArgument.ArgumentTypes.Sng),
                    new ScriptArgument("min", ScriptArgument.ArgumentTypes.Sng),
                    new ScriptArgument("max", ScriptArgument.ArgumentTypes.Sng)
                ],
                "Rolls a number over with min and max properties.", "", true));
        }

        private static void DoRival()
        {
            // Constructs:
            r(new ScriptCommand("rival", "name", "str", "Returns the rival's name", "", true));
            r(new ScriptCommand("rival", "skin", "str", "Returns the rival's skin", "", true));
        }

        private static void DoDaycare()
        {
            // Commands:
            r(new ScriptCommand("Daycare", "Clean",
                [new ScriptArgument("DaycareID", ScriptArgument.ArgumentTypes.Int)],
                "Cleans all data for the given Daycare. This doesn't remove the data, just rearranges it."));
            r(new ScriptCommand("Daycare", "ClearData",
                [new ScriptArgument("DaycareID", ScriptArgument.ArgumentTypes.Int)],
                "Clears all the data for one Daycare. That includes the Pokemon stored there and a potential Egg."));
            r(new ScriptCommand("Daycare", "LeavePokemon",
                [
                    new ScriptArgument("DaycareID", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("DaycareSlot", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("PartyIndex", ScriptArgument.ArgumentTypes.Int)
                ],
                "Removes a Pokemon from the player's party and fills the given Daycare's slot with that Pokemon."));
            r(new ScriptCommand("Daycare", "RemoveEgg",
                [new ScriptArgument("DaycareID", ScriptArgument.ArgumentTypes.Int)],
                "Removes the egg from the given Daycare permanently."));
            r(new ScriptCommand("Daycare", "TakeEgg",
                [new ScriptArgument("DaycareID", ScriptArgument.ArgumentTypes.Int)],
                "Removes the Egg from the Daycare and adds it to the player's party."));
            r(new ScriptCommand("Daycare", "TakePokemon",
                [
                    new ScriptArgument("DaycareID", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("DaycareSlot", ScriptArgument.ArgumentTypes.Int)
                ],
                "Takes the given Pokemon from the given Daycare to the player's party."));
            r(new ScriptCommand("Daycare", "Call",
                [new ScriptArgument("DaycareID", ScriptArgument.ArgumentTypes.Int)],
                "Initializes a call with the Daycare. This checks if the Daycare is registered in the Pokegear."));

            // Constructs:
            r(new ScriptCommand("Daycare", "PokemonID", "int",
                [
                    new ScriptArgument("DaycareID", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("DaycareSlot", ScriptArgument.ArgumentTypes.Int)
                ],
                "Returns the Pokemon ID of a Pokemon in the Daycare.", ",", true));
            r(new ScriptCommand("Daycare", "PokemonName", "str",
                [
                    new ScriptArgument("DaycareID", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("DaycareSlot", ScriptArgument.ArgumentTypes.Int)
                ],
                "Returns the name of a Pokemon in the Daycare.", ",", true));
            r(new ScriptCommand("Daycare", "PokemonSprite", "str",
                [
                    new ScriptArgument("DaycareID", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("DaycareSlot", ScriptArgument.ArgumentTypes.Int)
                ],
                "Returns the sprite of a Pokemon in the Daycare.", ",", true));
            r(new ScriptCommand("Daycare", "ShinyIndicator", "str",
                [
                    new ScriptArgument("DaycareID", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("DaycareSlot", ScriptArgument.ArgumentTypes.Int)
                ],
                "Returns the Shiny Indicator of a Pokemon in the Daycare (either \"N\" or \"S\").", ",", true));
            r(new ScriptCommand("Daycare", "CountPokemon", "int",
                [new ScriptArgument("DaycareID", ScriptArgument.ArgumentTypes.Int)],
                "Returns the amount of Pokemon in the Daycare.", ",", true));
            r(new ScriptCommand("Daycare", "HasPokemon", "bool",
                [
                    new ScriptArgument("DaycareID", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("DaycareSlot", ScriptArgument.ArgumentTypes.Int)
                ],
                "Returns whether the given Daycare's slot is occupied.", ",", true));
            r(new ScriptCommand("Daycare", "CanSwim", "bool",
                [
                    new ScriptArgument("DaycareID", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("DaycareSlot", ScriptArgument.ArgumentTypes.Int)
                ],
                "Returns if the Pokemon in the Daycare can swim.", ",", true));
            r(new ScriptCommand("Daycare", "HasEgg", "bool",
                [new ScriptArgument("DaycareID", ScriptArgument.ArgumentTypes.Int)],
                "Returns if the Daycare has an Egg.", ",", true));
            r(new ScriptCommand("Daycare", "GrownLevels", "int",
                [
                    new ScriptArgument("DaycareID", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("DaycareSlot", ScriptArgument.ArgumentTypes.Int)
                ],
                "Returns the amount of levels the Pokemon has grown in the Daycare.", ",", true));
            r(new ScriptCommand("Daycare", "CurrentLevel", "int",
                [
                    new ScriptArgument("DaycareID", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("DaycareSlot", ScriptArgument.ArgumentTypes.Int)
                ],
                "Returns the current level of the Pokemon in the Daycare.", ",", true));
            r(new ScriptCommand("Daycare", "CanBreed", "int",
                [
                    new ScriptArgument("DaycareID", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("DaycareSlot", ScriptArgument.ArgumentTypes.Int)
                ],
                "Returns the chance the Pokemon in the Daycare can breed (in %).", ",", true));
        }

        private static void DoOverworldPokemon()
        {
            // Commands:
            r(new ScriptCommand("Player", "Show", "Shows the following Pokemon."));
            r(new ScriptCommand("Player", "Hide", "Hides the following Pokemon."));
            r(new ScriptCommand("Player", "Toggle", "Toggles the following Pokemon's visibility."));
        }

        private static void DoPokemon()
        {
            // Commands:
            r(new ScriptCommand("Pokemon", "Cry",
                [new ScriptArgument("PokemonID", ScriptArgument.ArgumentTypes.Str)],
                "Plays the cry of the given Pokemon."));
            r(new ScriptCommand("Pokemon", "Remove",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Removes the Pokemon at the given index from the player's party."));
            r(new ScriptCommand("Pokemon", "Add",
                [new ScriptArgument("PokemonData", ScriptArgument.ArgumentTypes.PokemonData)],
                "Adds the Pokemon to the player's party."));
            r(new ScriptCommand("Pokemon", "Add",
                [
                    new ScriptArgument("PokemonID", ScriptArgument.ArgumentTypes.Str),
                    new ScriptArgument("Level", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("CatchMethod", ScriptArgument.ArgumentTypes.Str, true, "Somehow obtained at"),
                    new ScriptArgument("BallID", ScriptArgument.ArgumentTypes.Str, true, "5"),
                    new ScriptArgument("Location", ScriptArgument.ArgumentTypes.Str, true, "Current Map's Name"),
                    new ScriptArgument("IsEgg", ScriptArgument.ArgumentTypes.Bool, true, "false"),
                    new ScriptArgument("OriginalTrainer", ScriptArgument.ArgumentTypes.Str, true, "<Player.Name>")
                ],
                "Adds the Pokemon with the given arguments to the player's party."));
            r(new ScriptCommand("Pokemon", "setadditionalvalue",
                [
                    new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("data", ScriptArgument.ArgumentTypes.Str)
                ],
                "Set the additional data for a Pokemon in the player's party."));
            r(new ScriptCommand("Pokemon", "setnickname",
                [
                    new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("nickName", ScriptArgument.ArgumentTypes.Str)
                ],
                "Set the nickname for a Pokemon in the player's party."));
            r(new ScriptCommand("Pokemon", "SetStat",
                [
                    new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("StatName", ScriptArgument.ArgumentTypes.Str, ["maxhp", "hp", "chp", "atk", "attack", "def", "defense", "spatk", "specialattack", "spattack", "spdef", "specialdefense", "spdefense", "speed"]),
                    new ScriptArgument("StatValue", ScriptArgument.ArgumentTypes.Int)
                ],
                "Set the value of a stat for a Pokemon in the player's party."));
            r(new ScriptCommand("Pokemon", "Clear", "Removes all Pokemon from the player's party."));
            r(new ScriptCommand("Pokemon", "RemoveAttack",
                [
                    new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("AttackIndex", ScriptArgument.ArgumentTypes.Int)
                ],
                "Removes the move at the given index from a Pokemon in the player's party."));
            r(new ScriptCommand("Pokemon", "RemoveAttackID",
                [
                    new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("AttackID", ScriptArgument.ArgumentTypes.Int)
                ],
                "Removes a move with the given ID from a Pokemon in the player's party if available."));
            r(new ScriptCommand("Pokemon", "clearattacks",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Clears all moves from a Pokemon in the player's party."));
            r(new ScriptCommand("Pokemon", "AddAttack",
                [
                    new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("AttackID", ScriptArgument.ArgumentTypes.Int)
                ],
                "Adds the move to a Pokemon in the player's party."));
            r(new ScriptCommand("Pokemon", "setshiny",
                [
                    new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("shiny", ScriptArgument.ArgumentTypes.Bool)
                ],
                "Sets the Shiny value of a Pokemon in the player's party."));
            r(new ScriptCommand("Pokemon", "setshinyall",
                [new ScriptArgument("shiny", ScriptArgument.ArgumentTypes.Bool)],
                "Sets the Shiny value of all Pokemon in the player's party."));
            r(new ScriptCommand("Pokemon", "ChangeLevel",
                [
                    new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("NewLevel", ScriptArgument.ArgumentTypes.Int)
                ],
                "Sets the level of a Pokemon in the player's party."));
            r(new ScriptCommand("Pokemon", "GainExp",
                [
                    new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("expAmount", ScriptArgument.ArgumentTypes.Int)
                ],
                "Adds Experience to the Experience value of a Pokemon in the player's party."));
            r(new ScriptCommand("Pokemon", "setnature",
                [
                    new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("natureID", ScriptArgument.ArgumentTypes.Int, ["0-24"])
                ],
                "Sets the Nature of a Pokemon in the player's party."));
            r(new ScriptCommand("Pokemon", "npctrade",
                [
                    new ScriptArgument("ownPokemonID(s)", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("oppPokemonID(s)", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("level", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("genderID", ScriptArgument.ArgumentTypes.Int, ["0, 1, 2, or -1 for same as own"]),
                    new ScriptArgument("attackIDs", ScriptArgument.ArgumentTypes.IntArr),
                    new ScriptArgument("shiny", ScriptArgument.ArgumentTypes.Bool),
                    new ScriptArgument("OT", ScriptArgument.ArgumentTypes.Str),
                    new ScriptArgument("TrainerName", ScriptArgument.ArgumentTypes.Str),
                    new ScriptArgument("CatchBallID", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("ItemID", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("location", ScriptArgument.ArgumentTypes.Str),
                    new ScriptArgument("method", ScriptArgument.ArgumentTypes.Str),
                    new ScriptArgument("nickname", ScriptArgument.ArgumentTypes.Str),
                    new ScriptArgument("message1", ScriptArgument.ArgumentTypes.Str, true, ""),
                    new ScriptArgument("message2", ScriptArgument.ArgumentTypes.Str, true, ""),
                    new ScriptArgument("register", ScriptArgument.ArgumentTypes.Str),
                    new ScriptArgument("messageAfterTrade", ScriptArgument.ArgumentTypes.Str, true, "")
                ],
                "Trades a Pokemon with an NPC. You can add multiple requested and offered Pokemon IDs in the first and second arguments separated by commas.", "|", false));
            r(new ScriptCommand("Pokemon", "rename",
                [
                    new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Str, ["0-5", "last"]),
                    new ScriptArgument("OTcheck", ScriptArgument.ArgumentTypes.Bool)
                ],
                "Opens the Name Rater rename feature."));
            r(new ScriptCommand("Pokemon", "read",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Str, ["[empty],0-5"])],
                "Displays the reader's dialogue."));
            r(new ScriptCommand("Pokemon", "Heal",
                [new ScriptArgument("PokemonIndicies", ScriptArgument.ArgumentTypes.IntArr, true, "")],
                "Heals the given Pokemon."));
            r(new ScriptCommand("Pokemon", "SetFriendship",
                [
                    new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Str),
                    new ScriptArgument("Friendship", ScriptArgument.ArgumentTypes.Int)
                ],
                "Sets the friendship value for a Pokemon in the player's party."));
            r(new ScriptCommand("Pokemon", "AddFriendship",
                [
                    new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Str),
                    new ScriptArgument("Amount", ScriptArgument.ArgumentTypes.Int)
                ],
                "Adds the given amount to the friendship value of a Pokemon in the player's party."));
            r(new ScriptCommand("Pokemon", "RemoveFriendship",
                [
                    new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Str),
                    new ScriptArgument("Amount", ScriptArgument.ArgumentTypes.Int)
                ],
                "Removes the given amount from the friendship value of a Pokemon in the player's party."));
            r(new ScriptCommand("Pokemon", "select",
                [
                    new ScriptArgument("canExit", ScriptArgument.ArgumentTypes.Bool, true, "false"),
                    new ScriptArgument("canChooseEgg", ScriptArgument.ArgumentTypes.Bool, true, "true"),
                    new ScriptArgument("canChooseFainted", ScriptArgument.ArgumentTypes.Bool, true, "true"),
                    new ScriptArgument("canlearnAttack", ScriptArgument.ArgumentTypes.Int, true, "-1")
                ],
                "Opens the Pokemon select screen. If canLearnAttack is set to an attack ID, it will be visible which Pokemon can learn that move."));
            r(new ScriptCommand("Pokemon", "selectmove",
                [
                    new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("canChooseHMMove", ScriptArgument.ArgumentTypes.Bool),
                    new ScriptArgument("canExit", ScriptArgument.ArgumentTypes.Bool)
                ],
                "Opens the Move Selection screen."));
            r(new ScriptCommand("Pokemon", "CalcStats",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Recalculates the stats for the given Pokemon."));
            r(new ScriptCommand("Pokemon", "LearnAttack",
                [
                    new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("AttackID", ScriptArgument.ArgumentTypes.Int)
                ],
                "Teaches the given attack to the given party member."));
            r(new ScriptCommand("Pokemon", "setgender",
                [
                    new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("genderID", ScriptArgument.ArgumentTypes.Int, ["0-2"])
                ],
                "Sets a Pokemon's gender."));
            r(new ScriptCommand("Pokemon", "setability",
                [
                    new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("abilityID", ScriptArgument.ArgumentTypes.Int, ["0-310"])
                ],
                "Sets the Ability of a Pokemon in the player's party."));
            r(new ScriptCommand("Pokemon", "setability",
                [
                    new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("abilitySlot", ScriptArgument.ArgumentTypes.Str, ["A,B,C,H"])
                ],
                "Sets the Ability of a Pokemon in the player's party to the desired Ability Slot (Normal = A or B, Event = C, Hidden = H)."));
            r(new ScriptCommand("Pokemon", "SetEV",
                [
                    new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("evStat", ScriptArgument.ArgumentTypes.Str, ["hp", "atk", "attack", "def", "defense", "spatk", "specialattack", "spattack", "spdef", "specialdefense", "spdefense", "speed"]),
                    new ScriptArgument("evValue", ScriptArgument.ArgumentTypes.Int)
                ],
                "Sets the value of the Effort Value stat of a Pokemon in the player's party."));
            r(new ScriptCommand("Pokemon", "SetAllEVs",
                [
                    new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("evValue", ScriptArgument.ArgumentTypes.Int)
                ],
                "Sets the value of all Effort Value stats of a Pokemon in the player's party."));
            r(new ScriptCommand("Pokemon", "AddEV",
                [
                    new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("evStat", ScriptArgument.ArgumentTypes.Str, ["hp", "atk", "attack", "def", "defense", "spatk", "specialattack", "spattack", "spdef", "specialdefense", "spdefense", "speed"]),
                    new ScriptArgument("evValue", ScriptArgument.ArgumentTypes.Int)
                ],
                "Adds the evValue argument to the Effort Value stat of a Pokemon in the player's party and makes sure it's legal."));
            r(new ScriptCommand("Pokemon", "SetIV",
                [
                    new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("ivStat", ScriptArgument.ArgumentTypes.Str, ["hp", "atk", "attack", "def", "defense", "spatk", "specialattack", "spattack", "spdef", "specialdefense", "spdefense", "speed"]),
                    new ScriptArgument("ivValue", ScriptArgument.ArgumentTypes.Int)
                ],
                "Sets the value of the Individual Value stat of a Pokemon in the player's party."));
            r(new ScriptCommand("Pokemon", "SetAllIVs",
                [
                    new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("ivStat", ScriptArgument.ArgumentTypes.Str, ["hp", "atk", "attack", "def", "defense", "spatk", "specialattack", "spattack", "spdef", "specialdefense", "spdefense", "speed"]),
                    new ScriptArgument("ivValue", ScriptArgument.ArgumentTypes.Int)
                ],
                "Sets the value of all Individual Value stats of a Pokemon in the player's party."));
            r(new ScriptCommand("Pokemon", "registerhalloffame", "Registers the current party as new Hall of Fame entry."));
            r(new ScriptCommand("Pokemon", "setot",
                [
                    new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("newOT", ScriptArgument.ArgumentTypes.Str)
                ],
                "Sets the Original Trainer of a Pokemon in the player's party."));
            r(new ScriptCommand("Pokemon", "setitem",
                [
                    new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("itemID", ScriptArgument.ArgumentTypes.Str)
                ],
                "Sets the item of a Pokemon in the player's party."));
            r(new ScriptCommand("Pokemon", "setitemData",
                [
                    new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("itemData", ScriptArgument.ArgumentTypes.Str)
                ],
                "Sets the data of the item of a Pokemon in the player's party."));
            r(new ScriptCommand("Pokemon", "setcatchtrainer",
                [
                    new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("catchTrainer", ScriptArgument.ArgumentTypes.Str)
                ],
                "Sets the Catch Trainer of a Pokemon in the player's party."));
            r(new ScriptCommand("Pokemon", "setcatchball",
                [
                    new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("ballID", ScriptArgument.ArgumentTypes.Int)
                ],
                "Sets the Catch Ball of a Pokemon in the player's party."));
            r(new ScriptCommand("Pokemon", "setcatchmethod",
                [
                    new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("method", ScriptArgument.ArgumentTypes.Str)
                ],
                "Sets the Catch Method of a Pokemon in the player's party."));
            r(new ScriptCommand("Pokemon", "setcatchplace",
                [
                    new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("location", ScriptArgument.ArgumentTypes.Str)
                ],
                "Sets the Catch Location of a Pokemon in the player's party."));
            r(new ScriptCommand("Pokemon", "setcatchlocation",
                [
                    new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("location", ScriptArgument.ArgumentTypes.Str)
                ],
                "Sets the Catch Location of a Pokemon in the player's party."));
            r(new ScriptCommand("Pokemon", "setstatus",
                [
                    new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("status", ScriptArgument.ArgumentTypes.Str, ["brn", "frz", "prz", "psn", "bpsn", "slp", "fnt"])
                ],
                "Sets the status of a Pokemon in the player's party. Setting that to \"fnt\" (Fainted) will also set the Pokemon's HP to 0."));
            r(new ScriptCommand("Pokemon", "newroaming",
                [
                    new ScriptArgument("roamerID", ScriptArgument.ArgumentTypes.Str),
                    new ScriptArgument("PokemonID", ScriptArgument.ArgumentTypes.Str),
                    new ScriptArgument("level", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("regionID", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("startMap", ScriptArgument.ArgumentTypes.Str),
                    new ScriptArgument("shiny", ScriptArgument.ArgumentTypes.Bool, true, "-1"),
                    new ScriptArgument("scriptPath", ScriptArgument.ArgumentTypes.Str, true)
                ],
                "Adds a new Roaming Pokemon to the list of Roaming Pokemon.", "|", false));
            r(new ScriptCommand("Pokemon", "Evolve",
                [
                    new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("EvolutionTrigger", ScriptArgument.ArgumentTypes.Str, ["level", "none", "item", "trade"], true, "level"),
                    new ScriptArgument("EvolutionArgument", ScriptArgument.ArgumentTypes.Str, true, "")
                ],
                "Tries to evolve a Pokemon with the given conditions."));
            r(new ScriptCommand("Pokemon", "levelup",
                [
                    new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("levelAmount", ScriptArgument.ArgumentTypes.Int, ["[empty],0 - MaxLevel GameRule"], true, "1")
                ],
                "Raises a Pokemon's level by the given amount, checks for learnable level up moves."));
            r(new ScriptCommand("Pokemon", "reload",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Reloads the data for a Pokemon in the player's party to apply changes."));
            r(new ScriptCommand("Pokemon", "clone",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Clones the given Pokemon in the player's party."));
            r(new ScriptCommand("Pokemon", "sendtostorage",
                [
                    new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("boxIndex", ScriptArgument.ArgumentTypes.Int, true, "")
                ],
                "Sends the given Pokemon to the storage system, in the specified box if given."));
            r(new ScriptCommand("Pokemon", "addtostorage",
                [
                    new ScriptArgument("boxIndex", ScriptArgument.ArgumentTypes.Int, true, ""),
                    new ScriptArgument("PokemonData", ScriptArgument.ArgumentTypes.PokemonData)
                ],
                "Adds a Pokemon with the given Pokemon data to the storage system, in the specified box if given."));
            r(new ScriptCommand("Pokemon", "addtostorage",
                [
                    new ScriptArgument("PokemonID", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("level", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("method", ScriptArgument.ArgumentTypes.Str, true, "random reason"),
                    new ScriptArgument("ballID", ScriptArgument.ArgumentTypes.Int, true, "5"),
                    new ScriptArgument("location", ScriptArgument.ArgumentTypes.Str, true, "Current location"),
                    new ScriptArgument("isEgg", ScriptArgument.ArgumentTypes.Bool, true, "false"),
                    new ScriptArgument("trainerName", ScriptArgument.ArgumentTypes.Str, true, "Current TrainerName"),
                    new ScriptArgument("heldItemID", ScriptArgument.ArgumentTypes.Int, true, "0"),
                    new ScriptArgument("isShiny", ScriptArgument.ArgumentTypes.Bool, true, "false")
                ],
                "Adds a Pokemon with the given Pokemon properties to the storage system."));
            r(new ScriptCommand("Pokemon", "ride",
                [new ScriptArgument("PokemonID", ScriptArgument.ArgumentTypes.Int, true, "-1")],
                "Makes a Pokemon in the player's party use the field move Ride. If the argument is left empty, the first Pokemon who knows Ride gets selected."));

            // Constructs:
            r(new ScriptCommand("Pokemon", "id", "int",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns the ID of a Pokemon in the player's party.", ",", true));
            r(new ScriptCommand("Pokemon", "number", "int",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns the ID of a Pokemon in the player's party.", ",", true));
            r(new ScriptCommand("Pokemon", "data", "PokemonData",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns the save data for a Pokemon in the player's party.", ",", true));
            r(new ScriptCommand("Pokemon", "gender", "int",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns the gender for a Pokemon in the player's party.", ",", true));
            r(new ScriptCommand("Pokemon", "genderchance", "int",
                [new ScriptArgument("PokemonID", ScriptArgument.ArgumentTypes.Str)],
                "Returns the Male/Female chance (1-100) of a Pokemon as defined by its Data file.", ",", true));
            r(new ScriptCommand("Pokemon", "level", "int",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns the level of a Pokemon in the player's party.", ",", true));
            r(new ScriptCommand("Pokemon", "hasfullhp", "bool",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns if a Pokemon in the player's party has a full Hit Point count.", ",", true));
            r(new ScriptCommand("Pokemon", "hp", "int",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns the Hit Points of a Pokemon in the player's party.", ",", true));
            r(new ScriptCommand("Pokemon", "atk", "int",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns the Attack stat of a Pokemon in the player's party.", ",", true));
            r(new ScriptCommand("Pokemon", "def", "int",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns the Defense stat of a Pokemon in the player's party.", ",", true));
            r(new ScriptCommand("Pokemon", "spatk", "int",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns the Special Attack stat of a Pokemon in the player's party.", ",", true));
            r(new ScriptCommand("Pokemon", "spdef", "int",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns the Special Defense stat of a Pokemon in the player's party.", ",", true));
            r(new ScriptCommand("Pokemon", "speed", "int",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns the Speed stat of a Pokemon in the player's party.", ",", true));
            r(new ScriptCommand("Pokemon", "maxhp", "int",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns the Maximum Hit Points of a Pokemon in the player's party.", ",", true));
            r(new ScriptCommand("Pokemon", "isegg", "bool",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns if the Pokemon in the player's party is an Egg.", ",", true));
            r(new ScriptCommand("Pokemon", "additionaldata", "str",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns the additional data for the Pokemon in the player's party.", ",", true));
            r(new ScriptCommand("Pokemon", "mailsendername", "str",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns the name of the sender of a mail if there's one on the Pokemon in the player's party.", ",", true));
            r(new ScriptCommand("Pokemon", "mailsenderot", "str",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns the OT of the sender of a mail if there's one on the Pokemon in the player's party.", ",", true));
            r(new ScriptCommand("Pokemon", "nickname", "str",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns the nickname of a Pokemon in the player's party or \"Egg\" if it's in an egg.", ",", true));
            r(new ScriptCommand("Pokemon", "hasnickname", "bool",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns if a Pokemon in the player's party has a nickname.", ",", true));
            r(new ScriptCommand("Pokemon", "name", "str",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns the actual name of a Pokemon in the player's party, regardless if the Pokemon has a nickname or is in an egg (including form prefixes and suffixes).", ",", true));
            r(new ScriptCommand("Pokemon", "originalname", "str",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns the English name of a Pokemon in the player's party (including form prefixes and suffixes).", ",", true));
            r(new ScriptCommand("Pokemon", "ot", "str",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns the Original Trainer of a Pokemon in the player's party.", ",", true));
            r(new ScriptCommand("Pokemon", "trainer", "str",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns the trainer of a Pokemon in the player's party.", ",", true));
            r(new ScriptCommand("Pokemon", "itemid", "int",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns the ID of the item of a Pokemon in the player's party.", ",", true));
            r(new ScriptCommand("Pokemon", "friendship", "int",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns the friendship value of a Pokemon in the player's party.", ",", true));
            r(new ScriptCommand("Pokemon", "itemname", "str",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns the item name of the item of a Pokemon in the player's party.", ",", true));
            r(new ScriptCommand("Pokemon", "catchball", "int",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns the ID of the Poke Ball the Pokemon was caught in.", ",", true));
            r(new ScriptCommand("Pokemon", "catchmethod", "str",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns the method the Pokemon was caught.", ",", true));
            r(new ScriptCommand("Pokemon", "catchlocation", "str",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns the location the Pokemon was caught in.", ",", true));
            r(new ScriptCommand("Pokemon", "ability", "int",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns the Ability ID of the given Pokemon.", ",", true));
            r(new ScriptCommand("Pokemon", "abilityslot", "str",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns the Ability Slot of the Ability of the given Pokemon (A, B, C or H). Returns the Ability ID of the Pokemon if it has an Ability not defined in its data file.", ",", true));
            r(new ScriptCommand("Pokemon", "hasattack", "bool",
                [
                    new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("attackID", ScriptArgument.ArgumentTypes.Int)
                ],
                "Returns if the Pokemon in the player's party knows the specified move.", ",", true));
            r(new ScriptCommand("Pokemon", "countattacks", "int",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Counts the moves the Pokemon knows.", ",", true));
            r(new ScriptCommand("Pokemon", "attackname", "str",
                [
                    new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("moveIndex", ScriptArgument.ArgumentTypes.Int)
                ],
                "Returns the name of the move of a Pokemon in the player's party.", ",", true));
            r(new ScriptCommand("Pokemon", "levelattacks", "str",
                [
                    new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("maxLevel", ScriptArgument.ArgumentTypes.Int, true, "-1")
                ],
                "Returns a list of move IDs separated by commas that a Pokemon in the player's party can learn at or below its current level/the level specified by the maxLevel argument.", ",", true));
            r(new ScriptCommand("Pokemon", "canlearnattack", "bool",
                [
                    new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("attackID", ScriptArgument.ArgumentTypes.Int)
                ],
                "Returns if the Pokemon can learn the specified move.", ",", true));
            r(new ScriptCommand("Pokemon", "isshiny", "bool",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns if the Pokemon is Shiny.", ",", true));
            r(new ScriptCommand("Pokemon", "nature", "str",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns the nature of a Pokemon in the player's party.", ",", true));
            r(new ScriptCommand("Pokemon", "ownPokemon", "bool",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns if a Pokemon in the player's party was caught by the player.", ",", true));
            r(new ScriptCommand("Pokemon", "islegendary", "bool",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns if a Pokemon in the player's party is a legendary Pokemon.", ",", true));
            r(new ScriptCommand("Pokemon", "freeplaceinparty", "bool", "Checks if the player has a free place in their party.", ",", true));
            r(new ScriptCommand("Pokemon", "noPokemon", "bool", "Checks if the player has no Pokemon in their party.", ",", true));
            r(new ScriptCommand("Pokemon", "count", "int", "Returns the amount of Pokemon in the player's party.", ",", true));
            r(new ScriptCommand("Pokemon", "countbattle", "int", "Returns the amount of Pokemon that can battle in the player's party.", ",", true));
            r(new ScriptCommand("Pokemon", "has", "bool",
                [new ScriptArgument("PokemonID", ScriptArgument.ArgumentTypes.Int)],
                "Returns if the player has the specified Pokemon in their party.", ",", true));
            r(new ScriptCommand("Pokemon", "selected", "int", "Returns the index of the selector in the player's party. (Set with @pokemon.select)", ",", true));
            r(new ScriptCommand("Pokemon", "selectedmove", "int", "Returns the index of the move selected. (Set with @pokemon.selectmove)", ",", true));
            r(new ScriptCommand("Pokemon", "hasegg", "bool", "Returns if the player has an Egg in their party.", ",", true));
            r(new ScriptCommand("Pokemon", "maxpartylevel", "int", "Returns the maximum level a Pokemon has in the player's party.", ",", true));
            r(new ScriptCommand("Pokemon", "evhp", "int",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns the Hit Point Effort Values of a Pokemon in the player's party.", ",", true));
            r(new ScriptCommand("Pokemon", "evatk", "int",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns the Attack Effort Values of a Pokemon in the player's party.", ",", true));
            r(new ScriptCommand("Pokemon", "evdef", "int",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns the Defense Effort Values of a Pokemon in the player's party.", ",", true));
            r(new ScriptCommand("Pokemon", "evspatk", "int",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns the Special Attack Effort Values of a Pokemon in the player's party.", ",", true));
            r(new ScriptCommand("Pokemon", "evspdef", "int",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns the Special Defense Effort Values of a Pokemon in the player's party.", ",", true));
            r(new ScriptCommand("Pokemon", "evspeed", "int",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns the Speed Effort Values of a Pokemon in the player's party.", ",", true));
            r(new ScriptCommand("Pokemon", "ivhp", "int",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns the Hit Point Individual Values of a Pokemon in the player's party.", ",", true));
            r(new ScriptCommand("Pokemon", "ivatk", "int",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns the Attack Individual Values of a Pokemon in the player's party.", ",", true));
            r(new ScriptCommand("Pokemon", "ivdef", "int",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns the Defense Individual Values of a Pokemon in the player's party.", ",", true));
            r(new ScriptCommand("Pokemon", "ivspatk", "int",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns the Special Attack Individual Values of a Pokemon in the player's party.", ",", true));
            r(new ScriptCommand("Pokemon", "ivspdef", "int",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns the Special Defense Individual Values of a Pokemon in the player's party.", ",", true));
            r(new ScriptCommand("Pokemon", "spawnwild", "PokemonData", "Returns the data for a Pokemon that can spawn in the current location.", ",", true));
            r(new ScriptCommand("Pokemon", "itemdata", "str",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns the data of the item of a Pokemon in the player's party.", ",", true));
            r(new ScriptCommand("Pokemon", "countHallofFame", "int", "Counts the Hall of Fame entries.", ",", true));
            r(new ScriptCommand("Pokemon", "learnedTutorMove", "bool", "Returns if a Pokemon just learned a tutor move (from @screen.teachmoves)", ",", true));
            r(new ScriptCommand("Pokemon", "totalexp", "int",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns the amount of Experience a Pokemon received.", ",", true));
            r(new ScriptCommand("Pokemon", "needexp", "int",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns the amount of Experience a Pokemon needs in order to level up.", ",", true));
            r(new ScriptCommand("Pokemon", "currentexp", "int",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns the amount of Experience the Pokemon collected for its current level.", ",", true));
            r(new ScriptCommand("Pokemon", "generateFrontier", "PokemonData",
                [
                    new ScriptArgument("level", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("PokemonClass", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("IDList", ScriptArgument.ArgumentTypes.IntArr, true, "")
                ],
                "Generates a Frontier Pokemon within the set IDList (all Pokemon, if IDList is empty).", ",", true));
            r(new ScriptCommand("Pokemon", "spawn", "PokemonData",
                [
                    new ScriptArgument("PokemonID", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("level", ScriptArgument.ArgumentTypes.Int)
                ],
                "Returns the data for a Pokemon.", ",", true));
            r(new ScriptCommand("Pokemon", "otmatch", "bool,int,str",
                [
                    new ScriptArgument("checkOT", ScriptArgument.ArgumentTypes.Str),
                    new ScriptArgument("returnType", ScriptArgument.ArgumentTypes.Str, ["has", "id", "number", "name", "maxhits"])
                ],
                "Returns if the player owns a Pokemon with the given Original Trainer.", ",", true));
            r(new ScriptCommand("Pokemon", "randomot", "str", "Returns a random OT (5 digit number).", ",", true));
            r(new ScriptCommand("Pokemon", "status", "str",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns the status condition of a Pokemon in the player's party.", ",", true));
            r(new ScriptCommand("Pokemon", "canevolve",
                [
                    new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("evolutionTrigger", ScriptArgument.ArgumentTypes.Str, ["level", "none", "item", "trade"], true, "level"),
                    new ScriptArgument("evolutionArgument", ScriptArgument.ArgumentTypes.Str, true, "")
                ],
                "Returns if the Pokemon can be evolved via the given evolution method.", ",", true));
            r(new ScriptCommand("Pokemon", "type1", "str",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns the first type of a Pokemon in the player's party.", ",", true));
            r(new ScriptCommand("Pokemon", "type2", "str",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int)],
                "Returns the second type of a Pokemon in the player's party.", ",", true));
            r(new ScriptCommand("Pokemon", "istype", "bool",
                [
                    new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Int),
                    new ScriptArgument("type", ScriptArgument.ArgumentTypes.Str)
                ],
                "Checks if a Pokemon in the player's party has a specific type.", ",", true));
            r(new ScriptCommand("Pokemon", "isroaming", "bool",
                [new ScriptArgument("roamerID", ScriptArgument.ArgumentTypes.Str)],
                "Checks if the given roaming Pokemon is still active.", ",", true));
            r(new ScriptCommand("Pokemon", "fullyhealed", "bool",
                [new ScriptArgument("PokemonIndex", ScriptArgument.ArgumentTypes.Str, true, "")],
                "Checks if a specific Pokemon or all Pokemon in the party are fully healed.", ",", true));
        }

        // ---- GetHelpContent ----

        /// <summary>
        /// Gets the help content for a script command or construct.
        /// </summary>
        /// <param name="inputCommand">class.subclass</param>
        public static String GetHelpContent(String inputCommand, int pageSize)
        {
            if (inputCommand.ToLower().StartsWith("constructs") == true)
            {
                List<String> list = [];
                foreach (ScriptCommand sc in _scripts)
                {
                    if (sc.IsConstruct == true)
                    {
                        if (list.Contains(sc.MainClass.ToLower()) == false)
                        {
                            list.Add(sc.MainClass.ToLower());
                        }
                    }
                }
                String str = String.Empty;
                List<String> cList = [];
                foreach (String l in list)
                {
                    if (cList.Contains(l.ToLower()) == false)
                    {
                        if (str.Equals("") == false)
                        {
                            str += "; ";
                        }
                        str += l;
                        cList.Add(l.ToLower());
                    }
                }
                return "Constructs: " + str;
            }
            else if (inputCommand.ToLower().StartsWith("commands") == true)
            {
                List<String> list = [];
                foreach (ScriptCommand sc in _scripts)
                {
                    if (sc.IsConstruct == false)
                    {
                        if (list.Contains(sc.MainClass.ToLower()) == false)
                        {
                            list.Add(sc.MainClass.ToLower());
                        }
                    }
                }
                String str = String.Empty;
                List<String> cList = [];
                foreach (String l in list)
                {
                    if (cList.Contains(l.ToLower()) == false)
                    {
                        if (str.Equals("") == false)
                        {
                            str += "; ";
                        }
                        str += l;
                        cList.Add(l.ToLower());
                    }
                }
                return "Commands: " + str;
            }
            else if (inputCommand.Equals("") == true)
            {
                return "Type \"constructs\" or \"commands\" to view the list of main classes, or type a command or construct to view its details. Put a \",\" afterwards, to view different pages.";
            }

            if (inputCommand.Contains(".") == true)
            {
                String mainClass = inputCommand.Remove(inputCommand.IndexOf('.'));
                String subClass = inputCommand.Remove(0, inputCommand.IndexOf('.') + 1);

                int count = 1;
                int selected = 1;

                if (subClass.Contains(",") == true)
                {
                    if (StringHelper.IsNumeric(subClass.GetSplit(1)) == true)
                    {
                        selected = (int)ScriptConversion.ToInteger(subClass.GetSplit(1));
                        subClass = subClass.GetSplit(0);
                    }
                    else
                    {
                        Logger.Log(Logger.LogTypes.Warning, "ScriptLibrary.cs: The \"Selected\" argument has to be numeric.");
                    }
                }

                List<ScriptCommand> validScriptCommands = [];
                foreach (ScriptCommand sc in _scripts)
                {
                    if (sc.MatchesClass(mainClass, subClass) == true)
                    {
                        validScriptCommands.Add(sc);
                    }
                }

                if (validScriptCommands.Count > 0)
                {
                    count = validScriptCommands.Count;
                    selected = Math.Clamp(selected, 1, count);

                    String s = validScriptCommands[selected - 1].ToString();

                    if (count > 1)
                    {
                        s = $"({selected}/{count}) {s}";
                    }
                    return s;
                }
                else
                {
                    return $"No help content available for \"{mainClass}.{subClass}\".";
                }
            }
            else
            {
                String mainClass = inputCommand;
                int page = 1;

                if (mainClass.Contains(",") == true)
                {
                    if (StringHelper.IsNumeric(mainClass.GetSplit(1)) == true)
                    {
                        page = (int)ScriptConversion.ToInteger(mainClass.GetSplit(1));
                        mainClass = mainClass.GetSplit(0);
                    }
                    else
                    {
                        Logger.Log(Logger.LogTypes.Warning, "ScriptLibrary.cs: The \"Page\" argument has to be numeric.");
                    }
                }

                List<ScriptCommand> validScriptCommands = [];
                foreach (ScriptCommand sc in _scripts)
                {
                    if (sc.MainClass.ToLower().Equals(mainClass.ToLower()) == true)
                    {
                        validScriptCommands.Add(sc);
                    }
                }

                int pageCount = (int)Math.Ceiling((double)validScriptCommands.Count / pageSize);
                page = Math.Clamp(page, 1, Math.Max(pageCount, 1));

                if (validScriptCommands.Count > 0)
                {
                    String str = String.Empty;
                    for (int i = (page - 1) * pageSize; i <= page * pageSize; i++)
                    {
                        if (i <= validScriptCommands.Count - 1)
                        {
                            ScriptCommand sc = validScriptCommands[i];
                            String s = String.Empty;

                            if (sc.IsConstruct == true)
                            {
                                s = $"<{sc.MainClass}.{sc.SubClass}>";
                            }
                            else
                            {
                                s = $"@{sc.MainClass}.{sc.SubClass}";
                            }

                            if (str.Contains(s) == false)
                            {
                                if (str.Equals("") == false)
                                {
                                    str += "; ";
                                }
                                str += s;
                            }
                        }
                    }

                    if (pageCount > 1)
                    {
                        return $"{mainClass}: ({page}/{pageCount}) {str}";
                    }
                    else
                    {
                        return $"{mainClass}: {str}";
                    }
                }
                else
                {
                    return $"No Commands or Constructs available for \"{mainClass}\".";
                }
            }
        }
    }
}
