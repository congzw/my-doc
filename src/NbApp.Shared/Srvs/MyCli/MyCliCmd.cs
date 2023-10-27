using CliWrap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NbApp.Srvs.MyCli
{
    public class MyCliCmdBuilder
    {
        internal MyCliCmd TempCmd { get; set; } = new MyCliCmd();

        public static MyCliCmdBuilder Create(string target)
        {
            if (string.IsNullOrWhiteSpace(target))
            {
                throw new ArgumentException($"“{nameof(target)}”不能为 null 或空白。", nameof(target));
            }

            var builder = new MyCliCmdBuilder();
            builder.TempCmd.Target = target;
            return builder;
        }

        public MyCliCmd Build()
        {
            return this.TempCmd;
        }

        public MyCliCmdBuilder WithWorkDirectory(string workDirectory)
        {
            if (string.IsNullOrWhiteSpace(workDirectory))
            {
                return this;
            }
            TempCmd.WorkDirectory = workDirectory;
            return this;
        }

        public MyCliCmdBuilder WithArguments(params string[] arguments)
        {
            foreach (var argument in arguments)
            {
                if (!string.IsNullOrWhiteSpace(argument))
                {
                    TempCmd.Arguments.Add(argument.Trim());
                }
            }
            return this;
        }
    }

    public class MyCliCmd
    {
        public string Target { get; set; }
        public string WorkDirectory { get; set; } = "";
        public List<string> Arguments { get; set; } = new List<string>();
        public string GetArgumentsValue()
        {
            return string.Join(' ', Arguments);
        }

        public static string[] ParseToArgumentsArray(string argumentsValue, char separator)
        {
            if (string.IsNullOrWhiteSpace(argumentsValue))
            {
                return Array.Empty<string>();
            }
            var arguments = argumentsValue.Split(separator).Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToArray();
            return arguments;
        }
    }

    public class MyCliCmdResult
    {
        public int ExitCode { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset ExitTime { get; set; }
        public TimeSpan RunTime => ExitTime - StartTime;

        public string GetStandardOutput() => StandardOutputBuffer?.ToString();
        public string GetStandardError() => StandardErrorBuffer?.ToString();

        internal StringBuilder StandardOutputBuffer { get; set; }
        internal StringBuilder StandardErrorBuffer { get; set; }
    }

    public class MyCliExecuteContext
    {
        public MyCliCmd Cmd { get; set; }
        public int ProcessId { get; set; }
        public MyCliCmdResult CmdResult { get; set; }
    }

    public static class MyCliCmdExtensions
    {
        public static async Task<MyCliExecuteContext> Execute(this MyCliCmd myCmd, 
            bool awaitReturn, 
            bool withValidation = false,
            CancellationToken cancellationToken = default)
        {
            var stdOutBuffer = new StringBuilder();
            var stdErrBuffer = new StringBuilder();
            var command =  myCmd.CreateCommand(withValidation, stdOutBuffer, stdErrBuffer);

            ////应对相对复杂的场景，则直接使用内部的command进行操作
            //// Timeout and cancellation
            //using var cts = new CancellationTokenSource();
            //cts.CancelAfter(TimeSpan.FromSeconds(10));
            //try
            //{
            //    var resultTask = command.ExecuteAsync(cts);
            //}
            //catch (OperationCanceledException)
            //{
            //    // Command was canceled
            //}

            var resultTask = command.ExecuteAsync(cancellationToken);
            var ctx = new MyCliExecuteContext();
            ctx.Cmd = myCmd;
            ctx.ProcessId = resultTask.ProcessId;

            if (!awaitReturn)
            {
                return ctx;
            }

            var commandResult = await resultTask;
            var myCmdResult = new MyCliCmdResult();
            myCmdResult.ExitCode = commandResult.ExitCode;
            myCmdResult.StartTime = commandResult.StartTime;
            myCmdResult.ExitTime = commandResult.ExitTime;
            myCmdResult.StandardOutputBuffer = stdOutBuffer;
            myCmdResult.StandardErrorBuffer = stdErrBuffer;
            ctx.CmdResult = myCmdResult;

            return ctx;
        }

        public static Command CreateCommand(this MyCliCmd myCmd, bool withValidation, StringBuilder stdOutBuffer = null, StringBuilder stdErrBuffer = null)
        {
            var command = Cli.Wrap(myCmd.Target)
             .WithArguments(myCmd.Arguments);
            if (!string.IsNullOrWhiteSpace(myCmd.WorkDirectory))
            {
                command.WithWorkingDirectory(myCmd.WorkDirectory);
            }

            if (withValidation)
            {
                command = command.WithValidation(CommandResultValidation.ZeroExitCode);
            }
            else
            {
                command = command.WithValidation(CommandResultValidation.None);
            }

            if (stdOutBuffer != null)
            {
                command = command.WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOutBuffer));
            }

            if (stdErrBuffer != null)
            {
                command = command.WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrBuffer));
            }

            return command;
        }

        public static string[] ParseToArgumentsArray(this MyCliCmd myCmd, string argumentsValue, char separator)
        {
            return MyCliCmd.ParseToArgumentsArray(argumentsValue, separator);
        }
    }
}