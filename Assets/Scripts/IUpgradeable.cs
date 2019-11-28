public interface IUpgradeable
{
    bool CanUpgrade();
    bool HasMaxUpgrade();
    void Upgrade();
}