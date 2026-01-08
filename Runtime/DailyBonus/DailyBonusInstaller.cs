using Zenject;

namespace Spyke.Features.DailyBonus
{
    /// <summary>
    /// Zenject installer for DailyBonus feature bindings.
    /// </summary>
    public class DailyBonusInstaller : Installer<DailyBonusInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<DailyBonusModel>().AsSingle();
            Container.BindInterfacesAndSelfTo<DailyBonusController>().AsSingle();
        }
    }

    /// <summary>
    /// MonoInstaller for scene-based daily bonus setup.
    /// </summary>
    public class DailyBonusMonoInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            DailyBonusInstaller.Install(Container);
        }
    }
}
