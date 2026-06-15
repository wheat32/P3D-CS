using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D;

public class LevelLoader
{
    private const bool MULTITHREAD = false;

    public static List<Vector3> LoadedOffsetMapOffsets { get; } = [];
    public static List<String> LoadedOffsetMapNames { get; } = [];
    private bool _reload = false;

    private enum TagTypes
    {
        Entity,
        Floor,
        EntityField,
        Level,
        LevelActions,
        NPC,
        Shader,
        OffsetMap,
        Structure,
        Backdrop,
        None
    }

    private Vector3 _offset;
    private bool _loadOffsetMap = true;
    private int _offsetMapLevel = 0;
    private String _mapOrigin = String.Empty;
    private List<String> _sessionMapsLoaded = [];

    private static int _busy = 0;

    public static bool IsBusy => _busy > 0;

    private Object[]? _tempParams;

    // --------------------------------------------------------------------------
    // File loading
    // --------------------------------------------------------------------------

    public void LoadLevel(Object[] parameters, bool reload = false)
    {
        _busy += 1;
        _tempParams = parameters;
        _reload = reload;
        if (MULTITHREAD == true)
        {
            System.Threading.Thread t = new System.Threading.Thread(InternalLoad);
            t.IsBackground = true;
            t.Start();
        }
        else
        {
            InternalLoad();
        }
    }

