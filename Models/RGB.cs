namespace AmbiledService.Models
{
    public struct RGB
    {
        public byte red;
        public byte green;
        public byte blue;
        public RGB(RGB other)
        {
            red = other.red;
            green = other.green;
            blue = other.blue;
        }
        public RGB(byte red, byte green, byte blue)
        {
            this.red = red;
            this.green = green;
            this.blue = blue;
        }
    }
}
