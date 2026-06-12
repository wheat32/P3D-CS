using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using P3D;
using P3D.Items;

namespace P3D.BattleSystem;

// ---------------------------------------------------------------------------
// QueryObject base class
// ---------------------------------------------------------------------------

public abstract class QueryObject
{
    public enum QueryTypes
    {
        CameraMovement,
        Textbox,
        ToggleMenu,
        ToggleEntity,
        MathHP,
        Delay,
        EndBattle,
        PlayMusic,
        ChoosePokemon,
        ScreenFade,
        LearnMoves,
        InflictStatus,
        ChangeHP,
        SwitchPokemon,
        TriggerNewRound,
        RoamingPokemonFled,
        DisplayLevelUp,
        PlaySound,
        MoveAnimation,
        AfterFaint,
        StartRound,
    }

    public QueryTypes QueryType;
    public bool PassThis;
    public Pokemon? Pokemon;
    public bool RemoveFainted;

    protected QueryObject(QueryTypes queryType)
    {
        QueryType = queryType;
    }

    public virtual bool IsReady => true;
    public virtual void Update(BattleScreen bv2Screen) { }
    public virtual void Draw(BattleScreen bv2Screen) { }
    public virtual bool UpdateCamera => false;
    public virtual bool NeedForPVPData() => false;

    public static QueryObject? FromString(String input)
    {
        if (input.StartsWith("{") == false || input.EndsWith("}") == false)
        {
            return null;
        }

        input = input.Remove(input.Length - 1, 1).Remove(0, 1);
        String type = input.Remove(input.IndexOf('|'));
        String data = input.Remove(0, input.IndexOf('|') + 1);

        try
        {
            switch (type)
            {
                case "CAMERA":
                    return CameraQueryObject.FromString(data);
                case "DELAY":
                    return DelayQueryObject.FromString(data);
                case "ENDBATTLE":
                    return EndBattleQueryObject.FromString(data);
                case "MATHHP":
                    return MathHPQueryObject.FromString(data);
                case "MUSIC":
                    return PlayMusicQueryObject.FromString(data);
                case "SOUND":
                    return PlaySoundQueryObject.FromString(data);
                case "FADE":
                    return ScreenFadeQueryObject.FromString(data);
                case "TEXT":
                    return TextQueryObject.FromString(data);
                case "TOGGLEENTITY":
                    return ToggleEntityQueryObject.FromString(data);
                case "TOGGLEMENU":
                    return ToggleMenuQueryObject.FromString(data);
                case "TRIGGERNEWROUNDPVP":
                    return TriggerNewRoundPVPQueryObject.FromString(data);
                case "FAINT":
                    return AfterFaintQueryObject.FromString(data);
                case "STARTROUND":
                    return StartRoundQueryObject.FromString(data);
            }
        }
        catch (Exception)
        {
            Logger.Debug("QueryObjects.cs: Wrong data received, could not convert to [" + type + "] query object.");
            Logger.Debug(input);
            return null;
        }

        return null;
    }

    public override String ToString() => String.Empty;
}

// ---------------------------------------------------------------------------
// TextQueryObject
// ---------------------------------------------------------------------------

public class TextQueryObject : QueryObject
{
    private String _text = String.Empty;
    private Color _textColor = Color.White;
    private bool _ready;
    private int _textIndex;
    private float _textDelay = 0.02f;

    private bool TextReady => _textIndex >= _text.Length;

    public String Text => _text;

    public TextQueryObject(String text) : base(QueryTypes.Textbox)
    {
        _text = text;
        _text = _text.Replace("*", " ");
        _text = _text.Replace("~", " ");
        _text = _text.Replace("<player.name>", Core.Player.Name);
        _text = _text.Replace("<playername>", Core.Player.Name);
        _text = _text.Replace("<rival.name>", Core.Player.RivalName);
        _text = _text.Replace("<rivalname>", Core.Player.RivalName);
        _text = _text.Replace("[POKE]", "Poké");

        if (String.IsNullOrEmpty(_text) == true)
        {
            _ready = true;
        }
    }

    public TextQueryObject(String text, Color textColor) : base(QueryTypes.Textbox)
    {
        _text = text;
        _textColor = textColor;
        if (String.IsNullOrEmpty(_text) == true)
        {
            _ready = true;
        }
    }

    public override void Update(BattleScreen bv2Screen)
    {
        if (TextReady == false)
        {
            _textDelay -= 0.1f;
            if (_textDelay <= 0.0f)
            {
                _textDelay = GetTextSpeed();
                _textIndex += 1;
            }
            if (Controls.Accept(true, true) == true && _textIndex > 2)
            {
                _textIndex = _text.Length;
            }
        }
        else
        {
            if (Core.CurrentScreen.Identification.Equals(Screen.Identifications.PauseScreen) == false &&
                Core.CurrentScreen.Identification.Equals(Screen.Identifications.ChatScreen) == false)
            {
                if (Controls.Accept(true, true) == true)
                {
                    SoundManager.PlaySound("select");
                    _ready = true;
                }
            }
        }
    }

    private float GetTextSpeed()
    {
        switch (TextBox.TextSpeed)
        {
            case 1:
                return 0.3f;
            case 2:
                return 0.2f;
            case 3:
                return 0.1f;
            case 4:
                return 0.0f;
        }
        return 0.2f;
    }

    public override void Draw(BattleScreen bv2Screen)
    {
        Rectangle rec = new Rectangle(
            100, Core.windowSize.Height - 250,
            Core.windowSize.Width - 200, 200);

        String text = _text.Substring(0, _textIndex);
        if (text.Length > 0)
        {
            Canvas.DrawRectangle(rec, new Color(0, 0, 0, 150));
        }
        text = text.CropStringToWidth(FontManager.TextFont, 2.0f, Core.windowSize.Width - 300);

        Core.SpriteBatch.DrawString(
            FontManager.TextFont, text,
            new Vector2(rec.X + 20, rec.Y + 20),
            Color.White, 0.0f, Vector2.Zero, 2.0f, SpriteEffects.None, 0.0f);

        if (GamePad.GetState(PlayerIndex.One).IsConnected == true &&
            Core.GameOptions.GamePadEnabled == true &&
            bv2Screen.IsCurrentScreen() == true)
        {
            Dictionary<Buttons, String> d = [];
            d.Add(Buttons.A, Localization.GetString("global_ok", "OK"));
            bv2Screen.DrawGamePadControls(d, new Vector2(rec.X + rec.Width - 100, rec.Y + rec.Height - 40));
        }
        else
        {
            if (TextReady == true)
            {
                String okText = Localization.GetString("global_ok", "OK");
                float okWidth = FontManager.TextFont.MeasureString(okText).X * 2.0f;
                float okHeight = FontManager.TextFont.MeasureString(okText).Y * 2.0f;
                Core.SpriteBatch.DrawString(
                    FontManager.TextFont, okText,
                    new Vector2(rec.X + rec.Width - okWidth - 20, rec.Y + rec.Height - okHeight - 5),
                    Color.White, 0.0f, Vector2.Zero, 2.0f, SpriteEffects.None, 0.0f);
            }
        }
    }

    public override bool IsReady => _ready;
    public override bool NeedForPVPData() => true;

    public new static QueryObject FromString(String input)
    {
        String[] d = input.Split('|');
        return new TextQueryObject(
            d[0].Replace("*", System.Environment.NewLine),
            new Color((int)int.Parse(d[1]), (int)int.Parse(d[2]), (int)int.Parse(d[3])));
    }

    public override String ToString()
    {
        return "{TEXT|" + _text.Replace(System.Environment.NewLine, "*") + "|" +
               _textColor.R + "|" + _textColor.G + "|" + _textColor.B + "}";
    }
}

// ---------------------------------------------------------------------------
// DelayQueryObject
// ---------------------------------------------------------------------------

public class DelayQueryObject : QueryObject
{
    private int _delay;

    public DelayQueryObject(int delay) : base(QueryTypes.Delay)
    {
        _delay = delay;
    }

    public override void Update(BattleScreen bv2Screen)
    {
        if (_delay > 0)
        {
            _delay -= 1;
        }
    }

    public override bool IsReady => _delay.Equals(0);
    public override bool NeedForPVPData() => true;

    public new static QueryObject FromString(String input)
    {
        return new DelayQueryObject(int.Parse(input));
    }

    public override String ToString()
    {
        return "{DELAY|" + _delay.ToString() + "}";
    }
}

