using System;
using System.Globalization;
using SFS.UI.ModGUI;
using UnityEngine;
using UnityEngine.Events;
using System.Text.RegularExpressions;
using Button = SFS.UI.ModGUI.Button;

namespace Warpinator
{
    public class NumberInput
    {
        public TextInput textInput;
        public string oldText;
        public double defaultVal;
        public double currentVal;
        public double min;
        public double max;
        public int charLimit;
    }
    public static class CustomUI
    {
        public static NumberInput CreateNumberInput(Transform parent, int width, int height,
            double defaultVal, double min, double max, int charLimit = 100, UnityAction<string> onChange = null, int posX = 0,
            int posY = 0)
        {
            var ToReturn = new NumberInput
            {
                textInput = Builder.CreateTextInput(parent, width, height, posX, posY, defaultVal.ToString(CultureInfo.InvariantCulture), onChange),
                oldText = defaultVal.ToString(CultureInfo.InvariantCulture),
                defaultVal = defaultVal,
                currentVal = defaultVal,
                min = min,
                max = max,
                charLimit = charLimit,
            };
            ToReturn.textInput.OnChange += _ => Numberify(ToReturn);
            return ToReturn;
        }

        // This method is used to process and validate the input in the NumberInput object.
        private static void Numberify(NumberInput data)
        {
            // Retrieve the TextInput instance from the NumberInput object.
            TextInput input = data.textInput;

            // Trim the text to remove any leading or trailing whitespaces.
            input.Text = input.Text.Trim();

            // Try to parse the text input as a double using the InvariantCulture format.
            // If parsing fails, handle the different scenarios.
            if (!double.TryParse(input.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double parsedText))
            {
                // Case when the input is not a valid number.

                // Check if the minimum value allowed is non-negative.
                if (data.min >= 0)
                {
                    // If the input text is ".", set the currentVal to 0 and store the oldText.
                    if (input.Text == ".")
                    {
                        data.currentVal = 0;
                        data.oldText = input.Text;
                        return;
                    }
                    
                    // If the input text is empty, set the currentVal to 0, clear the input text, and store the oldText.
                    if (input.Text == "")
                    {
                        data.oldText = input.Text = "";
                        return;
                    }
                }
                else if (Regex.IsMatch(input.Text, "^-?\\.?$"))
                {
                    // If the input text matches the pattern for "-", "-.", or ".", set the currentVal to 0 and store the oldText.
                    data.currentVal = 0;
                    data.oldText = input.Text;
                    return;
                }

                // If none of the special cases apply, restore the old text in the input and return.
                input.Text = data.oldText;
                return;
            }

            // If parsing succeeded, the input is a valid number.
            // Check if the input length exceeds the character limit. If so, restore the old text and return.
            if (input.Text.Length > data.charLimit)
            {
                input.Text = data.oldText;
            }

            // If the input text starts with "-0" or "0.", remove the leading zeros.
            if (Regex.IsMatch(input.Text, "^-?0+[1-9.]"))
            {
                Regex regex = new Regex("0+");
                input.Text = regex.Replace(input.Text, "", 1);
            }

            // Check if the parsed number is less than the minimum allowed value.
            if (parsedText < data.min)
            {
                // Set the currentVal to the minimum value, rounded up to the nearest integer if data.min >= 0,
                // otherwise, round it down to the nearest integer.
                data.currentVal = parsedText >= 0 ? Math.Ceiling(data.min) : Math.Floor(data.min);

                // If the minimum value is less than or equal to 0, set the input text to the minimum value.
                if (data.min <= 0)
                {
                    input.Text = data.min.ToString(CultureInfo.InvariantCulture);
                }
            }
            // Check if the parsed number is greater than the maximum allowed value.
            else if (parsedText > data.max)
            {
                // Set the currentVal to the maximum value, rounded down to the nearest integer.
                data.currentVal = Math.Floor(data.max);

                // Set the input text to the maximum value.
                input.Text = data.currentVal.ToString(CultureInfo.InvariantCulture);
            }
            // If the number is within the valid range, round it to a precision of 0.000001.
            else
                data.currentVal = parsedText.Round(0.000001);

            // Store the current input text as the oldText for future reference.
            data.oldText = input.Text;
        }

        public static Button UnboundedButton(Window window, int width, int height, int posX, int posY,
            Action onClick = null, string text = "", bool keepButtonOutOfWindow = false)
        {
            Button output = Builder.CreateButton(window.gameObject.transform, width, height, 0, 0, onClick, text);
            
            posX -= (int)((window.Size.x / 2) - width / 2);

            if (keepButtonOutOfWindow && posY < 0) posY -= (int)(window.Size.y + Math.Round((double)height / 2));
            else posY -= (int)Math.Round((decimal)height / 2);
            
            output.gameObject.transform.localPosition = new Vector3(posX, posY);
            return output;
        }
    }
}