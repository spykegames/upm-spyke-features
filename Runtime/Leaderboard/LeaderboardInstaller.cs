using Zenject;

namespace Spyke.Features.Leaderboard
{
    /// <summary>
    /// Zenject installer for Leaderboard feature bindings.
    /// </summary>
    public class LeaderboardInstaller : Installer<LeaderboardInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<LeaderboardModel>().AsSingle();
            Container.BindInterfacesAndSelfTo<LeaderboardController>().AsSingle();
        }
    }

    /// <summary>
    /// MonoInstaller for scene-based leaderboard setup.
    /// </summary>
    public class LeaderboardMonoInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            LeaderboardInstaller.Install(Container);
        }
    }
}
