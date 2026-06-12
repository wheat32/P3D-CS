using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using P3D;

namespace P3D.BattleSystem;

// ---------------------------------------------------------------------------
// BattleAnimation3D — base class for all 3D battle animations
// ---------------------------------------------------------------------------

public class BattleAnimation3D : Entity
{
    public enum AnchorTypes { Top, Left, Right, Bottom }

    public enum AnimationTypes
    {
        Nothing,
        Move,
        Transition,
        Size,
        Rotation,
        Texture,
        Sound,
        Background,
        Camera
    }

    public AnimationTypes animationType = AnimationTypes.Nothing;
    public bool canRemove = false;
    public bool ready = false;
    public DateTime startDelay;
    public DateTime endDelay;

    private bool _spawnedEntity = false;
    private bool _started = false;
    private const float DELAY_DIVIDE = 6.0f;
    private float _startDelayWhole;
    private float _startDelayFraction;
    private float _endDelayWhole;
    private float _endDelayFraction;
    private bool _hasStartedEndDelay = false;

    public BattleAnimation3D(Vector3 position, Texture2D texture, Vector3 scale,
                              float startDelay, float endDelay, bool spawnedEntity = false)
        : base(position.X, position.Y, position.Z, "BattleAnimation",
               [texture], [0, 0], false, 0, scale, BaseModel.BillModel, 0, String.Empty, Vector3.One)
    {
        _startDelayWhole = (float)Math.Truncate((double)(startDelay / DELAY_DIVIDE));
        _startDelayFraction = startDelay / DELAY_DIVIDE - _startDelayWhole;
        _endDelayWhole = (float)Math.Truncate((double)(endDelay / DELAY_DIVIDE));
        _endDelayFraction = endDelay / DELAY_DIVIDE - _endDelayWhole;

        _spawnedEntity = spawnedEntity;
        CreateWorldEveryFrame = true;
        DropUpdateUnlessDrawn = false;
    }

    public override void Update()
    {
        if (_started == false)
        {
            this.startDelay = DateTime.Now + new TimeSpan(0, 0, 0,
                (int)_startDelayWhole, (int)(_startDelayFraction * 1000));
            _hasStartedEndDelay = false;
            _started = true;
        }
        if (canRemove == false)
        {
            if (ready == true)
            {
                if (_hasStartedEndDelay == false)
                {
                    this.endDelay = DateTime.Now + new TimeSpan(0, 0, 0,
                        (int)_endDelayWhole, (int)(_endDelayFraction * 1000));
                    _hasStartedEndDelay = true;
                }
                if (DateTime.Now >= endDelay)
                {
                    DoRemoveEntity();
                    canRemove = true;
                }
            }
            else
            {
                if (DateTime.Now >= startDelay)
                {
                    if (_spawnedEntity == true)
                    {
                        ready = true;
                    }
                    else
                    {
                        Visible = true;
                    }
                    DoActionActive();
                }
            }
        }
    }

    public override void UpdateEntity()
    {
        if (Rotation.Y != Screen.Camera!.Yaw)
        {
            Rotation.Y = Screen.Camera.Yaw;
        }

        DoActionUpdate();

        base.UpdateEntity();
    }

    public virtual void DoActionUpdate() { }
    public virtual void DoActionActive() { }
    public virtual void DoRemoveEntity() { }

    public override void Render()
    {
        if (DateTime.Now >= startDelay)
        {
            if (canRemove == false)
            {
                if (Model == null)
                {
                    Draw(BaseModel, Textures, true);
                }
                else
                {
                    UpdateModel();
                    Draw(BaseModel, Textures, true, Model);
                }
            }
        }
    }
}

// ---------------------------------------------------------------------------
// BABackground — fading overlay background texture
// ---------------------------------------------------------------------------

public class BABackground : BattleAnimation3D
{
    public enum FadeSteps { FadeIn, Duration, FadeOut }

    private const float BACKGROUND_DELAY_DIVIDE = 6.0f;

    private float _fadeInSpeed = 0.01f;
    private float _fadeOutSpeed = 0.01f;
    private float _backgroundOpacity = 0.0f;
    private Texture2D _texture;
    private bool _doTile = false;
    private int _animationWidth = -1;
    private float _afterFadeInOpacity = 1.0f;
    private FadeSteps _fadeProgress = FadeSteps.FadeIn;
    private DateTime _durationDate;
    private float _durationWhole;
    private float _durationFraction;
    private Animation? _backgroundAnimation;
    private Rectangle _currentRectangle = new Rectangle(0, 0, 0, 0);
    private int _textureScale = 4;

    public BABackground(Texture2D texture, float startDelay, float endDelay, float duration,
                        float afterFadeInOpacity = 1.0f, float fadeInSpeed = 0.125f,
                        float fadeOutSpeed = 0.125f, bool doTile = false,
                        int animationLength = 1, int animationSpeed = 2, int textureScale = 4)
        : base(Vector3.Zero, TextureManager.DefaultTexture!, Vector3.One, startDelay, endDelay)
    {
        _texture = texture;
        _afterFadeInOpacity = afterFadeInOpacity;
        _fadeInSpeed = fadeInSpeed;
        _fadeOutSpeed = fadeOutSpeed;
        _doTile = doTile;
        _animationWidth = texture.Width / animationLength;
        _durationWhole = (float)Math.Truncate((double)(duration / BACKGROUND_DELAY_DIVIDE));
        _durationFraction = (float)((duration / BACKGROUND_DELAY_DIVIDE - _durationWhole) * 1000);
        _textureScale = textureScale;

        if (_animationWidth != -1)
        {
            _backgroundAnimation = new Animation(_texture, 1, animationLength,
                _animationWidth, _texture.Height, animationSpeed * 24, 0, 0);
            _currentRectangle = _backgroundAnimation.TextureRectangle;
        }
        else
        {
            _animationWidth = texture.Width;
        }
        Visible = false;
        animationType = AnimationTypes.Background;
    }

    public override void Render()
    {
        RenderTarget2D backgroundTarget = new RenderTarget2D(Core.GraphicsDevice,
            Core.windowSize.Width, Core.windowSize.Height,
            false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
        Core.GraphicsDevice.SetRenderTarget(backgroundTarget);
        Core.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Transparent);

        if (DateTime.Now >= startDelay && _backgroundOpacity > 0.0f)
        {
            if (_doTile == false)
            {
                Core.SpriteBatch.Draw(_texture,
                    new Rectangle(0, 0, Core.windowSize.Width, Core.windowSize.Height),
                    _currentRectangle,
                    new Microsoft.Xna.Framework.Color(255, 255, 255, (int)(255 * _backgroundOpacity)));
            }
            else
            {
                for (int dx = 0; dx <= Core.windowSize.Width; dx += _animationWidth)
                {
                    for (int dy = 0; dy <= Core.windowSize.Height; dy += _texture.Height)
                    {
                        Core.SpriteBatch.Draw(_texture,
                            new Rectangle(dx * _textureScale, dy * _textureScale,
                                _animationWidth * _textureScale, _texture.Height * _textureScale),
                            _currentRectangle,
                            new Microsoft.Xna.Framework.Color(255, 255, 255, (int)(255 * _backgroundOpacity)));
                    }
                }
            }
        }
        Core.GraphicsDevice.SetRenderTarget(null);
        Core.SpriteBatch.Draw(backgroundTarget, Core.windowSize,
            new Microsoft.Xna.Framework.Color(255, 255, 255, (int)(255 * _backgroundOpacity)));
    }

    public override void DoActionActive()
    {
        if (_backgroundAnimation != null)
        {
            _backgroundAnimation.Update(0.005f);
            if (_currentRectangle != _backgroundAnimation.TextureRectangle)
            {
                _currentRectangle = _backgroundAnimation.TextureRectangle;
            }
        }
        switch (_fadeProgress)
        {
            case FadeSteps.FadeIn:
                if (_afterFadeInOpacity > _backgroundOpacity)
                {
                    _backgroundOpacity += _fadeInSpeed;
                    if (_backgroundOpacity >= _afterFadeInOpacity)
                    {
                        _durationDate = DateTime.Now + new TimeSpan(0, 0, 0,
                            (int)_durationWhole, (int)_durationFraction);
                        _fadeProgress = FadeSteps.Duration;
                        _backgroundOpacity = _afterFadeInOpacity;
                    }
                }
                else
                {
                    _fadeProgress = FadeSteps.Duration;
                    _backgroundOpacity = _afterFadeInOpacity;
                }
                break;

            case FadeSteps.Duration:
                if (DateTime.Now >= _durationDate)
                {
                    _fadeProgress = FadeSteps.FadeOut;
                }
                break;

            case FadeSteps.FadeOut:
                if (_backgroundOpacity > 0.0f)
                {
                    _backgroundOpacity -= _fadeOutSpeed;
                    if (_backgroundOpacity <= 0.0f)
                    {
                        _backgroundOpacity = 0.0f;
                        ready = true;
                    }
                }
                else
                {
                    _backgroundOpacity = 0.0f;
                    ready = true;
                }
                break;

            default:
                break;
        }
    }
}

// ---------------------------------------------------------------------------
// BACameraChangeAngle — instantly changes the battle camera angle
// ---------------------------------------------------------------------------

public class BACameraChangeAngle : BattleAnimation3D
{
    private int _cameraAngleID;
    private BattleScreen _bv2Screen;

    public BACameraChangeAngle(BattleScreen battlescreen, int cameraAngleID,
                                float startDelay, float endDelay)
        : base(Vector3.Zero, TextureManager.DefaultTexture!, Vector3.One, startDelay, endDelay)
    {
        _bv2Screen = battlescreen;
        _cameraAngleID = cameraAngleID;
        Visible = false;
        animationType = AnimationTypes.Camera;
    }

    public override void DoActionActive()
    {
        switch (_cameraAngleID)
        {
            case 0:
                _bv2Screen.Battle.ChangeCameraAngle(0, true, _bv2Screen);
                break;

            case 1:
                _bv2Screen.Battle.ChangeCameraAngle(1, true, _bv2Screen);
                break;

            case 2:
                _bv2Screen.Battle.ChangeCameraAngle(2, true, _bv2Screen);
                break;

            default:
                break;
        }
        ready = true;
    }
}

// ---------------------------------------------------------------------------
// BACameraOscillateMove — oscillates the battle camera back and forth
// ---------------------------------------------------------------------------

public class BACameraOscillateMove : BattleAnimation3D
{
    public enum Curves { Linear, Smooth }

