using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace P3D;

public class OverworldCamera : Camera
{
    private const float BUMP_SOUND_DELAY_RESET = 35f;
    private const float SCROLL_SPEED_INCREMENT_MOUSE = 0.01f;
    private const float SCROLL_SPEED_INCREMENT_STICK = 0.002f;
    private const float SCROLL_MAX_SPEED = 0.08f;
    private const float SCROLL_DECELERATION = 0.001f;
    private const float PITCH_CLAMP_MAX = 1.5f;
    private const float PITCH_CLAMP_MIN = -1.5f;
    private const float AIM_SPEED_MULTIPLIER = 40f;
    private const float MOUSE_SPEED_MULTIPLIER = 0.75f;
    private const float CONTROLLER_TURN_SPEED = 50f;
    private const float CONTROLLER_PITCH_SPEED = 30f;
    private const float MOUSE_CLAMP_NORMAL = 400f;
    private const float MOUSE_CLAMP_NEAR_EDGE = 240f;
    private const float MOUSE_Y_CLAMP_NORMAL = 320f;
    private const float MOUSE_Y_CLAMP_NEAR_EDGE = 128f;
    private const float MOUSE_EDGE_THRESHOLD_DIV = 15f;
    private const float DEFAULT_BOBBING_RIDING = 0.012f;
    private const float DEFAULT_BOBBING_RUNNING = 0.008f;
    private const float DEFAULT_BOBBING_WALKING = 0.004f;
    private const float BOBBING_STEP = 0.25f;
    private const float PLAYER_BOUNDING_HALF = 0.465f;
    private const float SURFING_FLOOR_Y = 0f;
    private const float THIRD_PERSON_OFFSET_Y_DEFAULT = 0.3f;
    private const float THIRD_PERSON_OFFSET_Z_DEFAULT = 1.5f;
    private const float THIRD_PERSON_OFFSET_Y_MAX = 1.32f;
    private const float THIRD_PERSON_OFFSET_Z_MAX = 2.7f;
    private const float THIRD_PERSON_OFFSET_Z_MIN = -0.1f;
    private const float COLLISION_Y_OFFSET = 0.1f;
    private const float ICE_CHECK_Y_OFFSET = 0.1f;
    private const float RAY_NEAR = 0f;
    private const float RAY_FAR = 1f;
    private const float RAY_DISTANCE_MIN = 1.3f;
    private const float RAY_DISTANCE_THIRD_PERSON_EXTRA = 1.8f;
    private const float PITCH_FORWARD_FIRST_PERSON = -0.1f;
    private const float PITCH_FORWARD_THIRD_PERSON = -0.25f;
    private const float POSITION_Y_OFFSET = 0.1f;
    private const float CONTROLLER_TURN_MODIFIER_SLOW = 0.25f;
    private const int THIRD_PERSON_TURN_WAIT = 5;
    private const int NOT_PRESSED_THRESHOLD = 3;

    // Camera focus
    private CameraFocusTypes _cameraFocusType = CameraFocusTypes.Player;
    private int _cameraFocusID = -1;

    // State
    private bool _freeCameraMode;
    private Vector3 _cPosition = Vector3.Zero;
    private int _aimDirection = -1;

    private float _bobbingTemp;
    private int _tempDirectionPressed = -1;
    private int _tempAmountOfSteps;

    private int _waitForThirdPersonTurning;
    private int _notPressedThirdPersonDirectionButton;

    private float _scrollSpeed;
    private int _scrollDirection = 1;

    private int _bumpSoundDelay = (int)BUMP_SOUND_DELAY_RESET;

    private MouseState _mouseState;

    public enum CameraFocusTypes
    {
        Player = 0,
        NPC = 1,
        Entity = 2,
    }

    // Public fields
    public Vector2 oldMousePos;
    public bool doMouseUpdate = true;
    public Vector2 mouseSpeed;
    public bool _debugWalk;
    public float _moved;

    // Properties
    public bool ThirdPerson { get; private set; }
    public bool FreeCameraMode => _freeCameraMode;
    public override Vector3 CPosition => _cPosition;
    public bool Fixed { get; set; }
    public bool PreventMovement { get; set; }
    public bool DidWalkAgainst { get; set; } = true;
    public bool IsSliding { get; set; }
    public bool IsPushingStrengthRock { get; set; }
    public bool YawLocked { get; set; }
    public Vector3 ThirdPersonOffset { get; set; } = new Vector3(0f, THIRD_PERSON_OFFSET_Y_DEFAULT, THIRD_PERSON_OFFSET_Z_DEFAULT);
    public Vector3 LastStepPosition { get; set; } = new Vector3(0f, -2f, 0f);
    public bool _canToggleThirdPerson = true;

    public override bool IsMoving => _moved > 0f;

    public CameraFocusTypes CameraFocusType
    {
        get => _cameraFocusType;
        set
        {
            _cameraFocusType = value;
            if (ThirdPerson == true)
            {
                SetThirdPerson(true, false);
                UpdateThirdPersonCamera();
            }
        }
    }

    public int CameraFocusID
    {
        get => _cameraFocusID;
        set
        {
            _cameraFocusID = value;
            if (ThirdPerson == true)
            {
                SetThirdPerson(true, false);
                UpdateThirdPersonCamera();
            }
        }
    }

    public void SetupFocus(CameraFocusTypes focusType, int id)
    {
        _cameraFocusType = focusType;
        _cameraFocusID = id;
        if (ThirdPerson == true)
        {
            SetThirdPerson(true, false);
            UpdateThirdPersonCamera();
        }
    }

    public bool IsPointingToNormalDirection()
    {
        return (Yaw == 0f || Yaw == MathHelper.Pi * 0.5f || Yaw == MathHelper.Pi || Yaw == MathHelper.Pi * 1.5f);
    }

    public void SetAimDirection(int direction)
    {
        _aimDirection = direction;
    }

    public OverworldCamera() : base("Overworld")
    {
        Position = Core.Player.StartPosition;
        ThirdPerson = Core.Player.StartThirdPerson;
        Yaw = Core.Player.StartRotation;
        if (ThirdPerson == true)
        {
            _cameraFocusType = CameraFocusTypes.Player;
        }
        RotationSpeed = (float)(Core.Player.StartRotationSpeed / 10000.0);
        FOV = Core.Player.StartFOV;
        _freeCameraMode = Core.Player.StartFreeCameraMode;

        Pitch = 0f;

        CreateProjectionMatrix();
        UpdateViewMatrix();
        UpdateFrustum();
    }