// ---------------------------------------------------------------------------
// EndBattleQueryObject
// ---------------------------------------------------------------------------

public class EndBattleQueryObject : QueryObject
{
    private bool _blackout;

    public EndBattleQueryObject(bool blackout) : base(QueryTypes.EndBattle)
    {
        _blackout = blackout;
    }

    public override void Update(BattleScreen bv2Screen)
    {
        bv2Screen.EndBattle(_blackout);
    }

    public override bool IsReady => true;
    public override bool NeedForPVPData() => true;

    public new static QueryObject FromString(String input)
    {
        return new EndBattleQueryObject(bool.Parse(input));
    }

    public override String ToString()
    {
        return "{ENDBATTLE|" + _blackout.ToNumberString() + "}";
    }
}

// ---------------------------------------------------------------------------
// PlaySoundQueryObject
// ---------------------------------------------------------------------------

public class PlaySoundQueryObject : QueryObject
{
    private String _sound = String.Empty;
    private bool _isPokemonSound;
    private String _crySuffix = String.Empty;
    private float _delay;
    private bool _stopMusic;

    public PlaySoundQueryObject(String sound, bool isPokemonSound, String crySuffix = "",
                                bool stopMusic = false)
        : this(sound, isPokemonSound, 0.0f, crySuffix, stopMusic)
    {
    }

    public PlaySoundQueryObject(String sound, bool isPokemonSound, float delay,
                                String crySuffix = "", bool stopMusic = false)
        : base(QueryTypes.PlaySound)
    {
        _sound = sound;
        _isPokemonSound = isPokemonSound;
        _crySuffix = crySuffix;
        _delay = delay;
        _stopMusic = stopMusic;
        PassThis = true;
    }

    public override void Update(BattleScreen bv2Screen)
    {
        if (_delay > 0.0f)
        {
            _delay -= 0.1f;
            if (_delay <= 0.0f)
            {
                InternalPlaySound();
            }
        }
        else
        {
            InternalPlaySound();
        }
    }

    private void InternalPlaySound()
    {
        if (_isPokemonSound == true)
        {
            SoundManager.PlayPokemonCry(int.Parse(_sound), _crySuffix);
        }
        else
        {
            SoundManager.PlaySound(_sound, _stopMusic);
        }
    }

    public override bool IsReady => _delay <= 0.0f;
    public override bool NeedForPVPData() => true;

    public new static QueryObject FromString(String input)
    {
        String[] d = input.Split('|');
        return new PlaySoundQueryObject(
            d[0],
            bool.Parse(d[1]),
            float.Parse(d[2].Replace(".", GameController.DecSeparator)));
    }

    public override String ToString()
    {
        return "{SOUND|" + _sound + "|" + _isPokemonSound.ToNumberString() + "|" +
               _delay.ToString().Replace(GameController.DecSeparator, ".") + "}";
    }
}

// ---------------------------------------------------------------------------
// PlayMusicQueryObject
// ---------------------------------------------------------------------------

public class PlayMusicQueryObject : QueryObject
{
    private String _music = String.Empty;
    private bool _fade;

    public PlayMusicQueryObject(String music) : base(QueryTypes.PlayMusic)
    {
        _music = music;
    }

    public override void Update(BattleScreen bv2Screen)
    {
        if (_fade == true)
        {
            MusicManager.Play(_music, true);
        }
        else
        {
            MusicManager.Play(_music, true, 0.0f);
        }
    }

    public override bool IsReady => true;
    public override bool NeedForPVPData() => true;

    public new static QueryObject FromString(String input)
    {
        return new PlayMusicQueryObject(input);
    }

    public override String ToString()
    {
        return "{MUSIC|" + _music + "}";
    }
}

// ---------------------------------------------------------------------------
// ToggleMenuQueryObject
// ---------------------------------------------------------------------------

public class ToggleMenuQueryObject : QueryObject
{
    private bool _ready;
    private bool _switchTo;

    public ToggleMenuQueryObject(bool menuVisible) : base(QueryTypes.ToggleMenu)
    {
        _switchTo = menuVisible == false;
    }

    public override void Update(BattleScreen bv2Screen)
    {
        bv2Screen.BattleMenu.Visible = _switchTo;
        _ready = true;
    }

    public override bool IsReady => _ready;
    public override bool NeedForPVPData() => true;

    public new static QueryObject FromString(String input)
    {
        return new ToggleMenuQueryObject(bool.Parse(input));
    }

    public override String ToString()
    {
        return "{TOGGLEMENU|" + _switchTo.ToNumberString() + "}";
    }
}

// ---------------------------------------------------------------------------
// ToggleEntityQueryObject
// ---------------------------------------------------------------------------

public class ToggleEntityQueryObject : QueryObject
{
    private bool _own;

    public enum BattleEntities
    {
        SelfPokemon = 0,
        OpponentPokemon = 1,
    }

    private BattleEntities _entity = BattleEntities.SelfPokemon;
    private bool _done;
    private int _toggleMode;
    private String _newTexture = String.Empty;
    private int _changeType;

    private bool _changedIDs;
    private int _ownModelID = -1;
    private int _ownNPCID = -1;
    private int _oppModelID = -1;
    private int _oppNPCID = -1;

    public ToggleEntityQueryObject(bool own, String newModel,
                                   int ownModelID, int ownNPCID,
                                   int oppModelID, int oppNPCID)
        : base(QueryTypes.ToggleEntity)
    {
        _own = own;
        _entity = own ? BattleEntities.SelfPokemon : BattleEntities.OpponentPokemon;
        _newTexture = newModel;
        _changeType = 2;
        SetupIDs(ownModelID, ownNPCID, oppModelID, oppNPCID);
    }

    public ToggleEntityQueryObject(bool own, BattleEntities entity, String newTexture,
                                   int ownModelID, int ownNPCID,
                                   int oppModelID, int oppNPCID)
        : base(QueryTypes.ToggleEntity)
    {
        _own = own;
        _entity = entity;
        if (own == false)
        {
            _entity = _entity.Equals(BattleEntities.SelfPokemon)
                ? BattleEntities.OpponentPokemon
                : BattleEntities.SelfPokemon;
        }
        _changeType = 1;
        _newTexture = newTexture;
        SetupIDs(ownModelID, ownNPCID, oppModelID, oppNPCID);
    }

    public ToggleEntityQueryObject(bool own, BattleEntities entity, int toggleMode,
                                   int ownModelID, int ownNPCID,
                                   int oppModelID, int oppNPCID)
        : base(QueryTypes.ToggleEntity)
    {
        _own = own;
        _entity = entity;
        if (own == false)
        {
            _entity = _entity.Equals(BattleEntities.SelfPokemon)
                ? BattleEntities.OpponentPokemon
                : BattleEntities.SelfPokemon;
        }
        _changeType = 0;
        _toggleMode = toggleMode;
        SetupIDs(ownModelID, ownNPCID, oppModelID, oppNPCID);
    }

    private void SetupIDs(int ownModelID, int ownNPCID, int oppModelID, int oppNPCID)
    {
        _ownModelID = ownModelID;
        _ownNPCID = ownNPCID;
        _oppModelID = oppModelID;
        _oppNPCID = oppNPCID;
    }

