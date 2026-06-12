using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D;

public class Minimap
{
    private const int DEFAULT_OBJECT_SCALE = 16;
    private const float DELAY_RESET = 5f;
    private const float DELAY_DECREMENT = 0.1f;
    private const int VIEWPORT_HALF_SIZE = 160;
    private const int VIEWPORT_SIZE = 336;
    private const int DRAW_OFFSET = 128;

    public List<MinimapSquare> Squares { get; } = [];
    public bool DrawTextures { get; set; } = true;
    public int objectScale = DEFAULT_OBJECT_SCALE;

    private float _delay = DELAY_RESET;

    public void Initialize()
    {
        Squares.Clear();

        bool hasCeiling = false;
        float playerY = Screen.Camera!.Position.Y;
        Rectangle playerR = new Rectangle((int)Screen.Camera.Position.X, (int)Screen.Camera.Position.Z, 1, 1);

        List<Entity> eList = [];
        List<Entity> fList = [];
        List<Entity> fullList = [];

        foreach (Entity newE in Screen.Level!.Entities)
        {
            eList.Add(newE);
            fullList.Add(newE);
        }
        foreach (Entity newF in Screen.Level.Floors)
        {
            fList.Add(newF);
            fullList.Add(newF);
        }
        foreach (Entity newE in Screen.Level.OffsetmapEntities)
        {
            eList.Add(newE);
            fullList.Add(newE);
        }
        foreach (Entity newF in Screen.Level.OffsetmapFloors)
        {
            fList.Add(newF);
            fullList.Add(newF);
        }

        foreach (Entity e in fullList)
        {
            if (e.Position.Y > playerY)
            {
                Rectangle entityR = new Rectangle((int)e.Position.X, (int)e.Position.Z, 1, 1);
                if (entityR.Intersects(playerR) == true)
                {
                    hasCeiling = true;
                    break;
                }
            }
        }

        foreach (Entity e in fList)
        {
            if (e.Visible == false) continue;

            if (e.Position.Y >= Screen.Camera.Position.Y + 1.5f)
            {
                if (hasCeiling == true) continue;
            }
            Squares.Add(new MinimapSquare(
                new Rectangle(
                    (int)(e.Position.X * objectScale),
                    (int)(e.Position.Z * objectScale),
                    (int)(e.Scale.X * objectScale),
                    (int)(e.Scale.Z * objectScale)),
                Color.Black,
                e.Textures[0],
                DrawTextures));
        }

        foreach (Entity e in eList)
        {
            if (e.Position.Y >= Screen.Camera.Position.Y + 1.5f)
            {
                if (hasCeiling == true) continue;
            }

            switch (e.EntityID.ToLower())
            {
                case "networkplayer":
                    Squares.Add(new MinimapSquare(
                        new Rectangle(
                            (int)(e.Position.X * objectScale),
                            (int)(e.Position.Z * objectScale),
                            (int)(e.Scale.X * objectScale),
                            (int)(e.Scale.Z * objectScale)),
                        Color.Black,
                        e.Textures[0],
                        DrawTextures));
                    break;
                case "networkpokemon":
                {
                    Texture2D t = ((NetworkPokemon)e).Textures[0];
                    Squares.Add(new MinimapSquare(
                        new Rectangle(
                            (int)(e.Position.X * objectScale),
                            (int)(e.Position.Z * objectScale),
                            (int)(e.Scale.X * objectScale),
                            (int)(e.Scale.Z * objectScale)),
                        Color.Black,
                        t,
                        DrawTextures));
                    break;
                }
                default:
                {
                    if (e.Visible == false) break;

                    Texture2D? t = GetTextureFromEntity(e);
                    if (t != null)
                    {
                        Vector2 sO = GetScaleOffset(e.Scale);
                        Squares.Add(new MinimapSquare(
                            new Rectangle(
                                (int)((e.Position.X + sO.X) * objectScale),
                                (int)((e.Position.Z + sO.Y) * objectScale),
                                (int)(e.Scale.X * objectScale),
                                (int)(e.Scale.Z * objectScale)),
                            Color.Black,
                            t,
                            DrawTextures));
                    }
                    break;
                }
            }
        }
    }

