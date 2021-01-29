using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Crimson
{
    public class TileData
    {
        public bool FlipH;
        public bool FlipV;
        public CTexture Texture;

        public TileData(CTexture texture, bool flipH = false, bool flipV = false)
        {
            Texture = texture;
            FlipH = flipH;
            FlipV = flipV;
        }
    }

    public class TileGrid : Component
    {
        public float Alpha = 1f;
        public Camera ClipCamera;
        public Color Color = Color.White;
        public Vector2 Position;
        public VirtualMap<TileData> Tiles;
        public int VisualExtend = 0;

        public TileGrid(int tileWidth, int tileHeight, int tilesX, int tilesY)
            : base(false, true)
        {
            TileWidth = tileWidth;
            TileHeight = tileHeight;
            Tiles = new VirtualMap<TileData>(tilesX, tilesY);
        }

        public void Populate(Tileset tileset, int[,] tiles, int offsetX = 0, int offsetY = 0)
        {
            for (var x = 0; x < tiles.GetLength(0) && x + offsetX < TilesX; x++)
            for (var y = 0; y < tiles.GetLength(1) && y + offsetY < TilesY; y++)
                Tiles[x + offsetX, y + offsetY] = new TileData(tileset[tiles[x, y]]);
        }

        public void Populate(Tileset tileset, int[,] tiles, bool[,] flipH, bool[,] flipV, int offsetX = 0,
            int offsetY = 0)
        {
            for (var x = 0; x < tiles.GetLength(0) && x + offsetX < TilesX; x++)
            for (var y = 0; y < tiles.GetLength(1) && y + offsetY < TilesY; y++)
                Tiles[x + offsetX, y + offsetY] = new TileData(tileset[tiles[x, y]], flipH[x, y], flipV[x, y]);
        }

        public void Overlay(Tileset tileset, int[,] tiles, int offsetX = 0, int offsetY = 0)
        {
            for (var x = 0; x < tiles.GetLength(0) && x + offsetX < TilesX; x++)
            for (var y = 0; y < tiles.GetLength(1) && y + offsetY < TilesY; y++)
                if (tiles[x, y] >= 0)
                    Tiles[x + offsetX, y + offsetY] = new TileData(tileset[tiles[x, y]]);
        }

        public void Overlay(Tileset tileset, int[,] tiles, bool[,] flipH, bool[,] flipV, int offsetX = 0,
            int offsetY = 0)
        {
            for (var x = 0; x < tiles.GetLength(0) && x + offsetX < TilesX; x++)
            for (var y = 0; y < tiles.GetLength(1) && y + offsetY < TilesY; y++)
                if (tiles[x, y] >= 0)
                    Tiles[x + offsetX, y + offsetY] = new TileData(tileset[tiles[x, y]], flipH[x, y], flipV[x, y]);
        }

        public void ExtendEmpty(int left, int right, int up, int down)
        {
            Position -= new Vector2(left * TileWidth, up * TileHeight);

            var newWidth = TilesX + left + right;
            var newHeight = TilesY + up + down;
            if (newWidth <= 0 || newHeight <= 0)
            {
                Tiles = new VirtualMap<TileData>(0, 0);
                return;
            }

            var newTiles = new VirtualMap<TileData>(newWidth, newHeight);

            //Center
            for (var x = 0; x < TilesX; x++)
            for (var y = 0; y < TilesY; y++)
            {
                var atX = x + left;
                var atY = y + up;

                if (atX >= 0 && atX < newWidth && atY >= 0 && atY < newHeight) newTiles[atX, atY] = Tiles[x, y];
            }

            //Left
            for (var x = 0; x < left; x++)
            for (var y = 0; y < newHeight; y++)
                newTiles[x, y] = Tiles.EmptyValue;

            //Right
            for (var x = newWidth - right; x < newWidth; x++)
            for (var y = 0; y < newHeight; y++)
                newTiles[x, y] = Tiles.EmptyValue;

            //Top
            for (var y = 0; y < up; y++)
            for (var x = 0; x < newWidth; x++)
                newTiles[x, y] = Tiles.EmptyValue;

            //Bottom
            for (var y = newHeight - down; y < newHeight; y++)
            for (var x = 0; x < newWidth; x++)
                newTiles[x, y] = Tiles.EmptyValue;

            Tiles = newTiles;
        }

        public void Extend(int left, int right, int up, int down)
        {
            Position -= new Vector2(left * TileWidth, up * TileHeight);

            var newWidth = TilesX + left + right;
            var newHeight = TilesY + up + down;
            if (newWidth <= 0 || newHeight <= 0)
            {
                Tiles = new VirtualMap<TileData>(0, 0);
                return;
            }

            var newTiles = new VirtualMap<TileData>(newWidth, newHeight);

            //Center
            for (var x = 0; x < TilesX; x++)
            for (var y = 0; y < TilesY; y++)
            {
                var atX = x + left;
                var atY = y + up;

                if (atX >= 0 && atX < newWidth && atY >= 0 && atY < newHeight) newTiles[atX, atY] = Tiles[x, y];
            }

            //Left
            for (var x = 0; x < left; x++)
            for (var y = 0; y < newHeight; y++)
                newTiles[x, y] = Tiles[0, Mathf.Clamp(y - up, 0, TilesY - 1)];

            //Right
            for (var x = newWidth - right; x < newWidth; x++)
            for (var y = 0; y < newHeight; y++)
                newTiles[x, y] = Tiles[TilesX - 1, Mathf.Clamp(y - up, 0, TilesY - 1)];

            //Top
            for (var y = 0; y < up; y++)
            for (var x = 0; x < newWidth; x++)
                newTiles[x, y] = Tiles[Mathf.Clamp(x - left, 0, TilesX - 1), 0];

            //Bottom
            for (var y = newHeight - down; y < newHeight; y++)
            for (var x = 0; x < newWidth; x++)
                newTiles[x, y] = Tiles[Mathf.Clamp(x - left, 0, TilesX - 1), TilesY - 1];

            Tiles = newTiles;
        }

        public void FillRect(int x, int y, int columns, int rows, CTexture tile, bool flipH = false, bool flipV = false)
        {
            var left = Mathf.Max(0, x);
            var top = Mathf.Max(0, y);
            var right = Mathf.Min(TilesX, x + columns);
            var bottom = Mathf.Min(TilesY, y + rows);

            for (var tx = left; tx < right; tx++)
            for (var ty = top; ty < bottom; ty++)
                Tiles[tx, ty] = new TileData(tile, flipH, flipV);
        }

        public void Clear()
        {
            for (var tx = 0; tx < TilesX; tx++)
            for (var ty = 0; ty < TilesY; ty++)
                Tiles[tx, ty] = null;
        }

        public Rectangle GetClippedRenderTiles()
        {
            var pos = Entity.Position + Position;

            int left, top, right, bottom;
            if (ClipCamera == null)
            {
                // throw new Exception("NULL CLIP: " + Entity.GetType().ToString());
                left = -VisualExtend;
                top = -VisualExtend;
                right = TilesX + VisualExtend;
                bottom = TilesY + VisualExtend;
            }
            else
            {
                Camera camera = ClipCamera;

                var corner1 = camera.ScreenToCamera(Vector2.Zero);
                var corner2 = camera.ScreenToCamera(new Vector2(camera.Viewport.Width, 0));
                var corner3 = camera.ScreenToCamera(new Vector2(0, camera.Viewport.Height));
                var corner4 = camera.ScreenToCamera(new Vector2(camera.Viewport.Width, camera.Viewport.Height));

                var cameraLeft = Mathf.Min(corner1.X, corner2.X, corner3.X, corner4.X);
                var cameraRight = Mathf.Max(corner1.X, corner2.X, corner3.X, corner4.X);
                var cameraTop = Mathf.Min(corner1.Y, corner2.Y, corner3.Y, corner4.Y);
                var cameraBottom = Mathf.Max(corner1.Y, corner2.Y, corner3.Y, corner4.Y);

                left = Mathf.Max(0, Mathf.FloorToInt((cameraLeft - pos.X) / TileWidth) - VisualExtend);
                top = Mathf.Max(0, Mathf.FloorToInt((cameraTop - pos.Y) / TileHeight) - VisualExtend);
                right = Mathf.Min(TilesX, Mathf.CeilToInt((cameraRight - pos.X) / TileWidth) + VisualExtend);
                bottom = Mathf.Min(TilesY, Mathf.CeilToInt((cameraBottom - pos.Y) / TileHeight) + VisualExtend);
            }

            // clamp
            left = Mathf.Max(left, 0);
            top = Mathf.Max(top, 0);
            right = Mathf.Min(right, TilesX);
            bottom = Mathf.Min(bottom, TilesY);

            return new Rectangle(left, top, right - left, bottom - top);
        }

        public override void Render()
        {
            RenderAt(Entity.Position + Position);
        }

        public void RenderAt(Vector2 position)
        {
            if (Alpha <= 0) return;

            var clip = GetClippedRenderTiles();
            var color = Color * Alpha;

            for (var tx = clip.Left; tx < clip.Right; tx++)
            for (var ty = clip.Top; ty < clip.Bottom; ty++)
            {
                TileData tile = Tiles[tx, ty];
                if (tile != null)
                {
                    var effects = SpriteEffects.None;
                    if (tile.FlipH) effects |= SpriteEffects.FlipHorizontally;

                    if (tile.FlipV) effects |= SpriteEffects.FlipVertically;

                    tile.Texture.Draw(position + new Vector2(tx * TileWidth, ty * TileHeight), Vector2.Zero, color,
                        Vector2.One, 0, effects);
                }
            }
        }

        #region Properties

        public int TileWidth { get; }
        public int TileHeight { get; }
        public int TilesX => Tiles.Columns;
        public int TilesY => Tiles.Rows;

        #endregion
    }
}