    public override void Update(BattleScreen bv2Screen)
    {
        if (_changedIDs == false)
        {
            _changedIDs = true;
            if (_ownNPCID > -1)
            {
                bv2Screen.SelfPokemonNPC!.ID = _ownNPCID;
            }
            if (_oppNPCID > -1)
            {
                bv2Screen.OpponentPokemonNPC!.ID = _oppNPCID;
            }
        }

        switch (_changeType)
        {
            case 0:
                switch (_entity)
                {
                    case BattleEntities.SelfPokemon:
                        bv2Screen.SelfPokemonNPC!.Visible = GetVisible(bv2Screen.SelfPokemonNPC.Visible);
                        break;
                    case BattleEntities.OpponentPokemon:
                        bv2Screen.OpponentPokemonNPC!.Visible = GetVisible(bv2Screen.OpponentPokemonNPC.Visible);
                        break;
                }
                break;

            case 1:
                switch (_entity)
                {
                    case BattleEntities.SelfPokemon:
                        bv2Screen.SelfPokemonNPC!.ModelPath = String.Empty;
                        bv2Screen.SelfPokemonNPC.Model = null;
                        bv2Screen.SelfPokemonNPC.Scale = new Vector3(1);
                        bv2Screen.SelfPokemonNPC.Rotation = NPC.GetRotationFromInteger(bv2Screen.SelfPokemonNPC.faceRotation);
                        bv2Screen.SelfPokemonNPC.SetupSprite(_newTexture, String.Empty, false);
                        break;
                    case BattleEntities.OpponentPokemon:
                        bv2Screen.OpponentPokemonNPC!.ModelPath = String.Empty;
                        bv2Screen.OpponentPokemonNPC.Model = null;
                        bv2Screen.OpponentPokemonNPC.Scale = new Vector3(1);
                        bv2Screen.OpponentPokemonNPC.Rotation = NPC.GetRotationFromInteger(bv2Screen.OpponentPokemonNPC.faceRotation);
                        bv2Screen.OpponentPokemonNPC.SetupSprite(_newTexture, String.Empty, false);
                        break;
                }
                break;

            case 2:
                switch (_entity)
                {
                    case BattleEntities.SelfPokemon:
                    {
                        bv2Screen.SelfPokemonNPC!.ModelPath = _newTexture;
                        bv2Screen.SelfPokemonNPC.Model = ModelManager.GetModel(bv2Screen.SelfPokemonNPC.ModelPath);
                        bv2Screen.SelfPokemonNPC.Scale =
                            new Vector3(bv2Screen.SelfPokemon!.GetModelProperties().Item1) *
                            ModelManager.MODELSCALE *
                            ModelManager.PokeModelScale(bv2Screen.SelfPokemonNPC.ModelPath);
                        bv2Screen.SelfPokemonNPC.Rotation =
                            NPC.GetRotationFromInteger(bv2Screen.SelfPokemonNPC.faceRotation) +
                            ModelManager.PokeModelRotation(bv2Screen.SelfPokemonNPC.ModelPath);
                        break;
                    }
                    case BattleEntities.OpponentPokemon:
                    {
                        bv2Screen.OpponentPokemonNPC!.ModelPath = _newTexture;
                        bv2Screen.OpponentPokemonNPC.Model = ModelManager.GetModel(bv2Screen.OpponentPokemonNPC.ModelPath);
                        bv2Screen.OpponentPokemonNPC.Scale =
                            new Vector3(bv2Screen.OpponentPokemon!.GetModelProperties().Item1) *
                            ModelManager.MODELSCALE *
                            ModelManager.PokeModelScale(bv2Screen.OpponentPokemonNPC.ModelPath);
                        bv2Screen.OpponentPokemonNPC.Rotation =
                            NPC.GetRotationFromInteger(bv2Screen.OpponentPokemonNPC.faceRotation) +
                            ModelManager.PokeModelRotation(bv2Screen.OpponentPokemonNPC.ModelPath);
                        break;
                    }
                }
                break;
        }

        _done = true;
    }

    private bool GetVisible(bool input)
    {
        switch (_toggleMode)
        {
            case 0:
                return input == false;
            case 1:
                return true;
            case 2:
                return false;
        }
        return input;
    }

    public override bool IsReady => _done;
    public override bool NeedForPVPData() => true;

    public new static QueryObject? FromString(String input)
    {
        String[] d = input.Split('|');
        switch (int.Parse(d[0]))
        {
            case 0:
                return new ToggleEntityQueryObject(
                    bool.Parse(d[1]), (BattleEntities)int.Parse(d[2]),
                    int.Parse(d[3]), int.Parse(d[4]), int.Parse(d[5]),
                    int.Parse(d[6]), int.Parse(d[7]));
            case 1:
                return new ToggleEntityQueryObject(
                    bool.Parse(d[1]), (BattleEntities)int.Parse(d[2]),
                    d[3], int.Parse(d[4]), int.Parse(d[5]),
                    int.Parse(d[6]), int.Parse(d[7]));
            case 2:
                return new ToggleEntityQueryObject(
                    bool.Parse(d[1]), d[2],
                    int.Parse(d[3]), int.Parse(d[4]),
                    int.Parse(d[5]), int.Parse(d[6]));
        }
        return null;
    }

    public override String ToString()
    {
        String s = _changeType.ToString() + "|";
        switch (_changeType)
        {
            case 0:
                s += "0|" + (int)_entity + "|" + _toggleMode + "|" +
                     _ownModelID + "|" + _ownNPCID + "|" + _oppModelID + "|" + _oppNPCID;
                break;
            case 1:
                s += "0|" + (int)_entity + "|" + _newTexture + "|" +
                     _ownModelID + "|" + _ownNPCID + "|" + _oppModelID + "|" + _oppNPCID;
                break;
            case 2:
                s += (_own == false).ToNumberString() + "|" + _newTexture + "|" +
                     _ownModelID + "|" + _ownNPCID + "|" + _oppModelID + "|" + _oppNPCID;
                break;
        }
        return "{TOGGLEENTITY|" + s + "}";
    }
}

// ---------------------------------------------------------------------------
// CameraQueryObject
// ---------------------------------------------------------------------------

public class CameraQueryObject : QueryObject
{
    private Vector3 _targetPosition;
    private Vector3 _startPosition;
    private float _startRotationSpeed = 0.008f;
    private float _targetRotationSpeed = 0.008f;
    private float _startSpeed = 0.04f;
    private float _targetSpeed = 0.04f;
    private float _startYaw;
    private float _targetYaw;
    private float _startPitch;
    private float _targetPitch;
    private bool _applied;
    private bool _ready;

    public bool ApplyCurrentCamera;
    public bool ReplacePVP;

    public Vector3 TargetPosition { get => _targetPosition; set => _targetPosition = value; }
    public Vector3 StartPosition { get => _startPosition; set => _startPosition = value; }
    public float TargetRotationSpeed { get => _targetRotationSpeed; set => _targetRotationSpeed = value; }
    public float StartRotationSpeed { get => _startRotationSpeed; set => _startRotationSpeed = value; }
    public float TargetYaw { get => _targetYaw; set => _targetYaw = value; }
    public float StartYaw { get => _startYaw; set => _startYaw = value; }
    public float TargetPitch { get => _targetPitch; set => _targetPitch = value; }
    public float StartPitch { get => _startPitch; set => _startPitch = value; }

    public CameraQueryObject(Vector3 targetPosition, Vector3 startPosition,
                             float startSpeed, float startYaw, float startPitch)
        : base(QueryTypes.CameraMovement)
    {
        _targetPosition = targetPosition;
        _startPosition = startPosition;
        _startSpeed = startSpeed;
        _targetSpeed = startSpeed;
        _startYaw = startYaw;
        _targetYaw = startYaw;
        _startPitch = startPitch;
        _targetPitch = startPitch;
    }

    public CameraQueryObject(Vector3 targetPosition, Vector3 startPosition,
                             float targetSpeed, float startSpeed,
                             float targetYaw, float startYaw,
                             float targetPitch, float startPitch)
        : base(QueryTypes.CameraMovement)
    {
        _targetPosition = targetPosition;
        _startPosition = startPosition;
        _startSpeed = startSpeed;
        _targetSpeed = targetSpeed;
        _startYaw = startYaw;
        _targetYaw = targetYaw;
        _startPitch = startPitch;
        _targetPitch = targetPitch;
    }

    public CameraQueryObject(Vector3 targetPosition, Vector3 startPosition,
                             float targetSpeed, float startSpeed,
                             float targetYaw, float startYaw,
                             float targetPitch, float startPitch,
                             float targetRotationSpeed, float startRotationSpeed)
        : base(QueryTypes.CameraMovement)
    {
        _targetPosition = targetPosition;
        _startPosition = startPosition;
        _startSpeed = startSpeed;
        _targetSpeed = targetSpeed;
        _startYaw = startYaw;
        _targetYaw = targetYaw;
        _startPitch = startPitch;
        _targetPitch = targetPitch;
        _startRotationSpeed = startRotationSpeed;
        _targetRotationSpeed = targetRotationSpeed;
    }

    private void Apply(BattleCamera c)
    {
        c.Position = _startPosition;
        c.TargetPosition = _targetPosition;
        c.Speed = _startSpeed;
        c.TargetSpeed = _targetSpeed;
        c.Yaw = _startYaw;
        c.TargetYaw = _targetYaw;
        c.Pitch = _startPitch;
        c.TargetPitch = _targetPitch;
        c.RotationSpeed = _startRotationSpeed;
        c.TargetRotationSpeed = _targetRotationSpeed;
    }

