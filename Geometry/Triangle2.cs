namespace JA.Geometry
{
    public readonly struct Triangle2
    {
        public Triangle2(Vector2 a, Vector2 b, Vector2 c)
        {
            this.A = a;
            this.B = b;
            this.C = c;

            this.Edges = new Line2[] {
                new Line2(a, b),
                new Line2(b, c),
                new Line2(c, a),
            };
        }
        public Vector2 A { get; }
        public Vector2 B { get; }
        public Vector2 C { get; }

        public Line2[] Edges { get; }

        public static Triangle2 operator +(Triangle2 triangle, Vector2 delta)
            => new Triangle2(
                triangle.A + delta,
                triangle.B + delta,
                triangle.C + delta);

        public override string ToString() => $"Triangle:{A},{B},{C}";
    }
}
