using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Crimson
{
    public class Quad
    {
        private readonly short[] Indices;
        private readonly VertexPositionTexture[] Vertices;

        public Quad()
        {
            Vertices = new[]
            {
                new VertexPositionTexture(
                    new Vector3(0, 0, 0),
                    new Vector2(1, 1)),
                new VertexPositionTexture(
                    new Vector3(0, 0, 0),
                    new Vector2(0, 1)),
                new VertexPositionTexture(
                    new Vector3(0, 0, 0),
                    new Vector2(0, 0)),
                new VertexPositionTexture(
                    new Vector3(0, 0, 0),
                    new Vector2(1, 0))
            };

            Indices = new short[] {0, 1, 2, 2, 3, 0};
        }

        public void Render()
        {
            Render(Vector2.One * -1, Vector2.One);
        }

        public void Render(Vector2 v1, Vector2 v2)
        {
            Vertices[0].Position.X = v2.X;
            Vertices[0].Position.Y = v1.Y;

            Vertices[1].Position.X = v1.X;
            Vertices[1].Position.Y = v1.Y;

            Vertices[2].Position.X = v1.X;
            Vertices[2].Position.Y = v2.Y;

            Vertices[3].Position.X = v2.X;
            Vertices[3].Position.Y = v2.Y;

            Engine.Instance.GraphicsDevice.DrawUserIndexedPrimitives(
                PrimitiveType.TriangleList,
                Vertices,
                0,
                4,
                Indices,
                0,
                2
            );
        }
    }
}