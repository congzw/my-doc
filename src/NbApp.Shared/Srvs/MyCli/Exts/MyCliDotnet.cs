namespace NbApp.Srvs.MyCli.Exts
{
    public class MyCliDotnet
    {
        public static MyCliDotnet Instance = new MyCliDotnet();
    }

    public static class MyCliDotnetExtensions
    {
        public static MyCliCmd GetVersion(this MyCliDotnet myCli)
        {
            var myCmd = MyCliCmdBuilder.Create("dotnet")
             .WithArguments("--version").Build();
            return myCmd;
        }
        public static MyCliCmd GetInfo(this MyCliDotnet myCli)
        {
            var myCmd = MyCliCmdBuilder.Create("dotnet")
             .WithArguments("--info").Build();
            return myCmd;
        }

        public static MyCliCmd GetSdks(this MyCliDotnet myCli)
        {
            var myCmd = MyCliCmdBuilder.Create("dotnet")
             .WithArguments("--list-sdks").Build();
            return myCmd;
        }

        public static MyCliCmd GetRuntimes(this MyCliDotnet myCli)
        {
            var myCmd = MyCliCmdBuilder.Create("dotnet")
             .WithArguments("--list-runtimes").Build();
            return myCmd;
        }
    }
}