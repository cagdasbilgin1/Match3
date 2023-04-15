using CollapseBlast.Manager;

namespace CollapseBlast.Abstracts
{
    public interface IBoosterAnim
    {
        void ExecuteSound();
        void ExecuteAnim(Cell boosterCell, LevelManager level);
    }
}