using Microsoft.Xna.Framework;

namespace Crimson
{
    public class Tileset
    {
        private readonly CTexture[,] tiles;

        public Tileset(CTexture texture, int tileWidth, int tileHeight)
        {
            Texture = texture;
            TileWidth = tileWidth;
            TileHeight = tileHeight;

            tiles = new CTexture[Texture.Width / tileWidth, Texture.Height / tileHeight];
            for (var x = 0; x < Texture.Width / tileWidth; x++)
            for (var y = 0; y < Texture.Height / tileHeight; y++)
                tiles[x, y] = new CTexture(Texture, x * tileWidth, y * tileHeight, tileWidth, tileHeight);
        }

        public CTexture this[int x, int y] => tiles[x, y];

        public CTexture this[int index]
        {
            get
            {
                if (index < 0) return null;

                return tiles[index % tiles.GetLength(0), index / tiles.GetLength(0)];
            }
        }

        public int getIndex(Vector2 position)
        {
            // Out of bounds
            if (position.X < 0 || position.X > Texture.Width || position.Y < 0 ||
                position.Y > Texture.Height) return -1;

            var x = Mathf.FloorToInt(position.X / TileWidth);
            var y = Mathf.FloorToInt(position.Y / TileHeight);
            return y * TilesX + x;
        }

        public int getIndex(System.Numerics.Vector2 position)
        {
            return getIndex(new Vector2(position.X, position.Y));
        }

        #region Properties

        public CTexture Texture { get; }
        public int TileWidth { get; }
        public int TileHeight { get; }

        public int Count => TilesX * TilesY;
        public int TilesX => Texture.Width / TileWidth;
        public int TilesY => Texture.Height / TileHeight;

        #endregion
    }
}