using GTA;
using GTA.Native;
using System.Collections.Generic;

namespace FusionLibrary
{
    public class InstrumentalButton
    {
        public Control Control { get; }
        public string Title { get; }

        public InstrumentalButton(Control control, string title)
        {
            Control = control;
            Title = title;
        }
    }

    public class InstrumentalMenu : ScaleformGui
    {
        private readonly List<InstrumentalButton> _buttonList;

        public InstrumentalMenu() : base("instructional_buttons")
        {
            _buttonList = new List<InstrumentalButton>();

            CallFunction("SET_DATA_SLOT_EMPTY");
        }

        public void AddControl(Control control, string title)
        {
            _buttonList.Add(new InstrumentalButton(control, title));
        }

        public void RemoveControls()
        {
            _buttonList.Clear();
        }

        // Needs to be called on tick to update button icons (Controller / Pc)
        public void UpdatePanel()
        {
            ClearPanel();

            foreach (InstrumentalButton button in _buttonList)
                CallFunction("SET_DATA_SLOT", _buttonList.IndexOf(button), GetButtonIdFromControl(button.Control), button.Title);

            SetButtons();
        }

        public int GetButtonIdFromControl(Control control)
        {
            string controlName = Function.Call<string>(Hash.GET_CONTROL_INSTRUCTIONAL_BUTTON, 2, control, true).Substring(2);
            
            return int.Parse(controlName);
        }

        public void ClearPanel()
        {
            CallFunction("SET_DATA_SLOT_EMPTY");
        }

        private void SetButtons()
        {
            CallFunction("DRAW_INSTRUCTIONAL_BUTTONS", "-1");
        }
    }
}