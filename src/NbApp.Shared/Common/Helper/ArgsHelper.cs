using System.Collections.Generic;
using System.Text;
using System;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace Common
{
    public class ArgsHelper
    {
        public static ArgsHelper Instance = new ArgsHelper();

        public string[] SplitCommandLine(string commandLine)
        {
            var lineArguments = commandLine.Split('\n')
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim()).ToArray();

            var items = new List<string>();
            foreach (var line in lineArguments)
            {
                items.AddRange(SplitSingleCommandLine(line));
            }
            return items.ToArray();
        }

        private string[] SplitSingleCommandLine(string commandLine)
        {
            var lineArguments = commandLine.Split('\n')
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim()).ToArray();

            var translatedArguments = new StringBuilder(commandLine);
            var escaped = false;
            for (var i = 0; i < translatedArguments.Length; i++)
            {
                if (translatedArguments[i] == '"')
                {
                    escaped = !escaped;
                }
                if (translatedArguments[i] == ' ' && !escaped)
                {
                    translatedArguments[i] = '\n';
                }
            }

            var toReturn = translatedArguments.ToString().Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < toReturn.Length; i++)
            {
                toReturn[i] = RemoveMatchingQuotes(toReturn[i]);
            }
            return toReturn;
        }

        public string RemoveMatchingQuotes(string stringToTrim)
        {
            var firstQuoteIndex = stringToTrim.IndexOf('"');
            var lastQuoteIndex = stringToTrim.LastIndexOf('"');
            while (firstQuoteIndex != lastQuoteIndex)
            {
                stringToTrim = stringToTrim.Remove(firstQuoteIndex, 1);
                stringToTrim = stringToTrim.Remove(lastQuoteIndex - 1, 1); //-1 because we've shifted the indicies left by one
                firstQuoteIndex = stringToTrim.IndexOf('"');
                lastQuoteIndex = stringToTrim.LastIndexOf('"');
            }

            return stringToTrim;
        }
    }    
}