    private void CreateProjectionMatrix()
    {
        Projection = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.ToRadians(FOV),
            Core.GraphicsDevice.Viewport.AspectRatio,
            0.01f,
            FarPlane);
    }

    public override void Update()
    {
        if (GameController.IS_DEBUG_ACTIVE == true || Core.Player.IsGameJoltSave == false)
        {
            if (KeyBoardHandler.KeyDown(KeyBindings.DebugKey) == true && KeyBoardHandler.KeyPressed(Keys.O) == true)
            {
                Core.Player.SandBoxMode = !Core.Player.SandBoxMode;
                if (Core.Player.SandBoxMode == true)
                {
                    Core.GameMessage.ShowMessage(Localization.GetString("game_message_sandbox_mode_on"), 12, FontManager.MainFont, Color.White);
                }
                else
                {
                    Core.GameMessage.ShowMessage(Localization.GetString("game_message_sandbox_mode_off"), 12, FontManager.MainFont, Color.White);
                }
            }
        }

        if (GameController.IS_DEBUG_ACTIVE == true || Core.Player.SandBoxMode == true)
        {
            if (KeyBoardHandler.KeyPressed(KeyBindings.DebugWalkKey) == true)
            {
                _debugWalk = !_debugWalk;
            }
        }

        if (KeyBoardHandler.KeyPressed(KeyBindings.RunKey) == true || ControllerHandler.ButtonPressed(Buttons.B) == true)
        {
            if (Screen.Level!.Riding == false && Screen.Level.Surfing == false && Core.Player.Inventory.HasRunningShoes == true)
            {
                Core.Player.RunToggled = !Core.Player.RunToggled;
            }
        }

        if (GameController.IsActiveWindow() == true && Core.CurrentScreen.Identification.Equals(Screen.Identifications.OverworldScreen) == true)
        {
            CursorClipper.LockCursor();
        }
        else
        {
            CursorClipper.ReleaseCursor();
        }
        _mouseState = Mouse.GetState();

        Ray = CreateRay();

        PlayerMovement();
        ScrollThirdPersonCamera();
        LockCamera();
        CheckEntities();
        AimCamera();
        ControlCamera();
        ControlThirdPersonCamera();
        UpdateThirdPersonCamera();
        SetSpeed();

        UpdateViewMatrix();
        UpdateFrustum();

        ResetCursor();
    }

    private void ControlCamera()
    {
        GamePadState gState = GamePad.GetState(PlayerIndex.One);

        Vector2 cameraD = Vector2.Zero;

        if (doMouseUpdate == true)
        {
            if ((int)(_mouseState.X - oldMousePos.X) >= MOUSE_CLAMP_NORMAL ||
                (int)(_mouseState.X - oldMousePos.X) <= -MOUSE_CLAMP_NORMAL ||
                (int)(_mouseState.Y - oldMousePos.Y) >= MOUSE_Y_CLAMP_NORMAL ||
                (int)(_mouseState.Y - oldMousePos.Y) <= -MOUSE_Y_CLAMP_NORMAL)
            {
                cameraD = new Vector2(
                    ((int)(_mouseState.X - oldMousePos.X)).Clamp(-(int)MOUSE_CLAMP_NEAR_EDGE, (int)MOUSE_CLAMP_NEAR_EDGE),
                    ((int)(_mouseState.Y - oldMousePos.Y)).Clamp(-(int)MOUSE_Y_CLAMP_NEAR_EDGE, (int)MOUSE_Y_CLAMP_NEAR_EDGE));
            }
            else
            {
                cameraD = new Vector2(
                    ((int)(_mouseState.X - oldMousePos.X)).Clamp(-(int)MOUSE_CLAMP_NORMAL, (int)MOUSE_CLAMP_NORMAL),
                    ((int)(_mouseState.Y - oldMousePos.Y)).Clamp(-(int)MOUSE_Y_CLAMP_NORMAL, (int)MOUSE_Y_CLAMP_NORMAL));
            }
            doMouseUpdate = false;
        }

        if (gState.ThumbSticks.Right.X != 0f && Core.GameOptions.GamePadEnabled == true)
        {
            cameraD.X = gState.ThumbSticks.Right.X * CONTROLLER_TURN_SPEED;
            if (Core.GameOptions.GamePadInvertRightStick.X != 0)
            {
                cameraD.X *= -1f;
            }
        }

        if (gState.ThumbSticks.Right.Y != 0f && Core.GameOptions.GamePadEnabled == true)
        {
            cameraD.Y = gState.ThumbSticks.Right.Y * 35f * -1f;
            if (Core.GameOptions.GamePadInvertRightStick.Y != 0)
            {
                cameraD.Y *= -1f;
            }
        }

        if (Fixed == false && (cameraD.X != 0f || cameraD.Y != 0f))
        {
            if (Core.CurrentScreen.Identification.Equals(Screen.Identifications.OverworldScreen) == true)
            {
                OverworldScreen os = (OverworldScreen)Core.CurrentScreen;
                if (os.NotificationPopupList.Count == 0 || os.NotificationPopupList[0]._forceAccept == false)
                {
                    if (_freeCameraMode == true && os.ActionScript.IsReady == true)
                    {
                        if (YawLocked == false)
                        {
                            Yaw += -RotationSpeed * MOUSE_SPEED_MULTIPLIER * cameraD.X;
                        }
                    }
                    if (os.ActionScript.IsReady == true)
                    {
                        Pitch += -RotationSpeed * cameraD.Y;
                    }
                }
            }
        }

        ClampYaw();
        ClampPitch();
    }

    private void LockCamera()
    {
        if (Core.CurrentScreen.Identification.Equals(Screen.Identifications.OverworldScreen) == false) return;

        OverworldScreen os = (OverworldScreen)Core.CurrentScreen;
        if (os.ActionScript.IsReady == false) return;

        if ((KeyBoardHandler.KeyPressed(KeyBindings.CameraLockKey) == true || ControllerHandler.ButtonPressed(Buttons.RightStick) == true) &&
            _moved == 0f && YawLocked == false)
        {
            switch (GetFacingDirection())
            {
                case 0:
                    Yaw = 0f;
                    break;
                case 1:
                    Yaw = MathHelper.Pi * 0.5f;
                    break;
                case 2:
                    Yaw = MathHelper.Pi;
                    break;
                case 3:
                    Yaw = MathHelper.Pi * 1.5f;
                    break;
            }

            _freeCameraMode = !_freeCameraMode;

            if (_freeCameraMode == false)
            {
                Core.GameMessage.ShowMessage(Localization.GetString("game_message_free_camera_off"), 12, FontManager.MainFont, Color.White);
            }
            else
            {
                Core.GameMessage.ShowMessage(Localization.GetString("game_message_free_camera_on"), 12, FontManager.MainFont, Color.White);
            }
        }
    }

    private void ScrollThirdPersonCamera()
    {
        if (Fixed == true) return;

        bool actionscriptReady = Core.CurrentScreen.Identification.Equals(Screen.Identifications.OverworldScreen) == false ||
                                 ((OverworldScreen)Core.CurrentScreen).ActionScript.IsReady;

        if (actionscriptReady == false) return;

        if (Controls.Down(true, false, true, false, false, false) == true)
        {
            if (_scrollSpeed == 0f || _scrollDirection != 1)
            {
                _scrollSpeed = SCROLL_SPEED_INCREMENT_MOUSE;
            }
            _scrollDirection = 1;
            _scrollSpeed += _scrollSpeed.Clamp(0f, SCROLL_SPEED_INCREMENT_MOUSE);
        }
        if (ControllerHandler.ButtonDown(Buttons.LeftTrigger) == true)
        {
            if (_scrollSpeed == 0f || _scrollDirection != 1)
            {
                _scrollSpeed = SCROLL_SPEED_INCREMENT_STICK;
            }
            _scrollDirection = 1;
            _scrollSpeed += _scrollSpeed.Clamp(0f, SCROLL_SPEED_INCREMENT_STICK);
        }
        if (Controls.Up(true, false, true, false, false, false) == true)
        {
            if (_scrollSpeed == 0f || _scrollDirection != -1)
            {
                _scrollSpeed = SCROLL_SPEED_INCREMENT_MOUSE;
            }
            _scrollDirection = -1;
            _scrollSpeed += _scrollSpeed.Clamp(0f, SCROLL_SPEED_INCREMENT_MOUSE);
        }
        if (ControllerHandler.ButtonDown(Buttons.RightTrigger) == true)
        {
            if (_scrollSpeed == 0f || _scrollDirection != -1)
            {
                _scrollSpeed = SCROLL_SPEED_INCREMENT_STICK;
            }
            _scrollDirection = -1;
            _scrollSpeed += _scrollSpeed.Clamp(0f, SCROLL_SPEED_INCREMENT_STICK);
        }

        _scrollSpeed = _scrollSpeed.Clamp(0f, SCROLL_MAX_SPEED);

        if (_scrollSpeed > 0f)
        {
            ThirdPersonOffset = new Vector3(
                ThirdPersonOffset.X,
                ThirdPersonOffset.Y + _scrollSpeed * _scrollDirection,
                ThirdPersonOffset.Z + _scrollSpeed * _scrollDirection);

            if (Core.Player.SandBoxMode == false)
            {
                ThirdPersonOffset = new Vector3(
                    ThirdPersonOffset.X,
                    ThirdPersonOffset.Y.Clamp(0f, THIRD_PERSON_OFFSET_Y_MAX),
                    ThirdPersonOffset.Z.Clamp(THIRD_PERSON_OFFSET_Z_MIN, THIRD_PERSON_OFFSET_Z_MAX));
            }

            _scrollSpeed -= SCROLL_DECELERATION;
            if (_scrollSpeed <= 0f)
            {
                _scrollSpeed = 0f;
            }
        }
    }

    public void UpdateThirdPersonCamera()
    {
        if (Fixed == true) return;

        if (PreventMovement == false &&
            (KeyBoardHandler.KeyPressed(KeyBindings.PerspectiveSwitchKey) == true || ControllerHandler.ButtonPressed(Buttons.LeftShoulder) == true))
        {
            bool actionscriptReady = true;
            bool forcedNotification = false;
            if (Core.CurrentScreen.Identification.Equals(Screen.Identifications.OverworldScreen) == true)
            {
                actionscriptReady = ((OverworldScreen)Core.CurrentScreen).ActionScript.IsReady;
                if (((OverworldScreen)Core.CurrentScreen).NotificationPopupList.Count > 0 &&
                    ((OverworldScreen)Core.CurrentScreen).NotificationPopupList[0]._forceAccept == true)
                {
                    forcedNotification = true;
                }
            }
            if (actionscriptReady == true && _canToggleThirdPerson == true && forcedNotification == false)
            {
                SetThirdPerson(!ThirdPerson, true);
            }
        }

        Vector3 usePosition = Position;
        switch (_cameraFocusType)
        {
            case CameraFocusTypes.Entity:
            {
                IEnumerable<Entity> entList = Screen.Level!.Entities.Where(ent => ent.ID == _cameraFocusID);
                if (entList.Any() == true)
                {
                    usePosition = entList.First().Position;
                    usePosition.Y += COLLISION_Y_OFFSET;
                }
                break;
            }
            case CameraFocusTypes.NPC:
            {
                IEnumerable<Entity> entList = Screen.Level!.Entities.Where(ent => ent.GetType() == typeof(NPC) && ((NPC)ent).NPCID == _cameraFocusID);
                if (entList.Any() == true)
                {
                    usePosition = entList.First().Position;
                    usePosition.Y += COLLISION_Y_OFFSET;
                }
                break;
            }
        }

        if (ThirdPerson == true)
        {
            Matrix rotationMatrix = Matrix.CreateRotationY(Yaw);
            Vector3 offset = ThirdPersonOffset;
            Vector3 transformedOffset = Vector3.Transform(offset, rotationMatrix);

            Vector3 diff = _cPosition - (usePosition + transformedOffset);
            foreach (Entity ent in Screen.Level!.Entities.Where(e => e.GetType() == typeof(Particle)))
            {
                ((Particle)ent).MoveWithCamera(diff);
            }

            _cPosition = usePosition + transformedOffset;
        }
        else
        {
            _cPosition = usePosition;
        }
    }

    public void SetThirdPerson(bool isThirdPerson, bool showMessage)
    {
        if (ThirdPerson == isThirdPerson) return;

        if (isThirdPerson == true && ThirdPerson == false)
        {
            _cameraFocusType = CameraFocusTypes.Player;
        }

        ThirdPerson = isThirdPerson;
        ThirdPersonOffset = new Vector3(0f, THIRD_PERSON_OFFSET_Y_DEFAULT, THIRD_PERSON_OFFSET_Z_DEFAULT);

        if (ThirdPerson == true)
        {
            Screen.Level!.OwnPlayer!.Opacity = 1f;
            if (showMessage == true)
            {
                Core.GameMessage.ShowMessage(Localization.GetString("game_message_third_person_on"), 12, FontManager.MainFont, Color.White);
            }
        }
        else
        {
            Yaw = GetAimYawFromDirection(GetFacingDirection());
            if (showMessage == true)
            {
                Core.GameMessage.ShowMessage(Localization.GetString("game_message_third_person_off"), 12, FontManager.MainFont, Color.White);
            }
        }
    }

    public void UpdateFrustum()
    {
        Matrix rotation = Matrix.CreateRotationX(Pitch) * Matrix.CreateRotationY(Yaw);
        Vector3 fPosition = new Vector3(_cPosition.X, _cPosition.Y + GetBobbing(), _cPosition.Z);
        Vector3 transformed = Vector3.Transform(new Vector3(0f, 0f, -1f), rotation);
        Vector3 lookAt = fPosition + transformed;
        BoundingFrustum = new BoundingFrustum(Matrix.CreateLookAt(fPosition, lookAt, Vector3.Up) * Projection);
    }

    public void UpdateViewMatrix()
    {
        Matrix rotation = Matrix.CreateRotationX(Pitch) * Matrix.CreateRotationY(Yaw);
        Vector3 transformed = Vector3.Transform(new Vector3(0f, 0f, -1f), rotation);
        Vector3 lookAt = new Vector3(_cPosition.X, _cPosition.Y + GetBobbing(), _cPosition.Z) + transformed;
        View = Matrix.CreateLookAt(_cPosition, lookAt, Vector3.Up);
    }

    public void ResetCursor()
    {
        if (GameController.IsActiveWindow() == false) return;

        float horizontalCutoff = Core.windowSize.Width / MOUSE_EDGE_THRESHOLD_DIV;
        float verticalCutoff = Core.windowSize.Height / MOUSE_EDGE_THRESHOLD_DIV;

        if (_mouseState.X <= horizontalCutoff ||
            _mouseState.X >= Core.windowSize.Width - horizontalCutoff ||
            _mouseState.Y <= verticalCutoff ||
            _mouseState.Y >= Core.windowSize.Height - verticalCutoff)
        {
            oldMousePos = new Vector2((int)(Core.windowSize.Width / 2), (int)(Core.windowSize.Height / 2));
            Mouse.SetPosition((int)(Core.windowSize.Width / 2), (int)(Core.windowSize.Height / 2));
        }
        else
        {
            oldMousePos = _mouseState.Position.ToVector2();
        }
        doMouseUpdate = true;
    }

    private void SetSpeed()
    {
        if (Core.CurrentScreen.Identification.Equals(Screen.Identifications.OverworldScreen) == true &&
            ((OverworldScreen)Core.CurrentScreen).ActionScript.IsReady == true)
        {
            if (Screen.Level!.Riding == true)
            {
                Speed = 0.08f;
            }
            else if (Screen.Level.Surfing == true)
            {
                Speed = 0.04f;
            }
            else if (Core.Player.IsRunning() == true)
            {
                Speed = 0.06f;
            }
            else
            {
                Speed = 0.04f;
            }
        }
        if (Screen.Level?.OverworldPokemon != null)
        {
            Screen.Level.OverworldPokemon.MoveSpeed = Speed;
        }
    }

    private Ray CreateRay()
    {
        int centerX = (int)(Core.windowSize.Width / 2);
        int centerY = (int)(Core.windowSize.Height / 2);

        Vector3 nearSource = new Vector3(centerX, centerY, RAY_NEAR);
        Vector3 farSource = new Vector3(centerX, centerY, RAY_FAR);

        Vector3 nearPoint = Core.GraphicsDevice.Viewport.Unproject(nearSource, Projection, View, Matrix.Identity);
        Vector3 farPoint = Core.GraphicsDevice.Viewport.Unproject(farSource, Projection, View, Matrix.Identity);

        Vector3 direction = farPoint - nearPoint;
        direction.Normalize();

        return new Ray(nearPoint, direction);
    }

    public void AimCamera()
    {
        if (_aimDirection <= -1 || Turning == false) return;

        float yawAim = GetAimYawFromDirection(_aimDirection);
        bool clockwise = true;

        if (Yaw >= 0f && Yaw < MathHelper.Pi * 0.5f)
        {
            switch (_aimDirection)
            {
                case 0:
                    clockwise = true;
                    break;
                case 1:
                    clockwise = false;
                    break;
                case 2:
                    clockwise = false;
                    break;
                case 3:
                    clockwise = true;
                    yawAim -= MathHelper.TwoPi;
                    break;
            }
        }
        else if (Yaw >= MathHelper.Pi * 0.5f && Yaw < MathHelper.Pi)
        {
            switch (_aimDirection)
            {
                case 0:
                    clockwise = true;
                    break;
                case 1:
                    clockwise = true;
                    break;
                case 2:
                    clockwise = false;
                    break;
                case 3:
                    clockwise = false;
                    break;
            }
        }
        else if (Yaw >= MathHelper.Pi && Yaw < MathHelper.Pi * 1.5f)
        {
            switch (_aimDirection)
            {
                case 0:
                    clockwise = false;
                    yawAim += MathHelper.TwoPi;
                    break;
                case 1:
                    clockwise = true;
                    break;
                case 2:
                    clockwise = true;
                    break;
                case 3:
                    clockwise = false;
                    break;
            }
        }
        else if (Yaw >= MathHelper.Pi * 1.5f && Yaw < MathHelper.TwoPi)
        {
            switch (_aimDirection)
            {
                case 0:
                    clockwise = false;
                    yawAim += MathHelper.TwoPi;
                    break;
                case 1:
                    clockwise = false;
                    yawAim += MathHelper.TwoPi;
                    break;
                case 2:
                    clockwise = true;
                    break;
                case 3:
                    clockwise = true;
                    break;
            }
        }

        if (clockwise == true)
        {
            ClampYaw();
            Yaw -= RotationSpeed * AIM_SPEED_MULTIPLIER;
            if (Yaw <= yawAim)
            {
                Turning = false;
                _aimDirection = -1;
                Yaw = yawAim;
                ClampYaw();
            }
        }
        else
        {
            ClampYaw();
            Yaw += RotationSpeed * AIM_SPEED_MULTIPLIER;
            if (Yaw >= yawAim)
            {
                Turning = false;
                _aimDirection = -1;
                Yaw = yawAim;
                ClampYaw();
            }
        }
    }

    private void ClampYaw()
    {
        while (Yaw < 0f)
        {
            Yaw += MathHelper.TwoPi;
        }
        while (Yaw >= MathHelper.TwoPi)
        {
            Yaw -= MathHelper.TwoPi;
        }
    }

    private void ClampPitch()
    {
        Pitch = MathHelper.Clamp(Pitch, PITCH_CLAMP_MIN, PITCH_CLAMP_MAX);
    }

    public void PitchForward()
    {
        float aim = PITCH_FORWARD_FIRST_PERSON;
        if (ThirdPerson == true)
        {
            aim = PITCH_FORWARD_THIRD_PERSON;
        }

        if (Pitch > aim)
        {
            Pitch -= RotationSpeed * AIM_SPEED_MULTIPLIER;
            if (Pitch < aim)
            {
                Pitch = aim;
            }
        }
        else if (Pitch < aim)
        {
            Pitch += RotationSpeed * AIM_SPEED_MULTIPLIER;
            if (Pitch > aim)
            {
                Pitch = aim;
            }
        }
    }

    private float GetBobbing()
    {
        if (ThirdPerson == true || IsSliding == true || IsMoving == false || Core.GameOptions.ViewBobbing == false)
        {
            return 0f;
        }
        if (Screen.Level?.Riding == true)
        {
            return (float)(Math.Sin(_bobbingTemp) * DEFAULT_BOBBING_RIDING);
        }
        else
        {
            if (Core.Player.IsRunning() == true)
            {
                return (float)(Math.Sin(_bobbingTemp) * DEFAULT_BOBBING_RUNNING);
            }
            else
            {
                return (float)(Math.Sin(_bobbingTemp) * DEFAULT_BOBBING_WALKING);
            }
        }
    }

    private void PlayerMovement()
    {
        if (_moved > 0f && Turning == false)
        {
            Vector3 v = _plannedMovement * Speed;

            if (Screen.Level!.OwnPlayer != null && Screen.Level.OwnPlayer.isDancing == false)
            {
                Position += v;
            }

            _moved -= Speed;
            if (_moved <= 0f)
            {
                StopMovement();
                Screen.Level.OwnPlayer!.isDancing = false;
                Position = new Vector3(
                    (float)Math.Round(Position.X),
                    (float)Math.Round(Position.Y) + POSITION_Y_OFFSET,
                    (float)Math.Round(Position.Z));

                if (Screen.Level.Surfing == true)
                {
                    Position = new Vector3(Position.X, (float)Math.Floor(Position.Y), Position.Z);
                }

                if (_tempDirectionPressed > -1)
                {
                    if (ThirdPerson == false)
                    {
                        Turn(_tempDirectionPressed);
                    }
                    else
                    {
                        Turn(_tempDirectionPressed, true, false);
                    }
                }
                _tempDirectionPressed = -1;
                Screen.Level.OwnPlayer.DoAnimation = true;

                if (Core.GameOptions.GraphicStyle > 0)
                {
                    if (World.NoParticlesList.Contains(Screen.Level.World.CurrentMapWeather) == false)
                    {
                        World.GenerateParticles(-1, Screen.Level.World.CurrentMapWeather);
                    }
                }

                Core.Player.TakeStep(_tempAmountOfSteps);
                _tempAmountOfSteps = 0;

                LastStepPosition = Position;
            }

            if (Screen.Level.Surfing == false && ThirdPerson == false && _cameraFocusType == CameraFocusTypes.Player)
            {
                _bobbingTemp += BOBBING_STEP;
            }
        }

        bool isActionscriptReady = false;
        bool forcedNotification = false;
        OverworldScreen? os = null;
        if (Core.CurrentScreen.Identification.Equals(Screen.Identifications.OverworldScreen) == true)
        {
            os = (OverworldScreen)Core.CurrentScreen;
            isActionscriptReady = os.ActionScript.IsReady;
            if (os.NotificationPopupList.Count > 0 && os.NotificationPopupList[0]._forceAccept == true)
            {
                forcedNotification = true;
            }
        }

        if (Core.CurrentScreen.Identification.Equals(Screen.Identifications.OverworldScreen) == true &&
            ((OverworldScreen)Core.CurrentScreen).ActivatedScriptedNotification == false &&
            isActionscriptReady == true &&
            ScriptBlock.TriggeredScriptBlock == false &&
            Screen.Level!.CanMove() == true &&
            PreventMovement == false &&
            forcedNotification == false)
        {
            if (ThirdPerson == false && _cameraFocusType == CameraFocusTypes.Player)
            {
                FirstPersonMovement();
            }
            else
            {
                ThirdPersonMovement();
            }
        }

        if (Screen.Level!.Surfing == true)
        {
            Screen.Level.OwnPlayer!.Opacity = 1f;
        }

        if (_bumpSoundDelay > 0)
        {
            _bumpSoundDelay -= 1;
        }
    }

    private void FirstPersonMovement()
    {
        int pressedDirection = -1;
        float controllerTurnModifier = 1f;
        if (ControllerHandler.ButtonDown(Buttons.RightThumbstickLeft) == true ||
            ControllerHandler.ButtonDown(Buttons.RightThumbstickRight) == true)
        {
            controllerTurnModifier = CONTROLLER_TURN_MODIFIER_SLOW;
        }

        if (YawLocked == false && Turning == false)
        {
            if ((KeyBoardHandler.KeyDown(KeyBindings.LeftMoveKey) == true ||
                 ControllerHandler.ButtonDown(Buttons.RightThumbstickLeft) == true ||
                 ControllerHandler.ButtonDown(Buttons.DPadLeft) == true) && Turning == false)
            {
                if (_freeCameraMode == true)
                {
                    Yaw += RotationSpeed * AIM_SPEED_MULTIPLIER * controllerTurnModifier;
                    ClampYaw();
                }
                else
                {
                    pressedDirection = 1;
                }
            }
            if ((KeyBoardHandler.KeyDown(KeyBindings.RightMoveKey) == true ||
                 ControllerHandler.ButtonDown(Buttons.RightThumbstickRight) == true ||
                 ControllerHandler.ButtonDown(Buttons.DPadRight) == true) && Turning == false)
            {
                if (_freeCameraMode == true)
                {
                    Yaw -= RotationSpeed * AIM_SPEED_MULTIPLIER * controllerTurnModifier;
                    ClampYaw();
                }
                else
                {
                    pressedDirection = 3;
                }
            }

            if (_freeCameraMode == false && pressedDirection > -1)
            {
                if (_moved <= 0f)
                {
                    Turn(pressedDirection);
                }
                else
                {
                    _tempDirectionPressed = pressedDirection;
                }
            }

            ClampYaw();

            if ((KeyBoardHandler.KeyDown(KeyBindings.ForwardMoveKey) == true ||
                 ControllerHandler.ButtonDown(Buttons.LeftThumbstickUp) == true ||
                 ControllerHandler.ButtonDown(Buttons.DPadUp) == true) && Turning == false)
            {
                MoveForward();
            }
        }
    }

    private void ThirdPersonMovement()
    {
        if (_moved > 0f)
        {
            _waitForThirdPersonTurning = 0;
            _notPressedThirdPersonDirectionButton = 0;
            return;
        }

        bool doMove = false;
        int newPlayerFacing = -1;

        if (IsThirdPersonMoveButtonDown(0) == true)
        {
            newPlayerFacing = GetFacingDirection() + 0;
            doMove = true;
        }
        else if (IsThirdPersonMoveButtonDown(1) == true)
        {
            newPlayerFacing = GetFacingDirection() + 1;
            doMove = true;
        }
        else if (IsThirdPersonMoveButtonDown(2) == true)
        {
            newPlayerFacing = GetFacingDirection() + 2;
            doMove = true;
        }
        else if (IsThirdPersonMoveButtonDown(3) == true)
        {
            newPlayerFacing = GetFacingDirection() + 3;
            doMove = true;
        }

        while (newPlayerFacing > 3)
        {
            newPlayerFacing -= 4;
        }

        if (doMove == true)
        {
            if (newPlayerFacing != _thirdPersonFacing)
            {
                if (IsThirdPersonMoveButtonDown(_thirdPersonFacing) == false &&
                    Core.Player.IsRunning() == false &&
                    _notPressedThirdPersonDirectionButton >= NOT_PRESSED_THRESHOLD)
                {
                    _waitForThirdPersonTurning = THIRD_PERSON_TURN_WAIT;
                }
                _thirdPersonFacing = newPlayerFacing;
                Screen.Level!.OwnPlayer!.Opacity = 1f;
            }
            else
            {
                if (_waitForThirdPersonTurning > 0)
                {
                    _waitForThirdPersonTurning -= 1;
                }
                else
                {
                    MoveForward();
                }
            }
            _notPressedThirdPersonDirectionButton = 0;
        }
        else
        {
            if (_notPressedThirdPersonDirectionButton < NOT_PRESSED_THRESHOLD)
            {
                _notPressedThirdPersonDirectionButton += 1;
            }
        }
    }

    private int _thirdPersonFacing;

    private bool IsThirdPersonMoveButtonDown(int facing)
    {
        switch (facing)
        {
            case 0:
                return KeyBoardHandler.KeyDown(KeyBindings.ForwardMoveKey) == true ||
                       ControllerHandler.ButtonDown(Buttons.LeftThumbstickUp) == true ||
                       ControllerHandler.ButtonDown(Buttons.DPadUp) == true;
            case 1:
                return KeyBoardHandler.KeyDown(KeyBindings.LeftMoveKey) == true ||
                       ControllerHandler.ButtonDown(Buttons.LeftThumbstickLeft) == true ||
                       ControllerHandler.ButtonDown(Buttons.DPadLeft) == true;
            case 2:
                return KeyBoardHandler.KeyDown(KeyBindings.BackwardMoveKey) == true ||
                       ControllerHandler.ButtonDown(Buttons.LeftThumbstickDown) == true ||
                       ControllerHandler.ButtonDown(Buttons.DPadDown) == true;
            case 3:
                return KeyBoardHandler.KeyDown(KeyBindings.RightMoveKey) == true ||
                       ControllerHandler.ButtonDown(Buttons.LeftThumbstickRight) == true ||
                       ControllerHandler.ButtonDown(Buttons.DPadRight) == true;
        }
        return false;
    }

    private void MoveForward()
    {
        if (_moved > 0f) return;

        if (CheckCollision(GetForwardMovedPosition()) == false)
        {
            Screen.Level!.OwnPlayer!.Opacity = 1f;

            int walkSteps = GetIceSteps(GetForwardMovedPosition());
            Screen.Level.OwnPlayer.DoAnimation = (walkSteps <= 1);

            Move(walkSteps);
            DidWalkAgainst = true;
        }
        else
        {
            if (Screen.Level!.Surfing == false)
            {
                DidWalkAgainst = true;
            }
            if (IsPushingStrengthRock == false)
            {
                if (ThirdPerson == true)
                {
                    if (DidWalkAgainst == true)
                    {
                        Screen.Level.OwnPlayer!.Opacity = 0.5f;
                    }
                }
                if (_bumpSoundDelay == 0)
                {
                    if (DidWalkAgainst == true)
                    {
                        SoundManager.PlaySound("bump");
                    }
                    _bumpSoundDelay = (int)BUMP_SOUND_DELAY_RESET;
                }
            }
        }
    }

    public bool CheckCollision(Vector3 newPosition)
    {
        bool cannotWalk = true;
        bool setSurfFalse = false;
        Vector3 position2D = new Vector3(newPosition.X, (float)Math.Floor(newPosition.Y), newPosition.Z);

        foreach (Entity floor in Screen.Level!.Floors)
        {
            if (floor.BoundingBox.Contains(position2D) == ContainmentType.Contains)
            {
                cannotWalk = false;
                setSurfFalse = true;
            }
        }

        BoundingBox playerBoundingBox = new BoundingBox(
            newPosition + new Vector3(-PLAYER_BOUNDING_HALF),
            newPosition + new Vector3(PLAYER_BOUNDING_HALF));

        if (cannotWalk == false)
        {
            foreach (Entity entity in Screen.Level.Entities)
            {
                if (entity.EntityID.ToLower().Equals("npc") == true)
                {
                    if (entity.ViewBox.Contains(playerBoundingBox) == ContainmentType.Intersects)
                    {
                        if (entity.Collision == true)
                        {
                            if (entity.WalkAgainstFunction() == true)
                            {
                                cannotWalk = true;
                            }
                        }
                        else
                        {
                            if (entity.WalkIntoFunction() == true)
                            {
                                cannotWalk = true;
                            }
                        }
                    }
                }
                else
                {
                    if (entity.BoundingBox.Contains(position2D) == ContainmentType.Contains)
                    {
                        if (entity.Collision == true)
                        {
                            if (entity.WalkAgainstFunction() == true)
                            {
                                cannotWalk = true;
                                if (Screen.Level.Surfing == true)
                                {
                                    DidWalkAgainst = true;
                                }
                            }
                        }
                        else
                        {
                            if (entity.WalkIntoFunction() == true)
                            {
                                cannotWalk = true;
                            }
                        }
                    }
                    else if (entity.BoundingBox.Contains(new Vector3(position2D.X, position2D.Y - 1f, position2D.Z)) == ContainmentType.Contains)
                    {
                        entity.WalkOntoFunction();
                    }
                }
            }
        }
        else
        {
            foreach (Entity entity in Screen.Level.Entities)
            {
                if (entity.BoundingBox.Contains(new Vector3(position2D.X, position2D.Y - 1f, position2D.Z)) == ContainmentType.Contains)
                {
                    entity.WalkOntoFunction();
                }
                if (Screen.Level.Surfing == true)
                {
                    if (entity.BoundingBox.Contains(position2D) == ContainmentType.Contains)
                    {
                        if (entity.Collision == true)
                        {
                            entity.WalkAgainstFunction();
                            if (entity.EntityID.Equals("AnimatedBlock") == false && entity.EntityID.Equals("Water") == false)
                            {
                                DidWalkAgainst = true;
                            }
                        }
                        else
                        {
                            entity.WalkIntoFunction();
                        }
                    }
                }
            }
        }

        if (cannotWalk == false && setSurfFalse == true)
        {
            if (Screen.Level.Surfing == true)
            {
                Screen.Level.Surfing = false;
                Core.Player.StartSurfing = false;
                Screen.Level.OwnPlayer!.SetTexture(Core.Player.TempSurfSkin, true);
                Core.Player.Skin = Core.Player.TempSurfSkin;

                Screen.Level.OverworldPokemon!.warped = true;
                Screen.Level.OverworldPokemon.Visible = false;

                if (Screen.Level.IsRadioOn == false ||
                    GameJolt.PokegearScreen.StationCanPlay(Screen.Level.SelectedRadioStation) == false)
                {
                    MusicManager.Play(Screen.Level.MusicLoop, true, 0.01f);
                }
            }
        }

        if (GameController.IS_DEBUG_ACTIVE == true || Core.Player.SandBoxMode == true)
        {
            if (_debugWalk == true &&
                Core.CurrentScreen.Identification.Equals(Screen.Identifications.OverworldScreen) == true &&
                ((OverworldScreen)Core.CurrentScreen).ActionScript.IsReady == true)
            {
                cannotWalk = false;
            }
        }

        return cannotWalk;
    }

    private int GetIceSteps(Vector3 newPosition)
    {
        Vector3 position2D = new Vector3(newPosition.X, newPosition.Y - ICE_CHECK_Y_OFFSET, newPosition.Z);
        foreach (Entity floor in Screen.Level!.Floors)
        {
            if (floor.BoundingBox.Contains(position2D) == ContainmentType.Contains)
            {
                if (((Floor)floor).IsIce == true)
                {
                    IsSliding = true;
                    return ((Floor)floor).GetIceFloors();
                }
                else
                {
                    IsSliding = false;
                }
            }
        }
        return 1;
    }

    public override Vector3 GetForwardMovedPosition()
    {
        return Position + GetMoveDirection();
    }

    public override Vector3 GetMoveDirection()
    {
        Vector3 v = _plannedMovement;

        switch (GetPlayerFacingDirection())
        {
            case 0:
                if (v.Z == 0f)
                {
                    v.Z = -1f;
                }
                break;
            case 1:
                if (v.X == 0f)
                {
                    v.X = -1f;
                }
                break;
            case 2:
                if (v.Z == 0f)
                {
                    v.Z = 1f;
                }
                break;
            case 3:
                if (v.X == 0f)
                {
                    v.X = 1f;
                }
                break;
        }

        if (GameController.IS_DEBUG_ACTIVE == true || Core.Player.SandBoxMode == true)
        {
            if (KeyBoardHandler.KeyDown(Keys.LeftAlt) == true)
            {
                if (KeyBoardHandler.KeyDown(KeyBindings.ForwardMoveKey) == true)
                {
                    v = new Vector3(0f, 1f, 0f);
                }
                else if (KeyBoardHandler.KeyDown(KeyBindings.BackwardMoveKey) == true)
                {
                    v = new Vector3(0f, -1f, 0f);
                }
            }
        }

        return v;
    }

    public override int GetPlayerFacingDirection()
    {
        if (ThirdPerson == false && _cameraFocusType == CameraFocusTypes.Player)
        {
            return GetFacingDirection();
        }
        else
        {
            return _thirdPersonFacing;
        }
    }

    public float GetAimYawFromDirection(int direction)
    {
        switch (direction)
        {
            case 0:
                return 0f;
            case 1:
                return MathHelper.Pi * 0.5f;
            case 2:
                return MathHelper.Pi;
            case 3:
                return MathHelper.Pi * 1.5f;
        }
        return 0f;
    }

    public override void Turn(int turns, bool forceCameraTurn = false, bool doPlayerTurn = true)
    {
        if (turns > 0)
        {
            if (ThirdPerson == true)
            {
                if (doPlayerTurn == true)
                {
                    _thirdPersonFacing += turns;
                    while (_thirdPersonFacing > 3)
                    {
                        _thirdPersonFacing -= 4;
                    }
                }
                Screen.Level!.OwnPlayer!.Opacity = 1f;
                if (forceCameraTurn == true)
                {
                    int facing;
                    if (doPlayerTurn == true)
                    {
                        facing = GetFacingDirection();
                        facing += _thirdPersonFacing - GetFacingDirection();
                        while (facing > 3)
                        {
                            facing -= 4;
                        }
                        Turning = true;
                        _aimDirection = facing;
                    }
                    else
                    {
                        facing = GetFacingDirection() + turns;
                        while (facing > 3)
                        {
                            facing -= 4;
                        }
                        Turning = true;
                        _aimDirection = facing;
                    }
                }
            }
            else
            {
                int facing = GetFacingDirection() + turns;
                while (facing > 3)
                {
                    facing -= 4;
                }
                Turning = true;
                _aimDirection = facing;
            }
        }
        else
        {
            if (ThirdPerson == true && forceCameraTurn == true)
            {
                int facing = GetFacingDirection();
                facing += _thirdPersonFacing - GetFacingDirection();
                while (facing > 3)
                {
                    facing -= 4;
                }
                Turning = true;
                _aimDirection = facing;
            }
        }
    }

    public override void InstantTurn(int turns)
    {
        if (turns <= 0) return;

        if (ThirdPerson == true)
        {
            Yaw += GetAimYawFromDirection(turns);
            ClampYaw();

            _thirdPersonFacing += turns;
            while (_thirdPersonFacing > 3)
            {
                _thirdPersonFacing -= 4;
            }
            Screen.Level!.OwnPlayer!.Opacity = 1f;
        }
        else
        {
            int newFacing = GetFacingDirection() + turns;
            while (newFacing > 3)
            {
                newFacing -= 4;
            }
            Yaw = GetAimYawFromDirection(newFacing);
        }
    }

    private void CheckEntities()
    {
        if (Controls.Accept() == false) return;
        if (_moved != 0f || Turning == true) return;

        Vector3 checkPosition = GetForwardMovedPosition();
        checkPosition.Y -= COLLISION_Y_OFFSET;

        for (int i = 0; i < Screen.Level!.Entities.Count; i++)
        {
            if (i <= Screen.Level.Entities.Count - 1)
            {
                float? result = Screen.Level.Entities[i].BoundingBox.Intersects(Ray);
                bool rayIntersects = true;
                if (result.HasValue == true)
                {
                    float minValue = RAY_DISTANCE_MIN;
                    if (ThirdPerson == true)
                    {
                        minValue += RAY_DISTANCE_THIRD_PERSON_EXTRA;
                    }
                    if (result.Value < minValue)
                    {
                        rayIntersects = true;
                    }
                }
                if (Core.CurrentScreen.Identification.Equals(Screen.Identifications.OverworldScreen) == true &&
                    ((OverworldScreen)Core.CurrentScreen).ActivatedScriptedNotification == false &&
                    rayIntersects == true &&
                    Screen.Level.Entities[i].BoundingBox.Contains(checkPosition) == ContainmentType.Contains)
                {
                    Screen.Level.Entities[i].ClickFunction();
                }
            }
            else
            {
                break;
            }
        }
    }

    public override void Move(float steps)
    {
        if (steps == 0f) return;

        _moved += steps;
        _tempAmountOfSteps += (int)Math.Ceiling(steps);
        if (_setPlannedMovement == false)
        {
            _plannedMovement = GetMoveDirection();
        }
    }

    public override void StopMovement()
    {
        _moved = 0f;
        _plannedMovement = Vector3.Zero;
        _setPlannedMovement = false;
    }

    private void ControlThirdPersonCamera()
    {
        if (GameController.IS_DEBUG_ACTIVE == true || Core.Player.SandBoxMode == true)
        {
            if (Controls.CtrlPressed() == true)
            {
                if (KeyBoardHandler.KeyDown(KeyBindings.UpKey) == true)
                {
                    ThirdPersonOffset = new Vector3(ThirdPersonOffset.X, ThirdPersonOffset.Y + Speed, ThirdPersonOffset.Z);
                }
                if (KeyBoardHandler.KeyDown(KeyBindings.DownKey) == true)
                {
                    ThirdPersonOffset = new Vector3(ThirdPersonOffset.X, ThirdPersonOffset.Y - Speed, ThirdPersonOffset.Z);
                }
            }
            else
            {
                if (KeyBoardHandler.KeyDown(KeyBindings.UpKey) == true)
                {
                    ThirdPersonOffset = new Vector3(ThirdPersonOffset.X, ThirdPersonOffset.Y, ThirdPersonOffset.Z - Speed);
                }
                if (KeyBoardHandler.KeyDown(KeyBindings.DownKey) == true)
                {
                    ThirdPersonOffset = new Vector3(ThirdPersonOffset.X, ThirdPersonOffset.Y, ThirdPersonOffset.Z + Speed);
                }
                if (KeyBoardHandler.KeyDown(KeyBindings.RightKey) == true)
                {
                    ThirdPersonOffset = new Vector3(ThirdPersonOffset.X + Speed, ThirdPersonOffset.Y, ThirdPersonOffset.Z);
                }
                if (KeyBoardHandler.KeyDown(KeyBindings.LeftKey) == true)
                {
                    ThirdPersonOffset = new Vector3(ThirdPersonOffset.X - Speed, ThirdPersonOffset.Y, ThirdPersonOffset.Z);
                }
            }
        }
        else
        {
            int pressedDirection = -1;
            float controllerTurnModifier = 1f;
            if (ControllerHandler.ButtonDown(Buttons.RightThumbstickLeft) == true ||
                ControllerHandler.ButtonDown(Buttons.RightThumbstickRight) == true)
            {
                controllerTurnModifier = CONTROLLER_TURN_MODIFIER_SLOW;
            }

            if (YawLocked == false && Turning == false)
            {
                if ((KeyBoardHandler.KeyDown(KeyBindings.LeftKey) == true || ControllerHandler.ButtonDown(Buttons.RightThumbstickLeft) == true) && Turning == false)
                {
                    if (_freeCameraMode == true && ControllerHandler.ButtonDown(Buttons.RightThumbstickLeft) == false)
                    {
                        Yaw += RotationSpeed * AIM_SPEED_MULTIPLIER * controllerTurnModifier;
                        ClampYaw();
                    }
                    else
                    {
                        pressedDirection = 1;
                    }
                }
                if ((KeyBoardHandler.KeyDown(KeyBindings.RightKey) == true || ControllerHandler.ButtonDown(Buttons.RightThumbstickRight) == true) && Turning == false)
                {
                    if (_freeCameraMode == true && ControllerHandler.ButtonDown(Buttons.RightThumbstickRight) == false)
                    {
                        Yaw -= RotationSpeed * AIM_SPEED_MULTIPLIER * controllerTurnModifier;
                        ClampYaw();
                    }
                    else
                    {
                        pressedDirection = 3;
                    }
                }

                if (_freeCameraMode == false && pressedDirection > -1)
                {
                    if (_moved <= 0f)
                    {
                        Turn(pressedDirection, true, false);
                    }
                    else
                    {
                        _tempDirectionPressed = pressedDirection;
                    }
                }

                ClampYaw();
            }

            if (Fixed == false)
            {
                if (Core.CurrentScreen.Identification.Equals(Screen.Identifications.OverworldScreen) == true)
                {
                    OverworldScreen os = (OverworldScreen)Core.CurrentScreen;
                    if (os.NotificationPopupList.Count == 0 || os.NotificationPopupList[0]._forceAccept == false)
                    {
                        if (os.ActionScript.IsReady == true)
                        {
                            if (KeyBoardHandler.KeyDown(KeyBindings.UpKey) == true && Turning == false)
                            {
                                Pitch += RotationSpeed * CONTROLLER_PITCH_SPEED * controllerTurnModifier;
                                ClampPitch();
                            }
                            if (KeyBoardHandler.KeyDown(KeyBindings.DownKey) == true && Turning == false)
                            {
                                Pitch -= RotationSpeed * CONTROLLER_PITCH_SPEED * controllerTurnModifier;
                                ClampPitch();
                            }
                        }
                    }
                }
            }
        }
    }
}
