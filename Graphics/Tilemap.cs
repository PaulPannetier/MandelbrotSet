using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace SME.TileMap
{
    public delegate void Update(Tile tile);

    public class Tile
    {
        public Update Update;
        public Dictionary<string, Animation> animations;
        public Animation currentAnimation;
        public bool isSolid;

        public Tile(Animation animation)
        {
            Builder(animation, false, null);
        }
        public Tile(Animation animation, in bool isSolid, Update update)
        {
            Builder(animation, isSolid, update);
        }
        private void Builder(Animation animation, in bool isSolid, Update update)
        {
            currentAnimation = animation;
            animations = new Dictionary<string, Animation>();
            AddAnimation(animation, animation.name);
            this.isSolid = isSolid;
            this.Update = update;
        }

        public void ChangeAnimation(string name)
        {
            if (animations.ContainsKey(name))
            {
                currentAnimation = animations[name];
            }
        }
        public void AddAnimation(Animation animation, string name)
        {
            animations.Add(name, animation);
        }
        public Tile Clone()
        {
            Animation a = currentAnimation.Clone();
            Tile t = new Tile(a, isSolid, Update);
            t.animations = new Dictionary<string, Animation>(animations);
            return t;
        }
    }
    public class TilePalette
    {
        public List<Tile> tiles;//tile.ID correspond a la place dans le tableau

        /// <param name="texture">The tilesheet</param>
        /// <param name="width">The number of tiles per column</param>
        /// <param name="height">The number of tiles per line</param>
        public TilePalette(Texture2D texture, in int width, in int height)
        {
            int tileWidth = texture.Width / width;
            int tileHeight = texture.Height / height;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Animation a = new Animation(1, 0f, texture, new Rectangle(x * tileWidth, y * tileHeight, tileWidth, tileHeight));
                    tiles.Add(new Tile(a));
                }
            }
        }

        /// <summary>
        /// Change the tile in the palette's index by the tile in argument
        /// </summary>
        public void ChangeTile(Tile tile, in int index)
        {
            tiles[index] = tile;
        }
        public void SetCollisionTiles(int[] collisionTiles, in bool isSolid)
        {
            for (int i = 0; i < collisionTiles.Length; i++)
            {
                tiles[collisionTiles[i]].isSolid = isSolid;
            }
        }
        public void SetCollisionTile(int indexTile, in bool isSolid)
        {
            tiles[indexTile].isSolid = isSolid;
        }
        public void SetTilesUpdate(int[] indexTiles, Update update)
        {
            for (int i = 0; i < indexTiles.Length; i++)
            {
                tiles[indexTiles[i]].Update = update;
            }
        }
        public void SetTileUpdate(int indexTile, Update update)
        {
            tiles[indexTile].Update = update;
        }
    }
    public class Tilemap
    {
        public float layerDepth;//ordred'affichage des calques
        public Tile[,] tileMap;
        public List<Collider> compositeCollider;

        public Tilemap(Tile[,] tileMap, float layerDepth)
        {
            this.tileMap = tileMap;
            this.layerDepth = layerDepth;
        }

        public void SetCompositeCollider(TilePalette tilePalette)
        {
            
        }

        public static bool operator >(Tilemap t1, Tilemap t2) => t1.layerDepth > t2.layerDepth;
        public static bool operator >=(Tilemap t1, Tilemap t2) => t1.layerDepth >= t2.layerDepth;
        public static bool operator <(Tilemap t1, Tilemap t2) => t1.layerDepth < t2.layerDepth;
        public static bool operator <=(Tilemap t1, Tilemap t2) => t1.layerDepth <= t2.layerDepth;
    }

    public class Map
    {
        public List<Tilemap> tilemaps;
        public Vector2[,] tilesPosition = null;
        public Vector2 gap, tileSize;//l'espace entre 2 tuiles
        public TilePalette tilePalette;
        public float layerDepthMin, layerDepthMax;//range poiur afficher la carte

        public Map(TilePalette tilePalette, in Vector2 tileSize, in Vector2 gap, in float layerDepthMin, in float layerDepthMax)
        {
            tilemaps = new List<Tilemap>();
            this.tilePalette = tilePalette;
            this.tileSize = tileSize * Screen.scaleFactor;
            this.gap = gap * Screen.scaleFactor;
            this.layerDepthMax = layerDepthMax;
            this.layerDepthMin = layerDepthMin;
        }

        private void CalculateTilesPositions()
        {
            for (int y = 0; y < tilemaps[0].tileMap.GetLength(1); y++)
            {
                for (int x = 0; x < tilemaps[0].tileMap.GetLength(0); x++)
                {
                    tilesPosition[x, y] = new Vector2(x * (tileSize.X + gap.X), y * (tileSize.Y + gap.Y)); 
                }
            }
        }

        public void AddLayer(int[,] tilemapTemplate, in float layerDepth)
        {
            Tile[,] tilemap = new Tile[tilemapTemplate.GetLength(0), tilemapTemplate.GetLength(1)];
            for (int y = 0; y < tilemap.GetLength(1); y++)
            {
                for (int x = 0; x < tilemap.GetLength(0); x++)
                {
                    tilemap[x, y] = tilePalette.tiles[tilemapTemplate[x, y]].Clone();
                }
            }
            AddLayer(new Tilemap(tilemap, layerDepth));
        }
        public void AddLayer(Tilemap tilemap)
        {
            tilemaps.Add(tilemap);
            tilemaps.Sort();
            if(tilesPosition == null)
            {
                CalculateTilesPositions();
            }
        }

        public void Update()
        {
            foreach (Tilemap tilemap in tilemaps)
            {
                for (int y = 0; y < tilemap.tileMap.GetLength(1); y++)
                {
                    for (int x = 0; x < tilemap.tileMap.GetLength(0); x++)
                    {
                        tilemap.tileMap[x, y].Update?.Invoke(tilemap.tileMap[x, y]);
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for(int i = 0; i < tilemaps.Count; i++)
            {
                Tilemap tilemap = tilemaps[i];
                for (int y = 0; y < tilemap.tileMap.GetLength(1); y++)
                {
                    for (int x = 0; x < tilemap.tileMap.GetLength(0); x++)
                    {
                        Tile t = tilemap.tileMap[x, y];
                        spriteBatch.Draw(t.currentAnimation.texture, tilesPosition[x, y] - Camera.mainCamera.position, t.currentAnimation.source, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, MathHelper.Lerp(layerDepthMin, layerDepthMax, i / (tilemaps.Count - 1)));
                    }
                }
            }
        }
    }
}
