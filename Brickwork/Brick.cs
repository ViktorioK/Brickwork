namespace Brickwork
{
    public class Brick
    {
        public Brick(int number)
        {
            this.Number = number;
            this.IsParent = false;
            this.IsChild = false;
        }

        //Unique number of brick pair.
        public int Number { get; set; }

        //The left or top portion of brick pair.
        public bool IsParent { get; set; }

        public int ParentRow { get; set; }

        public int ParentColumn { get; set; }

        //The right or bot portion of brick pair.
        public bool IsChild { get; set; }

        public int ChildRow { get; set; }

        public int ChildColumn { get; set; }
    }
}
