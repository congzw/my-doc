namespace NbApp.Srvs.MyCli.Exts
{
    public class MyCliGit
    {
        public static MyCliGit Instance = new MyCliGit();
    }

    public static class MyCliGitExtensions
    {
        public static MyCliCmd GetVersion(this MyCliGit myCli)
        {
            var myCmd = MyCliCmdBuilder.Create("git")
             .WithArguments("--version").Build();
            return myCmd;
        }
    }
}