using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Input.InputListeners;

namespace Farkle.Services;

public class InputManager : GameComponent
{
    private readonly Game _game;
    private InputListenerComponent _inputListenerComponent;

    private KeyboardListener _keyboardListener;
    private MouseListener _mouseListener;
    public MouseListener Mouse => _mouseListener;
    public KeyboardListener Keyboard => _keyboardListener;

    public bool GuiWantCaptureMouse { get; set; }
    public bool GuiWantCaptureMouseUnlessPopupClose { get; set; }
    public bool GuiWantCaptureKeyboard { get; set; }
    public bool GuiWantTextInput { get; set; }

    public InputManager(Game game)
        : base(game)
    {
        _game = game;
            
        _keyboardListener = new KeyboardListener();
        _mouseListener = new MouseListener(new MouseListenerSettings { DoubleClickMilliseconds = 100 });
    }

    public override void Initialize()
    {            
        _inputListenerComponent = new InputListenerComponent(_game, _keyboardListener, _mouseListener);
        
        base.Initialize();
    }

    public override void Update(GameTime gameTime)
    {
        _inputListenerComponent.Update(gameTime);
    }
}