    private Vector3 _startPosition;
    private Vector3 _returnToStart = Vector3.Zero;
    private Vector3 _halfDistance = Vector3.Zero;
    private Vector3 _destinationDistance = Vector3.Zero;
    private Vector3 _currentDistance = Vector3.Zero;
    private float _moveSpeed;
    private bool _moveBothWays = true;
    private Curves _movementCurve = Curves.Linear;
    private TimeSpan _duration;
    private DateTime _readyTime;
    private Vector3 _readyAxis = Vector3.Zero;
    private Vector3 _interpolationSpeed;
    private bool _interpolationDirection = true;

    public BACameraOscillateMove(Vector3 distance, float speed, bool bothWays,
                                  TimeSpan duration, float startDelay, float endDelay,
                                  int movementCurve = 0, Vector3 returnToStart = default)
        : base(Vector3.Zero, TextureManager.DefaultTexture!, Vector3.One, startDelay, endDelay)
    {
        _halfDistance = distance;
        _destinationDistance = _halfDistance;
        _moveSpeed = speed;
        _moveBothWays = bothWays;
        _duration = duration;
        _movementCurve = (Curves)movementCurve;

        Visible = false;
        switch (_movementCurve)
        {
            case Curves.Linear:
                _interpolationSpeed = new Vector3(_moveSpeed);
                break;

            case Curves.Smooth:
                _interpolationSpeed = Vector3.Zero;
                break;

            default:
                _interpolationSpeed = new Vector3(_moveSpeed);
                break;
        }
        if (returnToStart != Vector3.Zero)
        {
            _returnToStart = returnToStart;
        }
        animationType = AnimationTypes.Move;
    }

    public override void DoActionActive()
    {
        Move();
    }

    private void Move()
    {
        if (_startPosition == Vector3.Zero)
        {
            _startPosition = Screen.Camera!.Position;
        }
        if (_readyTime == default(DateTime))
        {
            _readyTime = DateTime.Now + _duration;
        }
        if (_movementCurve == Curves.Smooth)
        {
            if (_interpolationDirection == true)
            {
                if (_interpolationSpeed.X < _moveSpeed)
                {
                    _interpolationSpeed.X += _moveSpeed / 10;
                }
                else
                {
                    _interpolationSpeed.X = _moveSpeed;
                }
                if (_interpolationSpeed.Y < _moveSpeed)
                {
                    _interpolationSpeed.Y += _moveSpeed / 10;
                }
                else
                {
                    _interpolationSpeed.Y = _moveSpeed;
                }
                if (_interpolationSpeed.Z < _moveSpeed)
                {
                    _interpolationSpeed.Z += _moveSpeed / 10;
                }
                else
                {
                    _interpolationSpeed.Z = _moveSpeed;
                }
            }
            else
            {
                if (DateTime.Now < _readyTime)
                {
                    if (_interpolationSpeed.X > 0)
                    {
                        _interpolationSpeed.X -= _moveSpeed / 10;
                    }
                    else
                    {
                        _interpolationSpeed.X = 0;
                    }
                    if (_interpolationSpeed.Y > 0)
                    {
                        _interpolationSpeed.Y -= _moveSpeed / 10;
                    }
                    else
                    {
                        _interpolationSpeed.Y = 0;
                    }
                    if (_interpolationSpeed.Z > 0)
                    {
                        _interpolationSpeed.Z -= _moveSpeed / 10;
                    }
                    else
                    {
                        _interpolationSpeed.Z = 0;
                    }
                }
                else
                {
                    if (_interpolationSpeed.X > _moveSpeed / 10 * 3)
                    {
                        _interpolationSpeed.X -= _moveSpeed / 10;
                    }
                    else
                    {
                        _interpolationSpeed.X = _moveSpeed / 10 * 3;
                    }
                    if (_interpolationSpeed.Y > _moveSpeed / 10 * 3)
                    {
                        _interpolationSpeed.Y -= _moveSpeed / 10;
                    }
                    else
                    {
                        _interpolationSpeed.Y = _moveSpeed / 10 * 3;
                    }
                    if (_interpolationSpeed.Z > _moveSpeed / 10 * 3)
                    {
                        _interpolationSpeed.Z -= _moveSpeed / 10;
                    }
                    else
                    {
                        _interpolationSpeed.Z = _moveSpeed / 10 * 3;
                    }
                }
            }
        }

        MoveAxis_X();
        MoveAxis_Y();
        MoveAxis_Z();

        Camera cam = Screen.Camera!;
        Vector3 pos = cam.Position;
        if (_halfDistance.X != 0.0f)
        {
            pos.X = _startPosition.X + _currentDistance.X;
        }
        else
        {
            _readyAxis.X = 1.0f;
        }
        if (_halfDistance.Y != 0.0f)
        {
            pos.Y = _startPosition.Y + _currentDistance.Y;
        }
        else
        {
            _readyAxis.Y = 1.0f;
        }
        if (_halfDistance.Z != 0.0f)
        {
            pos.Z = _startPosition.Z + _currentDistance.Z;
        }
        else
        {
            _readyAxis.Z = 1.0f;
        }
        cam.Position = pos;

        if (DateTime.Now > _readyTime &&
            _readyAxis.X == 1.0f && _readyAxis.Y == 1.0f && _readyAxis.Z == 1.0f)
        {
            ready = true;
        }
    }

    private void MoveAxis_X()
    {
        if (_currentDistance.X != _destinationDistance.X)
        {
            if (_currentDistance.X < _destinationDistance.X)
            {
                _currentDistance.X += _interpolationSpeed.X;
            }
            else
            {
                _currentDistance.X -= _interpolationSpeed.X;
            }
            if (Math.Abs(_currentDistance.X) / Math.Abs(_halfDistance.X) > 0.75f)
            {
                _interpolationDirection = false;
            }
            if (_destinationDistance.X > 0.0f)
            {
                if (_currentDistance.X >= _destinationDistance.X)
                {
                    _currentDistance.X = _destinationDistance.X;
                }
            }
            else if (_destinationDistance.X < 0.0f)
            {
                if (_currentDistance.X <= _destinationDistance.X)
                {
                    _currentDistance.X = _destinationDistance.X;
                }
            }
            else
            {
                if (_currentDistance.X > _destinationDistance.X)
                {
                    if (_currentDistance.X - _interpolationSpeed.X <= _destinationDistance.X)
                    {
                        _currentDistance.X = _destinationDistance.X;
                    }
                }
                else
                {
                    if (_currentDistance.X + _interpolationSpeed.X >= _destinationDistance.X)
                    {
                        _currentDistance.X = _destinationDistance.X;
                    }
                }
            }
        }
        else
        {
            if (DateTime.Now < _readyTime)
            {
                if (_moveBothWays == true)
                {
                    _destinationDistance.X = _destinationDistance.X > 0.0f
                        ? 0.0f - Math.Abs(_halfDistance.X) * 2
                        : 0.0f + Math.Abs(_halfDistance.X) * 2;
                }
                else
                {
                    _destinationDistance.X = _destinationDistance.X > 0.0f ? 0.0f : _halfDistance.X;
                }
                _interpolationDirection = true;
            }
            else
            {
                if (_returnToStart.X == 0.0f)
                {
                    _readyAxis.X = 1.0f;
                }
                else
                {
                    if (_destinationDistance.X != 0.0f)
                    {
                        _destinationDistance.X = 0.0f;
                        _interpolationDirection = true;
                    }
                    if (_currentDistance.X == 0.0f)
                    {
                        _readyAxis.X = 1.0f;
                    }
                }
            }
        }
    }

    private void MoveAxis_Y()
    {
        if (_currentDistance.Y != _destinationDistance.Y)
        {
            if (_currentDistance.Y < _destinationDistance.Y)
            {
                _currentDistance.Y += _interpolationSpeed.Y;
            }
            else
            {
                _currentDistance.Y -= _interpolationSpeed.Y;
            }
            if (Math.Abs(_currentDistance.Y) / Math.Abs(_halfDistance.Y) > 0.75f)
            {
                _interpolationDirection = false;
            }
            if (_destinationDistance.Y > 0.0f)
            {
                if (_currentDistance.Y >= _destinationDistance.Y)
                {
                    _currentDistance.Y = _destinationDistance.Y;
                }
            }
            else if (_destinationDistance.Y < 0.0f)
            {
                if (_currentDistance.Y <= _destinationDistance.Y)
                {
                    _currentDistance.Y = _destinationDistance.Y;
                }
            }
            else
            {
                if (_currentDistance.Y > _destinationDistance.Y)
                {
                    if (_currentDistance.Y - _interpolationSpeed.Y <= _destinationDistance.Y)
                    {
                        _currentDistance.Y = _destinationDistance.Y;
                    }
                }
                else
                {
                    if (_currentDistance.Y + _interpolationSpeed.Y >= _destinationDistance.Y)
                    {
                        _currentDistance.Y = _destinationDistance.Y;
                    }
                }
            }
        }
        else
        {
            if (DateTime.Now < _readyTime)
            {
                if (_moveBothWays == true)
                {
                    _destinationDistance.Y = _destinationDistance.Y > 0.0f
                        ? 0.0f - Math.Abs(_halfDistance.Y) * 2
                        : 0.0f + Math.Abs(_halfDistance.Y) * 2;
                }
                else
                {
                    _destinationDistance.Y = _destinationDistance.Y > 0.0f ? 0.0f : _halfDistance.Y;
                }
                _interpolationDirection = true;
            }
            else
            {
                if (_returnToStart.Y == 0.0f)
                {
                    _readyAxis.Y = 1.0f;
                }
                else
                {
                    if (_destinationDistance.Y != 0.0f)
                    {
                        _destinationDistance.Y = 0.0f;
                        _interpolationDirection = true;
                    }
                    if (_currentDistance.Y == 0.0f)
                    {
                        _readyAxis.Y = 1.0f;
                    }
                }
            }
        }
    }

