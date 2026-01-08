using Zenject;

namespace Spyke.Features.Tutorial
{
    /// <summary>
    /// Zenject installer for Tutorial feature bindings.
    /// </summary>
    public class TutorialInstaller : Installer<TutorialInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<TutorialModel>().AsSingle();
            Container.BindInterfacesAndSelfTo<TutorialController>().AsSingle();
        }
    }

    /// <summary>
    /// MonoInstaller for scene-based tutorial setup.
    /// </summary>
    public class TutorialMonoInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            TutorialInstaller.Install(Container);
        }
    }
}
