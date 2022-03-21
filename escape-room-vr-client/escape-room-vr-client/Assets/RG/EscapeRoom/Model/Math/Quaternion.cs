namespace RG.EscapeRoom.Model.Math
{
    public struct Quaternion
    {
        private double w;
        private double x;
        private double y;
        private double z;

        public Quaternion(double w, double x, double y, double z)
        {
            this.w = w;
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}