using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AF_Lib.IO.Associations;

public class AFFileAssociations_Example
{
    public static void Main()
    {
        // Initializes a new AF_FileAssociator to associate the .ABC file extension.
        AF_FileAssociator assoc = new AF_FileAssociator(".abc");

        // Creates a new file association for the .ABC file extension. Data is overwritten if it already exists.
        assoc.Create("My_App",
            "My application's file association",
            new ProgramIcon(@"C:\Program Files\My_App\icon.ico"),
            new ExecApplication(@"C:\Program Files\My_App\myapp.exe"),
            new OpenWithList(new string[] { "My_App" }));

        // Gets each piece of association info individually, all as strings.
        string id = assoc.ID;
        string description = assoc.Description;
        string icon = assoc.DefaultIcon.IconPath;
        string execApp = assoc.Executable.Path;
        string[] openWithList = assoc.OpenWith.List;

        // Sets each peice of association info individually.
        ProgramIcon newDefIcon = new ProgramIcon(@"C:\Program Files\My_App\icon2.ico");
        ExecApplication newExecApp = new ExecApplication(@"C:\Program Files\My_App\myapp2.exe");
        OpenWithList newOpenWith = new OpenWithList(new string[] { "myapp2.exe" });

        assoc.ID = "My_App_2";
        assoc.Description = "My application's file association #2";
        assoc.DefaultIcon = newDefIcon;
        assoc.Executable = newExecApp;
        assoc.OpenWith = newOpenWith;

        // Gets the extension of the associator that was set when initializing it.
        string extension = assoc.Extension;

        // Deletes any keys that were associated with the .ABC file extension.
        assoc.Delete();
    }
}