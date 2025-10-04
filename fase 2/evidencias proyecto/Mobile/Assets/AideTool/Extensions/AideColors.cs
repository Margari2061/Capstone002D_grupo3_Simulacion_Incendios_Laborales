using System;
using UnityEngine;

namespace AideTool
{
    public static class AideColors
    {
        public static Color ChangeOpacity(this Color color, float opacity)
        {
            if (opacity > 1f)
                opacity = 1f;

            if (opacity < 0f)
                opacity = 0f;

            return new Color(color.r, color.g, color.b, opacity);
        }

        public static Color HexToColor(string hex)
        {
            bool isvalidFormat = true;

            if (hex.Length < 3 && hex.Length > 9)
                isvalidFormat = false;

            string validChars = "#0123456789abcdef";
            foreach (char c in hex.ToLower())
                if (!validChars.Contains(c))
                    isvalidFormat = false;

            if (!isvalidFormat)
                throw new FormatException(nameof(hex));

            if (hex[0] == '#')
                hex = hex.Remove(0, 1);

            if (hex.Length == 8)
                return ARGBHexToColor(hex);

            if (hex.Length == 6)
                return RGBHexToColor(hex);

            if(hex.Length == 4)
            {
                hex = DuplicateChannels(hex);
                return ARGBHexToColor(hex);
            }

            hex = DuplicateChannels(hex);
            return RGBHexToColor(hex);
        }

        private static float[] ProcessChannelCodes(string hex)
        {
            int length = hex.Length / 2;

            byte[] channelBytes = new byte[length];
            for (int i = 0; i < length; i++)
            {
                string channelHex = hex.Substring(i * 2, 2);
                channelBytes[i] = Convert.ToByte(channelHex, 16);
            }

            float[] channelPercs = new float[length];
            for(int i = 0; i < length; i++)
                channelPercs[i] = (float)channelBytes[i] / (float)byte.MaxValue;

            return channelPercs;
        }

        private static Color ARGBHexToColor(string argb)
        {
            float[] channels = ProcessChannelCodes(argb);
            return new Color(channels[1], channels[2], channels[3], channels[0]);
        }

        private static Color RGBHexToColor(string rgb)
        {
            float[] channels = ProcessChannelCodes(rgb);
            return new Color(channels[0], channels[1], channels[2]);
        }

        private static string DuplicateChannels(string hex)
        {
            string result = string.Empty;

            foreach (char c in hex)
                result = $"{result}{c}{c}";

            return result;
        }

        public static string ColorToHex(this Color color)
        {
            byte[] channels = new byte[]
            {
                (byte) (color.a * byte.MaxValue),
                (byte) (color.r * byte.MaxValue),
                (byte) (color.g * byte.MaxValue),
                (byte) (color.b * byte.MaxValue)
            };

            string[] channelsHex = new string[]
            {
                channels[0].ToString("x2"),
                channels[1].ToString("x2"),
                channels[2].ToString("x2"),
                channels[3].ToString("x2")
            };

            if (channels[0] == byte.MaxValue)
                return $"#{channelsHex[1]}{channelsHex[2]}{channelsHex[3]}";

            return $"#{channelsHex[0]}{channelsHex[1]}{channelsHex[2]}{channelsHex[3]}";
        }
    }
}
