﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Alba.CsConsoleFormat.CommandLineParser;
using static System.ConsoleColor;

// ReSharper disable AnnotateCanBeNullParameter
// ReSharper disable AnnotateNotNullParameter
namespace Alba.CsConsoleFormat.Sample.ProcessManager
{
    internal static class View
    {
        private static readonly LineThickness StrokeHeader = new LineThickness(LineWidth.None, LineWidth.Double);
        private static readonly LineThickness StrokeRight = new LineThickness(LineWidth.None, LineWidth.None, LineWidth.Single, LineWidth.None);

        public static Document PressEnter() =>
            new Document {
                Background = Black, Color = Gray,
                Children = {
                    "\nPress ",
                    new Span("ENTER") { Color = Black, Background = Gray },
                    " to continue..."
                }
            };

        public static Document ProcessList(IEnumerable<Process> processes) =>
            new Document {
                Background = Black, Color = Gray,
                Children = {
                    new Grid {
                        Stroke = StrokeHeader, StrokeColor = DarkGray,
                        Columns = {
                            new Column { Width = GridLength.Auto },
                            new Column { Width = GridLength.Auto, MaxWidth = 20 },
                            new Column { Width = GridLength.Star(1) },
                            new Column { Width = GridLength.Auto }
                        },
                        Children = {
                            new Cell("Id") { Stroke = StrokeHeader, Color = White },
                            new Cell("Name") { Stroke = StrokeHeader, Color = White },
                            new Cell("Main Window Title") { Stroke = StrokeHeader, Color = White },
                            new Cell("Private Memory") { Stroke = StrokeHeader, Color = White },
                            processes.Select(process => new[] {
                                new Cell {
                                    Stroke = StrokeRight,
                                    Children = { process.Id }
                                },
                                new Cell {
                                    Stroke = StrokeRight, Color = Yellow, TextWrap = TextWrap.NoWrap,
                                    Children = { process.ProcessName }
                                },
                                new Cell {
                                    Stroke = StrokeRight, Color = White, TextWrap = TextWrap.NoWrap,
                                    Children = { process.MainWindowTitle }
                                },
                                new Cell {
                                    Stroke = LineThickness.None, Align = Align.Right,
                                    Children = { process.PrivateMemorySize64.ToString("n0") }
                                },
                            })
                        }
                    }
                }
            };

        public static Document HelpOptionsList(IEnumerable<OptionInfo> options, string instruction) =>
            new Document {
                Background = Black, Color = Gray,
                Children = {
                    new Div(instruction) { Color = White },
                    "",
                    new Grid {
                        Stroke = LineThickness.None,
                        Columns = { GridLength.Auto, GridLength.Star(1) },
                        Children = { options.Select(OptionNameAndHelp) }
                    }
                }
            };

        public static Document HelpAllOptionsList(IEnumerable<OptionInfo> verbsWithOptions, string instruction) =>
            new Document {
                Background = Black, Color = Gray,
                Children = {
                    new Span($"{instruction}\n") { Color = White },
                    new Grid {
                        Stroke = LineThickness.None,
                        Columns = { GridLength.Auto, GridLength.Star(1) },
                        Children = {
                            verbsWithOptions.Select(verbWithOptions => new object[] {
                                OptionNameAndHelp(verbWithOptions),
                                new Grid {
                                    Stroke = LineThickness.None, Margin = new Thickness(4, 0, 0, 0),
                                    [Grid.ColumnSpanProperty] = 2,
                                    Columns = { GridLength.Auto, GridLength.Star(1) },
                                    Children = { verbWithOptions.SubOptions.Select(OptionNameAndHelp) }
                                }
                            })
                        }
                    }
                }
            };

        private static object[] OptionNameAndHelp(OptionInfo option) => new object[] {
            new Div(GetOptionSyntax(option)) { Margin = new Thickness(1, 0, 1, 1), Color = Yellow, MinWidth = 14 },
            new Div(option.HelpText) { Margin = new Thickness(1, 0, 1, 1) },
        };

        private static object GetOptionSyntax(OptionInfo option)
        {
            if (option.OptionKind == OptionKind.Verb)
                return option.Name;
            return option.ShortName == null
                ? $"--{option.Name}"
                : option.Name == null
                    ? $"-{option.ShortName}"
                    : $"--{option.Name}, -{option.ShortName}";
        }
    }
}