    private void MoveAxis_Z()
    {
        if (_currentDistance.Z != _destinationDistance.Z)
        {
            if (_currentDistance.Z < _destinationDistance.Z)
            {
                _currentDistance.Z += _interpolationSpeed.Z;
            }
            else
            {
                _currentDistance.Z -= _interpolationSpeed.Z;
            }
            if (Math.Abs(_currentDistance.Y) / Math.Abs(_halfDistance.Y) > 0.75f)
            {
                _interpolationDirection = false;
            }
            if (_destinationDistance.Z > 0.0f)
            {
                if (_currentDistance.Z >= _destinationDistance.Z)
                {
                    _currentDistance.Z = _destinationDistance.Z;
                }
            }
            else if (_destinationDistance.Z < 0.0f)
            {
                if (_currentDistance.Z <= _destinationDistance.Z)
                {
                    _currentDistance.Z = _destinationDistance.Z;
                }
            }
            else
            {
                if (_currentDistance.Z > _destinationDistance.Z)
                {
                    if (_currentDistance.Z - _interpolationSpeed.Z <= _destinationDistance.Z)
                    {
                        _currentDistance.Z = _destinationDistance.Z;
                    }
                }
                else
                {
                    if (_currentDistance.Z + _interpolationSpeed.Z >= _destinationDistance.Z)
                    {
                        _currentDistance.Z = _destinationDistance.Z;
                    }
                }
            }
        }
        else
        {
            if (DateTime.Now < _readyTime)
            {
                if (_moveBothWays == true)
                {
                    _destinationDistance.Z = _destinationDistance.Z > 0.0f
                        ? 0.0f - Math.Abs(_halfDistance.Z) * 2
                        : 0.0f + Math.Abs(_halfDistance.Z) * 2;
                }
                else
                {
                    _destinationDistance.Z = _destinationDistance.Z > 0.0f ? 0.0f : _halfDistance.Z;
                }
                _interpolationDirection = true;
            }
            else
            {
                if (_returnToStart.Z == 0.0f)
                {
                    _readyAxis.Z = 1.0f;
                }
                else
                {
                    if (_destinationDistance.Z != 0.0f)
                    {
                        _destinationDistance.Z = 0.0f;
                        _interpolationDirection = true;
                    }
                    if (_currentDistance.Z == 0.0f)
                    {
                        _readyAxis.Z = 1.0f;
                    }
                }
            }
        }
    }
}

// ---------------------------------------------------------------------------
// BAEntityColor — transitions an entity's color vector
// ---------------------------------------------------------------------------

public class BAEntityColor : BattleAnimation3D
{
    private Entity _targetEntity;
    private float _transitionSpeed = 0.01f;
    private float _transitionSpeedOut = 0.01f;
    private bool _returnToFromWhenDone = false;
    private bool _removeEntityAfter = false;
    private bool _initialColorSet = false;
    private bool _isReturning = false;
    private Vector3 _colorTo = Vector3.One;
    private Vector3 _colorFrom = Vector3.One;

    public BAEntityColor(Entity entity, bool removeEntityAfter, float transitionSpeedIn,
                          bool returnToFromWhenDone, float startDelay, float endDelay,
                          Vector3 colorTo, float transitionSpeedOut = -1,
                          Vector3 colorFrom = default)
        : base(Vector3.Zero, TextureManager.DefaultTexture!, Vector3.One, startDelay, endDelay)
    {
        _removeEntityAfter = removeEntityAfter;
        _transitionSpeedOut = transitionSpeedOut == -1 ? transitionSpeedIn : transitionSpeedOut;
        _transitionSpeed = transitionSpeedIn;
        _targetEntity = entity;
        _returnToFromWhenDone = returnToFromWhenDone;

        _colorFrom = colorFrom != Vector3.Zero ? colorFrom : _targetEntity.Color;
        _colorTo = colorTo;

        Visible = false;
        animationType = AnimationTypes.Transition;
    }

    public override void DoActionActive()
    {
        if (_initialColorSet == false)
        {
            _targetEntity.Color = _colorFrom;
            _initialColorSet = true;
        }

        if (_targetEntity.Color.X > _colorTo.X)
        {
            _targetEntity.Color.X -= _transitionSpeed;
            if (_targetEntity.Color.X <= _colorTo.X)
            {
                _targetEntity.Color.X = _colorTo.X;
            }
        }
        else if (_targetEntity.Color.X < _colorTo.X)
        {
            _targetEntity.Color.X += _transitionSpeed;
            if (_targetEntity.Color.X >= _colorTo.X)
            {
                _targetEntity.Color.X = _colorTo.X;
            }
        }

        if (_targetEntity.Color.Y > _colorTo.Y)
        {
            _targetEntity.Color.Y -= _transitionSpeed;
            if (_targetEntity.Color.Y <= _colorTo.Y)
            {
                _targetEntity.Color.Y = _colorTo.Y;
            }
        }
        else if (_targetEntity.Color.Y < _colorTo.Y)
        {
            _targetEntity.Color.Y += _transitionSpeed;
            if (_targetEntity.Color.Y >= _colorTo.Y)
            {
                _targetEntity.Color.Y = _colorTo.Y;
            }
        }

        if (_targetEntity.Color.Z > _colorTo.Z)
        {
            _targetEntity.Color.Z -= _transitionSpeed;
            if (_targetEntity.Color.Z <= _colorTo.Z)
            {
                _targetEntity.Color.Z = _colorTo.Z;
            }
        }
        else if (_targetEntity.Color.Z < _colorTo.Z)
        {
            _targetEntity.Color.Z += _transitionSpeed;
            if (_targetEntity.Color.Z >= _colorTo.Z)
            {
                _targetEntity.Color.Z = _colorTo.Z;
            }
        }

        if (_targetEntity.Color == _colorTo)
        {
            if (_returnToFromWhenDone == false)
            {
                ready = true;
            }
            else
            {
                if (_isReturning == false)
                {
                    _colorTo = _colorFrom;
                    _transitionSpeed = _transitionSpeedOut;
                    _isReturning = true;
                }
                else
                {
                    ready = true;
                }
            }
        }
    }

    public override void DoRemoveEntity()
    {
        if (_removeEntityAfter == true)
        {
            _targetEntity.CanBeRemoved = true;
        }
    }
}

// ---------------------------------------------------------------------------
// BAEntityFaceRotate — rotates an NPC's face direction in steps
// ---------------------------------------------------------------------------

public class BAEntityFaceRotate : BattleAnimation3D
{
    private const float FACE_ROTATE_DELAY_DIVIDE = 6.0f;

    private NPC _targetEntity;
    private int _endFaceRotation;
    private int _turnSteps = 0;
    private int _turnSpeed = 1;
    private DateTime _turnTime = DateTime.Now;
    private float _turnDelayWhole = 0.0f;
    private float _turnDelayFraction = 0.0f;
    private bool _initialRotationSet = false;

    public BAEntityFaceRotate(NPC targetEntity, int turnSteps, float startDelay, float endDelay,
                               int endFaceRotation = -1, float turnDelay = 0.25f,
                               int turnSpeed = 1)
        : base(Vector3.Zero, TextureManager.DefaultTexture!, Vector3.One, startDelay, endDelay)
    {
        _turnSteps = turnSteps.ToPositive();
        _targetEntity = targetEntity;
        _turnSpeed = turnSpeed;
        _endFaceRotation = endFaceRotation;

        _turnDelayWhole = (float)Math.Truncate((double)(turnDelay / FACE_ROTATE_DELAY_DIVIDE));
        _turnDelayFraction = turnDelay / FACE_ROTATE_DELAY_DIVIDE - _turnDelayWhole;

        animationType = AnimationTypes.Rotation;
    }

    public override void DoActionActive()
    {
        if (_initialRotationSet == false)
        {
            if (_endFaceRotation == -1)
            {
                _endFaceRotation = _targetEntity.faceRotation;
            }
            _initialRotationSet = true;
        }

        if (_turnSteps > 0)
        {
            if (DateTime.Now >= _turnTime)
            {
                _targetEntity.faceRotation += _turnSpeed;
                if (_targetEntity.faceRotation > 3)
                {
                    _targetEntity.faceRotation -= 4;
                }
                if (_targetEntity.faceRotation < 0)
                {
                    _targetEntity.faceRotation += 4;
                }
                _turnSteps -= _turnSpeed.ToPositive();
                _turnTime = DateTime.Now + new TimeSpan(0, 0, 0,
                    (int)_turnDelayWhole, (int)(_turnDelayFraction * 1000));
            }
        }
        else
        {
            if (_targetEntity.faceRotation != _endFaceRotation)
            {
                _targetEntity.faceRotation = _endFaceRotation;
            }
            ready = true;
        }
    }
}

// ---------------------------------------------------------------------------
// BAEntityMove — moves an entity to a destination with optional spin
// ---------------------------------------------------------------------------

public class BAEntityMove : BattleAnimation3D
{
    public enum Curves { EaseIn, EaseOut, EaseInAndOut, Linear }

    private Vector3 _startPosition;
    private Entity _targetEntity;
    private Vector3 _destination;
    private Vector3 _moveDistance = Vector3.Zero;
    private float _moveSpeed;
    private float _moveYSpeed;
    private float _interpolationSpeed;
    private float _interpolationYSpeed;
    private bool _spinX = false;
    private bool _spinZ = false;
    private float _spinSpeedX = 0.1f;
    private float _spinSpeedZ = 0.1f;
    private Curves _movementCurve = Curves.Linear;
    private bool _easedIn = false;
    private bool _easedOut = false;
    public bool removeEntityAfter;
    private Vector3 _readyAxis = Vector3.Zero;

    public BAEntityMove(Entity entity, bool removeEntityAfter, Vector3 destination,
                        float speed, bool spinX, bool spinZ,
                        float startDelay, float endDelay,
                        float spinXSpeed = 0.1f, float spinZSpeed = 0.1f,
                        int movementCurve = 3, float moveYSpeed = 0.0f)
        : base(Vector3.Zero, TextureManager.DefaultTexture!, Vector3.One, startDelay, endDelay)
    {
        this.removeEntityAfter = removeEntityAfter;
        _destination = destination;
        _moveSpeed = speed;
        _moveYSpeed = moveYSpeed == 0f ? _moveSpeed : moveYSpeed;
        _movementCurve = (Curves)movementCurve;

        _spinX = spinX;
        _spinZ = spinZ;
        _spinSpeedX = spinXSpeed;
        _spinSpeedZ = spinZSpeed;

        Visible = false;
        _targetEntity = entity;

        _startPosition = _targetEntity.Position;
        _moveDistance.X = Math.Abs(_startPosition.X - _destination.X);
        _moveDistance.Y = _targetEntity.Model != null
            ? Math.Abs(_startPosition.Y - _destination.Y - 0.5f)
            : Math.Abs(_startPosition.Y - _destination.Y);
        _moveDistance.Z = Math.Abs(_startPosition.Z - _destination.Z);

        switch (_movementCurve)
        {
            case Curves.EaseIn:
                _interpolationSpeed = 0.0f;
                _interpolationYSpeed = 0.0f;
                break;

            case Curves.EaseOut:
                _interpolationSpeed = _moveSpeed;
                _interpolationYSpeed = _moveYSpeed;
                break;

            case Curves.EaseInAndOut:
                _interpolationSpeed = 0.0f;
                _interpolationYSpeed = 0.0f;
                break;

            case Curves.Linear:
                _interpolationSpeed = _moveSpeed;
                _interpolationYSpeed = _moveYSpeed;
                break;

            default:
                _interpolationSpeed = _moveSpeed;
                _interpolationYSpeed = _moveYSpeed;
                break;
        }
        animationType = AnimationTypes.Move;
    }

