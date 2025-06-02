using System;
using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        while (true)
        {
            // Cancel any existing shutdown timers
            try
            {
                Process.Start("shutdown", "/a");
                Console.WriteLine("Any existing shutdown timers have been canceled.");
            }
            catch
            {
                // Ignore errors if no shutdown was scheduled
            }

            Console.WriteLine("\nEnter the number of hours until shutdown (enter 0 to keep shutdown canceled):");

            if (double.TryParse(Console.ReadLine(), out double hours))
            {
                Console.Clear();

                if (hours == 0)
                {
                    Console.WriteLine("No new shutdown scheduled.");
                }
                else if (hours > 0)
                {
                    // Convert hours to seconds (Windows shutdown command uses seconds)
                    int seconds = (int)(hours * 3600);

                    try
                    {
                        // Schedule shutdown
                        Process.Start("shutdown", $"/s /t {seconds}");
                        Console.WriteLine($"System will shut down in {hours} hour(s).");

                        // Ask for confirmation
                        Console.WriteLine("Is this correct? (y/n)");
                        string response = Console.ReadLine()?.Trim().ToLower();

                        if (response == "n" || response == "no")
                        {
                            try
                            {
                                Process.Start("shutdown", "/a");
                                Console.Clear();
                                Console.WriteLine("Shutdown canceled. Starting over...\n");
                                continue; // Restart the loop
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Failed to cancel shutdown: {ex.Message}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to schedule shutdown: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("Please enter a non-negative number.");
                }
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Invalid input. Please enter a valid number.");
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            break; // Exit the loop and program
        }
    }
}