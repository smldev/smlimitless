using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SMLimitless.Sprites.Collections;

namespace SMLimitless.Editor
{
    internal sealed class SectionExitFormData
    {
        public bool ParseSucceded { get; }

        public ExitSourceBehavior SourceBehavior { get; }
        public int SourceSectionID { get; }
        public int SourceID { get; }
        public int SourceX { get; }
        public int SourceY { get; }

        public ExitDestinationBehavior DestinationBehavior { get; }
        public int DestinationSectionID { get; }
        public int DestinationID { get; }
        public int DestinationX { get; }
        public int DestinationY { get; }

        public SectionExitFormData(EditorForm form, Level level)
        {
            bool success = false;
            ParseSucceded = true;

            SourceBehavior = TryGetSourceBehaviorFromForm(form, out success);
            if (!success)
            {
                ParseSucceded = false;
                ExitParametersNotValid("Please select a source behavior.");
            }

            DestinationBehavior = TryGetDestinationBehaviorFromForm(form, out success);
            if (!success)
            {
                ParseSucceded = false;
                ExitParametersNotValid("Please select a destination behavior.");
            }

            SourceSectionID = ParseIntFromTextBox(form.TextSourceSectionID,
                "Please add a source section ID number.\r\nIf you're not sure what to type, consult the Sections tab.");
            if (SourceSectionID < 0) { ParseSucceded = false; }

            DestinationSectionID = ParseIntFromTextBox(form.TextDestinationSectionID,
                "Please add a destination section ID number.\r\nIf you're not sure what to type, consult the Sections tab.");
            if (DestinationSectionID < 0) { ParseSucceded = false; }

            SourceX = ParseIntFromTextBox(form.TextSourceX,
                "Please input a whole number into the Source X textbox.");
            if (SourceX < 0) { ParseSucceded = false; }

            SourceY = ParseIntFromTextBox(form.TextSourceY,
                "Please input a whole number into the Source Y textbox.");
            if (SourceY < 0) { ParseSucceded = false; }

            DestinationX = ParseIntFromTextBox(form.TextDestinationX,
                "Please input a whole number into the Destination X textbox.");
            if (DestinationX < 0) { ParseSucceded = false; }

            DestinationY = ParseIntFromTextBox(form.TextDestinationY,
                "Please input a whole number into the Destination Y textbox.");
            if (DestinationY < 0) { ParseSucceded = false; }

            if (ParseSucceded)
            {
                SourceID = level.SectionExitIDGenerator.GetNewID();
                DestinationID = level.SectionExitIDGenerator.GetNewID();
            }
        }

        private static ExitSourceBehavior TryGetSourceBehaviorFromForm(EditorForm form, out bool success)
        {
            if (form.RadioSourcePipeDown.Checked) { success = true; return ExitSourceBehavior.PipeDown; }
            else if (form.RadioSourcePipeUp.Checked) { success = true; return ExitSourceBehavior.PipeUp; }
            else if (form.RadioSourcePipeLeft.Checked) { success = true; return ExitSourceBehavior.PipeLeft; }
            else if (form.RadioSourcePipeRight.Checked) { success = true; return ExitSourceBehavior.PipeRight; }
            else if (form.RadioSourceDoor.Checked) { success = true; return ExitSourceBehavior.Door; }
            else { success = false; return ExitSourceBehavior.NotASource; }
        }

        private static ExitDestinationBehavior TryGetDestinationBehaviorFromForm(EditorForm form, out bool success)
        {
            if (form.RadioDestinationPipeDown.Checked) { success = true; return ExitDestinationBehavior.PipeDown; }
            else if (form.RadioDestinationPipeUp.Checked) { success = true; return ExitDestinationBehavior.PipeUp; }
            else if (form.RadioDestinationPipeLeft.Checked) { success = true; return ExitDestinationBehavior.PipeLeft; }
            else if (form.RadioDestinationPipeRight.Checked) { success = true; return ExitDestinationBehavior.PipeRight; }
            else if (form.RadioDestinationDoor.Checked) { success = true; return ExitDestinationBehavior.None; }
            else { success = false; return ExitDestinationBehavior.NotADestination; }
        }

        private static int ParseIntFromTextBox(TextBox textBox, string failureMessage)
        {
            int result;
            if (!int.TryParse(textBox.Text, out result))
            {
                ExitParametersNotValid(failureMessage);
                return -1;
            }
            return result;
        }

        private static void ExitParametersNotValid(string message)
        {
            MessageBox.Show(message, "Super Mario Limitless", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