    public override void DoActionUpdate()
    {
        Spin();
    }

    public override void DoActionActive()
    {
        Move();
    }

    private void Spin()
    {
        if (_spinX == true)
        {
            _targetEntity.Rotation.X += _spinSpeedX;
        }
        if (_spinZ == true)
        {
            _targetEntity.Rotation.Z += _spinSpeedZ;
        }
    }

    private void Move()
    {
        switch (_movementCurve)
        {
            case Curves.EaseIn:
                if (_easedIn == false)
                {
                    if (_interpolationSpeed < _moveSpeed - 0.05f || _interpolationYSpeed < _moveYSpeed - 0.05f)
                    {
                        if (_interpolationSpeed < _moveSpeed - 0.05f)
                        {
                            _interpolationSpeed = MathHelper.Lerp(_interpolationSpeed, _moveSpeed, 0.75f);
                        }
                        if (_interpolationYSpeed < _moveYSpeed - 0.05f)
                        {
                            _interpolationYSpeed = MathHelper.Lerp(_interpolationYSpeed, _moveYSpeed, 0.75f);
                        }
                    }
                    else
                    {
                        _interpolationSpeed = _moveSpeed;
                        _interpolationYSpeed = _moveYSpeed;
                        _easedIn = true;
                    }
                }
                break;

            case Curves.EaseOut:
                if (_easedOut == false)
                {
                    if (_interpolationSpeed > 0.05f || _interpolationYSpeed > 0.05f)
                    {
                        if (_interpolationSpeed > 0.05f)
                        {
                            _interpolationSpeed = MathHelper.Lerp(_interpolationSpeed, 0.0f, 0.75f);
                        }
                        if (_interpolationYSpeed > 0.05f)
                        {
                            _interpolationYSpeed = MathHelper.Lerp(_interpolationYSpeed, 0.0f, 0.75f);
                        }
                    }
                    else
                    {
                        _interpolationYSpeed = 0;
                        _interpolationSpeed = 0;
                        _easedOut = true;
                    }
                }
                break;

            case Curves.EaseInAndOut:
                if (_easedIn == false)
                {
                    if (_interpolationSpeed < _moveSpeed - 0.05f || _interpolationYSpeed < _moveYSpeed - 0.05f)
                    {
                        if (_interpolationSpeed < _moveSpeed - 0.05f)
                        {
                            _interpolationSpeed = MathHelper.Lerp(_interpolationSpeed, _moveSpeed, 0.75f);
                        }
                        if (_interpolationYSpeed < _moveYSpeed - 0.05f)
                        {
                            _interpolationYSpeed = MathHelper.Lerp(_interpolationYSpeed, _moveYSpeed, 0.75f);
                        }
                    }
                    else
                    {
                        _interpolationSpeed = _moveSpeed;
                        _interpolationYSpeed = _moveYSpeed;
                        _easedIn = true;
                    }
                }
                else
                {
                    if (_easedOut == false)
                    {
                        if (_moveDistance.X <= 0.05f && _moveDistance.Y <= 0.05f && _moveDistance.Z <= 0.05f)
                        {
                            if (_interpolationSpeed > 0.05f || _interpolationYSpeed > 0.05f)
                            {
                                if (_interpolationSpeed > 0.05f)
                                {
                                    _interpolationSpeed = MathHelper.Lerp(_interpolationSpeed, 0.0f, 0.75f);
                                }
                                if (_interpolationYSpeed > 0.05f)
                                {
                                    _interpolationYSpeed = MathHelper.Lerp(_interpolationYSpeed, 0.0f, 0.75f);
                                }
                            }
                            else
                            {
                                _interpolationYSpeed = 0;
                                _interpolationSpeed = 0;
                                _easedOut = true;
                            }
                        }
                        else
                        {
                            if (_interpolationSpeed > 0.05f + _moveSpeed / 10 * 3 ||
                                _interpolationYSpeed > 0.05f + _moveYSpeed / 10 * 3)
                            {
                                if (_interpolationSpeed > 0.05f + _moveSpeed / 10 * 3)
                                {
                                    _interpolationSpeed = MathHelper.Lerp(_interpolationSpeed, 0.0f, 0.75f);
                                }
                                if (_interpolationYSpeed > 0.05f + _moveYSpeed / 10 * 3)
                                {
                                    _interpolationYSpeed = MathHelper.Lerp(_interpolationYSpeed, 0.0f, 0.75f);
                                }
                            }
                            else
                            {
                                _interpolationYSpeed = _moveSpeed / 10 * 3;
                                _interpolationSpeed = _moveSpeed / 10 * 3;
                            }
                        }
                    }
                }
                break;

            default:
                break;
        }

        if (_moveDistance.X > 0.05f)
        {
            if (_startPosition.X < _destination.X)
            {
                _targetEntity.Position.X += _interpolationSpeed;
                if (_targetEntity.Position.X >= _destination.X + 0.05)
                {
                    _targetEntity.Position.X = _destination.X;
                }
            }
            else if (_startPosition.X > _destination.X)
            {
                _targetEntity.Position.X -= _interpolationSpeed;
                if (_targetEntity.Position.X <= _destination.X + 0.05)
                {
                    _targetEntity.Position.X = _destination.X;
                }
            }
            _moveDistance.X -= _interpolationSpeed;
        }
        else
        {
            _readyAxis.X = 1.0f;
        }

        if (_moveDistance.Y > 0.05f)
        {
            if (_startPosition.Y < _destination.Y)
            {
                _targetEntity.Position.Y += _moveYSpeed;
                if (_targetEntity.Position.Y >= _destination.Y - 0.05)
                {
                    _targetEntity.Position.Y = _destination.Y;
                }
            }
            else if (_startPosition.Y > _destination.Y)
            {
                _targetEntity.Position.Y -= _moveYSpeed;
                if (_targetEntity.Position.Y <= _destination.Y + 0.05)
                {
                    _targetEntity.Position.Y = _destination.Y;
                }
            }
            _moveDistance.Y -= _moveYSpeed;
        }
        else
        {
            _readyAxis.Y = 1.0f;
        }

        if (_moveDistance.Z > 0.05f)
        {
            if (_startPosition.Z < _destination.Z)
            {
                _targetEntity.Position.Z += _interpolationSpeed;
                if (_targetEntity.Position.Z >= _destination.Z - 0.05)
                {
                    _targetEntity.Position.Z = _destination.Z;
                }
            }
            else if (_startPosition.Z > _destination.Z)
            {
                _targetEntity.Position.Z -= _interpolationSpeed;
                if (_targetEntity.Position.Z <= _destination.Z + 0.05)
                {
                    _targetEntity.Position.Z = _destination.Z;
                }
            }
            _moveDistance.Z -= _moveYSpeed;
        }
        else
        {
            _readyAxis.Z = 1.0f;
        }

        if (_readyAxis.X == 1.0f && _readyAxis.Y == 1.0f && _readyAxis.Z == 1.0f)
        {
            ready = true;
        }
    }

    public override void DoRemoveEntity()
    {
        if (removeEntityAfter == true)
        {
            _targetEntity.CanBeRemoved = true;
        }
    }
}

// ---------------------------------------------------------------------------
// BAEntityOpacity — fades an entity's opacity to a target value
// ---------------------------------------------------------------------------

public class BAEntityOpacity : BattleAnimation3D
{
    private Entity _targetEntity;
    private float _transitionSpeed = 0.01f;
    private float _endState = 0.0f;
    private float _startState = 1.0f;
    private bool _removeEntityAfter = false;
    private bool _initialOpacitySet = false;

    public BAEntityOpacity(Entity entity, bool removeEntityAfter, float transitionSpeed,
                            float endState, float startDelay, float endDelay,
                            float startState = 1.0f)
        : base(Vector3.Zero, TextureManager.DefaultTexture!, Vector3.One, startDelay, endDelay)
    {
        _removeEntityAfter = removeEntityAfter;
        _endState = endState;
        _transitionSpeed = transitionSpeed;
        _targetEntity = entity;
        _startState = startState;

        Visible = false;
        animationType = AnimationTypes.Transition;
    }

    public override void DoActionActive()
    {
        if (_initialOpacitySet == false)
        {
            _targetEntity.NormalOpacity = _startState;
            _initialOpacitySet = true;
        }

        if (_endState > _startState)
        {
            if (_endState > _targetEntity.NormalOpacity)
            {
                _targetEntity.NormalOpacity += _transitionSpeed;
                if (_targetEntity.NormalOpacity >= _endState)
                {
                    _targetEntity.NormalOpacity = _endState;
                }
            }
        }
        else if (_endState < _startState)
        {
            if (_endState < _targetEntity.NormalOpacity)
            {
                _targetEntity.NormalOpacity -= _transitionSpeed;
                if (_targetEntity.NormalOpacity <= _endState)
                {
                    _targetEntity.NormalOpacity = _endState;
                }
            }
        }
        else
        {
            _targetEntity.NormalOpacity = _endState;
        }

        if (_targetEntity.NormalOpacity == _endState)
        {
            ready = true;
        }
    }

    public override void DoRemoveEntity()
    {
        if (_removeEntityAfter == true)
        {
            _targetEntity.CanBeRemoved = true;
        }
    }
}

// ---------------------------------------------------------------------------
// BAEntityOscillateMove — oscillates an entity back and forth
// ---------------------------------------------------------------------------

public class BAEntityOscillateMove : BattleAnimation3D
{
    public enum Curves { Linear, Smooth }