    public override void Update(BattleScreen bv2Screen)
    {
        if (ApplyCurrentCamera == true)
        {
            ApplyCurrentCamera = false;
            StartPosition = Screen.Camera.Position;
            StartYaw = Screen.Camera.Yaw;
            StartPitch = Screen.Camera.Pitch;
            StartRotationSpeed = Screen.Camera.RotationSpeed;
        }

        if (_applied == false)
        {
            _applied = true;
            Apply((BattleCamera)Screen.Camera);
        }

        if (((BattleCamera)Screen.Camera).IsReady == true)
        {
            _ready = true;
        }
    }

    public void SetTargetToStart()
    {
        _startPitch = _targetPitch;
        _startPosition = _targetPosition;
        _startRotationSpeed = _targetRotationSpeed;
        _startSpeed = _targetSpeed;
        _startYaw = _targetYaw;
    }

    public override bool IsReady => _ready;
    public override bool UpdateCamera => true;
    public override bool NeedForPVPData() => true;

    public new static QueryObject FromString(String input)
    {
        String[] d = input.Split('|');
        CameraQueryObject c = new CameraQueryObject(
            new Vector3(S(d[0]), S(d[1]), S(d[2])),
            new Vector3(S(d[3]), S(d[4]), S(d[5])),
            S(d[6]), S(d[7]), S(d[8]), S(d[9]),
            S(d[10]), S(d[11]), S(d[12]), S(d[13]));
        c.ApplyCurrentCamera = bool.Parse(d[14]);
        c.PassThis = bool.Parse(d[15]);
        return c;
    }

    private static float S(String s) =>
        float.Parse(s.Replace(".", GameController.DecSeparator));

    private static String E(float v) =>
        v.ToString().Replace(GameController.DecSeparator, ".");

    public override String ToString()
    {
        return "{CAMERA|" +
               E(_targetPosition.X) + "|" + E(_targetPosition.Y) + "|" + E(_targetPosition.Z) + "|" +
               E(_startPosition.X) + "|" + E(_startPosition.Y) + "|" + E(_startPosition.Z) + "|" +
               E(_targetSpeed) + "|" + E(_startSpeed) + "|" +
               E(_targetYaw) + "|" + E(_startYaw) + "|" +
               E(_targetPitch) + "|" + E(_startPitch) + "|" +
               E(_targetRotationSpeed) + "|" + E(_startRotationSpeed) + "|" +
               ApplyCurrentCamera.ToNumberString() + "|" +
               PassThis.ToNumberString() + "}";
    }
}

// ---------------------------------------------------------------------------
// ScreenFadeQueryObject
// ---------------------------------------------------------------------------

public class ScreenFadeQueryObject : QueryObject
{
    public enum FadeTypes
    {
        Horizontal,
        Vertical,
        CloseRight,
        CloseLeft,
    }

    public Color _color;
    public FadeTypes _fadeType;

    private bool _ready;
    private int _current;
    private int _goal;
    private int _animationSpeed = 6;
    private bool _changedOverlay;
    private bool _appear;

    public ScreenFadeQueryObject(FadeTypes fadeType, Color c, bool appear, int speed)
        : base(QueryTypes.ScreenFade)
    {
        _fadeType = fadeType;
        _color = c;
        _animationSpeed = speed;
        _appear = appear;
        Initialize();
    }

    private void Initialize()
    {
        switch (_fadeType)
        {
            case FadeTypes.Horizontal:
                if (_appear == true)
                {
                    _goal = Core.windowSize.Width / 2;
                    _current = 0;
                }
                else
                {
                    _goal = 0;
                    _current = Core.windowSize.Width / 2;
                }
                break;

            case FadeTypes.Vertical:
                if (_appear == true)
                {
                    _goal = Core.windowSize.Height / 2;
                    _current = 0;
                }
                else
                {
                    _goal = 0;
                    _current = Core.windowSize.Height / 2;
                }
                break;

            case FadeTypes.CloseLeft:
            case FadeTypes.CloseRight:
                if (_appear == true)
                {
                    _goal = Core.windowSize.Width;
                    _current = 0;
                }
                else
                {
                    _goal = 0;
                    _current = Core.windowSize.Width;
                }
                break;
        }
    }

    public override void Draw(BattleScreen bv2Screen)
    {
        if (_appear == false && bv2Screen.DrawColoredScreen == true)
        {
            ChangeOverlay(bv2Screen);
        }

        switch (_fadeType)
        {
            case FadeTypes.Vertical:
                Canvas.DrawRectangle(new Rectangle(0, 0, Core.windowSize.Width, _current), _color);
                Canvas.DrawRectangle(new Rectangle(0, Core.windowSize.Height - _current, Core.windowSize.Width, _current), _color);
                break;
            case FadeTypes.Horizontal:
                Canvas.DrawRectangle(new Rectangle(0, 0, _current, Core.windowSize.Height), _color);
                Canvas.DrawRectangle(new Rectangle(Core.windowSize.Width - _current, 0, _current, Core.windowSize.Height), _color);
                break;
            case FadeTypes.CloseLeft:
                Canvas.DrawRectangle(new Rectangle(Core.windowSize.Width - _current, 0, _current, Core.windowSize.Height), _color);
                break;
            case FadeTypes.CloseRight:
                Canvas.DrawRectangle(new Rectangle(0, 0, _current, Core.windowSize.Height), _color);
                break;
        }
    }

    private void ChangeOverlay(BattleScreen bv2Screen)
    {
        if (_changedOverlay == false)
        {
            bv2Screen.DrawColoredScreen = bv2Screen.DrawColoredScreen == false;
            _changedOverlay = true;
        }
    }

    public override void Update(BattleScreen bv2Screen)
    {
        if (_ready == true)
        {
            return;
        }

        if (_current < _goal)
        {
            _current += GetAnimationSpeed();
            if (_current >= _goal)
            {
                _ready = true;
                if (_appear == true)
                {
                    ChangeOverlay(bv2Screen);
                }
            }
        }
        else
        {
            _current -= GetAnimationSpeed();
            if (_current <= _goal)
            {
                _ready = true;
                if (_appear == true)
                {
                    ChangeOverlay(bv2Screen);
                }
            }
        }
    }

    private int GetAnimationSpeed()
    {
        float multiplier = 1.0f;
        switch (_fadeType)
        {
            case FadeTypes.CloseLeft:
            case FadeTypes.CloseRight:
            case FadeTypes.Horizontal:
                multiplier = Core.windowSize.Width / 1200.0f;
                break;
            case FadeTypes.Vertical:
                multiplier = Core.windowSize.Height / 680.0f;
                break;
        }
        int s = (int)(multiplier * _animationSpeed);
        if (s < 1)
        {
            s = 1;
        }
        return s;
    }

    public override bool IsReady => _ready;
    public override bool NeedForPVPData() => true;

    public new static QueryObject FromString(String input)
    {
        String[] d = input.Split('|');
        FadeTypes fadeType = (FadeTypes)int.Parse(d[0]);
        ScreenFadeQueryObject q = new ScreenFadeQueryObject(
            fadeType,
            new Color(int.Parse(d[1]), int.Parse(d[2]), int.Parse(d[3])),
            bool.Parse(d[4]),
            int.Parse(d[5]));
        q.PassThis = bool.Parse(d[6]);
        return q;
    }

    public override String ToString()
    {
        return "{FADE|" +
               (int)_fadeType + "|" +
               _color.R + "|" + _color.G + "|" + _color.B + "|" +
               _appear.ToNumberString() + "|" +
               _animationSpeed + "|" +
               PassThis.ToNumberString() + "}";
    }
}

// ---------------------------------------------------------------------------
// MathHPQueryObject
// ---------------------------------------------------------------------------

public class MathHPQueryObject : QueryObject
{
    private const int HP_BAR_WIDTH = 300;
    private const int HP_BAR_HEIGHT = 30;
    private const int HP_BAR_BORDER = 4;
    private const int HP_BAR_BORDER_OFFSET = 2;
    private const int HP_SCALE = 1000;
    private const int HP_STEP = 8;
    private const int PARTICLE_SIZE = 4;

    private int _current;
    private int _max;
    private int _damage;
    private int _target;
    private int _amount = HP_SCALE;
    private float _delay = 1.0f;
    private int _startAmount = HP_SCALE;
    private Vector2 _position;

    private List<HpParticle> _particles = [];
    private Vector2 _shake = Vector2.Zero;

