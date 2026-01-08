using Zenject;

namespace Spyke.Features.Chest
{
    /// <summary>
    /// Zenject installer for Chest feature bindings.
    /// </summary>
    public class ChestInstaller : Installer<ChestInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<ChestController>().AsSingle();
        }
    }

    /// <summary>
    /// MonoInstaller for scene-based chest setup.
    /// </summary>
    public class ChestMonoInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            ChestInstaller.Install(Container);
        }
    }
}