    private Vector3 _startPosition;
    private Vector3 _returnToStart = new Vector3(1.0f);
    private Entity _targetEntity;
    private Vector3 _halfDistance = Vector3.Zero;
    private Vector3 _destinationDistance = Vector3.Zero;
    private Vector3 _currentDistance = Vector3.Zero;
    private float _moveSpeed;
    private bool _moveBothWays = true;
    private Curves _movementCurve = Curves.Linear;
    private bool _removeEntityAfter;
    private TimeSpan _duration;
    private DateTime _readyTime;
    private Vector3 _readyAxis = Vector3.Zero;
    private Vector3 _interpolationSpeed;
    private bool _interpolationDirection = true;

    public BAEntityOscillateMove(Entity entity, bool removeEntityAfter, Vector3 distance,
                                  float speed, bool bothWays, TimeSpan duration,
                                  float startDelay, float endDelay,
                                  int movementCurve = 0, Vector3 returnToStart = default)
        : base(Vector3.Zero, TextureManager.DefaultTexture!, Vector3.One, startDelay, endDelay)
    {
        _removeEntityAfter = removeEntityAfter;
        _halfDistance = distance;
        _destinationDistance = _halfDistance;
        _moveSpeed = speed;
        _moveBothWays = bothWays;
        _duration = duration;
        _movementCurve = (Curves)movementCurve;

        Visible = false;
        _targetEntity = entity;
        switch (_movementCurve)
        {
            case Curves.Linear:
                _interpolationSpeed = new Vector3(_moveSpeed);
                break;

            case Curves.Smooth:
                _interpolationSpeed = Vector3.Zero;
                break;

            default:
                _interpolationSpeed = new Vector3(_moveSpeed);
                break;
        }
        if (returnToStart != Vector3.Zero)
        {
            _returnToStart = returnToStart;
        }
        animationType = AnimationTypes.Move;
    }

    public override void DoActionActive()
    {
        Move();
    }

    private void Move()
    {
        if (_startPosition == Vector3.Zero)
        {
            _startPosition = _targetEntity.Position;
        }
        if (_readyTime == default(DateTime))
        {
            _readyTime = DateTime.Now + _duration;
        }
        if (_movementCurve == Curves.Smooth)
        {
            if (_interpolationDirection == true)
            {
                if (_interpolationSpeed.X < _moveSpeed)
                {
                    _interpolationSpeed.X += _moveSpeed / 10;
                }
                else
                {
                    _interpolationSpeed.X = _moveSpeed;
                }
                if (_interpolationSpeed.Y < _moveSpeed)
                {
                    _interpolationSpeed.Y += _moveSpeed / 10;
                }
                else
                {
                    _interpolationSpeed.Y = _moveSpeed;
                }
                if (_interpolationSpeed.Z < _moveSpeed)
                {
                    _interpolationSpeed.Z += _moveSpeed / 10;
                }
                else
                {
                    _interpolationSpeed.Z = _moveSpeed;
                }
            }
            else
            {
                if (DateTime.Now < _readyTime)
                {
                    if (_interpolationSpeed.X > 0)
                    {
                        _interpolationSpeed.X -= _moveSpeed / 10;
                    }
                    else
                    {
                        _interpolationSpeed.X = 0;
                    }
                    if (_interpolationSpeed.Y > 0)
                    {
                        _interpolationSpeed.Y -= _moveSpeed / 10;
                    }
                    else
                    {
                        _interpolationSpeed.Y = 0;
                    }
                    if (_interpolationSpeed.Z > 0)
                    {
                        _interpolationSpeed.Z -= _moveSpeed / 10;
                    }
                    else
                    {
                        _interpolationSpeed.Z = 0;
                    }
                }
                else
                {
                    if (_interpolationSpeed.X > _moveSpeed / 10 * 3)
                    {
                        _interpolationSpeed.X -= _moveSpeed / 10;
                    }
                    else
                    {
                        _interpolationSpeed.X = _moveSpeed / 10 * 3;
                    }
                    if (_interpolationSpeed.Y > _moveSpeed / 10 * 3)
                    {
                        _interpolationSpeed.Y -= _moveSpeed / 10;
                    }
                    else
                    {
                        _interpolationSpeed.Y = _moveSpeed / 10 * 3;
                    }
                    if (_interpolationSpeed.Z > _moveSpeed / 10 * 3)
                    {
                        _interpolationSpeed.Z -= _moveSpeed / 10;
                    }
                    else
                    {
                        _interpolationSpeed.Z = _moveSpeed / 10 * 3;
                    }
                }
            }
        }

        OscillateAxis_X();
        OscillateAxis_Y();
        OscillateAxis_Z();

        if (_halfDistance.X != 0.0f)
        {
            _targetEntity.Position.X = _startPosition.X + _currentDistance.X;
        }
        else
        {
            _readyAxis.X = 1.0f;
        }
        if (_halfDistance.Y != 0.0f)
        {
            _targetEntity.Position.Y = _startPosition.Y + _currentDistance.Y;
        }
        else
        {
            _readyAxis.Y = 1.0f;
        }
        if (_halfDistance.Z != 0.0f)
        {
            _targetEntity.Position.Z = _startPosition.Z + _currentDistance.Z;
        }
        else
        {
            _readyAxis.Z = 1.0f;
        }

        if (DateTime.Now > _readyTime &&
            _readyAxis.X == 1.0f && _readyAxis.Y == 1.0f && _readyAxis.Z == 1.0f)
        {
            ready = true;
        }
    }

    private void OscillateAxis_X()
    {
        if (_currentDistance.X != _destinationDistance.X)
        {
            if (_currentDistance.X < _destinationDistance.X)
            {
                _currentDistance.X += _interpolationSpeed.X;
            }
            else
            {
                _currentDistance.X -= _interpolationSpeed.X;
            }
            if (Math.Abs(_currentDistance.X) / Math.Abs(_halfDistance.X) > 0.75f)
            {
                _interpolationDirection = false;
            }
            if (_destinationDistance.X > 0.0f)
            {
                if (_currentDistance.X >= _destinationDistance.X)
                {
                    _currentDistance.X = _destinationDistance.X;
                }
            }
            else if (_destinationDistance.X < 0.0f)
            {
                if (_currentDistance.X <= _destinationDistance.X)
                {
                    _currentDistance.X = _destinationDistance.X;
                }
            }
            else
            {
                if (_currentDistance.X > _destinationDistance.X)
                {
                    if (_currentDistance.X - _interpolationSpeed.X <= _destinationDistance.X)
                    {
                        _currentDistance.X = _destinationDistance.X;
                    }
                }
                else
                {
                    if (_currentDistance.X + _interpolationSpeed.X >= _destinationDistance.X)
                    {
                        _currentDistance.X = _destinationDistance.X;
                    }
                }
            }
        }
        else
        {
            if (DateTime.Now < _readyTime)
            {
                if (_moveBothWays == true)
                {
                    _destinationDistance.X = _destinationDistance.X > 0.0f
                        ? 0.0f - Math.Abs(_halfDistance.X) * 2
                        : 0.0f + Math.Abs(_halfDistance.X) * 2;
                }
                else
                {
                    _destinationDistance.X = _destinationDistance.X > 0.0f ? 0.0f : _halfDistance.X;
                }
                _interpolationDirection = true;
            }
            else
            {
                if (_returnToStart.X == 0.0f)
                {
                    _readyAxis.X = 1.0f;
                }
                else
                {
                    if (_destinationDistance.X != 0.0f)
                    {
                        _destinationDistance.X = 0.0f;
                        _interpolationDirection = true;
                    }
                    if (_currentDistance.X == 0.0f)
                    {
                        _readyAxis.X = 1.0f;
                    }
                }
            }
        }
    }

    private void OscillateAxis_Y()
    {
        if (_currentDistance.Y != _destinationDistance.Y)
        {
            if (_currentDistance.Y < _destinationDistance.Y)
            {
                _currentDistance.Y += _interpolationSpeed.Y;
            }
            else
            {
                _currentDistance.Y -= _interpolationSpeed.Y;
            }
            if (Math.Abs(_currentDistance.Y) / Math.Abs(_halfDistance.Y) > 0.75f)
            {
                _interpolationDirection = false;
            }
            if (_destinationDistance.Y > 0.0f)
            {
                if (_currentDistance.Y >= _destinationDistance.Y)
                {
                    _currentDistance.Y = _destinationDistance.Y;
                }
            }
            else if (_destinationDistance.Y < 0.0f)
            {
                if (_currentDistance.Y <= _destinationDistance.Y)
                {
                    _currentDistance.Y = _destinationDistance.Y;
                }
            }
            else
            {
                if (_currentDistance.Y > _destinationDistance.Y)
                {
                    if (_currentDistance.Y - _interpolationSpeed.Y <= _destinationDistance.Y)
                    {
                        _currentDistance.Y = _destinationDistance.Y;
                    }
                }
                else
                {
                    if (_currentDistance.Y + _interpolationSpeed.Y >= _destinationDistance.Y)
                    {
                        _currentDistance.Y = _destinationDistance.Y;
                    }
                }
            }
        }
        else
        {
            if (DateTime.Now < _readyTime)
            {
                if (_moveBothWays == true)
                {
                    _destinationDistance.Y = _destinationDistance.Y > 0.0f
                        ? 0.0f - Math.Abs(_halfDistance.Y) * 2
                        : 0.0f + Math.Abs(_halfDistance.Y) * 2;
                }
                else
                {
                    _destinationDistance.Y = _destinationDistance.Y > 0.0f ? 0.0f : _halfDistance.Y;
                }
                _interpolationDirection = true;
            }
            else
            {
                if (_returnToStart.Y == 0.0f)
                {
                    _readyAxis.Y = 1.0f;
                }
                else
                {
                    if (_destinationDistance.Y != 0.0f)
                    {
                        _destinationDistance.Y = 0.0f;
                        _interpolationDirection = true;
                    }
                    if (_currentDistance.Y == 0.0f)
                    {
                        _readyAxis.Y = 1.0f;
                    }
                }
            }
        }
    }