    public MathHPQueryObject(int current, int max, int damage, Vector2 position)
        : base(QueryTypes.MathHP)
    {
        _current = current;
        _max = max;
        _damage = damage;
        _amount = (int)((HP_SCALE / (float)max) * current);
        _startAmount = _amount;

        int endState = current - damage;
        if (endState < 0)
        {
            endState = 0;
        }
        _target = (int)((HP_SCALE / (float)max) * endState);
        _position = position;
    }

    public override void Update(BattleScreen bv2Screen)
    {
        if (_target < _amount)
        {
            _amount -= HP_STEP;
            if (_amount < _target)
            {
                _amount = _target;
            }
            _particles.Add(new HpParticle(
                new Vector2((float)((_amount / (float)HP_SCALE) * HP_BAR_WIDTH + _position.X),
                            _position.Y + Core.Random.Next(0, 34)),
                GetColor()));
            _shake = new Vector2(
                _shake.X + (Core.Random.Next(0, 3) - 1),
                _shake.Y + (Core.Random.Next(0, 3) - 1));
        }
        if (_target > _amount)
        {
            _amount += HP_STEP;
            if (_amount > _target)
            {
                _amount = _target;
            }
        }
        if (_target.Equals(_amount) == true)
        {
            if (_delay > 0.0f)
            {
                _delay -= 0.01f;
                if (_delay <= 0.0f)
                {
                    _delay = 0.0f;
                }
                if (Controls.Accept(true, true) == true)
                {
                    _delay = 0.0f;
                }
            }
        }
    }

    public override void Draw(BattleScreen bv2Screen)
    {
        Canvas.DrawRectangle(
            new Rectangle((int)(_position.X + _shake.X), (int)(_position.Y + _shake.Y),
                          HP_BAR_WIDTH, HP_BAR_HEIGHT),
            Color.White);

        if (_amount >= _target)
        {
            Canvas.DrawScrollBar(
                new Vector2(_position.X + _shake.X, _position.Y + _shake.Y),
                HP_SCALE, _startAmount, 0,
                new Size(HP_BAR_WIDTH, HP_BAR_HEIGHT), true,
                new Color(0, 0, 0, 0), Color.DarkGray);
        }

        Canvas.DrawScrollBar(
            new Vector2(_position.X + _shake.X, _position.Y + _shake.Y),
            HP_SCALE, _amount, 0,
            new Size(HP_BAR_WIDTH, HP_BAR_HEIGHT), true,
            new Color(0, 0, 0, 0), GetColor());

        Canvas.DrawBorder(
            HP_BAR_BORDER,
            new Rectangle(
                (int)(_position.X - HP_BAR_BORDER_OFFSET + _shake.X),
                (int)(_position.Y - HP_BAR_BORDER_OFFSET + _shake.Y),
                HP_BAR_WIDTH + HP_BAR_BORDER, HP_BAR_HEIGHT + HP_BAR_BORDER),
            Color.Gray);

        for (int i = 0; i < _particles.Count; i++)
        {
            if (i <= _particles.Count - 1)
            {
                HpParticle p = _particles[i];
                p.Draw(_shake);
                if (p.CanBeRemoved == true)
                {
                    _particles.RemoveAt(i);
                    i -= 1;
                }
            }
        }
    }

    private Color GetColor()
    {
        int percent = (int)((_amount / (float)HP_SCALE) * 100);
        if (percent > 75)
        {
            return new Color(64, 191, 0);
        }
        if (percent > 50)
        {
            return new Color(157, 191, 0);
        }
        if (percent > 25)
        {
            return new Color(222, 191, 0);
        }
        return new Color(192, 63, 0);
    }

    public override bool IsReady => _amount.Equals(_target) && _delay.Equals(0.0f);
    public override bool NeedForPVPData() => true;

    public new static QueryObject FromString(String input)
    {
        String[] d = input.Split('|');
        return new MathHPQueryObject(
            int.Parse(d[0]), int.Parse(d[1]), int.Parse(d[2]),
            new Vector2(F(d[3]), F(d[4])));
    }

    private static float F(String s) =>
        float.Parse(s.Replace(".", GameController.DecSeparator));

    private static String E(float v) =>
        v.ToString().Replace(GameController.DecSeparator, ".");

    public override String ToString()
    {
        return "{MATHHP|" +
               _current + "|" + _max + "|" + _damage + "|" +
               E(_position.X) + "|" + E(_position.Y) + "}";
    }

    private class HpParticle
    {
        public float Delay;
        public Vector2 Position;
        public Color Color;
        public bool CanBeRemoved;

        public HpParticle(Vector2 position, Color color)
        {
            Position = position;
            Color = color;
            Delay = 0.15f;
        }

        public void Draw(Vector2 offset)
        {
            Delay -= 0.01f;
            if (Delay <= 0.0f)
            {
                Delay = 0.0f;
                CanBeRemoved = true;
            }
            Canvas.DrawRectangle(
                new Rectangle(
                    (int)(Position.X + offset.X), (int)(Position.Y + offset.Y),
                    PARTICLE_SIZE, PARTICLE_SIZE),
                Color);
        }
    }
}

// ---------------------------------------------------------------------------
// RoamingPokemonFledQueryObject
// ---------------------------------------------------------------------------

public class RoamingPokemonFledQueryObject : QueryObject
{
    private bool _ready;

    public RoamingPokemonFledQueryObject() : base(QueryTypes.RoamingPokemonFled)
    {
    }

    public override void Update(BattleScreen bv2Screen)
    {
        bv2Screen.FieldEffects.RoamingFled = true;
        _ready = true;
    }

    public override bool IsReady => _ready;
}

// ---------------------------------------------------------------------------
// ChoosePokemonQueryObject
// ---------------------------------------------------------------------------

public class ChoosePokemonQueryObject : QueryObject
{
    private bool _ready;

    public ChoosePokemonQueryObject() : base(QueryTypes.ChoosePokemon)
    {
    }

    public override void Update(BattleScreen bv2Screen)
    {
    }

    public override bool IsReady => _ready;
}

// ---------------------------------------------------------------------------
// AfterFaintQueryObject
// ---------------------------------------------------------------------------

public class AfterFaintQueryObject : QueryObject
{
    private bool _isHost;
    private bool _ready;

    public AfterFaintQueryObject(bool isHost) : base(QueryTypes.AfterFaint)
    {
        _isHost = isHost;
    }

    public override void Update(BattleScreen bv2Screen)
    {
        if (bv2Screen.IsHost == true)
        {
            if (_isHost == true)
            {
                bv2Screen.OwnFaint = true;
            }
            else
            {
                bv2Screen.OppFaint = true;
            }
        }
        else
        {
            if (_isHost == true)
            {
                Logger.Debug("[Battle]: The host's pokemon faints");
                bv2Screen.OppFaint = true;
            }
            else
            {
                Logger.Debug("[Battle]: The client's pokemon faints");
                bv2Screen.OwnFaint = true;
            }
        }
        _ready = true;
    }

    public override bool IsReady => _ready;
    public override bool NeedForPVPData() => false;

    public new static QueryObject FromString(String input)
    {
        return new AfterFaintQueryObject(bool.Parse(input));
    }

    public override String ToString()
    {
        return "{FAINT|" + _isHost.ToString() + "}";
    }
}

// ---------------------------------------------------------------------------
// StartRoundQueryObject
// ---------------------------------------------------------------------------

public class StartRoundQueryObject : QueryObject
{
    private bool _ready;

    public StartRoundQueryObject() : base(QueryTypes.StartRound)
    {
    }

    public override void Update(BattleScreen bv2Screen)
    {
        bv2Screen.Battle.StartRound(bv2Screen);
        _ready = true;
    }

    public override bool IsReady => _ready;
    public override bool NeedForPVPData() => true;

    public new static QueryObject FromString(String input)
    {
        return new StartRoundQueryObject();
    }

    public override String ToString()
    {
        return "{STARTROUND|0}";
    }
}

// ---------------------------------------------------------------------------
// TriggerNewRoundPVPQueryObject
// ---------------------------------------------------------------------------

public class TriggerNewRoundPVPQueryObject : QueryObject
{
    private bool _ready;

    public TriggerNewRoundPVPQueryObject() : base(QueryTypes.TriggerNewRound)
    {
    }

    public override void Update(BattleScreen bv2Screen)
    {
        if (BattleScreen.FirstRound == false)
        {
            BattleScreen.ReceivedInput = String.Empty;
        }
        bv2Screen.SentHostData = false;
        BattleScreen.ReceivedQuery = String.Empty;
        bv2Screen.SentInput = false;

        if (bv2Screen.IsHost == false)
        {
            bv2Screen.BattleMenu.Visible = true;
            bv2Screen.ClientWaitForData = true;
        }
        else
        {
            bv2Screen.SendEndRoundData();
        }

        _ready = true;
    }

