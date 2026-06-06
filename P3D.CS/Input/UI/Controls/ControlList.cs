using Microsoft.Xna.Framework.Input;

namespace P3D.UI.GameControls;

/// <summary>A list of controls that manages mutual focus and tab navigation.</summary>
public class ControlList
{
    private List<Control> _list = [];

    public void Add(Control ctl)
    {
        ctl.Focused += FocusedControl;
        _list.Add(ctl);
    }

    public void AddRange(Control[] ctls)
    {
        foreach (Control ctl in ctls)
        {
            Add(ctl);
        }
    }

    public void Remove(Control ctl)
    {
        if (_list.Contains(ctl) == true)
        {
            ctl.Focused -= FocusedControl;
            _list.Remove(ctl);
        }
    }

    private void FocusedControl(Object? sender, EventArgs e)
    {
        foreach (Control ctl in _list)
        {
            if (sender != null && sender.Equals(ctl) == false)
            {
                ctl.IsFocused = false;
            }
        }
    }

    public void Update()
    {
        if ((KeyBoardHandler.KeyPressed(Keys.Tab) ||
             Controls.Down(true, false, false, false, true, true) ||
             Controls.Up(true, false, false, false, true, true)) &&
            _list.Count > 0)
        {
            if (_list.Count == 1)
            {
                if (_list[0].IsFocused == false)
                {
                    _list[0].IsFocused = true;
                }
            }
            else
            {
                bool hasFocusedControl = false;
                int controlListIndex = 0;

                for (int i = 0; i < _list.Count; i++)
                {
                    if (_list[i].IsFocused)
                    {
                        hasFocusedControl = true;
                        controlListIndex = i;
                    }
                }

                if (hasFocusedControl)
                {
                    int tabDirection = 1;
                    if (Controls.ShiftDown(triggerButtons: false) == true ||
                        Controls.Up(false, false, false, false, true, true))
                    {
                        tabDirection = -1;
                    }

                    int focusIndex = controlListIndex + tabDirection;
                    if (focusIndex == _list.Count)
                    {
                        focusIndex = 0;
                    }
                    else if (focusIndex == -1)
                    {
                        focusIndex = _list.Count - 1;
                    }
                    _list[focusIndex].IsFocused = true;
                }
                else
                {
                    _list[0].IsFocused = true;
                }
            }
        }
    }
}