    private void OscillateAxis_Z()
    {
        if (_currentDistance.Z != _destinationDistance.Z)
        {
            if (_currentDistance.Z < _destinationDistance.Z)
            {
                _currentDistance.Z += _interpolationSpeed.Z;
            }
            else
            {
                _currentDistance.Z -= _interpolationSpeed.Z;
            }
            if (Math.Abs(_currentDistance.Y) / Math.Abs(_halfDistance.Y) > 0.75f)
            {
                _interpolationDirection = false;
            }
            if (_destinationDistance.Z > 0.0f)
            {
                if (_currentDistance.Z >= _destinationDistance.Z)
                {
                    _currentDistance.Z = _destinationDistance.Z;
                }
            }
            else if (_destinationDistance.Z < 0.0f)
            {
                if (_currentDistance.Z <= _destinationDistance.Z)
                {
                    _currentDistance.Z = _destinationDistance.Z;
                }
            }
            else
            {
                if (_currentDistance.Z > _destinationDistance.Z)
                {
                    if (_currentDistance.Z - _interpolationSpeed.Z <= _destinationDistance.Z)
                    {
                        _currentDistance.Z = _destinationDistance.Z;
                    }
                }
                else
                {
                    if (_currentDistance.Z + _interpolationSpeed.Z >= _destinationDistance.Z)
                    {
                        _currentDistance.Z = _destinationDistance.Z;
                    }
                }
            }
        }
        else
        {
            if (DateTime.Now < _readyTime)
            {
                if (_moveBothWays == true)
                {
                    _destinationDistance.Z = _destinationDistance.Z > 0.0f
                        ? 0.0f - Math.Abs(_halfDistance.Z) * 2
                        : 0.0f + Math.Abs(_halfDistance.Z) * 2;
                }
                else
                {
                    _destinationDistance.Z = _destinationDistance.Z > 0.0f ? 0.0f : _halfDistance.Z;
                }
                _interpolationDirection = true;
            }
            else
            {
                if (_returnToStart.Z == 0.0f)
                {
                    _readyAxis.Z = 1.0f;
                }
                else
                {
                    if (_destinationDistance.Z != 0.0f)
                    {
                        _destinationDistance.Z = 0.0f;
                        _interpolationDirection = true;
                    }
                    if (_currentDistance.Z == 0.0f)
                    {
                        _readyAxis.Z = 1.0f;
                    }
                }
            }
        }
    }

    public override void DoRemoveEntity()
    {
        if (_removeEntityAfter == true)
        {
            _targetEntity.CanBeRemoved = true;
        }
    }
}

// ---------------------------------------------------------------------------
// BAEntityRotate — rotates an entity's rotation vector by increments
// ---------------------------------------------------------------------------

public class BAEntityRotate : BattleAnimation3D
{
    private const float DEGREES_TO_RADIANS_CALC = (float)(Math.PI / 180.0);

    private Entity _targetEntity;
    private Vector3 _rotationSpeedVector;
    private Vector3 _endRotation;
    private bool _doReturn = false;
    private Vector3 _returnVector;
    private bool _hasReturned = false;
    private Vector3 _doRotation = Vector3.One;
    private Vector3 _amountRotated = Vector3.Zero;
    private Vector3 _readyAxis = Vector3.Zero;
    private bool _removeEntityAfter = false;

    public BAEntityRotate(Entity entity, bool removeEntityAfter, Vector3 rotationSpeedVector,
                           Vector3 endRotation, float startDelay, float endDelay,
                           bool doReturn = false, bool convertDegreesToRadians = false)
        : base(Vector3.Zero, TextureManager.DefaultTexture!, Vector3.One, startDelay, endDelay)
    {
        _removeEntityAfter = removeEntityAfter;
        if (convertDegreesToRadians == false)
        {
            _rotationSpeedVector = rotationSpeedVector;
            _endRotation = endRotation;
        }
        else
        {
            _rotationSpeedVector = new Vector3(
                rotationSpeedVector.X * DEGREES_TO_RADIANS_CALC,
                rotationSpeedVector.Y * DEGREES_TO_RADIANS_CALC,
                rotationSpeedVector.Z * DEGREES_TO_RADIANS_CALC);
            _endRotation = new Vector3(
                endRotation.X * DEGREES_TO_RADIANS_CALC,
                endRotation.Y * DEGREES_TO_RADIANS_CALC,
                endRotation.Z * DEGREES_TO_RADIANS_CALC);
        }
        _targetEntity = entity;
        _returnVector = _targetEntity.Rotation;

        if (rotationSpeedVector.X == 0.0f)
        {
            _doRotation.X = 0.0f;
            _readyAxis.X = 1.0f;
        }
        if (rotationSpeedVector.Y == 0.0f)
        {
            _doRotation.Y = 0.0f;
            _readyAxis.Y = 1.0f;
        }
        if (rotationSpeedVector.Z == 0.0f)
        {
            _doRotation.Z = 0.0f;
            _readyAxis.Z = 1.0f;
        }
        _doReturn = doReturn;
        animationType = AnimationTypes.Rotation;
    }

    public override void DoActionActive()
    {
        if (_doRotation.X == 1.0f)
        {
            if (_amountRotated.X < Math.Abs(_endRotation.X))
            {
                if (_targetEntity.Rotation.X > _endRotation.X)
                {
                    _targetEntity.Rotation.X += _rotationSpeedVector.X;
                    if (_targetEntity.Rotation.X <= _endRotation.X)
                    {
                        _targetEntity.Rotation.X = _endRotation.X;
                    }
                }
                else if (_targetEntity.Rotation.X < _endRotation.X)
                {
                    _targetEntity.Rotation.X += _rotationSpeedVector.X;
                    if (_targetEntity.Rotation.X >= _endRotation.X)
                    {
                        _targetEntity.Rotation.X = _endRotation.X;
                    }
                }
                _amountRotated.X += Math.Abs(_rotationSpeedVector.X);
            }
            else
            {
                _readyAxis.X = 1.0f;
            }
        }

        if (_doRotation.Y == 1.0f)
        {
            if (_amountRotated.Y < Math.Abs(_endRotation.Y))
            {
                if (_targetEntity.Rotation.Y > _endRotation.Y)
                {
                    _targetEntity.Rotation.Y += _rotationSpeedVector.Y;
                    if (_targetEntity.Rotation.Y <= _endRotation.Y)
                    {
                        _targetEntity.Rotation.Y = _endRotation.Y;
                    }
                }
                else if (_targetEntity.Rotation.Y < _endRotation.Y)
                {
                    _targetEntity.Rotation.Y += _rotationSpeedVector.Y;
                    if (_targetEntity.Rotation.Y >= _endRotation.Y)
                    {
                        _targetEntity.Rotation.Y = _endRotation.Y;
                    }
                }
                _amountRotated.Y += Math.Abs(_rotationSpeedVector.Y);
            }
            else
            {
                _readyAxis.Y = 1.0f;
            }
        }

        if (_doRotation.Z == 1.0f)
        {
            if (_amountRotated.Z < Math.Abs(_endRotation.Z))
            {
                if (_targetEntity.Rotation.Z > _endRotation.Z)
                {
                    _targetEntity.Rotation.Z += _rotationSpeedVector.Z;
                    if (_targetEntity.Rotation.Z <= _endRotation.Z)
                    {
                        _targetEntity.Rotation.Z = _endRotation.Z;
                    }
                }
                else if (_targetEntity.Rotation.Z < _endRotation.Z)
                {
                    _targetEntity.Rotation.Z += _rotationSpeedVector.Z;
                    if (_targetEntity.Rotation.Z >= _endRotation.Z)
                    {
                        _targetEntity.Rotation.Z = _endRotation.Z;
                    }
                }
                _amountRotated.Z += Math.Abs(_rotationSpeedVector.Z);
            }
            else
            {
                _readyAxis.Z = 1.0f;
            }
        }

        if (_readyAxis.X == 1.0f && _readyAxis.Y == 1.0f && _readyAxis.Z == 1.0f)
        {
            RotationReady();
        }
    }

    private void RotationReady()
    {
        if (_doReturn == true && _hasReturned == false)
        {
            _hasReturned = true;
            _endRotation = _returnVector;
            _rotationSpeedVector = new Vector3(
                _rotationSpeedVector.X * -1,
                _rotationSpeedVector.Y * -1,
                _rotationSpeedVector.Z * -1);
            if (_doRotation.X == 1.0f)
            {
                _readyAxis.X = 0.0f;
                _amountRotated.X -= _amountRotated.X * 2;
            }
            if (_doRotation.Y == 1.0f)
            {
                _readyAxis.Y = 0.0f;
                _amountRotated.Y -= _amountRotated.Y * 2;
            }
            if (_doRotation.Z == 1.0f)
            {
                _readyAxis.Z = 0.0f;
                _amountRotated.Z -= _amountRotated.Z * 2;
            }
        }
        else
        {
            ready = true;
        }
    }

    public override void DoRemoveEntity()
    {
        if (_removeEntityAfter == true)
        {
            _targetEntity.CanBeRemoved = true;
        }
    }
}

// ---------------------------------------------------------------------------
// BAEntityScale — scales an entity toward an end size with optional anchoring
// ---------------------------------------------------------------------------

public class BAEntityScale : BattleAnimation3D
{
    private bool _initialScaleSet = false;
    private Vector3 _endSize;
    private Vector3 _startSize;
    private float _sizeSpeed = 0.01f;
    private Entity _targetEntity;
    private String _anchors = String.Empty;
    private Vector3 _speedMultiplier = Vector3.One;
    private bool _removeEntityAfter;

    public BAEntityScale(Entity entity, bool removeEntityAfter, Vector3 scale, Vector3 endSize,
                          float sizeSpeed, float startDelay, float endDelay,
                          String anchors, Vector3 speedMultiplier = default)
        : base(Vector3.Zero, TextureManager.DefaultTexture!, scale, startDelay, endDelay)
    {
        _removeEntityAfter = removeEntityAfter;
        _anchors = anchors;
        _endSize = endSize;
        _sizeSpeed = sizeSpeed;
        _targetEntity = entity;

        if (speedMultiplier != Vector3.Zero)
        {
            _speedMultiplier = speedMultiplier;
        }
        animationType = AnimationTypes.Size;
    }