    public override bool IsReady => _ready;
    public override bool NeedForPVPData() => true;

    public new static QueryObject FromString(String input)
    {
        return new TriggerNewRoundPVPQueryObject();
    }

    public override String ToString()
    {
        return "{TRIGGERNEWROUNDPVP| }";
    }
}

// ---------------------------------------------------------------------------
// DisplayLevelUpQueryObject
// ---------------------------------------------------------------------------

public class DisplayLevelUpQueryObject : QueryObject
{
    private Pokemon _pokemon;
    private int[] _oldStats;

    public DisplayLevelUpQueryObject(Pokemon p, int[] oldStats) : base(QueryTypes.DisplayLevelUp)
    {
        _pokemon = Pokemon.GetPokemonByData(p.GetSaveData());
        _oldStats = oldStats;
    }

    public override void Update(BattleScreen bv2Screen)
    {
        Core.SetScreen(new BattleGrowStatsScreen(Core.CurrentScreen, _pokemon, _oldStats));
    }
}

// ---------------------------------------------------------------------------
// LearnMovesQueryObject
// ---------------------------------------------------------------------------

public class LearnMovesQueryObject : QueryObject
{
    private Pokemon _pokemon;
    private Attack _attack;
    private bool _hasAttack;

    public static int AddedAttacks;

    public LearnMovesQueryObject(Pokemon p, Attack a, BattleScreen bv2Screen)
        : base(QueryTypes.LearnMoves)
    {
        _pokemon = p;
        _attack = a;

        foreach (Attack existing in p.Attacks)
        {
            if (existing.ID.Equals(a.ID) == true)
            {
                _hasAttack = true;
                break;
            }
        }

        if (p.Attacks.Count + AddedAttacks < 4 && _hasAttack == false)
        {
            AddedAttacks += 1;
            bv2Screen.BattleQuery.Add(new PlaySoundQueryObject("success_small", false));
            bv2Screen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " learned " + a.Name + "!"));
            PlayerStatistics.Track("Moves learned", 1);
        }
    }

    public override void Update(BattleScreen bv2Screen)
    {
        if (_hasAttack == false)
        {
            if (_pokemon.Attacks.Count < 4)
            {
                _pokemon.Attacks.Add(_attack);
            }
            else
            {
                Core.SetScreen(new LearnAttackScreen(Core.CurrentScreen, _pokemon, _attack));
            }
        }
    }

    public override bool IsReady => true;

    public static void ClearCache()
    {
        AddedAttacks = 0;
    }
}

// ---------------------------------------------------------------------------
// SwitchPokemonQueryObject
// ---------------------------------------------------------------------------

public class SwitchPokemonQueryObject : QueryObject
{
    private bool _ready;

    // Text display
    private String _text = String.Empty;
    private Color _textColor = Color.White;
    private int _textIndex;
    private float _textDelay = 0.015f;

    private bool TextReady => _textIndex >= _text.Length;

    private void TransformText(String text)
    {
        _text = text;
        _text = _text.Replace("*", " ");
        _text = _text.Replace("~", " ");
        _text = _text.Replace("<player.name>", Core.Player.Name);
        _text = _text.Replace("<playername>", Core.Player.Name);
        _text = _text.Replace("<rival.name>", Core.Player.RivalName);
        _text = _text.Replace("<rivalname>", Core.Player.RivalName);
        _text = _text.Replace("[POKE]", "Poké");
    }

    private void UpdateText()
    {
        if (TextReady == true)
        {
            return;
        }
        _textDelay -= 0.01f;
        if (_textDelay <= 0.0f)
        {
            _textDelay = 0.015f;
            _textIndex += 1;
        }
        if (Controls.Accept(true, true) == true && _textIndex > 2)
        {
            _textIndex = _text.Length;
        }
    }

    private void DrawText(BattleScreen bv2Screen)
    {
        Rectangle rec = new Rectangle(
            100, Core.windowSize.Height - 250,
            Core.windowSize.Width - 200, 200);

        Canvas.DrawRectangle(rec, new Color(0, 0, 0, 150));

        String text = _text.Substring(0, _textIndex);
        text = text.CropStringToWidth(FontManager.InGameFont, Core.windowSize.Width - 300);

        Core.SpriteBatch.DrawString(
            FontManager.InGameFont, text,
            new Vector2(rec.X + 20, rec.Y + 20), Color.White);

        if (GamePad.GetState(PlayerIndex.One).IsConnected == true &&
            Core.GameOptions.GamePadEnabled == true &&
            bv2Screen.IsCurrentScreen() == true)
        {
            Dictionary<Buttons, String> d = [];
            d.Add(Buttons.A, Localization.GetString("global_ok", "OK"));
            bv2Screen.DrawGamePadControls(d, new Vector2(rec.X + rec.Width - 100, rec.Y + rec.Height - 40));
        }
        else
        {
            if (TextReady == true)
            {
                String okText = Localization.GetString("global_ok", "OK");
                Core.SpriteBatch.DrawString(
                    FontManager.InGameFont, okText,
                    new Vector2(
                        rec.X + rec.Width - FontManager.InGameFont.MeasureString(okText).X - 20,
                        rec.Y + rec.Height - FontManager.InGameFont.MeasureString(okText).Y - 5),
                    Color.White);
            }
        }
    }

    // Choose UI
    private int _chooseIndex;
    private float _delay = 2.0f;
    private BattleScreen _tempScreen = null!;

    private void UpdateChoose()
    {
        if (Controls.Down(true, true, true, true, true) == true)
        {
            _chooseIndex += 1;
        }
        if (Controls.Up(true, true, true, true, true) == true)
        {
            _chooseIndex -= 1;
        }

        _chooseIndex = _chooseIndex.Clamp(0, 1);

        Rectangle rec = new Rectangle(
            Core.windowSize.Width - 250, Core.windowSize.Height - 450, 150, 150);

        if (rec.Contains(MouseHandler.MousePosition) == false)
        {
            if (Controls.Accept(true, true, true) == true)
            {
                if (_chooseIndex.Equals(0) == true)
                {
                    OpenPartyScreen();
                }
                else
                {
                    ChooseNotSwitch();
                }
            }
        }
        else
        {
            if (Controls.Accept(false, true, true) == true)
            {
                if (_chooseIndex.Equals(0) == true)
                {
                    OpenPartyScreen();
                }
                else
                {
                    ChooseNotSwitch();
                }
            }
            if (Controls.Accept(true, false, false) == true)
            {
                if (new Rectangle(Core.windowSize.Width - 213, Core.windowSize.Height - 438, 80, 50)
                    .Contains(MouseHandler.MousePosition) == true)
                {
                    _chooseIndex = 0;
                    OpenPartyScreen();
                }
                if (new Rectangle(Core.windowSize.Width - 213, Core.windowSize.Height - 378, 80, 50)
                    .Contains(MouseHandler.MousePosition) == true)
                {
                    _chooseIndex = 1;
                    ChooseNotSwitch();
                }
            }
        }

        if (Controls.Dismiss(true, true, true) == true)
        {
            ChooseNotSwitch();
        }
    }

    private void OpenPartyScreen()
    {
        PartyScreen selScreen = new PartyScreen(
            Core.CurrentScreen,
            Item.GetItemByID("5"),
            i => { ChoosePokemon(i); return true; },
            Localization.GetString("battle_choose_pokemon"),
            false);
        selScreen.Mode = Screens.UI.ISelectionScreen.ScreenMode.Selection;
        selScreen.CanExit = true;
        selScreen.SelectedObject += ChoosePokemonHandler;
        SoundManager.PlaySound("select");
        Core.SetScreen(selScreen);
    }

    private void ChooseNotSwitch()
    {
        SoundManager.PlaySound("select");
        _tempScreen.BattleQuery.Clear();
        FinishOppSwitchAnimation(_tempScreen);
        FinishOppSwitchEffects(_tempScreen);
        ScreenFadeQueryObject cq1 = new ScreenFadeQueryObject(ScreenFadeQueryObject.FadeTypes.Vertical, Color.Black, true, 16);
        ScreenFadeQueryObject cq2 = new ScreenFadeQueryObject(ScreenFadeQueryObject.FadeTypes.Vertical, Color.Black, false, 16);
        cq2.PassThis = true;
        _tempScreen.BattleQuery.AddRange([cq1, cq2]);
        _tempScreen.HasSwitchedOwn = false;
        _tempScreen.ShiftCanContinue = true;
        _tempScreen.BattleQuery.Add(new StartRoundQueryObject());
        _ready = true;
    }

