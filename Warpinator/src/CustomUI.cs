using System;
using System.Globalization;
using SFS.UI.ModGUI;
using UnityEngine;
using UnityEngine.Events;

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
            NumberInput ToReturn = new NumberInput
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
        static void Numberify(NumberInput data)
        {
            try
            {
                double.Parse(data.textInput.Text, CultureInfo.InvariantCulture);
            }
            catch
            {
                if (data.textInput.Text is "." or "")
                    return;

                data.textInput.Text = data.oldText;
                return;
            }


            if (data.textInput.Text.Length > data.charLimit)
            {
                data.textInput.Text = data.oldText;
            }

            double numCheck = double.Parse(data.textInput.Text, CultureInfo.InvariantCulture);

            if (numCheck == 0)
                data.currentVal = data.defaultVal;
            else if (numCheck < data.min)
            {
                data.currentVal = Math.Floor(data.min);
                data.textInput.Text = data.currentVal.ToString(CultureInfo.InvariantCulture);
            }
            else if (numCheck > data.max)
            {
                data.currentVal = Math.Floor(data.max);
                data.textInput.Text = data.currentVal.ToString(CultureInfo.InvariantCulture);
            }
            else
                data.currentVal = numCheck.Round(0.0001);

            data.oldText = data.textInput.Text;
        }
    }
}