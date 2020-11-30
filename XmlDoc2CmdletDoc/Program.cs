using System;
using System.Collections.Generic;
using System.Linq;
using XmlDoc2CmdletDoc.Core;

namespace XmlDoc2CmdletDoc
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var options = ParseArguments(args);
            Console.WriteLine(options);
            var engine = new Engine();
            var exitCode = engine.GenerateHelp(options);
            Console.WriteLine("GenerateHelp completed with exit code '{0}'", exitCode);
            Environment.Exit((int)exitCode);
        }

        private static Options ParseArguments(IReadOnlyList<string> args)
        {
            const string strictSwitch = "-strict";
            const string excludeParameterSetSwitch = "-excludeParameterSets";
            const string rootUrlSwitch = "-rootUrl";
            const string outputHelpFilePathSwitch = "-out";

            try
            {
                var treatWarningsAsErrors = false;
                var excludedParameterSets = new List<string>();
                string assemblyPath = null;
                string rootUrl = null;
                string outputHelpFilePath = null;

                for (var i = 0; i < args.Count; i++)
                {
                    switch (args[i])
                    {
                        case strictSwitch:
                            treatWarningsAsErrors = true;
                            break;
                        case excludeParameterSetSwitch:
                        {
                            i++;
                            if (i >= args.Count) throw new ArgumentException();
                            excludedParameterSets.AddRange(args[i].Split(new [] {','}, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()));
                            break;
                        }
                        case rootUrlSwitch:
                        {
                            rootUrl = args[++i];
                            break;
                        }
                        case outputHelpFilePathSwitch:
                        {
                            outputHelpFilePath = args[++i];
                            break;
                        }
                        default:
                        {
                            if (assemblyPath == null)
                            {
                                assemblyPath = args[i];
                            }
                            else
                            {
                                throw new ArgumentException();
                            }

                            break;
                        }
                    }
                }

                if (assemblyPath == null)
                {
                    throw new ArgumentException();
                }

                return new Options(
                                   treatWarningsAsErrors,
                                   assemblyPath,
                                   excludedParameterSets.Contains,
                                   outputHelpFilePath,
                                   null,
                                   rootUrl);
            }
            catch (ArgumentException)
            {
                Console.Error.WriteLine($"Usage: XmlDoc2CmdletDoc.exe [{strictSwitch}] [{excludeParameterSetSwitch} parameterSetToExclude1,parameterSetToExclude2] assemblyPath");
                Environment.Exit(-1);
                throw;
            }
        }
    }
}
