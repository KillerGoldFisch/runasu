using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;

using VastAbyss;
using System.Diagnostics;

namespace runasu {
    class Program {
        static void Main(string[] args) {
            bool waitForProcess = false;

            RunAs runAs = new RunAs();

            runAs.UserName = "[Dektop]";
            runAs.CommandLine = "cmd.exe";
            runAs.ApplicationName = "cmd.exe";
            runAs.Domain = System.Net.Dns.GetHostName();
            runAs.CreationFlagsInstance = RunAs.CreationFlags.NewProcessGroup;

            //if (args.Length == 0) {
            //    PrintHelp();
            //    return;
            //}

            string CurrCommand = "";

            try {
                foreach (string command in args) {

                    if (command.ToLower().StartsWith("/help")) {
                        PrintHelp();
                        return;
                    }

                    CurrCommand = command;
                    string CName = GetCName(command).ToLower();
                    string CComm = GetCComm(command);

                    if (CName == "user") {
                        runAs.UserName = CComm;
                    } else if (CName == "pass") {
                        runAs.Password = CComm;
                    } else if (CName == "domain") {
                        runAs.Domain = CComm;
                    } else if (CName == "workdir") {
                        runAs.CurrentDirectory = CComm;
                    } else if (CName == "app") {
                        runAs.ApplicationName = CComm;
                    } else if (CName == "command") {
                        runAs.CommandLine = CComm;
                    } else if (CName == "creationflag") {
                        try
                        {
                            runAs.CreationFlagsInstance = (RunAs.CreationFlags)Enum.Parse(typeof(RunAs.CreationFlags), CComm);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Creation flag not found : " + CComm);
                            Console.WriteLine("Use one of this flags:");
                            foreach (string sflag in Enum.GetNames(typeof(RunAs.CreationFlags)))
                                Console.WriteLine("   " + sflag);
                            return;
                        }
                    } else if (CName == "logonflag") {
                        try
                        {
                            runAs.LogonFlagsInstance = (RunAs.LogonFlags)Enum.Parse(typeof(RunAs.LogonFlags), CComm);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Logon flag not found : " + CComm);
                            Console.WriteLine("Use one of this flags:");
                            foreach (string sflag in Enum.GetNames(typeof(RunAs.LogonFlags)))
                                Console.WriteLine("   " + sflag);
                            return;
                        }
                    } else if (CName == "wait") {
                        if (!bool.TryParse(CComm, out waitForProcess))
                        {
                            throw new Exception("invalid boolean value : \"" + CComm + "\" try \"true\" or \"false\"");
                        }
                    } else {
                        Console.WriteLine(string.Format("Unknown Parameter : {0}\nUse /help", CName));
                        return;
                    }
                }
            } catch (Exception ex) {
                Console.WriteLine("Error while processing parameter : " + CurrCommand);
                Console.WriteLine(ex.Message);
                return;
            }

            try {
                PrintOptions(runAs);
                if (runAs.UserName == "[Dektop]")
                {
                    if (Environment.UserInteractive)
                    {
                        //No Windows Service
                        Process process = Process.Start(runAs.ApplicationName, runAs.CommandLine);
                        if (waitForProcess)
                            process.WaitForExit();
                    }
                    else
                    {
                        //Windows Service
                        UserProcess.ProcessStarter ps = new UserProcess.ProcessStarter(runAs.ApplicationName,
                                                                                       runAs.ApplicationName,
                                                                                       runAs.CommandLine);
                        ps.Run();
                        if (waitForProcess)
                            ps.WaitForExit();
                    }
                }
                else
                {
                    Process process;
                    process = runAs.StartProcess();
                    if (waitForProcess)
                        process.WaitForExit();
                }

            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }

        static string GetCName(string command) {
            return command.Substring(1, command.IndexOf(':') - 1);
        }

        static string GetCComm(string command) {
            int indexOf = command.IndexOf(':');
            return command.Substring(indexOf + 1, command.Length - indexOf - 1).Replace("\"", "");
        }

        static string MakePWStars(string charcount)
        {
            string buf = "";
            for (int i = 0; i < charcount.Length; i++)
                buf += "*";
            return buf;
        }

        static bool isAdmin()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        static void PrintOptions(RunAs r)
        {
            if (r.UserName == "[Desktop]")
            {
                Console.WriteLine(string.Format(@"
Conditions:
    Application : {0}
    Command     : {1}
",
                                                r.ApplicationName,
                                                r.CommandLine));
                if (!Environment.UserInteractive)
                    Console.WriteLine("try to get Interactive User-Token");
            }
            else
            {
                Console.WriteLine(string.Format(@"
Conditions:
    Username    : {0}
    Password    : {1}
    Domain      : {2}
    Application : {3}
    Command     : {4}
    Directory   : {5}
            ",
                                                r.UserName,
                                                MakePWStars(r.Password),
                                                r.Domain,
                                                r.ApplicationName,
                                                r.CommandLine,
                                                r.CurrentDirectory
                                      ));
            }
        }


        static void PrintHelp() {
            Console.WriteLine(@"
runasu version 1.1 [2012-11-03]
-------------------------------

Usage: runasu.exe /user:<username> /pass:<password> /domain:<domain> /command:<execute command>

All Parameter are optional!

Parameter:
    
    user:   Target Username
            
            Default: [Desktop] -> If the current Process is not interactive with the Desktop (z.B. Windows Service), it will start it with the current interactive User-Token


    pass:   Password of Targetuser

            Default: Empty


    domain: Domain of Targetuser

            Default: Local Domain


    app:    File to execute

            Default: cmd.exe


    command:Execution command

            Default: cmd.exe


    workdir:Working Directory
       
            Default: Current directory


    wait:   Wait for process (true|false)

            Default: false


    creationflag:   
            Controls how the process is created. The DefaultErrorMode, NewConsole, and NewProcessGroup flags are enabled by default— even if you do not set the flag, the system will function as if it were set.

            Flags:
                Suspended           :The primary thread of the new process is created in a suspended state, and does not run until the ResumeThread function is called.
                NewConsole          :The new process has a new console, instead of inheriting the parent's console.
                NewProcessGroup     :The new process is the root process of a new process group.
                SeperateWOWVDM      :This flag is only valid starting a 16-bit Windows-based application. If set, the new process runs in a private Virtual DOS Machine (VDM). By default, all 16-bit Windows-based applications run in a single, shared VDM. 
                UnicodeEnvironment  :Indicates the format of the lpEnvironment parameter. If this flag is set, the environment block pointed to by lpEnvironment uses Unicode characters.
                DefaultErrorMode    :The new process does not inherit the error mode of the calling process.

            Defaut: NewProcessGroup


    logonflag: 
            Logon option

            Flags:
                WithProfile             :Log on, then load the user's profile in the HKEY_USERS registry key. The function returns after the profile has been loaded. Loading the profile can be time-consuming, so it is best to use this value only if you must access the information in the HKEY_CURRENT_USER registry key.
                NetworkCredentialsOnly  :Log on, but use the specified credentials on the network only. The new process uses the same token as the caller, but the system creates a new logon session within LSA, and the process uses the specified credentials as the default credentials.
            ");
        }
    }
}
