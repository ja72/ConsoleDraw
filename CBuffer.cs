using System;

namespace JA
{
    public class CBuffer
    {
        private readonly char[] empty;
        private readonly string[] spaces;
        public CBuffer() : this(Console.WindowWidth, Console.WindowHeight)
        { }

        public CBuffer(int width, int height)
        {
            this.UseSpaces = true;
            this.ClearBeforeRender = true;
            this.Buffer = new char[height*width];            
            this.Height= height;
            this.Width= width;
            empty = new string(' ', width*height).ToCharArray();
            spaces = new string[width];
            for (int i = 0; i < width; i++)
            {
                spaces[i] = new string(' ', i+1);
            }
            ClearAll();
        }

        public char this[int x, int y]
        {
            get => Buffer[y*Width+x];
            set => Buffer[y*Width+x] =value;
        }
        public void Clear(int x, int y)
        {
            Buffer[y*Width+x] = ' ';
        }

        public void ClearAll()
        {
            Array.Copy(empty, Buffer, empty.Length);
        }
        
        public int Height { get; }
        public int Width { get; }
        public char[] Buffer { get; }
        public bool UseSpaces { get; set; }
        public bool ClearBeforeRender { get; set; }

        public string GetRow(int y)
        {
            char[] data = new char[Width];
            Array.Copy(Buffer, y*Width, data, 0, data.Length);
            return new string(data);
        }
        public void AddText(int x, int y, string text, int width = -1)
        {
            if (width==-1)
            {
                width= text.Length;
            }
            width = Math.Min(width, Width-x);
            if (text.Length>width)
            {                
                AddText(x, y, text.Substring(0, width), width);
                text = text.Substring(width);
                AddText(x, y+1, text, width);
            }
            else
            {
                var len = Math.Min(text.Length, width);
                Array.Copy(text.ToCharArray(), 0, Buffer, y*Width+x, len);
            }
        }
        public override string ToString()
        {
            return new string(Buffer);
        }
        public void Render()
        {
            if (ClearBeforeRender && !UseSpaces)
            {
                Console.Clear();
            }
            
            if (UseSpaces)
            { 
                Console.SetCursorPosition(0, 0);
                Console.Write(ToString());
            }
            else
            {
                for (int i = 0; i < Height; i++)
                {
                    var text = GetRow(i);
                    for (int n = Width - 1; n >= 1; n--)
                    {
                        text = text.Replace(spaces[n], AnsiCodes.CursorForward(n+1));
                    }
                    Console.SetCursorPosition(0, i);
                    Console.Write(text);
                }
            }
        }
    }
}
