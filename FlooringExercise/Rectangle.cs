namespace FlooringExercise
{
    public class Rectangle
    {
        private readonly Location _upperLeft;
        private readonly Location _upperRight;
        private readonly Location _bottomLeft;
        private readonly Location _bottomRight;

        public Rectangle(Location upperLeft, Location upperRight, Location bottomLeft, Location bottomRight)
        {
            _upperLeft = upperLeft;
            _upperRight = upperRight;
            _bottomLeft = bottomLeft;
            _bottomRight = bottomRight;
        }

        public Location GetUpperLeft() => _upperLeft;
        
        public Location GetUpperRight() => _upperRight;
        
        public Location GetBottomLeft() => _bottomLeft;
        
        public Location GetBottomRight() => _bottomRight;
    }
}
