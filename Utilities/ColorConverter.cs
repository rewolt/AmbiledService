using AmbiledService.Models;

namespace AmbiledService.Utilities
{
    static class ColorConverter
    {
        public static RGB ConvertCpuUsageToRGB(int cpuUsagePercent)
        {
            byte red, green;

            if (cpuUsagePercent > 50)
                red = 255;
            else
                red = (byte)(255 / 50 * cpuUsagePercent);

            if (cpuUsagePercent <= 50)
                green = 255;
            else
                green = (byte)(255 / 50 * (cpuUsagePercent - 50));


            return new RGB(red, green, 0);
        }
    }
}