    public override void DoActionActive()
    {
        Vector3 saveScale = _targetEntity.Scale;

        float changeX = _sizeSpeed * _speedMultiplier.X;
        float changeY = _sizeSpeed * _speedMultiplier.Y;
        float changeZ = _sizeSpeed * _speedMultiplier.Z;

        if (_initialScaleSet == false)
        {
            _startSize = _targetEntity.Scale;
            _initialScaleSet = true;
        }

        if (_startSize.X < _endSize.X)
        {
            if (_targetEntity.Scale.X < _endSize.X)
            {
                _targetEntity.Scale.X += changeX;
                if (_targetEntity.Scale.X >= _endSize.X)
                {
                    _targetEntity.Scale.X = _endSize.X;
                }
            }
        }
        else
        {
            if (_targetEntity.Scale.X > _endSize.X)
            {
                _targetEntity.Scale.X -= changeX;
                if (_targetEntity.Scale.X <= _endSize.X)
                {
                    _targetEntity.Scale.X = _endSize.X;
                }
            }
        }

        if (_startSize.Y < _endSize.Y)
        {
            if (_targetEntity.Scale.Y < _endSize.Y)
            {
                _targetEntity.Scale.Y += changeY;
                if (_targetEntity.Scale.Y >= _endSize.Y)
                {
                    _targetEntity.Scale.Y = _endSize.Y;
                }
            }
        }
        else
        {
            if (_targetEntity.Scale.Y > _endSize.Y)
            {
                _targetEntity.Scale.Y -= changeY;
                if (_targetEntity.Scale.Y <= _endSize.Y)
                {
                    _targetEntity.Scale.Y = _endSize.Y;
                }
            }
        }

        if (_startSize.Z < _endSize.Z)
        {
            if (_targetEntity.Scale.Z < _endSize.Z)
            {
                _targetEntity.Scale.Z += changeZ;
                if (_targetEntity.Scale.Z >= _endSize.Z)
                {
                    _targetEntity.Scale.Z = _endSize.Z;
                }
            }
        }
        else
        {
            if (_targetEntity.Scale.Z > _endSize.Z)
            {
                _targetEntity.Scale.Z -= changeZ;
                if (_targetEntity.Scale.Z <= _endSize.Z)
                {
                    _targetEntity.Scale.Z = _endSize.Z;
                }
            }
        }

        if (_anchors.ToLower().Contains("b") == true)
        {
            float diffY = saveScale.Y - _targetEntity.Scale.Y;
            _targetEntity.Position.Y -= diffY / 4;
        }
        if (_anchors.ToLower().Contains("t") == true)
        {
            float diffY = saveScale.Y - _targetEntity.Scale.Y;
            _targetEntity.Position.Y += diffY / 4;
        }
        if (_anchors.ToLower().Contains("l") == true)
        {
            float diffX = saveScale.X - _targetEntity.Scale.X;
            _targetEntity.Position.X -= diffX / 4;
        }
        if (_anchors.ToLower().Contains("r") == true)
        {
            float diffX = saveScale.X - _targetEntity.Scale.X;
            _targetEntity.Position.X += diffX / 4;
        }

        if (_endSize == _targetEntity.Scale)
        {
            ready = true;
        }
    }

    public override void DoRemoveEntity()
    {
        if (_removeEntityAfter == true)
        {
            _targetEntity.CanBeRemoved = true;
        }
    }
}

// ---------------------------------------------------------------------------
// BAEntitySetPosition — instantly sets an entity's position
// ---------------------------------------------------------------------------

public class BAEntitySetPosition : BattleAnimation3D
{
    private Entity _targetEntity;
    private Vector3 _setPosition;
    private bool _removeEntityAfter;

    public BAEntitySetPosition(Entity entity, bool removeEntityAfter, Vector3 setPosition,
                                float startDelay, float endDelay)
        : base(Vector3.Zero, TextureManager.DefaultTexture!, Vector3.One, startDelay, endDelay)
    {
        _removeEntityAfter = removeEntityAfter;
        _setPosition = setPosition;

        Visible = false;
        _targetEntity = entity;
        animationType = AnimationTypes.Move;
    }

    public override void DoActionActive()
    {
        Vector3 setPositionOffset = Vector3.Zero;
        if (_targetEntity.Model != null)
        {
            setPositionOffset = new Vector3(0, -0.5f, 0);
        }
        _targetEntity.Position = _setPosition + setPositionOffset;
        ready = true;
    }

    public override void DoRemoveEntity()
    {
        if (_removeEntityAfter == true)
        {
            _targetEntity.CanBeRemoved = true;
        }
    }
}

// ---------------------------------------------------------------------------
// BAEntityTextureChange — swaps an entity's texture
// ---------------------------------------------------------------------------

public class BAEntityTextureChange : BattleAnimation3D
{
    private Texture2D _texture;
    private Entity _targetEntity;
    private bool _removeEntityAfter;

    public BAEntityTextureChange(Entity entity, bool removeEntityAfter, Texture2D texture,
                                  float startDelay, float endDelay)
        : base(Vector3.Zero, TextureManager.DefaultTexture!, Vector3.One, startDelay, endDelay)
    {
        _removeEntityAfter = removeEntityAfter;
        _targetEntity = entity;
        _texture = texture;
        animationType = AnimationTypes.Texture;
    }

    public override void DoActionActive()
    {
        _targetEntity.Textures = [_texture];
        ready = true;
    }

    public override void DoRemoveEntity()
    {
        if (_removeEntityAfter == true)
        {
            _targetEntity.CanBeRemoved = true;
        }
    }
}

// ---------------------------------------------------------------------------
// BAPlaySound — plays a sound effect or Pokémon cry at a given delay
// ---------------------------------------------------------------------------

public class BAPlaySound : BattleAnimation3D
{
    private String _soundFile = String.Empty;
    private bool _stopMusic;
    private bool _isPokemon;
    private String _crySuffix = String.Empty;

    public BAPlaySound(String sound, float startDelay, float endDelay,
                       bool stopMusic = false, bool isPokemon = false, String crySuffix = "")
        : base(Vector3.Zero, TextureManager.DefaultTexture!, Vector3.One, startDelay, endDelay)
    {
        Scale = Vector3.One;
        _soundFile = sound;
        Visible = false;
        _stopMusic = stopMusic;
        _isPokemon = isPokemon;
        _crySuffix = crySuffix;
        animationType = AnimationTypes.Sound;
    }

    public override void DoActionActive()
    {
        if (_isPokemon == true)
        {
            SoundManager.PlayPokemonCry(int.Parse(_soundFile), _crySuffix);
        }
        else
        {
            SoundManager.PlaySound(_soundFile, _stopMusic);
        }
        ready = true;
    }
}

// ---------------------------------------------------------------------------
// AnimationQueryObject — manages a sequence of BattleAnimation3D objects
// ---------------------------------------------------------------------------

public class AnimationQueryObject : QueryObject
{
    private bool _animationStarted = false;
    private bool _animationEnded = false;
    private bool _battleFlipped = false;
    private List<BattleAnimation3D> _animationSequence = [];
    private List<Entity> _spawnedEntities = [];
    private Entity? _currentEntity;
    public bool drawBeforeEntities;

    private List<Entity> _backgrounds = [];
    private List<Entity> _renderObjects = [];

    public override bool IsReady => _animationEnded;

    public AnimationQueryObject(Entity? entity, bool battleFlipped,
                                 bool drawBeforeEntities = false)
        : base(QueryTypes.MoveAnimation)
    {
        _animationSequence = [];
        _spawnedEntities = [];
        this.drawBeforeEntities = drawBeforeEntities;
        _battleFlipped = battleFlipped;
        _backgrounds = [];
        _renderObjects = [];

        if (entity != null)
        {
            _currentEntity = entity;
        }
        AnimationSequenceBegin();
    }

    public override void Draw(BattleScreen bv2Screen)
    {
        foreach (BattleAnimation3D a in _animationSequence)
        {
            if (_backgrounds.Contains(a) == false &&
                a.animationType == BattleAnimation3D.AnimationTypes.Background)
            {
                _backgrounds.Add(a);
            }
        }
        foreach (BattleAnimation3D entity in _spawnedEntities)
        {
            if (_renderObjects.Contains(entity) == false)
            {
                _renderObjects.Add(entity);
            }
        }
        if (_renderObjects.Count > 0)
        {
            _renderObjects = _renderObjects.OrderByDescending(r => r.CameraDistance).ToList();
        }
        foreach (Entity obj in _backgrounds)
        {
            obj.Render();
        }
        foreach (Entity obj in _renderObjects)
        {
            obj.UpdateModel();
            obj.Render();
        }
        _renderObjects.Clear();
        _backgrounds.Clear();
    }

    public override void Update(BattleScreen bv2Screen)
    {
        if (_animationStarted == true)
        {
            for (int i = 0; i <= _animationSequence.Count - 1; i++)
            {
                if (i <= _animationSequence.Count - 1)
                {
                    BattleAnimation3D a = _animationSequence[i];
                    if (a.canRemove == true)
                    {
                        i -= 1;
                        _animationSequence.Remove(a);
                    }
                    else
                    {
                        a.Update();
                    }
                }
            }
            if (_animationSequence.Count <= 0)
            {
                AnimationSequenceEnd();
            }

            foreach (BattleAnimation3D anim in _animationSequence)
            {
                anim.UpdateEntity();
            }
            foreach (Entity ent in _spawnedEntities)
            {
                ent.Update();
                ent.UpdateEntity();
            }
            for (int i = 0; i <= _spawnedEntities.Count - 1; i++)
            {
                if (i <= _spawnedEntities.Count - 1)
                {
                    Entity ent = _spawnedEntities[i];
                    if (ent.CanBeRemoved == true)
                    {
                        i -= 1;
                        RemoveEntity(ent);
                    }
                }
            }
        }
    }

    public void AnimationSequenceBegin()
    {
        _animationStarted = true;
    }

    public void AnimationSequenceEnd()
    {
        _spawnedEntities.Clear();
        _backgrounds.Clear();
        _renderObjects.Clear();
        _animationEnded = true;
    }

    public Entity SpawnEntity(Vector3 position, Texture2D texture, Vector3 scale,
                               float opacity, float startDelay = 0.0f, float endDelay = 0.0f,
                               String modelPath = "")
    {
        Vector3 newPosition;
        if (position != Vector3.Zero)
        {
            if (_currentEntity != null)
            {
                if (_battleFlipped == true)
                {
                    position.X *= -1;
                }
                newPosition = _currentEntity.Position + position;
                if (_currentEntity.Model != null)
                {
                    newPosition.Y += 0.5f;
                }
            }
            else
            {
                newPosition = position;
            }
        }
        else
        {
            if (_currentEntity != null)
            {
                newPosition = _currentEntity.Position;
                if (_currentEntity.Model != null)
                {
                    newPosition.Y += 0.5f;
                }
            }
            else
            {
                newPosition = Vector3.Zero;
            }
        }
        BattleAnimation3D spawnedEntity = new BattleAnimation3D(newPosition, texture, scale,
            startDelay, endDelay, false);
        spawnedEntity.Opacity = opacity;
        spawnedEntity.Visible = false;

        if (modelPath.Equals(String.Empty) == false)
        {
            spawnedEntity.ModelPath = modelPath;
            spawnedEntity.Model = ModelManager.GetModel(spawnedEntity.ModelPath);
            spawnedEntity.Scale *= ModelManager.MODELSCALE;
            if (_battleFlipped == true)
            {
                int flipRotation = Entity.GetRotationFromVector(spawnedEntity.Rotation);
                flipRotation += 2;
                if (flipRotation > 3)
                {
                    flipRotation -= 4;
                }
                spawnedEntity.Rotation.Y = Entity.GetRotationFromInteger(flipRotation).Y;
            }
        }
        _spawnedEntities.Add(spawnedEntity);
        return spawnedEntity;
    }

