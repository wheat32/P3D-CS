using Microsoft.Xna.Framework;

namespace P3D.Items;

public abstract class GemItem : Item
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public override ItemTypes ItemType { get; } = ItemTypes.Standard;
    public override int PokeDollarPrice { get; protected set; } = 200;

    protected GemItem(Element.Types type)
    {
        _textureSource = @"Items\Gems";
        _textureRectangle = GetTextureRectangle(type);
    }

    private static Rectangle GetTextureRectangle(Element.Types type)
    {
        List<Element.Types> typeArray =
        [
            Element.Types.Bug, Element.Types.Dark, Element.Types.Dragon, Element.Types.Electric,
            Element.Types.Fairy, Element.Types.Fighting, Element.Types.Fire, Element.Types.Flying,
            Element.Types.Ghost, Element.Types.Grass, Element.Types.Ground, Element.Types.Ice,
            Element.Types.Poison, Element.Types.Psychic, Element.Types.Rock, Element.Types.Steel,
            Element.Types.Water, Element.Types.Normal,
        ];
        int i = typeArray.IndexOf(type);
        int x = i;
        int y = 0;
        while (x > 4)
        {
            x -= 5;
            y++;
        }
        return new Rectangle(x * 24, y * 24, 24, 24);
    }
}