    private static Vector2 GetScaleOffset(Vector3 v)
    {
        float scaleX = (v.X - 1) / 2f;
        float scaleY = (v.Z - 1) / 2f;
        return new Vector2(scaleX * -1f, scaleY * -1f);
    }

    private static Texture2D? GetTextureFromEntity(Entity e)
    {
        if (e.Textures == null) return TextureManager.DefaultTexture;
        if (e.Textures.Count() > 0 && e.EntityID.Equals(String.Empty) == false)
        {
            int i = 0;

            switch (e.BaseModel.ID)
            {
                case 0:  i = 0; break;
                case 1:  i = Extensions.Clamp(8, 0, e.TextureIndex.Length - 1); break;
                case 2:  i = Extensions.Clamp(4, 0, e.TextureIndex.Length - 1); break;
                case 3:  i = 0; break;
                case 4:  i = Extensions.Clamp(2, 0, e.TextureIndex.Length - 1); break;
                case 5:  i = Extensions.Clamp(4, 0, e.TextureIndex.Length - 1); break;
                case 6:  i = Extensions.Clamp(4, 0, e.TextureIndex.Length - 1); break;
                case 7:  i = Extensions.Clamp(4, 0, e.TextureIndex.Length - 1); break;
                case 8:  i = Extensions.Clamp(4, 0, e.TextureIndex.Length - 1); break;
                case 9:  i = 0; break;
                case 10: i = 0; break;
                case 11: i = 0; break;
                case 12: i = Extensions.Clamp(8, 0, e.TextureIndex.Length - 1); break;
                default: i = 0; break;
            }

            if (e.TextureIndex[i] < 0) return null;

            return e.Textures[e.TextureIndex[i]];
        }
        else
        {
            return TextureManager.DefaultTexture;
        }
    }

    public void Draw(Vector2 drawOffset)
    {
        if (_delay > 0f)
        {
            _delay -= DELAY_DECREMENT;

            if (_delay <= 0f)
            {
                _delay = DELAY_RESET;
                Initialize();
            }
        }

        Rectangle conRec = new Rectangle(
            (int)(Screen.Camera!.Position.X * objectScale - VIEWPORT_HALF_SIZE),
            (int)(Screen.Camera.Position.Z * objectScale - VIEWPORT_HALF_SIZE),
            VIEWPORT_SIZE,
            VIEWPORT_SIZE);
        Vector2 offset = new Vector2(
            Screen.Camera.Position.X * objectScale - DRAW_OFFSET,
            Screen.Camera.Position.Z * objectScale - DRAW_OFFSET);

        offset += drawOffset;

        foreach (MinimapSquare sq in Squares)
        {
            if (conRec.Intersects(sq.r) == true)
            {
                sq.Draw(DrawTextures, offset);
            }
        }
    }
}

public class MinimapSquare
{
    public Rectangle r;
    public Color c;
    public Texture2D t;

    public MinimapSquare(Rectangle r, Color c, Texture2D t, bool drawTextures)
    {
        this.r = r;
        this.t = t;

        if (drawTextures == false)
        {
            int colorR = 0;
            int colorG = 0;
            int colorB = 0;

            Color[] cs = new Color[t.Width * t.Height];
            t.GetData(cs);
            int pixelCount = t.Width * t.Height;

            for (int i = 0; i < cs.Length; i++)
            {
                colorR += cs[i].R;
                colorG += cs[i].G;
                colorB += cs[i].B;
            }

            this.c = new Color(new Vector3(
                (int)((colorR / pixelCount) / 255),
                (int)((colorG / pixelCount) / 255),
                (int)((colorB / pixelCount) / 255)));
        }
        else
        {
            this.c = c;
        }
    }

    public void Draw(bool drawTextures, Vector2 offset)
    {
        if (drawTextures == true)
        {
            Core.SpriteBatch.Draw(t, new Rectangle(
                (int)(r.X - offset.X),
                (int)(r.Y - offset.Y),
                r.Width,
                r.Height),
                Color.White);
        }
        else
        {
            Canvas.DrawRectangle(new Rectangle(
                (int)(r.X - offset.X),
                (int)(r.Y - offset.Y),
                r.Width,
                r.Height),
                c);
        }
    }
}
