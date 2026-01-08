using Zenject;

namespace Spyke.Features.Inbox
{
    /// <summary>
    /// Zenject installer for Inbox feature bindings.
    /// </summary>
    public class InboxInstaller : Installer<InboxInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<InboxModel>().AsSingle();
            Container.BindInterfacesAndSelfTo<InboxController>().AsSingle();
        }
    }

    /// <summary>
    /// MonoInstaller for scene-based inbox setup.
    /// </summary>
    public class InboxMonoInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            InboxInstaller.Install(Container);
        }
    }
}