    private void DrawChoose()
    {
        Rectangle rec = new Rectangle(
            Core.windowSize.Width - 250, Core.windowSize.Height - 450, 150, 150);
        Canvas.DrawRectangle(rec, new Color(0, 0, 0, 150));

        if (_chooseIndex.Equals(0) == true)
        {
            Canvas.DrawRectangle(
                new Rectangle(Core.windowSize.Width - 213, Core.windowSize.Height - 438, 80, 50),
                Color.White);
            Core.SpriteBatch.DrawString(FontManager.InGameFont,
                Localization.GetString("global_yes"),
                new Vector2(Core.windowSize.Width - 200, Core.windowSize.Height - 430),
                Color.Black);
            Core.SpriteBatch.DrawString(FontManager.InGameFont,
                Localization.GetString("global_no"),
                new Vector2(Core.windowSize.Width - 200, Core.windowSize.Height - 370),
                Color.White);
        }
        else
        {
            Canvas.DrawRectangle(
                new Rectangle(Core.windowSize.Width - 213, Core.windowSize.Height - 378, 80, 50),
                Color.White);
            Core.SpriteBatch.DrawString(FontManager.InGameFont,
                Localization.GetString("global_yes"),
                new Vector2(Core.windowSize.Width - 200, Core.windowSize.Height - 430),
                Color.White);
            Core.SpriteBatch.DrawString(FontManager.InGameFont,
                Localization.GetString("global_no"),
                new Vector2(Core.windowSize.Width - 200, Core.windowSize.Height - 370),
                Color.Black);
        }
    }

    private void ChoosePokemonHandler(Object[] parms)
    {
        ChoosePokemon((int)parms[0]);
    }

    private void ChoosePokemon(int pokeIndex)
    {
        Pokemon pokemon = Core.Player.Pokemons[pokeIndex];
        if (pokeIndex.Equals(_tempScreen.SelfPokemonIndex) == true)
        {
            Screen.TextBox.Show(pokemon.GetDisplayName() + " " +
                Localization.GetString("battle_switch_already_in_battle", "is already~in battle!"),
                [], true, false);
            return;
        }

        if (pokemon.IsEgg == true)
        {
            Screen.TextBox.Show(
                Localization.GetString("battle_switch_egg", "Cannot switch in~the egg!"),
                [], true, false);
            return;
        }

        if (pokemon.Status.Equals(Pokemon.StatusProblems.Fainted) == true)
        {
            Screen.TextBox.Show(pokemon.GetDisplayName() + " " +
                Localization.GetString("battle_switch_fainted", "is fainted!"),
                [], true, false);
            return;
        }

        if (BattleCalculation.CanSwitch(_tempScreen, true) == false)
        {
            Screen.TextBox.Show(
                Localization.GetString("battle_cannot_switch", "Cannot switch out."),
                [], true, false);
            return;
        }

        if (pokeIndex.Equals(_tempScreen.SelfPokemonIndex) == true)
        {
            return;
        }

        _tempScreen.ParticipatedPokemon.Clear();
        _tempScreen.ParticipatedPokemon.Add(pokeIndex);

        if (_tempScreen.IsRemoteBattle == true && _tempScreen.IsHost == false)
        {
            _tempScreen.OppFaint = false;
            _tempScreen.SelfStatistics.Switches += 1;
            _tempScreen.BattleQuery.Clear();
            FinishOppSwitchAnimation(_tempScreen);
            _tempScreen.Battle.SwitchOutSelf(_tempScreen, pokeIndex, -1);
            FinishOppSwitchEffects(_tempScreen);
        }
        else
        {
            _tempScreen.BattleQuery.Clear();
            FinishOppSwitchAnimation(_tempScreen);
            _tempScreen.Battle.SwitchOutSelf(_tempScreen, pokeIndex, -1);
            FinishOppSwitchEffects(_tempScreen);
        }

        ScreenFadeQueryObject cq1 = new ScreenFadeQueryObject(ScreenFadeQueryObject.FadeTypes.Vertical, Color.Black, true, 16);
        ScreenFadeQueryObject cq2 = new ScreenFadeQueryObject(ScreenFadeQueryObject.FadeTypes.Vertical, Color.Black, false, 16);
        cq2.PassThis = true;
        _tempScreen.BattleQuery.AddRange([cq1, cq2]);
        _tempScreen.HasSwitchedOwn = true;
        _tempScreen.ShiftCanContinue = true;
        _tempScreen.BattleQuery.Add(new StartRoundQueryObject());
        _ready = true;
    }

    public SwitchPokemonQueryObject(BattleScreen battleScreen, Pokemon newPokemon)
        : base(QueryTypes.SwitchPokemon)
    {
        _tempScreen = battleScreen;

        String secondPart = Localization.GetString("battle_trainer_about_to_send_out_2");
        if ("!?¡¿-,;.'\"~".Contains(secondPart[0]) == false)
        {
            secondPart = " " + secondPart;
        }

        TransformText(battleScreen.Trainer!.Name + " " +
            Localization.GetString("battle_trainer_about_to_send_out_1") + " " +
            newPokemon.GetDisplayName() + secondPart);
    }

    public void FinishOppSwitchAnimation(BattleScreen battleScreen)
    {
        String oppModel = battleScreen.GetModelName(false);

        if (String.IsNullOrEmpty(oppModel) == true)
        {
            battleScreen.BattleQuery.Add(new ToggleEntityQueryObject(
                true, ToggleEntityQueryObject.BattleEntities.OpponentPokemon,
                PokemonForms.GetOverworldSpriteName(battleScreen.OpponentPokemon!, true),
                -1, -1, 0, 1));
        }
        else
        {
            battleScreen.BattleQuery.Add(new ToggleEntityQueryObject(false, oppModel, -1, -1, 1, 0));
        }

        float sendBallOffsetY = 0.0f;
        float sendPokeOffsetY = 0.0f;
        if (battleScreen.OpponentPokemonNPC!.Model != null)
        {
            sendBallOffsetY = 0.5f;
            sendPokeOffsetY = -0.5f;
        }

        battleScreen.BattleQuery.Add(new ToggleEntityQueryObject(
            true, ToggleEntityQueryObject.BattleEntities.OpponentPokemon, 1, -1, -1, -1, -1));

        if (Core.Player.ShowBattleAnimations.Equals(0) == true || battleScreen.IsPVPBattle == true)
        {
            battleScreen.BattleQuery.Add(new PlaySoundQueryObject(
                battleScreen.OpponentPokemon!.Number.ToString(), true));
        }

        battleScreen.BattleQuery.Add(new TextQueryObject(
            battleScreen.Trainer!.Name + ": \"Go, " +
            battleScreen.OpponentPokemon!.GetDisplayName() + "!\""));

        AnimationQueryObject ballThrow = new AnimationQueryObject(
            battleScreen.OpponentPokemonNPC, false);

        if (Core.Player.ShowBattleAnimations.Equals(0) == false && battleScreen.IsPVPBattle == false)
        {
            ballThrow.AnimationPlaySound("Battle\\Pokeball\\Throw", 0, 0);
            Entity ballEntity = ballThrow.SpawnEntity(
                new Vector3(2, -0.15f, 0),
                battleScreen.OpponentPokemon.catchBall?.Texture ?? TextureManager.GetTexture("Items\\5"),
                new Vector3(0.3f), 1.0f);
            ballThrow.AnimationMove(ballEntity, true, 0, 0.35f + sendBallOffsetY, 0,
                                   0.1f, false, true, 0.0f, 0.5f);
            ballThrow.AnimationPlaySound("Battle\\Pokeball\\Open", 3, 0);

            int smokeSpawned = 0;
            while (smokeSpawned <= 38)
            {
                Vector3 smokePos = new Vector3(0, 0.35f + sendBallOffsetY, 0);
                Vector3 smokeDest = new Vector3(
                    Core.Random.Next(-10, 10) / 10.0f,
                    Core.Random.Next(-10, 10) / 10.0f + sendBallOffsetY,
                    Core.Random.Next(-10, 10) / 10.0f);
                Texture2D smokeTexture = TextureManager.GetTexture("Textures\\Battle\\Smoke");
                Vector3 smokeScale = new Vector3(Core.Random.Next(2, 6) / 10.0f);
                float smokeSpeed = Core.Random.Next(1, 3) / 20.0f;
                Entity smokeEntity = ballThrow.SpawnEntity(smokePos, smokeTexture, smokeScale, 1.0f, 3.0f);
                ballThrow.AnimationMove(smokeEntity, true,
                    smokeDest.X, smokeDest.Y, smokeDest.Z,
                    smokeSpeed, false, false, 3.0f, 0.0f);
                smokeSpawned += 1;
            }
        }
        else
        {
            battleScreen.Battle.ChangeCameraAngle(1, false, battleScreen);
        }

        String crySuffix = PokemonForms.GetCrySuffix(battleScreen.OpponentPokemon!);
        if (Core.Player.ShowBattleAnimations.Equals(0) == false && battleScreen.IsPVPBattle == false)
        {
            ballThrow.AnimationSetPosition(null, false, 15, 0.5f, 13, 0, 0);
            ballThrow.AnimationFade(null, false, 1, 1.0f, 3, 0);
            ballThrow.AnimationPlaySound(
                battleScreen.OpponentPokemon.Number.ToString(), 4, 0, false, true, crySuffix);
            ballThrow.AnimationMove(null, false, 0, -0.5f + sendPokeOffsetY, 0, 0.05f, false, false, 5, 0);
            battleScreen.BattleQuery.Add(ballThrow);
        }

        battleScreen.TrainerSendOutOpp += 1;
        if (battleScreen.Trainer!.CountUseablePokemon > 1)
        {
            if (battleScreen.Trainer.SendOutXOppMessage.ContainsKey(battleScreen.TrainerSendOutOpp) == true)
            {
                QueryObject s1 = battleScreen.FocusOppPlayer();
                TextQueryObject s2 = new TextQueryObject(
                    ScriptVersion2.ScriptCommander.Parse(
                        battleScreen.Trainer.SendOutXOppMessage[battleScreen.TrainerSendOutOpp])
                    .ToString());
                battleScreen.BattleQuery.AddRange([s1, s2]);
            }
        }
        else
        {
            if (String.IsNullOrEmpty(battleScreen.Trainer.SendOutLastOppMessage) == false)
            {
                QueryObject s1 = battleScreen.FocusOppPlayer();
                TextQueryObject s2 = new TextQueryObject(
                    ScriptVersion2.ScriptCommander.Parse(battleScreen.Trainer.SendOutLastOppMessage)
                    .ToString());
                battleScreen.BattleQuery.AddRange([s1, s2]);
            }
        }
    }

