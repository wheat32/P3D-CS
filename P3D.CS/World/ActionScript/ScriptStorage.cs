namespace P3D;

public static class ScriptStorage
{
    private static Dictionary<String, Pokemon> _pokemons = [];
    private static Dictionary<String, String> _strings = [];
    private static Dictionary<String, int> _integers = [];
    private static Dictionary<String, bool> _booleans = [];
    private static Dictionary<String, Items.Item> _items = [];
    private static Dictionary<String, float> _singles = [];
    private static Dictionary<String, double> _doubles = [];

    public static Object GetObject(String type, String name)
    {
        switch (type.ToLower())
        {
            case "pokemon":
                if (_pokemons.ContainsKey(name) == true)
                {
                    return _pokemons[name];
                }
                break;
            case "string":
            case "str":
                if (_strings.ContainsKey(name) == true)
                {
                    return _strings[name];
                }
                break;
            case "integer":
            case "int":
                if (_integers.ContainsKey(name) == true)
                {
                    return _integers[name];
                }
                break;
            case "boolean":
            case "bool":
                if (_booleans.ContainsKey(name) == true)
                {
                    return _booleans[name];
                }
                break;
            case "item":
                if (_items.ContainsKey(name) == true)
                {
                    return _items[name];
                }
                break;
            case "single":
            case "sng":
                if (_singles.ContainsKey(name) == true)
                {
                    return _singles[name];
                }
                break;
            case "double":
            case "dbl":
                if (_doubles.ContainsKey(name) == true)
                {
                    return _doubles[name];
                }
                break;
        }

        return ScriptVersion2.ScriptComparer.DefaultNull;
    }

    public static void SetObject(String type, String name, Object newContent)
    {
        switch (type.ToLower())
        {
            case "pokemon":
                if (_pokemons.ContainsKey(name) == true)
                {
                    _pokemons[name] = (Pokemon)newContent;
                }
                else
                {
                    _pokemons.Add(name, (Pokemon)newContent);
                }
                break;
            case "string":
            case "str":
                if (_strings.ContainsKey(name) == true)
                {
                    _strings[name] = newContent.ToString() ?? "";
                }
                else
                {
                    _strings.Add(name, newContent.ToString() ?? "");
                }
                break;
            case "integer":
            case "int":
                if (_integers.ContainsKey(name) == true)
                {
                    _integers[name] = ScriptConversion.ToInteger(newContent);
                }
                else
                {
                    _integers.Add(name, ScriptConversion.ToInteger(newContent));
                }
                break;
            case "boolean":
            case "bool":
                if (_booleans.ContainsKey(name) == true)
                {
                    _booleans[name] = Convert.ToBoolean(newContent);
                }
                else
                {
                    _booleans.Add(name, Convert.ToBoolean(newContent));
                }
                break;
            case "item":
                if (_items.ContainsKey(name) == true)
                {
                    _items[name] = (Items.Item)newContent;
                }
                else
                {
                    _items.Add(name, (Items.Item)newContent);
                }
                break;
            case "single":
            case "sng":
                if (_singles.ContainsKey(name) == true)
                {
                    _singles[name] = ScriptConversion.ToSingle(newContent);
                }
                else
                {
                    _singles.Add(name, ScriptConversion.ToSingle(newContent));
                }
                break;
            case "double":
            case "dbl":
                if (_doubles.ContainsKey(name) == true)
                {
                    _doubles[name] = ScriptConversion.ToDouble(newContent);
                }
                else
                {
                    _doubles.Add(name, ScriptConversion.ToDouble(newContent));
                }
                break;
        }
    }

    public static void Clear()
    {
        _pokemons.Clear();
        _strings.Clear();
        _integers.Clear();
        _booleans.Clear();
        _items.Clear();
        _singles.Clear();
        _doubles.Clear();
    }

    public static int Count(String type)
    {
        if (type.Equals("") == true)
        {
            return _pokemons.Count + _strings.Count + _integers.Count + _booleans.Count + _items.Count;
        }
        else
        {
            switch (type.ToLower())
            {
                case "pokemon":
                    return _pokemons.Count;
                case "string":
                case "str":
                    return _strings.Count;
                case "integer":
                case "int":
                    return _integers.Count;
                case "boolean":
                case "bool":
                    return _booleans.Count;
                case "item":
                    return _items.Count;
                case "single":
                case "sng":
                    return _singles.Count;
                case "double":
                case "dbl":
                    return _doubles.Count;
            }
        }

        return 0;
    }
}
