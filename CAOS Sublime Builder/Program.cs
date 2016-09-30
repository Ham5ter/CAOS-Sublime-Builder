using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CAOS;

namespace CAOS_Sublime_Builder
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("You must provide a cos File as the first comandline parameter!");
                return;
            }
            CaosInjector CI;
            try
            {
                CI = new CaosInjector("Docking Station");
            }
            catch (Exception)
            {
                try
                {
                    CI = new CaosInjector("Creatures 3");
                }
                catch (Exception)
                {
                    Console.WriteLine("CaosInjector could not be initialized! is the Game started?");
                    return;
                }
            }

            int ParseSwitch = 1;
            // 0 SCRIPT
            // 1 Normal CAOS Code
            // 2 Remove Script
            List<string>[] CAOS = new List<string>[3];
            for (int i = 0; i < 3; i++)
            {
                CAOS[i] = new List<string>();
            }

            foreach (string Line in File.ReadAllLines(args[0]))
            {
                if (Line.StartsWith("*") || Line.Trim() == "")
                {
                    // Ignore Comments and empty Lines!
                }
                else if (Line.ToLower().Trim().StartsWith("scrp"))
                {
                    ParseSwitch = 0;
                    CAOS[ParseSwitch].Add(Line);
                }
                else if (Line.ToLower().Trim().StartsWith("endm"))
                {
                    CAOS[ParseSwitch].Add(Line);
                    ParseSwitch = 1; 
                }
                else if (Line.ToLower().Trim().StartsWith("rscr"))
                {
                    ParseSwitch = 2;
                }
                else
                {
                    CAOS[ParseSwitch].Add(Line);
                }                
            }
            List<string[]> Scripts = new List<string[]>();
            string[] Script = new string[2];
            foreach (string Line in CAOS[0])
            {

                if (Line.ToLower().Trim().StartsWith("scrp"))
                {
                    Script = new string[2];
                    Script[0] = Line;
                }
                else if (Line.ToLower().Trim().StartsWith("endm"))
                {
                    Scripts.Add(Script);
                }
                else
                {
                    Script[1] =  Script[1] + " " + Line;
                }
            }

            CaosResult Result;
            Result = CI.ExecuteCaosGetResult(string.Join(" ", CAOS[2]));
            if (Result.Failed)
            {
                Console.WriteLine(Result.Content);
                return;
            }
            foreach (string[] item in Scripts)
            {
                Result = CI.ExecuteCaosGetResult(item[1],item[0]);
                if (Result.Failed)
                {
                    Console.WriteLine(Result.Content);
                    break;
                }  
            }
            Result = CI.ExecuteCaosGetResult(string.Join(" ", CAOS[1]));
            if (Result.Failed)
            {
                Console.WriteLine(Result.Content);
            }
        }
    }
}
