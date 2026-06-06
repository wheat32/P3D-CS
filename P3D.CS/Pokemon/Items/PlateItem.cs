using Microsoft.Xna.Framework;

namespace P3D.Items;

public abstract class PlateItem : Item
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public override ItemTypes ItemType { get; } = ItemTypes.Standard;
    public override String Description { get; protected set; }
    public override int PokeDollarPrice { get; protected set; } = 1000;

    protected PlateItem(Element.Types type)
    {
        Element element = BattleSystem.GameModeElementLoader.GetElementByID((int)type);
        Description = $"An item to be held by a Pokémon. It's a stone tablet that boosts the power of {element}-type moves.";
        _textureSource = @"Items\Plates";
        _textureRectangle = GetTextureRectangle((int)type);
    }

    private Rectangle GetTextureRectangle(int type)
    {
        if (type <= 19)
        {
            List<Element.Types> typeArray =
            [
                Element.Types.Bug, Element.Types.Dark, Element.Types.Dragon, Element.Types.Electric,
                Element.Types.Fairy, Element.Types.Fighting, Element.Types.Fire, Element.Types.Flying,
                Element.Types.Ghost, Element.Types.Grass, Element.Types.Ground, Element.Types.Ice,
                Element.Types.Poison, Element.Types.Psychic, Element.Types.Rock, Element.Types.Steel,
                Element.Types.Water,
            ];
            int i = typeArray.IndexOf((Element.Types)type);
            int x = i;
            int y = 0;
            while (x > 4)
            {
                x -= 5;
                y++;
            }
            return new Rectangle(x * 24, y * 24, 24, 24);
        }
        if (IsGameModeItem == true)
        {
            return gmTextureRectangle;
        }
        return new Rectangle(0, 0, 24, 24);
    }
}
