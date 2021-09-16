namespace AmbiledService.Models
{
    struct PreciseRGB
    {
        public float red;
        public float green;
        public float blue;
        public PreciseRGB(RGB led)
        {
            red = led.red;
            green = led.green;
            blue = led.blue;
        }
    }
}