    private void InternalLoad()
    {
        String levelPath = (String)_tempParams![0];
        bool loadOffsetMap = (bool)_tempParams[1];
        Vector3 offset = (Vector3)_tempParams[2];
        _offsetMapLevel = (int)_tempParams[3];
        _sessionMapsLoaded = (List<String>)_tempParams[4];

        Stopwatch timer = new Stopwatch();
        timer.Start();

        _loadOffsetMap = loadOffsetMap;
        _mapOrigin = levelPath;

        if (loadOffsetMap == false)
        {
            Screen.Level.LevelFile = levelPath;

            Core.Player.LastSavePlace = Screen.Level.LevelFile;
            Core.Player.LastSavePlacePosition = Player.Temp.LastPosition.X + "," +
                Player.Temp.LastPosition.Y.ToString().Replace(GameController.DecSeparator, ".") + "," +
                Player.Temp.LastPosition.Z;

            Core.OffsetMaps.Clear();
            Screen.Level.Entities.Clear();
            Screen.Level.Floors.Clear();
            Screen.Level.Shaders.Clear();
            Screen.Level.BackdropRenderer.Clear();

            Screen.Level.OffsetmapFloors.Clear();
            Screen.Level.OffsetmapEntities.Clear();

            Screen.Level.WildPokemonFloor = false;
            Screen.Level.WalkedSteps = 0;

            LoadedOffsetMapNames.Clear();
            LoadedOffsetMapOffsets.Clear();
            Floor.ClearFloorTemp();

            Player.Temp.MapSteps = 0;

            _sessionMapsLoaded.Add(levelPath);
        }

        levelPath = GameModeManager.GetMapPath(levelPath);
        Logger.Debug("Loading map: " + levelPath.Remove(0, GameController.GamePath.Length));
        Security.FileValidation.CheckFileValid(levelPath, false, "LevelLoader.cs");

        if (System.IO.File.Exists(levelPath) == false)
        {
            Logger.Log(Logger.LogTypes.ErrorMessage,
                "LevelLoader.cs: Error accessing map file \"" + levelPath + "\". File not found.");
            _busy -= 1;

            if (Core.CurrentScreen.Identification == Screen.Identifications.OverworldScreen &&
                loadOffsetMap == false)
            {
                ((OverworldScreen)Core.CurrentScreen).Titles.Add(
                    new OverworldScreen.Title("Couldn't find map file!", 20.0f, Microsoft.Xna.Framework.Color.White, 6.0f, Microsoft.Xna.Framework.Vector2.Zero, true));
            }

            return;
        }

        List<String> data = [.. System.IO.File.ReadAllLines(levelPath)];
        Dictionary<String, Object> tags = [];

        _offset = offset;

        foreach (String rawLine in data)
        {
            String commentLine = rawLine;
            if (commentLine.Contains("{") == true)
            {
                commentLine = commentLine.Remove(0, commentLine.IndexOf("{"));

                if (commentLine.StartsWith("{\"Comment\"{COM") == true)
                {
                    commentLine = commentLine.Remove(0, commentLine.IndexOf("[") + 1);
                    commentLine = commentLine.Remove(commentLine.IndexOf("]"));

                    Logger.Log(Logger.LogTypes.Debug, commentLine);
                }
            }
        }

        int countLines = 0;

        for (int i = 0; i < int.MaxValue; i++)
        {
            if (i > data.Count - 1)
            {
                break;
            }

            String line = data[i];
            tags.Clear();
            if (line.Contains("{") == true && line.Contains("}") == true)
            {
                try
                {
                    TagTypes tagType = TagTypes.None;
                    line = line.Remove(0, line.IndexOf("{") + 2);

                    if (line.ToLower().StartsWith("structure\"") == true)
                    {
                        tagType = TagTypes.Structure;
                    }

                    if (tagType == TagTypes.Structure)
                    {
                        line = line.Remove(0, line.IndexOf("[") + 1);
                        line = line.Remove(line.Length - 3, 3);

                        tags = GetTags(line);

                        String[] newLines = AddStructure(tags);

                        data.InsertRange(i + 1, newLines);
                    }
                }
                catch (System.Exception ex)
                {
                    Logger.Log(Logger.LogTypes.Warning,
                        "LevelLoader.cs: Failed to load map object! (Index: " + countLines + ") from mapfile: " + levelPath + "; Error message: " + ex.Message);
                }
            }
        }

        foreach (String rawLine in data)
        {
            tags.Clear();
            String orgLine = rawLine;
            String line = rawLine;
            countLines += 1;

            if (line.Contains("{") == true && line.Contains("}") == true)
            {
                try
                {
                    TagTypes tagType = TagTypes.None;
                    line = line.Remove(0, line.IndexOf("{") + 2);

                    if (line.ToLower().StartsWith("entity\"") == true)
                        tagType = TagTypes.Entity;
                    else if (line.ToLower().StartsWith("floor\"") == true)
                        tagType = TagTypes.Floor;
                    else if (line.ToLower().StartsWith("entityfield\"") == true)
                        tagType = TagTypes.EntityField;
                    else if (line.ToLower().StartsWith("level\"") == true)
                        tagType = TagTypes.Level;
                    else if (line.ToLower().StartsWith("actions\"") == true)
                        tagType = TagTypes.LevelActions;
                    else if (line.ToLower().StartsWith("npc\"") == true)
                        tagType = TagTypes.NPC;
                    else if (line.ToLower().StartsWith("shader\"") == true)
                        tagType = TagTypes.Shader;
                    else if (line.ToLower().StartsWith("offsetmap\"") == true)
                        tagType = TagTypes.OffsetMap;
                    else if (line.ToLower().StartsWith("backdrop\"") == true)
                        tagType = TagTypes.Backdrop;

                    if (tagType != TagTypes.None)
                    {
                        line = line.Remove(0, line.IndexOf("[") + 1);
                        line = line.Remove(line.Length - 3, 3);

                        tags = GetTags(line);

                        switch (tagType)
                        {
                            case TagTypes.EntityField:
                                EntityField(tags);
                                break;
                            case TagTypes.Entity:
                                AddEntity(tags, new Size(1, 1), 1, true, new Vector3(1, 1, 1));
                                break;
                            case TagTypes.Floor:
                                AddFloor(tags, loadOffsetMap);
                                break;
                            case TagTypes.Level:
                                if (loadOffsetMap == false)
                                {
                                    SetupLevel(tags);
                                }
                                break;
                            case TagTypes.LevelActions:
                                if (loadOffsetMap == false)
                                {
                                    SetupActions(tags);
                                }
                                break;
                            case TagTypes.NPC:
                                AddNPC(tags);
                                break;
                            case TagTypes.Shader:
                                AddShader(tags);
                                break;
                            case TagTypes.OffsetMap:
                                if (loadOffsetMap == false ||
                                    _offsetMapLevel <= Core.GameOptions.MaxOffsetLevel)
                                {
                                    AddOffsetMap(tags);
                                }
                                break;
                            case TagTypes.Backdrop:
                                if (loadOffsetMap == false)
                                {
                                    AddBackdrop(tags);
                                }
                                break;
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Logger.Log(Logger.LogTypes.Warning,
                        "LevelLoader.cs: Failed to load map object! (Index: " + countLines + ") (Line: " + orgLine + ") from mapfile: " + levelPath + "; Error message: " + ex.Message);
                }
            }
        }

        if (loadOffsetMap == false)
        {
            LoadBerries();
        }

        foreach (Shader s in Screen.Level.Shaders)
        {
            if (s.HasBeenApplied == false)
            {
                s.ApplyShader(Screen.Level.Entities.ToArray());
                s.ApplyShader(Screen.Level.Floors.ToArray());
                s.ApplyShader(Screen.Level.OffsetmapEntities.ToArray());
                s.ApplyShader(Screen.Level.OffsetmapFloors.ToArray());
            }
        }

        Logger.Debug("Map loading finished: " + levelPath.Remove(0, GameController.GamePath.Length));
        Logger.Debug("Loaded textures: " + TextureManager.TextureList.Count.ToString());
        timer.Stop();
        Logger.Debug("Map loading time: " + timer.ElapsedTicks + " Ticks; " + timer.ElapsedMilliseconds + " Milliseconds.");

        _busy -= 1;

        if (_busy == 0)
        {
            Screen.Level.StartOffsetMapUpdate();
        }
    }

    // --------------------------------------------------------------------------
    // Tag parsing
    // --------------------------------------------------------------------------

    private Dictionary<String, Object> GetTags(String line)
    {
        Dictionary<String, Object> tags = [];

        String[] tagList = line.Split(["}{"], System.StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < tagList.Length; i++)
        {
            String currentTag = tagList[i];
            if (currentTag.EndsWith("}}") == false)
            {
                currentTag += "}";
            }
            if (currentTag.StartsWith("{") == false)
            {
                currentTag = "{" + currentTag;
            }
            ProcessTag(tags, currentTag);
        }

        return tags;
    }

    private void ProcessTag(Dictionary<String, Object> dictionary, String tag)
    {
        String tagName;
        String tagContent;

        tag = tag.Remove(0, 1);
        tag = tag.Remove(tag.Length - 1, 1);

        tagName = tag.Remove(tag.IndexOf("{") - 1).Remove(0, 1);
        tagContent = tag.Remove(0, tag.IndexOf("{"));

        String[] contentRows = tagContent.Split('}');
        foreach (String rawSubTag in contentRows)
        {
            String subTag = rawSubTag;
            if (subTag.Length > 0)
            {
                subTag = subTag.Remove(0, 1);

                String subTagType = subTag.Remove(subTag.IndexOf("["));
                String subTagValue = subTag.Remove(0, subTag.IndexOf("[") + 1);
                subTagValue = subTagValue.Remove(subTagValue.Length - 1, 1);

                switch (subTagType.ToLower())
                {
                    case "int":
                        dictionary.Add(tagName, int.Parse(subTagValue));
                        break;
                    case "str":
                        dictionary.Add(tagName, subTagValue);
                        break;
                    case "sng":
                        subTagValue = subTagValue.Replace(".", GameController.DecSeparator);
                        dictionary.Add(tagName, float.Parse(subTagValue));
                        break;
                    case "bool":
                        dictionary.Add(tagName, subTagValue == "1" || (subTagValue != "0" && bool.Parse(subTagValue)));
                        break;
                    case "intarr":
                    {
                        String[] values = subTagValue.Split(',');
                        List<int> arr = [];
                        foreach (String value in values)
                        {
                            arr.Add(int.Parse(value));
                        }
                        dictionary.Add(tagName, arr);
                        break;
                    }
                    case "intarr2d":
                    {
                        String[] rows = subTagValue.Split(']');
                        List<List<int>> arr = [];
                        foreach (String rawRow in rows)
                        {
                            String row = rawRow;
                            if (row.Length > 0)
                            {
                                row = row.Remove(0, 1);
                                List<int> list = [];
                                foreach (String value in row.Split(','))
                                {
                                    list.Add(int.Parse(value));
                                }
                                arr.Add(list);
                            }
                        }
                        dictionary.Add(tagName, arr);
                        break;
                    }
                    case "rec":
                    {
                        String[] content = subTagValue.Split(',');
                        dictionary.Add(tagName, new Microsoft.Xna.Framework.Rectangle(
                            int.Parse(content[0]), int.Parse(content[1]),
                            int.Parse(content[2]), int.Parse(content[3])));
                        break;
                    }
                    case "recarr":
                    {
                        String[] values = subTagValue.Split(']');
                        List<Microsoft.Xna.Framework.Rectangle> arr = [];
                        foreach (String rawValue in values)
                        {
                            String value = rawValue;
                            if (value.Length > 0)
                            {
                                value = value.Remove(0, 1);
                                String[] content = value.Split(',');
                                arr.Add(new Microsoft.Xna.Framework.Rectangle(
                                    int.Parse(content[0]), int.Parse(content[1]),
                                    int.Parse(content[2]), int.Parse(content[3])));
                            }
                        }
                        dictionary.Add(tagName, arr);
                        break;
                    }
                    case "sngarr":
                    {
                        String[] values = subTagValue.Split(',');
                        List<float> arr = [];
                        foreach (String rawValue in values)
                        {
                            String value = rawValue.Replace(".", GameController.DecSeparator);
                            arr.Add(float.Parse(value));
                        }
                        dictionary.Add(tagName, arr);
                        break;
                    }
                }
            }
        }
    }

    private Object? GetTag(Dictionary<String, Object> tags, String tagName)
    {
        if (tags.ContainsKey(tagName) == true)
        {
            return tags[tagName];
        }

        foreach (System.Collections.Generic.KeyValuePair<String, Object> pair in tags)
        {
            if (pair.Key.ToLower().Equals(tagName.ToLower()) == true)
            {
                return pair.Value;
            }
        }

        return null;
    }

    private bool TagExists(Dictionary<String, Object> tags, String tagName)
    {
        if (tags.ContainsKey(tagName) == true)
        {
            return true;
        }

        foreach (System.Collections.Generic.KeyValuePair<String, Object> pair in tags)
        {
            if (pair.Key.ToLower().Equals(tagName.ToLower()) == true)
            {
                return true;
            }
        }

        return false;
    }

    // --------------------------------------------------------------------------
    // OffsetMap
    // --------------------------------------------------------------------------

    private void AddOffsetMap(Dictionary<String, Object> tags)
    {
        if (Core.GameOptions.LoadOffsetMaps > 0)
        {
            List<float> offsetList = (List<float>)GetTag(tags, "Offset")!;
            Vector3 mapOffset = new Vector3(offsetList[0], 0, offsetList[1]);
            if (offsetList.Count >= 3)
            {
                mapOffset = new Vector3(offsetList[0], offsetList[1], offsetList[2]);
            }

            String mapName = (String)GetTag(tags, "Map")!;

            if (_loadOffsetMap == true)
            {
                if (_sessionMapsLoaded.Contains(mapName) == true)
                {
                    return;
                }
            }
            _sessionMapsLoaded.Add(mapName);

            LoadedOffsetMapNames.Add(mapName);
            LoadedOffsetMapOffsets.Add(mapOffset);

            String listName = Screen.Level.LevelFile + "|" + mapName + "|" +
                Screen.Level.World.CurrentMapWeather + "|" +
                World.GetCurrentRegionWeather() + "|" +
                World.GetTime() + "|" +
                World.CurrentSeason;

            if (Core.OffsetMaps.ContainsKey(listName) == false)
            {
                List<List<Entity>> mapList = [];

                List<Object> paramsList = [];
                paramsList.AddRange(new Object[] { mapName, true, mapOffset + _offset, _offsetMapLevel + 1, _sessionMapsLoaded });

                int offsetEntityCount = Screen.Level.OffsetmapEntities.Count;
                int offsetFloorCount = Screen.Level.OffsetmapFloors.Count;

                LevelLoader levelLoader = new LevelLoader();
                levelLoader.LoadLevel(paramsList.ToArray());

                List<Entity> entList = [];
                List<Entity> floorList = [];

                for (int i = offsetEntityCount; i < Screen.Level.OffsetmapEntities.Count; i++)
                {
                    entList.Add(Screen.Level.OffsetmapEntities[i]);
                }
                for (int i = offsetFloorCount; i < Screen.Level.OffsetmapFloors.Count; i++)
                {
                    floorList.Add(Screen.Level.OffsetmapFloors[i]);
                }
                mapList.AddRange([entList, floorList]);

                Core.OffsetMaps.Add(listName, mapList);
            }
            else
            {
                Logger.Debug("Loaded Offsetmap from store: " + mapName);

                foreach (Entity e in Core.OffsetMaps[listName][0])
                {
                    if (e.MapOrigin.Equals(mapName) == true)
                    {
                        e.IsOffsetMapContent = true;
                        Screen.Level.OffsetmapEntities.Add(e);
                    }
                }
                foreach (Entity e in Core.OffsetMaps[listName][1])
                {
                    if (e.MapOrigin.Equals(mapName) == true)
                    {
                        e.IsOffsetMapContent = true;
                        Screen.Level.OffsetmapFloors.Add(e);
                    }
                }
            }
            Logger.Debug("Offset maps in store: " + Core.OffsetMaps.Count);

            Screen.Level.OffsetmapEntities = Screen.Level.OffsetmapEntities
                .OrderByDescending(e => e.CameraDistance).ToList();

            foreach (Entity entity in Screen.Level.OffsetmapEntities)
            {
                entity.UpdateEntity();
            }
            foreach (Entity floor in Screen.Level.OffsetmapFloors)
            {
                floor.UpdateEntity();
            }
        }
    }

    // --------------------------------------------------------------------------
    // Structure loading
    // --------------------------------------------------------------------------

    private static Dictionary<String, List<String>> _tempStructureList = [];

    public static void ClearTempStructures()
    {
        _tempStructureList.Clear();
    }

    private String[] AddStructure(Dictionary<String, Object> tags)
    {
        List<float> offsetList = (List<float>)GetTag(tags, "Offset")!;
        Vector3 mapOffset = new Vector3(offsetList[0], 0, offsetList[1]);
        if (offsetList.Count >= 3)
        {
            mapOffset = new Vector3(offsetList[0], offsetList[1], offsetList[2]);
        }

        int mapRotation = -1;
        if (TagExists(tags, "Rotation") == true)
        {
            mapRotation = (int)GetTag(tags, "Rotation")!;
        }

        String mapName = (String)GetTag(tags, "Map")!;
        if (mapName.EndsWith(".dat") == false)
        {
            mapName = mapName + ".dat";
        }

        bool addNPC = false;
        if (TagExists(tags, "AddNPC") == true)
        {
            addNPC = (bool)GetTag(tags, "AddNPC")!;
        }

        String structureKey = mapOffset.X.ToString() + "|" + mapOffset.Y.ToString() + "|" + mapOffset.Z.ToString() + "|" + mapName;

        if (_tempStructureList.ContainsKey(structureKey) == false)
        {
            String filepath = GameModeManager.GetMapPath(mapName);
            Security.FileValidation.CheckFileValid(filepath, false, "LevelLoader.cs/StructureSpawner");

            if (System.IO.File.Exists(filepath) == false)
            {
                Logger.Log(Logger.LogTypes.ErrorMessage,
                    "LevelLoader.cs: Error loading structure from \"" + filepath + "\". File not found.");
                return [];
            }

            String[] mapContent = System.IO.File.ReadAllLines(filepath);
            List<String> structureList = [];

            foreach (String rawLine in mapContent)
            {
                String line = rawLine;
                if (line.EndsWith("}") == true)
                {
                    bool addLine = false;
                    String trimmed = line.Trim(' ', StringHelper.Tab);
                    if (trimmed.StartsWith("{\"Entity\"{ENT[") == true)
                    {
                        addLine = true;
                    }
                    else if (trimmed.StartsWith("{\"Floor\"{ENT[") == true)
                    {
                        addLine = true;
                    }
                    else if (trimmed.StartsWith("{\"EntityField\"{ENT[") == true)
                    {
                        addLine = true;
                    }
                    else if (trimmed.StartsWith("{\"NPC\"{NPC[") == true)
                    {
                        if (addNPC == true)
                        {
                            addLine = true;
                        }
                    }
                    else if (trimmed.StartsWith("{\"Shader\"{SHA[") == true)
                    {
                        addLine = true;
                    }
                    else if (trimmed.StartsWith("{\"Structure\"{STR[") == true)
                    {
                        addLine = true;
                    }

                    if (addLine == true)
                    {
                        line = ReplaceStructurePosition(line, mapOffset);

                        if (mapRotation > -1)
                        {
                            line = ReplaceStructureRotation(line, mapRotation);
                        }

                        structureList.Add(line);
                    }
                }
            }

            _tempStructureList.Add(structureKey, structureList);
        }

        return _tempStructureList[structureKey].ToArray();
    }

    private String ReplaceStructureRotation(String line, int mapRotation)
    {
        String replaceString = String.Empty;

        if (line.ToLower().Contains("{\"rotation\"{int[") == true)
        {
            replaceString = "{\"rotation\"{int[";
        }

        if (replaceString.Equals("") == false)
        {
            String rotationString = line.Remove(0, line.ToLower().IndexOf(replaceString));
            rotationString = rotationString.Remove(rotationString.IndexOf("]}}") + 3);

            String rotationData = rotationString.Remove(0, rotationString.IndexOf("[") + 1);
            rotationData = rotationData.Remove(rotationData.IndexOf("]"));

            int newRotation = int.Parse(rotationData) + mapRotation;
            while (newRotation > 3)
            {
                newRotation -= 4;
            }

            line = line.Replace(rotationString, "{\"rotation\"{int[" + newRotation.ToString() + "]}}");
        }

        return line;
    }

    private String ReplaceStructurePosition(String line, Vector3 mapOffset)
    {
        String replaceString = String.Empty;

        if (line.ToLower().Contains("{\"position\"{sngarr[") == true)
        {
            replaceString = "{\"position\"{sngarr[";
        }
        else if (line.ToLower().Contains("{\"position\"{intarr[") == true)
        {
            replaceString = "{\"position\"{intarr[";
        }

        if (line.ToLower().Contains("{\"offset\"{sngarr[") == true)
        {
            replaceString = "{\"offset\"{sngarr[";
        }

        if (replaceString.Equals("") == false)
        {
            String positionString = line.Remove(0, line.ToLower().IndexOf(replaceString));
            positionString = positionString.Remove(positionString.IndexOf("]}}") + 3);

            String positionData = positionString.Remove(0, positionString.IndexOf("[") + 1);
            positionData = positionData.Remove(positionData.IndexOf("]"));

            String[] posArr = positionData.Split(',');
            Vector3 newPosition = new Vector3(
                ScriptConversion.ToSingle(posArr[0].Replace(".", GameController.DecSeparator)) + mapOffset.X,
                ScriptConversion.ToSingle(posArr[1].Replace(".", GameController.DecSeparator)) + mapOffset.Y,
                (float)ScriptConversion.ToDouble(posArr[2].Replace(".", GameController.DecSeparator)) + mapOffset.Z);

            if (line.ToLower().Contains("{\"position\"{sngarr[") == true)
            {
                line = line.Replace(positionString,
                    "{\"position\"{sngarr[" +
                    newPosition.X.ToString().Replace(GameController.DecSeparator, ".") + "," +
                    newPosition.Y.ToString().Replace(GameController.DecSeparator, ".") + "," +
                    newPosition.Z.ToString().Replace(GameController.DecSeparator, ".") + "]}}");
            }
            else if (line.ToLower().Contains("{\"position\"{intarr[") == true)
            {
                line = line.Replace(positionString,
                    "{\"position\"{intarr[" +
                    ((int)newPosition.X).ToString() + "," +
                    ((int)newPosition.Y).ToString() + "," +
                    ((int)newPosition.Z).ToString() + "]}}");
            }
            else
            {
                line = line.Replace(positionString,
                    "{\"offset\"{sngarr[" +
                    newPosition.X.ToString().Replace(GameController.DecSeparator, ".") + "," +
                    newPosition.Y.ToString().Replace(GameController.DecSeparator, ".") + "," +
                    newPosition.Z.ToString().Replace(GameController.DecSeparator, ".") + "]}}");
            }
        }

        return line;
    }

    // --------------------------------------------------------------------------
    // Element builders
    // --------------------------------------------------------------------------

    private void EntityField(Dictionary<String, Object> tags)
    {
        List<int> sizeList = (List<int>)GetTag(tags, "Size")!;
        bool fill = true;
        if (TagExists(tags, "Fill") == true)
        {
            fill = (bool)GetTag(tags, "Fill")!;
        }
        Vector3 steps = new Vector3(1, 1, 1);
        if (TagExists(tags, "Steps") == true)
        {
            List<float> stepList = (List<float>)GetTag(tags, "Steps")!;
            if (stepList.Count == 3)
            {
                steps = new Vector3(stepList[0], stepList[1], stepList[2]);
            }
            else
            {
                steps = new Vector3(stepList[0], 1, stepList[1]);
            }
        }

        if (sizeList.Count == 3)
        {
            AddEntity(tags, new Size(sizeList[0], sizeList[2]), sizeList[1], fill, steps);
        }
        else
        {
            AddEntity(tags, new Size(sizeList[0], sizeList[1]), 1, fill, steps);
        }
    }

    private void AddNPC(Dictionary<String, Object> tags)
    {
        List<float> posList = (List<float>)GetTag(tags, "Position")!;
        Vector3 position = new Vector3(posList[0] + _offset.X, posList[1] + _offset.Y, posList[2] + _offset.Z);

        Vector3 scale = new Vector3(1);
        if (TagExists(tags, "Scale") == true)
        {
            List<float> scaleList = (List<float>)GetTag(tags, "Scale")!;
            scale = new Vector3(scaleList[0], scaleList[1], scaleList[2]);
        }
        bool collision = true;
        if (TagExists(tags, "Collision") == true)
        {
            collision = (bool)GetTag(tags, "Collision")!;
        }
        String textureID = (String)GetTag(tags, "TextureID")!;
        int rotation = (int)GetTag(tags, "Rotation")!;
        int actionValue = (int)GetTag(tags, "Action")!;
        String additionalValue = (String)GetTag(tags, "AdditionalValue")!;
        String name = (String)GetTag(tags, "Name")!;
        int id = (int)GetTag(tags, "ID")!;
        String modelPath = String.Empty;
        if (TagExists(tags, "ModelPath") == true)
        {
            String rawPath = (String)GetTag(tags, "ModelPath")!;
            if (rawPath.Contains("<") == true)
            {
                modelPath = ScriptVersion2.ScriptCommander.Parse(rawPath).ToString() ?? "";
            }
            else
            {
                modelPath = rawPath;
            }
        }
        String movement = (String)GetTag(tags, "Movement")!;
        List<Microsoft.Xna.Framework.Rectangle> moveRectangles = (List<Microsoft.Xna.Framework.Rectangle>)GetTag(tags, "MoveRectangles")!;

        float cameraDistanceDelta = 0.0f;
        if (TagExists(tags, "CameraDistanceDelta") == true)
        {
            cameraDistanceDelta = (float)GetTag(tags, "CameraDistanceDelta")!;
        }

        Vector3 shader = new Vector3(1.0f);
        if (TagExists(tags, "Shader") == true)
        {
            List<float> shaderList = (List<float>)GetTag(tags, "Shader")!;
            shader = new Vector3(shaderList[0], shaderList[1], shaderList[2]);
        }

        bool animateIdle = false;
        if (TagExists(tags, "AnimateIdle") == true)
        {
            animateIdle = (bool)GetTag(tags, "AnimateIdle")!;
        }

        NPC npc = (NPC)Entity.GetNewEntity("NPC", position,
            [null!], [0, 0], collision, new Vector3(0), scale,
            BaseModel.BillModel, actionValue, additionalValue, true, shader,
            -1, _mapOrigin, "", _offset,
            new Object[] { textureID, rotation, name, id, animateIdle, movement, moveRectangles },
            1.0f, null, cameraDistanceDelta, modelPath);

        if (_loadOffsetMap == false)
        {
            Screen.Level.Entities.Add(npc);
        }
        else
        {
            Screen.Level.OffsetmapEntities.Add(npc);
        }
    }

    private void AddFloor(Dictionary<String, Object> tags, bool isOffsetFloor = false)
    {
        List<int> sizeList = (List<int>)GetTag(tags, "Size")!;
        Size size = new Size(sizeList[0], sizeList[1]);

        List<int> posList = (List<int>)GetTag(tags, "Position")!;
        Vector3 position = new Vector3(posList[0] + _offset.X, posList[1] + _offset.Y, posList[2] + _offset.Z);

        String texturePath = (String)GetTag(tags, "TexturePath")!;
        Microsoft.Xna.Framework.Rectangle textureRectangle = (Microsoft.Xna.Framework.Rectangle)GetTag(tags, "Texture")!;
        Texture2D texture = TextureManager.GetTexture(texturePath, textureRectangle);

        bool visible = true;
        if (TagExists(tags, "Visible") == true)
        {
            visible = (bool)GetTag(tags, "Visible")!;
        }

        Vector3 shader = new Vector3(1.0f);
        if (TagExists(tags, "Shader") == true)
        {
            List<float> shaderList = (List<float>)GetTag(tags, "Shader")!;
            shader = new Vector3(shaderList[0], shaderList[1], shaderList[2]);
        }

        bool removeFloor = false;
        if (TagExists(tags, "Remove") == true)
        {
            removeFloor = (bool)GetTag(tags, "Remove")!;
        }

        bool hasSnow = true;
        if (TagExists(tags, "hasSnow") == true)
        {
            hasSnow = (bool)GetTag(tags, "hasSnow")!;
        }

        bool hasSand = true;
        if (TagExists(tags, "hasSand") == true)
        {
            hasSand = (bool)GetTag(tags, "hasSand")!;
        }

        bool hasIce = false;
        if (TagExists(tags, "isIce") == true)
        {
            hasIce = (bool)GetTag(tags, "isIce")!;
        }

        int rotation = 0;
        if (TagExists(tags, "Rotation") == true)
        {
            rotation = (int)GetTag(tags, "Rotation")!;
        }

        String seasonTexture = String.Empty;
        if (TagExists(tags, "SeasonTexture") == true)
        {
            seasonTexture = (String)GetTag(tags, "SeasonTexture")!;
        }

        List<Entity> floorList = Screen.Level.Floors;
        if (_loadOffsetMap == true)
        {
            floorList = Screen.Level.OffsetmapFloors;
        }

        if (isOffsetFloor == false || (isOffsetFloor == true && visible == true))
        {
            if (removeFloor == false)
            {
                for (int x = 0; x < size.Width; x++)
                {
                    for (int z = 0; z < size.Height; z++)
                    {
                        int iZ = z;
                        int iX = x;

                        Entity? existingEnt;
                        if (_loadOffsetMap == true)
                        {
                            existingEnt = Screen.Level.OffsetmapFloors.Find(e =>
                                e.Position == new Vector3(position.X + iX, position.Y, position.Z + iZ));
                        }
                        else
                        {
                            existingEnt = Screen.Level.Floors.Find(e =>
                                e.Position == new Vector3(position.X + iX, position.Y, position.Z + iZ));
                        }

                        if (existingEnt != null)
                        {
                            existingEnt.Textures = [texture];
                            existingEnt.Visible = visible;
                            existingEnt.SeasonColorTexture = seasonTexture;
                            existingEnt.LoadSeasonTextures();
                            ((Floor)existingEnt).SetRotation(rotation);
                            ((Floor)existingEnt).hasSnow = hasSnow;
                            ((Floor)existingEnt).IsIce = hasIce;
                            ((Floor)existingEnt).hasSand = hasSand;
                        }
                        else
                        {
                            Floor f = new Floor(position.X + x, position.Y, position.Z + z,
                                [TextureManager.GetTexture(texturePath, textureRectangle)],
                                [0, 0], false, rotation, new Vector3(1.0f),
                                BaseModel.FloorModel, 0, "", visible, shader, hasSnow, hasIce, hasSand);
                            f.MapOrigin = _mapOrigin;
                            f.SeasonColorTexture = seasonTexture;
                            f.LoadSeasonTextures();
                            f.IsOffsetMapContent = _loadOffsetMap;
                            floorList.Add(f);
                        }
                    }
                }
            }
            else
            {
                for (int x = 0; x < size.Width; x++)
                {
                    for (int z = 0; z < size.Height; z++)
                    {
                        for (int i = 0; i <= floorList.Count; i++)
                        {
                            if (i < floorList.Count)
                            {
                                Entity floor = floorList[i];
                                if (floor.Position.X == position.X + x &&
                                    floor.Position.Y == position.Y &&
                                    floor.Position.Z == position.Z + z)
                                {
                                    floorList.RemoveAt(i);
                                    i -= 1;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void AddEntity(Dictionary<String, Object> tags, Size size, int sizeY, bool fill, Vector3 steps)
    {
        String entityID = (String)GetTag(tags, "EntityID")!;

        int id = -1;
        if (TagExists(tags, "ID") == true)
        {
            id = (int)GetTag(tags, "ID")!;
        }

        List<float> posList = (List<float>)GetTag(tags, "Position")!;
        Vector3 position = new Vector3(posList[0] + _offset.X, posList[1] + _offset.Y, posList[2] + _offset.Z);

        List<Microsoft.Xna.Framework.Rectangle> texList = (List<Microsoft.Xna.Framework.Rectangle>)GetTag(tags, "Textures")!;
        List<Texture2D> textureList = [];
        String texturePath = (String)GetTag(tags, "TexturePath")!;
        foreach (Microsoft.Xna.Framework.Rectangle textureRectangle in texList)
        {
            textureList.Add(TextureManager.GetTexture(texturePath, textureRectangle));
        }
        Texture2D[] textureArray = textureList.ToArray();

        List<int> textureIndexList = (List<int>)GetTag(tags, "TextureIndex")!;
        int[] textureIndex = textureIndexList.ToArray();

        Vector3 scale = new Vector3(1);
        if (TagExists(tags, "Scale") == true)
        {
            List<float> scaleList = (List<float>)GetTag(tags, "Scale")!;
            scale = new Vector3(scaleList[0], scaleList[1], scaleList[2]);
        }

        bool collision = (bool)GetTag(tags, "Collision")!;

        int modelID = (int)GetTag(tags, "ModelID")!;

        String modelPath = String.Empty;
        if (TagExists(tags, "ModelPath") == true)
        {
            String rawPath = (String)GetTag(tags, "ModelPath")!;
            if (rawPath.Contains("<") == true)
            {
                modelPath = ScriptVersion2.ScriptCommander.Parse(rawPath).ToString() ?? "";
            }
            else
            {
                modelPath = rawPath;
            }
        }

        int actionValue = (int)GetTag(tags, "Action")!;

        String additionalValue = String.Empty;
        if (TagExists(tags, "AdditionalValue") == true)
        {
            additionalValue = (String)GetTag(tags, "AdditionalValue")!;
        }

        List<List<int>>? animationData = null;
        if (TagExists(tags, "AnimationData") == true)
        {
            animationData = (List<List<int>>)GetTag(tags, "AnimationData")!;
        }

        Vector3 rotation = Entity.GetRotationFromInteger((int)GetTag(tags, "Rotation")!);

        if (TagExists(tags, "RotationXYZ") == true)
        {
            List<float> rotationList = (List<float>)GetTag(tags, "RotationXYZ")!;
            rotation = new Vector3(rotationList[0], rotationList[1], rotationList[2]);
        }

        if (modelID == 21)
        {
            rotation.Z += MathHelper.Pi;
        }

        bool visible = true;
        if (TagExists(tags, "Visible") == true)
        {
            visible = (bool)GetTag(tags, "Visible")!;
        }

        Vector3 shader = new Vector3(1.0f);
        if (TagExists(tags, "Shader") == true)
        {
            List<float> shaderList = (List<float>)GetTag(tags, "Shader")!;
            shader = new Vector3(shaderList[0], shaderList[1], shaderList[2]);
        }

        String seasonTexture = String.Empty;
        if (TagExists(tags, "SeasonTexture") == true)
        {
            seasonTexture = (String)GetTag(tags, "SeasonTexture")!;
        }

        String seasonToggle = String.Empty;
        if (TagExists(tags, "SeasonToggle") == true)
        {
            seasonToggle = (String)GetTag(tags, "SeasonToggle")!;
        }

        float opacity = 1.0f;
        if (TagExists(tags, "Opacity") == true)
        {
            opacity = (float)GetTag(tags, "Opacity")!;
        }

        float cameraDistanceDelta = 0.0f;
        if (TagExists(tags, "CameraDistanceDelta") == true)
        {
            cameraDistanceDelta = (float)GetTag(tags, "CameraDistanceDelta")!;
        }

        for (float x = 0; x < size.Width; x += steps.X)
        {
            for (float z = 0; z < size.Height; z += steps.Z)
            {
                for (float y = 0; y < sizeY; y += steps.Y)
                {
                    bool doAdd = false;
                    if (fill == false)
                    {
                        if (x == 0 || z == 0 || z == size.Height - 1 || x == size.Width - 1)
                        {
                            doAdd = true;
                        }
                    }
                    else
                    {
                        doAdd = true;
                    }

                    if (seasonToggle.Equals("") == false)
                    {
                        if (seasonToggle.Contains(",") == false)
                        {
                            if (seasonToggle.ToLower().Equals(World.CurrentSeason.ToString().ToLower()) == false)
                            {
                                doAdd = false;
                            }
                        }
                        else
                        {
                            String[] seasons = seasonToggle.ToLower().Split(',');
                            if (System.Array.IndexOf(seasons, World.CurrentSeason.ToString().ToLower()) < 0)
                            {
                                doAdd = false;
                            }
                        }
                    }

                    if (doAdd == true)
                    {
                        Entity newEnt = Entity.GetNewEntity(entityID,
                            new Vector3(position.X + x, position.Y + y, position.Z + z),
                            textureArray,
                            textureIndex,
                            collision,
                            rotation,
                            scale,
                            BaseModel.GetModelByID(modelID),
                            actionValue,
                            additionalValue,
                            visible,
                            shader,
                            id,
                            _mapOrigin,
                            seasonTexture,
                            _offset,
                            [],
                            opacity,
                            animationData,
                            cameraDistanceDelta,
                            modelPath);
                        newEnt.IsOffsetMapContent = _loadOffsetMap;

                        if (newEnt != null)
                        {
                            if (_loadOffsetMap == false)
                            {
                                Screen.Level.Entities.Add(newEnt);
                            }
                            else
                            {
                                Screen.Level.OffsetmapEntities.Add(newEnt);
                            }
                        }
                    }
                }
            }
        }
    }

    private void SetupLevel(Dictionary<String, Object> tags)
    {
        String name = (String)GetTag(tags, "Name")!;
        String musicLoop = (String)GetTag(tags, "MusicLoop")!;

        if (TagExists(tags, "WildPokemon") == true)
        {
            Screen.Level.WildPokemonFloor = (bool)GetTag(tags, "WildPokemon")!;
        }
        else
        {
            Screen.Level.WildPokemonFloor = false;
        }

        if (TagExists(tags, "OverworldPokemon") == true)
        {
            Screen.Level.ShowOverworldPokemon = (bool)GetTag(tags, "OverworldPokemon")!;
        }
        else
        {
            Screen.Level.ShowOverworldPokemon = true;
        }

        if (TagExists(tags, "CurrentRegion") == true)
        {
            Screen.Level.CurrentRegion = (String)GetTag(tags, "CurrentRegion")!;
        }
        else
        {
            Screen.Level.CurrentRegion = "Johto";
        }

        if (TagExists(tags, "RegionalForm") == true)
        {
            Screen.Level.RegionalForm = (String)GetTag(tags, "RegionalForm")!;
        }
        else
        {
            Screen.Level.RegionalForm = String.Empty;
        }

        if (TagExists(tags, "HiddenAbility") == true)
        {
            Screen.Level.HiddenAbilityChance = (int)GetTag(tags, "HiddenAbility")!;
        }
        else
        {
            Screen.Level.HiddenAbilityChance = 0;
        }

        Screen.Level.MapName = name;
        Screen.Level.MusicLoop = musicLoop;
    }

    public static String MapScript = String.Empty;

    private void SetupActions(Dictionary<String, Object> tags)
    {
        if (TagExists(tags, "CanTeleport") == true)
        {
            Screen.Level.CanTeleport = (bool)GetTag(tags, "CanTeleport")!;
        }
        else
        {
            Screen.Level.CanTeleport = false;
        }

        if (TagExists(tags, "CanDig") == true)
        {
            Screen.Level.CanDig = (bool)GetTag(tags, "CanDig")!;
        }
        else
        {
            Screen.Level.CanDig = false;
        }

        if (TagExists(tags, "CanFly") == true)
        {
            Screen.Level.CanFly = (bool)GetTag(tags, "CanFly")!;
        }
        else
        {
            Screen.Level.CanFly = false;
        }

        if (TagExists(tags, "RideType") == true)
        {
            Screen.Level.RideType = (int)GetTag(tags, "RideType")!;
        }
        else
        {
            Screen.Level.RideType = 0;
        }

        if (TagExists(tags, "DisabledMenus") == true)
        {
            Screen.Level.DisabledMenus = (String)GetTag(tags, "DisabledMenus")!;
        }
        else
        {
            Screen.Level.DisabledMenus = "None";
        }

        if (TagExists(tags, "BattleVariables") == true)
        {
            Screen.Level.BattleVariables = (String)GetTag(tags, "BattleVariables")!;
            Screen.Level.SetBattleVariables(Screen.Level.BattleVariables);
        }
        else
        {
            Screen.Level.BattleVariables = String.Empty;
        }

        if (TagExists(tags, "BlackOutScript") == true)
        {
            Screen.Level.BlackOutScript = ScriptVersion2.ScriptCommander.Parse((String)GetTag(tags, "BlackOutScript")!).ToString() ?? "";
        }
        else
        {
            Screen.Level.BlackOutScript = String.Empty;
        }

        if (_reload == false)
        {
            if (TagExists(tags, "EnvironmentType") == true)
            {
                Screen.Level.EnvironmentType = (int)GetTag(tags, "EnvironmentType")!;
            }
            else
            {
                Screen.Level.EnvironmentType = 0;
            }

            if (TagExists(tags, "Season") == true)
            {
                int seasonValue = (int)GetTag(tags, "Season")!;
                if (seasonValue != -1)
                {
                    World.setSeason = seasonValue;
                }
                else
                {
                    World.setSeason = -1;
                }
            }

            if (TagExists(tags, "Weather") == true)
            {
                Screen.Level.WeatherType = (int)GetTag(tags, "Weather")!;
            }
            else
            {
                Screen.Level.WeatherType = 0;
            }

            if (TagExists(tags, "DayTime") == true)
            {
                Screen.Level.DayTime = (int)GetTag(tags, "DayTime")!;
            }
            else
            {
                Screen.Level.DayTime = (int)World.GetTime() + 1;
            }
        }

        if (TagExists(tags, "Lighting") == true)
        {
            Screen.Level.LightingType = (int)GetTag(tags, "Lighting")!;
        }
        else
        {
            Screen.Level.LightingType = 1;
        }

        if (TagExists(tags, "IsDark") == true)
        {
            Screen.Level.IsDark = (bool)GetTag(tags, "IsDark")!;
        }
        else
        {
            Screen.Level.IsDark = false;
        }

        if (TagExists(tags, "IsAurora") == true)
        {
            World.IsAurora = (bool)GetTag(tags, "IsAurora")!;
        }
        else
        {
            if (Screen.Level.DayTime == (int)World.DayTimes.Night)
            {
                if (World.IsAurora == false)
                {
                    int chance = Core.Random.Next(0, 250);
                    if (chance == 0)
                    {
                        World.IsAurora = true;
                    }
                }
            }
            else
            {
                World.IsAurora = false;
            }
        }

        if (TagExists(tags, "Terrain") == true)
        {
            Screen.Level.Terrain.TerrainType = Terrain.FromString((String)GetTag(tags, "Terrain")!);
        }
        else
        {
            Screen.Level.Terrain.TerrainType = Terrain.TerrainTypes.Plain;
        }

        if (TagExists(tags, "IsSafariZone") == true)
        {
            Screen.Level.IsSafariZone = (bool)GetTag(tags, "IsSafariZone")!;
        }
        else
        {
            Screen.Level.IsSafariZone = false;
        }

        if (TagExists(tags, "IsOutside") == true)
        {
            Screen.Level.IsOutside = (bool)GetTag(tags, "IsOutside")!;
        }
        else
        {
            Screen.Level.IsOutside = false;
        }

        if (TagExists(tags, "BugCatchingContest") == true)
        {
            Screen.Level.IsBugCatchingContest = true;
            Screen.Level.BugCatchingContestData = (String)GetTag(tags, "BugCatchingContest")!;
        }
        else
        {
            Screen.Level.IsBugCatchingContest = false;
            Screen.Level.BugCatchingContestData = String.Empty;
        }

        if (TagExists(tags, "MapScript") == true)
        {
            String scriptName = (String)GetTag(tags, "MapScript")!;
            if (Core.CurrentScreen.Identification == Screen.Identifications.OverworldScreen)
            {
                ActionScript runner = ((OverworldScreen)Core.CurrentScreen).ActionScript;
                if (runner.IsReady == true)
                {
                    runner.reDelay = 0.0f;
                    runner.StartScript(ScriptVersion2.ScriptCommander.Parse(scriptName).ToString() ?? "", 0);
                }
                else
                {
                    MapScript = ScriptVersion2.ScriptCommander.Parse(scriptName).ToString() ?? "";
                }
            }
            else
            {
                MapScript = ScriptVersion2.ScriptCommander.Parse(scriptName).ToString() ?? "";
            }
        }
        else
        {
            MapScript = String.Empty;
        }

        if (TagExists(tags, "RadioChannels") == true)
        {
            String[] channels = ((String)GetTag(tags, "RadioChannels")!).Split(',');
            foreach (String c in channels)
            {
                Screen.Level.AllowedRadioChannels.Add(decimal.Parse(c.Replace(".", GameController.DecSeparator)));
            }
        }
        else
        {
            Screen.Level.AllowedRadioChannels.Clear();
        }

        if (TagExists(tags, "BattleMap") == true)
        {
            Screen.Level.BattleMapData = (String)GetTag(tags, "BattleMap")!;
        }
        else
        {
            Screen.Level.BattleMapData = String.Empty;
        }

        if (TagExists(tags, "SurfingBattleMap") == true)
        {
            Screen.Level.SurfingBattleMapData = (String)GetTag(tags, "SurfingBattleMap")!;
        }
        else
        {
            Screen.Level.SurfingBattleMapData = String.Empty;
        }

        Screen.Level.World = new World(Screen.Level.EnvironmentType, Screen.Level.WeatherType);
    }

    private void AddShader(Dictionary<String, Object> tags)
    {
        List<int> sizeList = (List<int>)GetTag(tags, "Size")!;
        Vector3 size = new Vector3(sizeList[0], 1, sizeList[1]);
        if (sizeList.Count == 3)
        {
            size = new Vector3(sizeList[0], sizeList[1], sizeList[2]);
        }

        List<float> shaderList = (List<float>)GetTag(tags, "Shader")!;
        Vector3 shader = new Vector3(shaderList[0], shaderList[1], shaderList[2]);

        bool stopOnContact = (bool)GetTag(tags, "StopOnContact")!;

        List<int> posList = (List<int>)GetTag(tags, "Position")!;
        Vector3 position = new Vector3(posList[0] + _offset.X, posList[1] + _offset.Y, posList[2] + _offset.Z);

        List<int> dayTime = [];
        if (TagExists(tags, "DayTime") == true)
        {
            dayTime = (List<int>)GetTag(tags, "DayTime")!;
        }

        bool disableWhenNoLighting = false;
        if (TagExists(tags, "DisableWhenNoLighting") == true)
        {
            disableWhenNoLighting = (bool)GetTag(tags, "DisableWhenNoLighting")!;
        }

        World.DayTimes currentTime = World.GetTime();
        switch (Screen.Level.DayTime)
        {
            case 1:
                currentTime = World.DayTimes.Night;
                break;
            case 2:
                currentTime = World.DayTimes.Morning;
                break;
            case 3:
                currentTime = World.DayTimes.Day;
                break;
            case 4:
                currentTime = World.DayTimes.Evening;
                break;
        }

        if (dayTime.Contains((int)currentTime) || dayTime.Contains(-1) || dayTime.Count == 0)
        {
            Shader newShader = new Shader(position, size, shader, stopOnContact, disableWhenNoLighting);
            Screen.Level.Shaders.Add(newShader);
        }
    }

    private void AddBackdrop(Dictionary<String, Object> tags)
    {
        List<int> sizeList = (List<int>)GetTag(tags, "Size")!;
        int width = sizeList[0];
        int height = sizeList[1];

        List<float> posList = (List<float>)GetTag(tags, "Position")!;
        Vector3 position = new Vector3(posList[0] + _offset.X, posList[1] + _offset.Y, posList[2] + _offset.Z);

        Vector3 rotation = Vector3.Zero;
        if (TagExists(tags, "Rotation") == true)
        {
            List<float> rotationList = (List<float>)GetTag(tags, "Rotation")!;
            rotation = new Vector3(rotationList[0], rotationList[1], rotationList[2]);
        }

        String backdropType = (String)GetTag(tags, "Type")!;

        String texturePath = (String)GetTag(tags, "TexturePath")!;
        Microsoft.Xna.Framework.Rectangle textureRectangle = (Microsoft.Xna.Framework.Rectangle)GetTag(tags, "Texture")!;
        Texture2D texture = TextureManager.GetTexture(texturePath, textureRectangle);

        int animationSpeed = (int)GetTag(tags, "AnimationSpeed")!;
        int frameCount = (int)GetTag(tags, "FrameCount")!;

        String trigger = String.Empty;
        bool isTriggered = true;

        if (TagExists(tags, "Trigger") == true)
        {
            trigger = (String)GetTag(tags, "Trigger")!;
        }

        switch (trigger.ToLower())
        {
            case "offset":
                if (Core.GameOptions.LoadOffsetMaps == 0)
                {
                    isTriggered = false;
                }
                break;
            case "notoffset":
                if (Core.GameOptions.LoadOffsetMaps > 0)
                {
                    isTriggered = false;
                }
                break;
        }

        if (isTriggered == true)
        {
            Screen.Level.BackdropRenderer.AddBackdrop(
                new BackdropRenderer.Backdrop(backdropType, position, rotation, width, height, texture, animationSpeed, frameCount));
        }
    }

    // --------------------------------------------------------------------------
    // Berry loading
    // --------------------------------------------------------------------------

    private void LoadBerries()
    {
        String[] berryEntries = Core.Player.BerryData
            .Replace("}" + System.Environment.NewLine, "}")
            .Split('}');

        foreach (String rawBerry in berryEntries)
        {
            String berry = rawBerry;
            if (berry.Contains("{") == true)
            {
                berry = berry.Remove(0, berry.IndexOf("{"));
                berry = berry.Remove(0, 1);

                List<String> bData = [.. berry.Split('|')];
                String[] pData = bData[1].Split(',');

                if (bData.Count == 6)
                {
                    bData.Add("0");
                }

                if (bData[0].ToLower().Equals(Screen.Level.LevelFile.ToLower()) == true)
                {
                    Entity newEnt = Entity.GetNewEntity("BerryPlant",
                        new Vector3(float.Parse(pData[0]), float.Parse(pData[1]), float.Parse(pData[2])),
                        [null], [0, 0], true, new Vector3(0), new Vector3(1),
                        BaseModel.BillModel, 0, "", true, new Vector3(1.0f),
                        -1, _mapOrigin, "", _offset);
                    ((BerryPlant)newEnt).Initialize(
                        int.Parse(bData[2]), int.Parse(bData[3]),
                        bData[4], bData[5], bool.Parse(bData[6]));

                    Screen.Level.Entities.Add(newEnt);
                }
            }
        }
    }
}
