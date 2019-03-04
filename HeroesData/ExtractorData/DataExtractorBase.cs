﻿using Heroes.Models;
using HeroesData.Parser;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HeroesData.ExtractorData
{
    public abstract class DataExtractorBase<T, TParser>
        where T : IExtractable
        where TParser : IParser<T, TParser>
    {
        private string ValidationWarningId = "Unknown";
        private HashSet<string> ValidationWarnings = new HashSet<string>();

        public DataExtractorBase(TParser parser)
        {
            Parser = parser;
        }

        /// <summary>
        /// Gets a list of validation warnings.
        /// </summary>
        public List<string> Warnings
        {
            get
            {
                return ValidationWarnings.ToList();
            }
        }

        /// <summary>
        /// Gets the amount of validation warnings that were ignored.
        /// </summary>
        public int WarningsIgnoredCount { get; private set; } = 0;

        /// <summary>
        /// Type of data that is being parsed.
        /// </summary>
        public abstract string Name { get; }

        protected ConcurrentDictionary<string, T> ParsedData { get; } = new ConcurrentDictionary<string, T>();

        protected TParser Parser { get; }

        /// <summary>
        /// Parses the items and returns a collection in ascending order.
        /// </summary>
        /// <param name="localization"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> Parse(Localization localization)
        {
            Stopwatch time = new Stopwatch();

            Console.WriteLine($"Parsing {Name} data...");

            time.Start();

            int currentCount = 0;

            IList<string[]> items = Parser.Items;

            Console.Write($"\r{currentCount,6} / {items.Count} total {Name}");

            try
            {
                Parallel.ForEach(items, new ParallelOptions { MaxDegreeOfParallelism = App.MaxParallelism }, item =>
                {
                    ParsedData.GetOrAdd(string.Join(" ", item), Parser.GetInstance().Parse(item));
                    Console.Write($"\r{Interlocked.Increment(ref currentCount),6} / {items.Count} total {Name}");
                });
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;

                App.WriteExceptionLog($"{Name.Where(x => !char.IsWhiteSpace(x))}", ex);

                Console.WriteLine();
                Console.WriteLine($"Failed to parse {Name} [Check logs for details]");
                Console.WriteLine();

                Console.ResetColor();
                Environment.Exit(1);
            }

            time.Stop();

            Console.WriteLine();
            Console.WriteLine($"Finished in {time.Elapsed.Seconds} seconds {time.Elapsed.Milliseconds} milliseconds");
            Console.WriteLine();

            return ParsedData.Values.OrderBy(x => x.Id);
        }

        /// <summary>
        /// Checks all the parsed data for missing or inaccurate data.
        /// </summary>
        public void Validate(Localization localization)
        {
            foreach (var t in ParsedData)
            {
                ValidationWarningId = t.Value.Id;
                Validation(t.Value);
            }

            if (Warnings.Count > 0)
            {
                List<string> nonTooltips = new List<string>(Warnings.Where(x => !x.ToLower().Contains("tooltip")));
                List<string> tooltips = new List<string>(Warnings.Where(x => x.ToLower().Contains("tooltip")));

                using (StreamWriter writer = new StreamWriter(Path.Combine(App.AssemblyPath, $"VerificationCheck_{Name}_{localization.ToString().ToLower()}.txt"), false))
                {
                    if (nonTooltips.Count > 0)
                    {
                        nonTooltips.ForEach((warning) =>
                        {
                            writer.WriteLine(warning);
                            if (App.ShowValidationWarnings)
                                Console.WriteLine(warning);
                        });
                    }

                    if (tooltips.Count > 0)
                    {
                        writer.WriteLine();
                        tooltips.ForEach((warning) =>
                        {
                            writer.WriteLine(warning);
                            if (App.ShowValidationWarnings)
                                Console.WriteLine(warning);
                        });
                    }

                    writer.WriteLine($"{Environment.NewLine}{Warnings.Count} warnings ({WarningsIgnoredCount} ignored)");
                }

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"[{Name}] {Warnings.Count} warnings");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($"[{Name}] {Warnings.Count} warnings");
            }

            if (WarningsIgnoredCount > 0)
                Console.Write($" ({WarningsIgnoredCount} ignored)");

            Console.WriteLine();
            Console.ResetColor();
        }

        protected abstract void Validation(T t);

        protected void AddWarning(string message)
        {
            string genericMessage = $"[${typeof(T).Name.ToLowerInvariant()}] {message}";

            message = $"[{ValidationWarningId}] {message}".Trim();

            if (!App.ValidationIgnoreLines.Contains(message) && !App.ValidationIgnoreLines.Contains(genericMessage))
                ValidationWarnings.Add(message);
            else
                WarningsIgnoredCount++;
        }
    }
}
