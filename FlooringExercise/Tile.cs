namespace FlooringExercise
{
    public  class Tile
    {
        private int Width { get; }
        
        private int Height { get; }

        public Tile(int height, int width)
        {
            Width = width;
            Height = height;
        }

        public int GetWidth() => Width;

        public int GetHeight() => Height;
    }
}