    public void RemoveEntity(Entity entity)
    {
        _spawnedEntities.Remove(entity);
    }

    public void AnimationChangeTexture(Entity? entity, bool removeEntityAfter, Texture2D texture,
                                        float startDelay, float endDelay)
    {
        Entity textureChangeEntity = entity ?? _currentEntity!;
        BAEntityTextureChange ba = new BAEntityTextureChange(textureChangeEntity, removeEntityAfter,
            texture, startDelay, endDelay);
        _animationSequence.Add(ba);
    }

    public void AnimationMove(Entity? entity, bool removeEntityAfter,
                               float destinationX, float destinationY, float destinationZ,
                               float moveSpeed, bool spinX, bool spinZ,
                               float startDelay, float endDelay,
                               float spinXSpeed = 0.1f, float spinZSpeed = 0.1f,
                               float moveYSpeed = 0.0f, int movementCurve = 3)
    {
        Entity moveEntity = entity ?? _currentEntity!;
        Vector3 destination;

        if (_battleFlipped == true)
        {
            destinationX *= -1.0f;
            if (spinZ == true)
            {
                spinXSpeed *= -1.0f;
                spinZSpeed *= -1.0f;
            }
        }

        if (_currentEntity == null)
        {
            destination = moveEntity.Position + new Vector3(destinationX, destinationY, destinationZ);
        }
        else
        {
            destination = _currentEntity.Position + new Vector3(destinationX, destinationY, destinationZ);
        }

        BAEntityMove ba = new BAEntityMove(moveEntity, removeEntityAfter, destination,
            moveSpeed, spinX, spinZ, startDelay, endDelay,
            spinXSpeed, spinZSpeed, movementCurve, moveYSpeed);
        _animationSequence.Add(ba);
    }

    public void AnimationOscillateMove(Entity? entity, bool removeEntityAfter,
                                        Vector3 distance, float moveSpeed, bool bothWays,
                                        float duration, float startDelay, float endDelay,
                                        int movementCurve = 0, Vector3 returnToStart = default)
    {
        Entity moveEntity = entity ?? _currentEntity!;

        if (_battleFlipped == true)
        {
            distance.Z *= -1.0f;
        }

        float durationWhole = (float)Math.Truncate((double)(duration / 6.0f));
        float durationFraction = (float)((duration / 6.0f - durationWhole) * 1000);
        TimeSpan durationTime = new TimeSpan(0, 0, 0, (int)durationWhole, (int)durationFraction);
        BAEntityOscillateMove ba = new BAEntityOscillateMove(moveEntity, removeEntityAfter,
            distance, moveSpeed, bothWays, durationTime, startDelay, endDelay,
            movementCurve, returnToStart);
        _animationSequence.Add(ba);
    }

    public void AnimationSetPosition(Entity? entity, bool removeEntityAfter,
                                      float positionX, float positionY, float positionZ,
                                      float startDelay, float endDelay)
    {
        Entity setEntity = entity ?? _currentEntity!;
        Vector3 setPosition = new Vector3(positionX, positionY, positionZ) + BattleScreen.BattleMapOffset;
        BAEntitySetPosition ba = new BAEntitySetPosition(setEntity, removeEntityAfter,
            setPosition, startDelay, endDelay);
        _animationSequence.Add(ba);
    }

    public void AnimationColor(Entity? entity, bool removeEntityAfter,
                                float transitionSpeedIn, bool returnToFromWhenDone,
                                float startDelay, float endDelay, Vector3 vectorColorTo,
                                float transitionSpeedOut = -1, Vector3 vectorColorFrom = default)
    {
        Entity colorEntity = entity ?? _currentEntity!;
        BAEntityColor ba = new BAEntityColor(colorEntity, removeEntityAfter,
            transitionSpeedIn, returnToFromWhenDone, startDelay, endDelay,
            vectorColorTo, transitionSpeedOut, vectorColorFrom);
        _animationSequence.Add(ba);
    }

    public void AnimationColor(Entity? entity, bool removeEntityAfter,
                                float transitionSpeedIn, bool returnToFromWhenDone,
                                float startDelay, float endDelay,
                                Microsoft.Xna.Framework.Color colorTo,
                                float transitionSpeedOut = -1.0f,
                                Microsoft.Xna.Framework.Color colorFrom = default)
    {
        Entity colorEntity = entity ?? _currentEntity!;
        Vector3 vectorColorTo = colorTo.ToVector3();
        Vector3 vectorColorFrom = colorFrom != default(Microsoft.Xna.Framework.Color)
            ? colorFrom.ToVector3()
            : Vector3.Zero;
        BAEntityColor ba = new BAEntityColor(colorEntity, removeEntityAfter,
            transitionSpeedIn, returnToFromWhenDone, startDelay, endDelay,
            vectorColorTo, transitionSpeedOut, vectorColorFrom);
        _animationSequence.Add(ba);
    }

    public void AnimationFade(Entity? entity, bool removeEntityAfter,
                               float transitionSpeed, float endState,
                               float startDelay, float endDelay, float startState = -1.0f)
    {
        Entity fadeEntity = entity ?? _currentEntity!;
        if (startState == -1.0f)
        {
            startState = fadeEntity.NormalOpacity;
        }
        BAEntityOpacity ba = new BAEntityOpacity(fadeEntity, removeEntityAfter,
            transitionSpeed, endState, startDelay, endDelay, startState);
        _animationSequence.Add(ba);
    }

    public void AnimationRotate(Entity? entity, bool removeEntityAfter,
                                 float rotationSpeedX, float rotationSpeedY, float rotationSpeedZ,
                                 float endRotationX, float endRotationY, float endRotationZ,
                                 float startDelay, float endDelay, bool doReturn = false)
    {
        Entity rotateEntity = entity ?? _currentEntity!;
        Vector3 rotationSpeedVector = new Vector3(rotationSpeedX, rotationSpeedY, rotationSpeedZ);
        Vector3 endRotation = new Vector3(endRotationX, endRotationY, endRotationZ);
        BAEntityRotate ba = new BAEntityRotate(rotateEntity, removeEntityAfter,
            rotationSpeedVector, endRotation, startDelay, endDelay, doReturn);
        _animationSequence.Add(ba);
    }

    public void AnimationTurnNPC(int turnSteps, float startDelay, float endDelay,
                                  int endFaceRotation = -1, float turnDelay = 0.5f,
                                  int turnSpeed = 1)
    {
        NPC? turnNPC = _currentEntity as NPC;

        if (_battleFlipped == true)
        {
            endFaceRotation += 2;
            turnSpeed *= -1;
        }

        BAEntityFaceRotate ba = new BAEntityFaceRotate(turnNPC!, turnSteps,
            startDelay, endDelay, endFaceRotation, turnDelay, turnSpeed);
        _animationSequence.Add(ba);
    }

    public void AnimationScale(Entity? entity, bool removeEntityAfter,
                                float endSizeX, float endSizeY, float endSizeZ,
                                float sizeSpeed, float startDelay, float endDelay,
                                String anchors = "", Vector3 speedMultiplier = default)
    {
        Entity scaleEntity;
        if (entity == null)
        {
            scaleEntity = _currentEntity!;
            if (scaleEntity.Model != null)
            {
                endSizeX *= ModelManager.MODELSCALE;
                endSizeY *= ModelManager.MODELSCALE;
                endSizeZ *= ModelManager.MODELSCALE;
                sizeSpeed *= ModelManager.MODELSCALE;
            }
        }
        else
        {
            scaleEntity = entity;
        }
        Vector3 scale = scaleEntity.Scale;
        Vector3 endSize = new Vector3(endSizeX, endSizeY, endSizeZ);
        BAEntityScale ba = new BAEntityScale(scaleEntity, removeEntityAfter, scale, endSize,
            sizeSpeed, startDelay, endDelay, anchors, speedMultiplier);
        _animationSequence.Add(ba);
    }

    public void AnimationPlaySound(String sound, float startDelay, float endDelay,
                                    bool stopMusic = false, bool isPokemon = false,
                                    String crySuffix = "")
    {
        BAPlaySound ba = new BAPlaySound(sound, startDelay, endDelay,
            stopMusic, isPokemon, crySuffix);
        _animationSequence.Add(ba);
    }

    public void AnimationBackground(Texture2D texture, float startDelay, float endDelay,
                                     float duration, float afterFadeInOpacity = 1.0f,
                                     float fadeInSpeed = 0.125f, float fadeOutSpeed = 0.125f,
                                     bool doTile = false, int animationLength = 1,
                                     int animationSpeed = 4, int scale = 4)
    {
        BABackground ba = new BABackground(texture, startDelay, endDelay, duration,
            afterFadeInOpacity, fadeInSpeed, fadeOutSpeed, doTile,
            animationLength, animationSpeed, scale);
        _animationSequence.Add(ba);
    }

    public void AnimationCameraChangeAngle(BattleScreen battlescreen, int cameraAngleID,
                                            float startDelay, float endDelay)
    {
        BACameraChangeAngle ba = new BACameraChangeAngle(battlescreen, cameraAngleID,
            startDelay, endDelay);
        _animationSequence.Add(ba);
    }

    public void AnimationCameraOscillateMove(Vector3 distance, float speed, bool bothWays,
                                              float duration, float startDelay, float endDelay,
                                              int movementCurve = 0, Vector3 returnToStart = default)
    {
        if (_battleFlipped == true)
        {
            distance.Z *= -1.0f;
        }

        float durationWhole = (float)Math.Truncate((double)(duration / 6.0f));
        float durationFraction = (float)((duration / 6.0f - durationWhole) * 1000);
        TimeSpan durationTime = new TimeSpan(0, 0, 0, (int)durationWhole, (int)durationFraction);
        BACameraOscillateMove ba = new BACameraOscillateMove(distance, speed, bothWays,
            durationTime, startDelay, endDelay, movementCurve, returnToStart);
        _animationSequence.Add(ba);
    }
}