    public void FinishOppSwitchEffects(BattleScreen battleScreen)
    {
        Pokemon p = battleScreen.OpponentPokemon!;
        bool spikeAffected = battleScreen.FieldEffects.IsGrounded(false, battleScreen);

        if (spikeAffected == true)
        {
            if (battleScreen.FieldEffects.Spikes.Self > 0 &&
                "magic guard".Equals(p.Ability.Name.ToLower()) == false)
            {
                double spikeDamage = 1.0;
                switch (battleScreen.FieldEffects.Spikes.Self)
                {
                    case 1:
                        spikeDamage = (p.MaxHP / 100.0) * 12.5;
                        break;
                    case 2:
                        spikeDamage = (p.MaxHP / 100.0) * 16.7;
                        break;
                    case 3:
                        spikeDamage = (p.MaxHP / 100.0) * 25.0;
                        break;
                }
                battleScreen.Battle.ReduceHP(
                    (int)spikeDamage, false, true, battleScreen,
                    "The Spikes hurt " + p.GetDisplayName() + "!", "spikes");
            }

            if (battleScreen.FieldEffects.StickyWeb.Self > 0)
            {
                battleScreen.Battle.LowerStat(
                    false, false, battleScreen, "Speed", 1,
                    "The opposing pokemon was caught in a Sticky Web!", "stickyweb");
            }

            if (battleScreen.FieldEffects.ToxicSpikes.Self > 0 &&
                p.Status.Equals(Pokemon.StatusProblems.None) == true &&
                p.Type1.Type.Equals(Element.Types.Poison) == false &&
                p.Type2.Type.Equals(Element.Types.Poison) == false)
            {
                switch (battleScreen.FieldEffects.ToxicSpikes.Self)
                {
                    case 1:
                        battleScreen.Battle.InflictPoison(
                            false, true, battleScreen, false,
                            "The Toxic Spikes hurt " + p.GetDisplayName() + "!", "toxicspikes");
                        break;
                    case 2:
                        battleScreen.Battle.InflictPoison(
                            false, true, battleScreen, true,
                            "The Toxic Spikes hurt " + p.GetDisplayName() + "!", "toxicspikes");
                        break;
                }
            }

            if (battleScreen.FieldEffects.ToxicSpikes.Self > 0)
            {
                if (p.Type1.Type.Equals(Element.Types.Poison) == true ||
                    p.Type2.Type.Equals(Element.Types.Poison) == true)
                {
                    battleScreen.BattleQuery.Add(new TextQueryObject(
                        p.GetDisplayName() + " removed the Toxic Spikes!"));
                    battleScreen.FieldEffects.ToxicSpikes.Self = 0;
                }
            }
        }

        if (battleScreen.FieldEffects.StealthRock.Self > 0 &&
            "magic guard".Equals(p.Ability.Name.ToLower()) == false)
        {
            double rocksDamage = 1.0;
            float effectiveness =
                BattleCalculation.ReverseTypeEffectiveness(
                    Element.GetElementMultiplier(new Element(Element.Types.Rock), p.Type1)) *
                BattleCalculation.ReverseTypeEffectiveness(
                    Element.GetElementMultiplier(new Element(Element.Types.Rock), p.Type2));

            switch (effectiveness)
            {
                case 0.25f:
                    rocksDamage = (p.MaxHP / 100.0) * 3.125;
                    break;
                case 0.5f:
                    rocksDamage = (p.MaxHP / 100.0) * 6.25;
                    break;
                case 1.0f:
                    rocksDamage = (p.MaxHP / 100.0) * 12.5;
                    break;
                case 2.0f:
                    rocksDamage = (p.MaxHP / 100.0) * 25.0;
                    break;
                case 4.0f:
                    rocksDamage = (p.MaxHP / 100.0) * 50.0;
                    break;
            }

            battleScreen.Battle.ReduceHP(
                (int)rocksDamage, false, true, battleScreen,
                "The Stealth Rocks hurt " + p.GetDisplayName() + "!", "stealthrocks");
        }

        battleScreen.Battle.TriggerAbilityEffect(battleScreen, false);
        battleScreen.Battle.TriggerItemEffect(battleScreen, false);

        if (battleScreen.OpponentPokemon!.Status.Equals(Pokemon.StatusProblems.Sleep) == true)
        {
            battleScreen.FieldEffects.SleepTurns.Opponent = Core.Random.Next(1, 4);
        }

        if (battleScreen.FieldEffects.HealingWish.Opponent == true)
        {
            battleScreen.FieldEffects.HealingWish.Opponent = false;
            if (battleScreen.OpponentPokemon.HP < battleScreen.OpponentPokemon.MaxHP ||
                battleScreen.OpponentPokemon.Status.Equals(Pokemon.StatusProblems.None) == false)
            {
                battleScreen.Battle.GainHP(
                    battleScreen.OpponentPokemon.MaxHP - battleScreen.OpponentPokemon.HP,
                    false, false, battleScreen,
                    "The Healing Wish came true for " + battleScreen.OpponentPokemon.GetDisplayName() + "!",
                    "move:healingwish");
                battleScreen.Battle.CureStatusProblem(
                    false, false, battleScreen, String.Empty, "move:healingwish");
            }
        }
    }

    public override void Update(BattleScreen bv2Screen)
    {
        if (TextReady == false)
        {
            UpdateText();
        }
        else
        {
            _delay -= 0.1f;
            if (_delay < 0.0f)
            {
                _delay = 0.0f;
            }
            if (_delay.Equals(0.0f) == true && bv2Screen.IsCurrentScreen() == true)
            {
                UpdateChoose();
            }
        }
    }

    public override void Draw(BattleScreen bv2Screen)
    {
        if (_ready == true)
        {
            return;
        }
        DrawText(bv2Screen);
        if (TextReady == true)
        {
            DrawChoose();
        }
    }

    public override bool IsReady => _ready;
}
