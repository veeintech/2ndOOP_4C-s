using System;
using System.Windows.Forms;

namespace _4C_s_Management_System.Classess
{
    /// <summary>Helpers for common Windows Forms input controls.</summary>
    public static class FormControls
    {
        /// <summary>Clears supported inputs inside a form or container.</summary>
        public static void ClearInputs(Control? root)
        {
            if (root is null)
                return;

            ClearOne(root);

            foreach (Control child in root.Controls)
                ClearInputs(child);
        }

        /// <summary>Clears the text of a supported input control.</summary>
        public static void ClearText(Control? control)
        {
            if (control is not null && !IsDisplayOnly(control))
                control.Text = string.Empty;
        }

        /// <summary>Returns whether a supported input has a value.</summary>
        public static bool Required(Control? control, string? fieldName = null)
        {
            if (control is null)
                return false;

            if (control is CheckBox checkBox)
                return checkBox.Checked;

            if (control is RadioButton radioButton)
                return radioButton.Checked;

            if (control is ComboBox comboBox)
            {
                return comboBox.DropDownStyle == ComboBoxStyle.DropDownList
                    ? comboBox.SelectedIndex >= 0
                    : !string.IsNullOrWhiteSpace(comboBox.Text);
            }

            if (control is NumericUpDown or DateTimePicker)
                return true;

            if (IsDisplayOnly(control) || control.HasChildren)
                return true;

            return !string.IsNullOrWhiteSpace(control.Text);
        }

        /// <summary>Returns the first empty required input, or null when all are valid.</summary>
        public static Control? FirstRequired(params Control?[]? controls)
        {
            if (controls is null)
                return null;

            foreach (Control? control in controls)
            {
                if (control is not null && !Required(control, control.Name))
                    return control;
            }

            return null;
        }

        private static void ClearOne(Control control)
        {
            if (control is TextBoxBase textBox)
            {
                textBox.Clear();
                return;
            }

            if (control is ComboBox comboBox)
            {
                comboBox.SelectedIndex = -1;
                if (comboBox.DropDownStyle != ComboBoxStyle.DropDownList)
                    comboBox.Text = string.Empty;
                return;
            }

            if (control is DateTimePicker dateTimePicker)
            {
                DateTime today = DateTime.Today;
                dateTimePicker.Value = today < dateTimePicker.MinDate
                    ? dateTimePicker.MinDate
                    : today > dateTimePicker.MaxDate
                        ? dateTimePicker.MaxDate
                        : today;
                return;
            }

            if (control is CheckBox checkBox)
            {
                checkBox.Checked = false;
                return;
            }

            if (control is RadioButton radioButton)
            {
                radioButton.Checked = false;
                return;
            }

            if (control is NumericUpDown numericUpDown)
            {
                numericUpDown.Value = numericUpDown.Minimum;
                return;
            }

            if (!IsDisplayOnly(control) && !control.HasChildren)
                control.Text = string.Empty;
        }

        private static bool IsDisplayOnly(Control control)
        {
            return control is Label
                or LinkLabel
                or ButtonBase
                or GroupBox
                or TabPage
                or TabControl
                or Panel
                or ContainerControl;
        }
    }
}