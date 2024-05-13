using System;
using System.Runtime.InteropServices;

namespace JA
{
    public enum Color
    {
        Black = 0,
        Red = 1,
        Green = 2,
        Yellow = 3,
        Blue = 4,
        Magenta = 5,
        Cyan = 6,
        White = 7,
    }

    public enum Attributes
    {
        Bold = 1,
        Underline = 4,
        Blink = 5,
        Reverse = 7,
        Conceal = 8,
    }

    /// <summary>
    /// Support for ANSI terminal codes.
    /// This class automatically adds support for ANSI codes in Console applications
    /// by calling <see cref="EnableInConsole"/> in the static constructor.
    /// </summary>
    public static class AnsiCodes 
    {
        static AnsiCodes()
        {
            IsEnabledInInput  = false;
            IsEnabledInOutput = false;

            EnableInConsole();
        }

        public static bool IsEnabledInOutput { get; private set; }
        public static bool IsEnabledInInput { get; private set; }

        #region Win32 API
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int nStdHandle);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        enum StandardHandle
        {
            INVALID_HANDLE_VALUE = -1,
            STD_OUTPUT_HANDLE = -11,
            STD_INPUT_HANDLE = -10,
            STD_ERROR_HANDLE = -12,
        }


        [Flags]
        enum ConsoleInputModes : uint
        {
            ENABLE_PROCESSED_INPUT = 0x0001,
            ENABLE_LINE_INPUT = 0x0002,
            ENABLE_ECHO_INPUT = 0x0004,
            ENABLE_WINDOW_INPUT = 0x0008,
            ENABLE_MOUSE_INPUT = 0x0010,
            ENABLE_INSERT_MODE = 0x0020,
            ENABLE_QUICK_EDIT_MODE = 0x0040,
            ENABLE_EXTENDED_FLAGS = 0x0080,
            ENABLE_AUTO_POSITION = 0x0100,
            ENABLE_VIRTUAL_TERMINAL_INPUT = 0x0200
        }

        [Flags]
        enum ConsoleOutputModes : uint
        {
            ENABLE_PROCESSED_OUTPUT = 0x0001,
            ENABLE_WRAP_AT_EOL_OUTPUT = 0x0002,
            ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004,
            DISABLE_NEWLINE_AUTO_RETURN = 0x0008,
            ENABLE_LVB_GRID_WORLDWIDE = 0x0010
        }

        #endregion

        /// <summary>
        /// Enables the virtual terminal processing.       
        /// </summary>
        /// <remarks>
        /// Reference:
        ///  *  http://ascii-table.com/ansi-escape-sequences.php
        ///  *  https://www.lihaoyi.com/post/BuildyourownCommandLinewithANSIescapecodes.html
        ///  *  https://www.jerriepelser.com/blog/using-ansi-color-codes-in-net-console-apps/
        /// </remarks>
        public static void EnableInConsole()
        {
            var iStdIn = GetStdHandle((int)StandardHandle.STD_INPUT_HANDLE);
            var iStdOut = GetStdHandle((int)StandardHandle.STD_OUTPUT_HANDLE);

            if (GetConsoleMode(iStdIn, out uint inConsoleMode))
            {
                inConsoleMode |= (uint)ConsoleInputModes.ENABLE_VIRTUAL_TERMINAL_INPUT;

                if (SetConsoleMode(iStdIn, inConsoleMode))
                {
                    IsEnabledInInput = true;
                }
                else
                {
                    Console.Error.WriteLine($"Cannot set input console with {inConsoleMode}");
                }
            }
            if (GetConsoleMode(iStdOut, out uint outConsoleMode))
            {
                outConsoleMode |= (uint)ConsoleOutputModes.ENABLE_VIRTUAL_TERMINAL_PROCESSING
                    | (uint)ConsoleOutputModes.DISABLE_NEWLINE_AUTO_RETURN;

                if (SetConsoleMode(iStdOut, outConsoleMode))
                {
                    IsEnabledInOutput = true;
                }
                else
                {
                    Console.Error.WriteLine($"Cannot set output console with {outConsoleMode}");
                }
            }
        }

        #region Codes
        public static readonly string Esc = "\u001B[";
        public static readonly string SaveCursorPosition = $"{Esc}s";
        public static readonly string RestoreCursorPosition = $"{Esc}u";
        public static readonly string ClearScreen = $"{Esc}2J";
        public static readonly string EraseLine = $"{Esc}K";
        public static readonly string ResetAttributes = $"{Esc}0m";

        public static string CursorUp(int count) => $"{Esc}{count}A";
        public static string CursorDown(int count) => $"{Esc}{count}B";
        public static string CursorForward(int count) => $"{Esc}{count}C";
        public static string CursorBackward(int count) => $"{Esc}{count}C";

        public static string CursorPosition(int line, int column) => $"{Esc}{line};{column}H";
        public static string TextAttributes(Attributes attributes) => $"{Esc}{(int)attributes}m";
        public static string TextColor(Color foreground, bool bright)
            => bright ? $"{Esc}{30+(int)foreground};1m" : $"{Esc}{30+(int)foreground}m";
        public static string TextColor(byte foreground)
            => $"{Esc}38;5;{foreground}m";
        public static string BackgroundColor(Color background, bool bright)
            => bright ? $"{Esc}{40+(int)background};1m" : $"{Esc}{40+(int)background}m";
        public static string BackgroundColor(byte background)
            => $"{Esc}48;5;{background}m"; 
        #endregion

    }
}
