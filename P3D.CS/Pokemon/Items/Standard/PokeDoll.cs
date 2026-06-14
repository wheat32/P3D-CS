using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(112, "Poké Doll")]
public class PokéDoll : Item
{
    public override String Description { get; protected set; } = "Helps flee from a battle.";
    public override bool CanBeUsedInBattle { get; } = true;

    public PokéDoll()
    {
        _textureRectangle = new Rectangle(0, 120, 24, 24);
    }

    public override bool UseOnPokemon(int pokeIndex)
    {
        Screen s = Core.CurrentScreen;
        while (s.Identification != Screen.Identifications.BattleScreen && s.PreScreen != null)
            s = s.PreScreen;
        if (s.Identification != Screen.Identifications.BattleScreen)
            return false;
        BattleSystem.BattleScreen bs = (BattleSystem.BattleScreen)s;
        if (bs.BattleMode != BattleSystem.BattleScreen.BattleModes.Standard &&
            bs.BattleMode != BattleSystem.BattleScreen.BattleModes.BugContest)
            return false;
        bs.BattleQuery.Clear();
        bs.BattleQuery.Insert(0, new BattleSystem.ToggleMenuQueryObject(true));
        bs.BattleQuery.Add(bs.FocusOwnPlayer());
        bs.BattleQuery.Add(new BattleSystem.PlaySoundQueryObject(@"Battle\running", false));
        bs.BattleQuery.Add(new BattleSystem.TextQueryObject("Got away safely!"));
        bs.BattleQuery.Add(new BattleSystem.EndBattleQueryObject(false));
        Battle.Won = true;
        Battle.Fled = true;
        RemoveItem();
        return true;
    }
}
