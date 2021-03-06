﻿using System;
using System.Text;
using System.Threading;

namespace Lobstermania
{
    public static class Program
    {

        // METHODS

        // IMPORTANT NUMBERS
        // -----------------
        // Theoretical _spins per JACKPOT 8,107,500
        // Number of all slot combinations (47x46x48x50x50 -- 1 slot per reel) = 259,440,000
        // Number of all possible symbol combinations on a line (11x11x11x10x10) = 133,100

        private static int GetIntValue(string prompt, int min, int max, Action onError = null)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();
                if (int.TryParse(input, out int value) && min <= value && value <= max)
                    return value;
                onError?.Invoke();
            }
        }

        private static long GetLongValue(string prompt, long min, long max, Action onError = null)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();
                if (long.TryParse(input, out long value) && min <= value && value <= max)
                    return value;
                onError?.Invoke();
            }
        }

        private static void RunBulkGameSpins()
        {
            var numSpins = GetLongValue("Enter the number of spins: ",
                                        min: 1, max: long.MaxValue,
                                        () => Console.Error.WriteLine("***ERROR: Please enter a positive number greater than 0 !!!"));

            var paylines = GetIntValue("Enter the number of active paylines (1 through 15): ",
                                       min: 1, max: 15,
                                       () => Console.Error.WriteLine("***ERROR: Please enter a positive number between 1 and 15 !!!"));

            Console.Clear();
            BulkGameSpins(numSpins, paylines);
            Console.WriteLine("Press any key to continue to Main Menu ...");
            Console.ReadKey();
        }

        private static string BuildMenuString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("PRESS:");
            sb.AppendLine();
            sb.AppendLine("\t1 for Bulk Game Spins");
            sb.AppendLine("\t2 for Individual Games");
            sb.AppendLine("\t3 for Absolute Game Metrics");
            sb.AppendLine("\t4 to quit");
            sb.AppendLine();
            sb.Append("Your choice: ");
            return sb.ToString();
        }

        public static void Main()
        {
            var menu = BuildMenuString();
            int num;
            do
            {
                Console.Clear();

                num = GetIntValue(prompt: menu,
                                  min: 1, max: 4,
                                  onError: () =>
                                  {
                                      Console.WriteLine("***ERROR: Please enter number 1, 2, 3, or 4 !!!");
                                      Thread.Sleep(1500);
                                      Console.Clear();
                                  });
                Console.Clear();
                switch (num)
                {
                    case 1:
                        RunBulkGameSpins();
                        break;

                    case 2:
                        IndividualGames();
                        break;
                    case 3:
                        TestMetrics();
                        break;

                }
            } while (num != 4);
        }

        private static void TestMetrics()
        {
            int count = 1;
            do
            {

                LM962 game = new LM962()
                {
                    // All properties relevant to LM962.TestMetrics() -- set to default values
                    PrintReels = false,
                    PrintPaylines = false, // will write to a text file paylines.txt if true
                    UseRandomReelLayout = false // if true, use fixed reels with 96.25% payback, 5.15% hit frequency
                };

                Console.WriteLine("\nRunning Session {0} game metrics ...\n", count);
                game.TestMetrics();
                count++;
            
            } while (count <= 1);

            Console.Write("\nPress Enter to continue to Main Menu...");
            Console.ReadLine();

        } // End method TestMetrics

        private static void BulkGameSpins(long numSpins, int numPayLines)
        {
            LM962 game = new LM962()
            {
                ActivePaylines = numPayLines
            };

            DateTime start_t = DateTime.Now;

            Console.WriteLine("\nPlaying {0} active paylines.\n", game.ActivePaylines);
            Console.WriteLine("Progress Bar ({0:N0} spins)\n", numSpins);
            Console.WriteLine("10%       100%");
            Console.WriteLine("|---+----|");

            if (numSpins <= 10)
            {
                Console.WriteLine("**********"); // 10 markers
                for (long i = 1; i <= numSpins; i++)
                    game.Spin();
            }
            else
            {
                int marks = 1; // Number of printed marks
                long markerEvery = (long)Math.Ceiling((double)numSpins / (double)10); // print progression marker at every 1/10th of total _spins.

                for (long i = 1; i <= numSpins; i++)
                {
                    game.Spin();
                    if ((i % markerEvery == 0))
                    {
                        Console.Write("*");
                        marks++;
                    }
                }

                for (int i = marks; i <= 10; i++)
                    Console.Write("*");
            }

            Console.WriteLine();
            game.Stats.DisplaySessionStats(numPayLines);

            DateTime end_t = DateTime.Now;
            TimeSpan runtime = end_t - start_t;
            Console.WriteLine("\nRun completed in {0:t}\n", runtime);

        } // End method BulkGameSpins

        private static void IndividualGames()
        {
            LM962 game = new LM962
            {
                PrintGameboard = true,
                PrintPaylines = true
            };

            while (true)
            {
                Console.Clear();
                Console.WriteLine("\nPlaying {0} active paylines.\n", game.ActivePaylines);

                game.Spin();
                game.Stats.DisplayGameStats();
                game.Stats.ResetGameStats();

                Console.WriteLine("Press the P key to change the number of pay lines");
                Console.WriteLine("Press the Escape key to return to the Main Menu");
                Console.WriteLine("\nPress any other key to continue playing.");
                ConsoleKeyInfo cki = Console.ReadKey(true);

                if (cki.KeyChar == 'p' || cki.KeyChar == 'P')
                {
                    game.ActivePaylines = GetIntValue("\nEnter the new number of active paylines (1 through 15): ",
                                                      1, 15,
                                                      () => Console.WriteLine("\n***ERROR: Please enter a positive number between 1 and 15 !!!"));
                }

                if (cki.Key == ConsoleKey.Escape)
                    break;
            }
        }

    } // End class Program

} // end namespace
