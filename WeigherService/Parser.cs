using Logger;
using System;
using System.Globalization;
using System.Linq;

namespace WeigherService
{
    
    public static class Parser
    {
        #region LoggerProperties
        public static int LogStatus { get; set; }
        public static string LogFilePath { get; set; }
        public static bool LogEnable { get; set; }
        public static string DateTimeFormat { get; set; }
        #endregion

        static public float GetFloatValue(string txFromScale)
        {
            float weighFloat = -1;
            try
            {
                //standart answer from device is:   "SI          0.059kg 11" or 
                //                                  "SI?         1.250kg 11"
                //substring(3,14) is :              "         x.xxx"
                string[] weighStr = txFromScale.Substring(3, 14).Split(new char[] { ' ' });
                weighFloat = float.Parse(weighStr[weighStr.Length - 1], CultureInfo.InvariantCulture.NumberFormat);
                Log.Write($"Weigh.Parser.GetFloatValue(): inputString: {txFromScale}, parse value: {weighFloat}");
            }
            catch(Exception ex)
            {
                Log.Write($"Weigh.Parser.GetFloatValue() Exception: {ex}");
                return weighFloat;
            }
            return weighFloat;
        }
        public static float GetFilteredWeight(float[] data)
        {
            int lenth = data.Length;
            int hw;
            float valueWithoutHW = 0;
            float valueWithHW = 0;
            float result = 0;
            int step = 0;

            try
            {
                double centrLine;
                if (lenth % 2 == 0)
                {
                    centrLine = lenth / 2;
                }
                else
                {
                    centrLine = ((double)lenth / 2) - 0.5;
                }
                for (int i = 0; i < lenth; i++)
                {
                    //firsPart
                    if (i < centrLine)
                    {
                        valueWithHW = 0;
                        hw = i;
                        if (i == 0)
                        {
                            valueWithoutHW = data[i];
                        }
                        else
                        {
                            step = 0;
                            for (int j = -hw; j <= hw; j++)
                            {
                                step++;
                                valueWithHW += data[i - j];
                            }
                            valueWithHW = valueWithHW / step;
                        }
                    }
                    result = valueWithoutHW + valueWithHW;
                    //centralPart
                    if (i == centrLine)
                    {
                        step = 0;
                        valueWithHW = 0;
                        foreach (float val in data)
                        {
                            step++;
                            valueWithHW += val;
                        }
                        valueWithHW = valueWithHW / step;
                    }
                    result += valueWithHW;
                    //secondPart
                    if (i > centrLine)
                    {
                        valueWithHW = 0;
                        hw = data.Length - 1 - i;
                        if (i == data.Length - 1)
                        {
                            valueWithoutHW = data[i];
                        }
                        else
                        {
                            step = 0;
                            for (int j = -hw; j <= hw; j++)
                            {
                                step++;
                                valueWithHW += data[i - j];
                            }
                            valueWithHW = valueWithHW / step;
                        }
                    }
                }
                result += (valueWithoutHW + valueWithHW);
                result = result / 4;
                return (result>0?result:0);
            }
            catch(Exception ex)
            {
                Log.Write($"Weigh.Parser.GetFilteredWeight() Exception: {ex}");
                return (float)Status.Failed;
            }
        }
        public static float GetWeight(float[] data)
        {
            try
            {
                float val = 0;
                foreach (float value in data)
                {
                    val += value;
                }
                return val / data.Count();
            }
            catch(Exception ex)
            {
                Log.Write($"Weigh.Parser.GetWeight() Exception: {ex}");
                return (float)Status.Failed;
            }
            
        }
    }
}
