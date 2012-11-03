runasu
======

A Windows Programm to start a Command with different User Profiles

Based on the CodeProject Articles:

The RunAs Class
http://www.codeproject.com/Articles/7168/RunAs-Class

The ProcessStarter Class
http://www.codeproject.com/Articles/36581/Interaction-between-services-and-applications-at-u



Usage: 
------

	runasu.exe /user:<username> /pass:<password> /domain:<domain> /command:<execute command>

All Parameter are optional!

Parameter:
----------
    
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
 