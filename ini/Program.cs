using AyrA.IO;
using System;
using System.Linq;
using static System.Console;

namespace ini
{
    class Program
    {
        private enum Mode
        {
            None, Get, Set, List
        }
        private struct Args
        {
            public bool Valid;
            public Mode Mode;
            public string FileName;
            public string Section;
            public string Key;
            public string Value;
        }
        static void Main(string[] args)
        {
            if (args.Contains("/?"))
            {
                Help();
            }
            else
            {
                var A = ParseArgs(args);
                if (A.Valid)
                {
                    switch (A.Mode)
                    {
                        case Mode.Get:
                            GetValue(A.FileName, A.Section, A.Key);
                            break;
                        case Mode.List:
                            ListValues(A.FileName, A.Section);
                            break;
                        case Mode.Set:
                            SetValue(A.FileName, A.Section, A.Key, A.Value);
                            break;
                        default:
                            throw new NotImplementedException($"Unsupported mode: {A.Mode}");

                    }
                }
                else
                {
                    Help();
                }
            }
        }

        private static void GetValue(string FileName, string Section, string Key)
        {
            WriteLine(INI.getSetting(FileName, Section, Key));
        }

        private static void SetValue(string FileName, string Section, string Key, string Value)
        {
            if (Value == null)
            {
                var Sections = INI.getSections(FileName);
                if(Sections.Contains(Section))
                {
                    var Sec = INI.completeINI(FileName).First(m => m.Section == Section);
                    Sec.Settings.Remove(Key);
                    if (Sec.Settings.Count > 0)
                    {
                        INI.Delete(FileName, Section, Key);
                    }
                    else
                    {
                        INI.Delete(FileName, Section);
                    }
                }
            }
            else
            {
                INI.Save(FileName, Section, Key, Value, false);
            }
        }

        private static void ListValues(string FileName, string Section)
        {
            var Sections = INI.completeINI(FileName);
            if (string.IsNullOrEmpty(Section))
            {
                foreach(var S in Sections)
                {
                    foreach(var E in S.Settings.AllKeys)
                    {
                        WriteLine("{0}.{1}={2}", S.Section, E, S.Settings[E]);
                    }
                }
            }
            else
            {
                var S = Sections.FirstOrDefault(m => m.Section == Section);
                if (S != null)
                {
                    foreach (var E in S.Settings.AllKeys)
                    {
                        WriteLine("{0}={1}", E, S.Settings[E]);
                    }
                }
            }
        }

        private static Args ParseArgs(string[] args)
        {
            var A = new Args();
            A.Valid = true;
            foreach (var Arg in args)
            {
                switch (Arg.ToUpper())
                {
                    case "/G":
                        if (A.Mode == Mode.None)
                        {
                            A.Mode = Mode.Get;
                        }
                        else
                        {
                            A.Valid = false;
                            Error.WriteLine("Mode already set to 'Get' before");
                        }
                        break;
                    case "/S":
                        if (A.Mode == Mode.None)
                        {
                            A.Mode = Mode.Set;
                        }
                        else
                        {
                            A.Valid = false;
                            Error.WriteLine("Mode already set to 'Set' before");
                        }
                        break;
                    case "/L":
                        if (A.Mode == Mode.None)
                        {
                            A.Mode = Mode.List;
                        }
                        else
                        {
                            A.Valid = false;
                            Error.WriteLine("Mode already set to 'List' before");
                        }
                        break;
                    default:
                        if (string.IsNullOrEmpty(A.FileName))
                        {
                            A.FileName = Arg;
                        }
                        else if (string.IsNullOrEmpty(A.Section))
                        {
                            A.Section = Arg;
                        }
                        else if (string.IsNullOrEmpty(A.Key))
                        {
                            A.Key = Arg;
                        }
                        else if (string.IsNullOrEmpty(A.Value))
                        {
                            A.Value = Arg;
                        }
                        else
                        {
                            Error.WriteLine("Too many arguments, starting at '{0}'", Arg);
                            A.Valid = false;
                        }
                        break;
                }
                if (!A.Valid)
                {
                    return A;
                }
            }
            if (A.Mode == Mode.None)
            {
                Error.WriteLine("No mode selected");
                A.Valid = false;
            }
            else if (A.Mode == Mode.Set)
            {
                A.Valid = !string.IsNullOrEmpty(A.Section) && !string.IsNullOrEmpty(A.Key);
                if (!A.Valid)
                {
                    Error.WriteLine("'Set' requires 'Section' and 'Key' argument");
                }
            }
            else if (A.Mode == Mode.Get)
            {
                A.Valid = !string.IsNullOrEmpty(A.Section) && !string.IsNullOrEmpty(A.Key);
                if(!A.Valid)
                {
                    Error.WriteLine("'Get' requires 'Section' and 'Key' argument");
                }
            }
            else if (A.Mode == Mode.List)
            {
                A.Valid = string.IsNullOrEmpty(A.Key);
                if (!A.Valid)
                {
                    Error.WriteLine("'List' must not be supplied with 'Key' argument");
                }
            }


            return A;
        }

        private static void Help()
        {
            Error.WriteLine(@"ini /G|/S|/L <Filename> [Section [Key [Value]]]
gets, sets or lists ini values

/G        - Get the value of the given 'key' in the given 'Section'
/S        - Set the value of the given 'Key' in the given 'Section' to 'Value'
/L        - Lists all ini values in 'Section.Key=Value' format

The mode should be specified first

Filename  - File to read/write. Created in 'set' mode if it doesn't exists.
            Must be specified before the ini Section/Key/Value argument

Section   - INI Section
            get:  (required) Section to read
            set:  (required) Section to search key in
            list: (optional) Section to list values of

Key       - Key of the section
            get:  (required) key value to read
            set:  (required) key value to set
            list: Not supported

Value     - Value to write to the given key
            get:  Not supported
            set:  (optional) Value to write to the key. If missing, the key is deleted
            list: Not supported

A section is deleted if deleting a key results in it being empty.");
        }
    }
}
