using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SamLu.IO
{
    /// <summary>
    /// 表示字符串段落读取器。
    /// </summary>
    public class StringLineReader : NodeReader<string, char>
    {
        private int line = 0;
        /// <summary>
        /// 获取 <see cref="StringLineReader"/> 的当前位置的行号。
        /// </summary>
        public int Line { get { return this.line + 1; } }

        private int column = 0;
        /// <summary>
        /// 获取 <see cref="StringLineReader"/> 的当前位置的列号。
        /// </summary>
        public int Column { get { return this.column + 1; } }

        /// <summary>
        /// 获取或设置 <see cref="StringLineReader"/> 在处理字符串或字符序列中的换行时的行为。
        /// </summary>
        public EndOfLineOptions EndOfLineOptions { get; set; } = EndOfLineOptions.Auto;

        /// <summary>
        /// 内部的行缓存列表。
        /// </summary>
        protected List<List<char>> linesBuffer { get; } = new List<List<char>>() { new List<char>() };
        /// <summary>
        /// 获取 <see cref="StringLineReader"/> 的缓冲区。
        /// </summary>
        public string[] LinesBuffer { get { return this.linesBuffer.Select(chars => new string(chars.ToArray())).ToArray(); } }

        /// <summary>
        /// 获取 <see cref="StringLineReader"/> 对象中的指针位置。
        /// </summary>
        public override int Position
        {
            get
            {
                return base.Position;
            }
            set
            {
                if (value < 0) throw new InvalidOperationException("指针指向一个不合法的位置。");

                if (value >= this.buffer.Count) this._ReadBuffer(value - this.buffer.Count);
                if (value >/*=*/ this.buffer.Count) throw new InvalidOperationException("指针指向一个不合法的位置。");

                if (this.position == value) return;
                else if (this.position < value) // 指针后移
                {
                    int distance = value - this.position;

                    if (this.column >= this.linesBuffer[this.line].Count)
                    {
                        if (this.EndOfLineOptions == EndOfLineOptions.Auto)
                        {
                            int _distance = Environment.NewLine.Length - (this.column - this.linesBuffer[this.line].Count);
                            if (distance >= _distance) // 跨行
                            {
                                this.line++;
                                this.column = 0;

                                this.position += _distance;
                                distance -= _distance;
                            }
                            else
                            {
                                this.position += distance;
                                column += distance;
                                return;
                            }
                        }
                        else // if (this.EndOfLineOptions & EndOfLineOptions.CRLF != 0)
                        {
                            this.line++;
                            this.column = 0;

                            this.position++;
                            distance--;
                        }
                    }

                    while (true)
                    {
                        int _distance = this.linesBuffer[this.line].Count - this.column;
                        if (distance <= _distance)
                        {
                            this.position += distance;
                            this.column += distance;
                            break;
                        }
                        else
                        {
                            this.position += _distance;
                            distance -= _distance;
                        }

                        int column = 0;
                        switch (this.EndOfLineOptions)
                        {
                            case EndOfLineOptions.Auto:
                                if (distance >= Environment.NewLine.Length) // 跨行
                                {
                                    this.line++;
                                    this.column = 0;

                                    this.position += Environment.NewLine.Length;
                                    distance -= Environment.NewLine.Length;
                                }
                                else
                                {
                                    this.position += distance;
                                    column += distance;
                                    return;
                                }
                                break;
                            case EndOfLineOptions.CR:
                            case EndOfLineOptions.LF:
                            case EndOfLineOptions.CR | EndOfLineOptions.LF:
                                this.line++;
                                this.column = 0;

                                this.position++;
                                distance--;
                                break;
                            case EndOfLineOptions.CR | EndOfLineOptions.CRLF:
                                if ((distance == 1) &&
                                    (this.position + 1 < this.buffer.Count &&
                                        this.buffer[this.position + 1] != '\n')
                                )
                                {
                                    this.position++;
                                    this.column++;
                                    distance--;
                                }
                                else
                                {
                                    this.line++;
                                    this.column = 0;

                                    if (this.buffer[this.position + 1] != '\n')
                                    {
                                        this.position++;
                                        distance--;
                                    }
                                    else
                                    {
                                        this.position += 2;
                                        distance -= 2;
                                    }
                                }
                                break;
                            case EndOfLineOptions.LF | EndOfLineOptions.CRLF:
                                if (distance == 1 && this.buffer[this.position] == '\r')
                                {
                                    this.position++;
                                    this.column++;
                                    distance--;
                                }
                                else
                                {
                                    this.line++;
                                    this.column = 0;

                                    if (this.buffer[this.position] == '\n')
                                    {
                                        this.position++;
                                        distance--;
                                    }
                                    else
                                    {
                                        this.position += 2;
                                        distance -= 2;
                                    }
                                }
                                break;
                            case EndOfLineOptions.CR | EndOfLineOptions.LF | EndOfLineOptions.CRLF:
                                if (distance == 1 &&
                                    (
                                        (this.buffer[this.position] == '\r') &&
                                        (this.position + 1 < this.buffer.Count &&
                                            this.buffer[this.position + 1] == '\n')
                                    )
                                )
                                {
                                    this.position++;
                                    this.column++;
                                    distance--;
                                }
                                else
                                {
                                    this.line++;
                                    this.column = 0;

                                    if ((this.buffer[this.position] == '\r') &&
                                        (this.buffer[this.position + 1] == '\n')
                                    )
                                    {
                                        this.position += 2;
                                        distance -= 2;
                                    }
                                    else
                                    {
                                        this.position++;
                                        distance--;
                                    }
                                }
                                break;
                        }
                    }
                }
                else if (this.position > value) // 指针前移
                {
                    int distance = this.position - value;

                    while (true)
                    {
                        if (distance <= this.column)
                        {
                            this.position -= distance;
                            this.column -= distance;
                            break;
                        }
                        else
                        {
                            this.position -= this.column;
                            distance -= this.column;
                        }

                        this.line--;
                        int column = this.linesBuffer[this.line].Count;
                        switch (this.EndOfLineOptions)
                        {
                            case EndOfLineOptions.Auto:
                                column += Environment.NewLine.Length;
                                break;
                            case EndOfLineOptions.CR:
                            case EndOfLineOptions.LF:
                            case EndOfLineOptions.CR | EndOfLineOptions.LF:
                                column++;
                                break;
                            case EndOfLineOptions.CR | EndOfLineOptions.CRLF:
                                if (this.buffer[this.position - 1] == '\r')
                                    column++;
                                else
                                    column += 2;
                                break;
                            case EndOfLineOptions.LF | EndOfLineOptions.CRLF:
                                if (this.buffer[this.position - 2] == '\r')
                                    column += 2;
                                else column++;
                                break;
                            case EndOfLineOptions.CR | EndOfLineOptions.LF | EndOfLineOptions.CRLF:
                                if (this.buffer[this.position - 1] == '\n' && this.buffer[this.position - 2] == '\r')
                                    column += 2;
                                else column++;
                                break;
                        }
                        this.column = column;
                    }
                }
            }
        }

        /// <summary>
        /// 使用指定的字符序列初始化 <see cref="StringLineReader"/> 类的新实例。
        /// </summary>
        /// <param name="charArray">指定的字符序列。</param>
        /// <exception cref="ArgumentNullException"><paramref name="charArray"/> 的值为 null 。</exception>
        public StringLineReader(char[] charArray) :
            this(new string(charArray ?? throw new ArgumentNullException(nameof(charArray))))
        { }

        /// <summary>
        /// 使用指定的字符串初始化 <see cref="StringLineReader"/> 类的新实例。
        /// </summary>
        /// <param name="value">指定的字符串。</param>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> 的值为 null 。</exception>
        public StringLineReader(string value) :
            base((value ?? throw new ArgumentNullException(nameof(value))).GetEnumerator())
        {
            this.Reader = value;
        }

        /// <summary>
        /// 调用内部读取器的读取方法向缓冲区读入指定长度的节点。
        /// </summary>
        /// <param name="count">读取节点序列的长度。</param>
        /// <returns>实际读取的节点序列的长度。</returns>
        protected override int _ReadBuffer(int count)
        {
            int i = 0;
            while (i++ < count && !this.eofFunc(this.Reader))
            {
                char c = this.readFunc(this.Reader);
                this.buffer.Add(c);

                switch (this.EndOfLineOptions)
                {
                    case EndOfLineOptions.Auto:
                        string s = string.Empty;
                        string nl = Environment.NewLine;
                        int _i = 0;
                        while (true)
                        {
                            if (c != nl[_i++]) break;
                            else s += c;

                            if (this.eofFunc(this.Reader)) break;
                            else if (_i == nl.Length) break;
                            else
                            {
                                i++;

                                c = this.readFunc(this.Reader);
                                this.buffer.Add(c);
                            }
                        }

                        if (s == Environment.NewLine)
                            this.linesBuffer.Add(new List<char>());
                        else if (s == string.Empty)
                            this.linesBuffer.Last().Add(c);
                        else
                            this.linesBuffer.Last().AddRange(s.ToCharArray());
                        break;
                    case EndOfLineOptions.CR:
                        if (c == '\r')
                            this.linesBuffer.Add(new List<char>());
                        break;
                    case EndOfLineOptions.LF:
                        if (c == '\n')
                            this.linesBuffer.Add(new List<char>());
                        break;
                    case EndOfLineOptions.CRLF:
                        if (c == '\r')
                        {
                            if (this.eofFunc(this.Reader))
                                this.linesBuffer.Last().Add(c);
                            else
                            {
                                i++;
                                c = this.readFunc(this.Reader);
                                this.buffer.Add(c);
                                if (c == '\n')
                                    this.linesBuffer.Add(new List<char>());
                                else
                                    this.linesBuffer.Last().AddRange(new char[] { '\r', c });
                            }
                        }
                        break;
                    case EndOfLineOptions.CR | EndOfLineOptions.LF:
                        if (c == '\r' || c == '\n')
                            this.linesBuffer.Add(new List<char>());
                        break;
                    case EndOfLineOptions.CR | EndOfLineOptions.CRLF:
                        if (c == '\r')
                        {
                            this.linesBuffer.Add(new List<char>());

                            i++;
                            c = this.readFunc(this.Reader);
                            this.buffer.Add(c);
                            if (c != '\n')
                            {
                                this.linesBuffer.Last().Add(c);
                            }
                        }
                        else this.linesBuffer.Last().Add(c);
                        break;
                    case EndOfLineOptions.LF | EndOfLineOptions.CRLF:
                        if (c == '\n')
                            this.linesBuffer.Add(new List<char>());
                        else if (c == '\r')
                        {
                            if (this.eofFunc(this.Reader))
                                this.linesBuffer.Last().Add(c);
                            else
                            {
                                i++;
                                c = this.readFunc(this.Reader);
                                this.buffer.Add(c);
                                if (c == '\n')
                                    this.linesBuffer.Add(new List<char>());
                                else
                                    this.linesBuffer.Last().AddRange(new char[] { '\r', c });
                            }
                        }
                        break;
                    case EndOfLineOptions.CR | EndOfLineOptions.LF | EndOfLineOptions.CRLF:
                        if (c == '\n')
                            this.linesBuffer.Add(new List<char>());
                        else if (c == '\r')
                        {
                            this.linesBuffer.Add(new List<char>());

                            i++;
                            c = this.readFunc(this.Reader);
                            this.buffer.Add(c);
                            if (c != '\n')
                            {
                                this.linesBuffer.Last().Add(c);
                            }
                        }
                        else this.linesBuffer.Last().Add(c);
                        break;
                }
            }

            return i - 1;
        }
    }

    /// <summary>
    /// 指定 <see cref="StringLineReader"/> 在处理字符串或字符序列中的换行时的行为。
    /// </summary>
    [Flags]
    public enum EndOfLineOptions
    {
        /// <summary>
        /// 随系统环境改变，等价于 <see cref="Environment.NewLine"/> 。
        /// </summary>
        Auto = 0,
        /// <summary>
        /// 使用回车（CR, ASCII 13, \r）作为换行符。
        /// </summary>
        CR = 0x1,
        /// <summary>
        /// 使用换行（LF, ASCII 10, \n）作为换行符。
        /// </summary>
        LF = 0x2,
        /// <summary>
        /// 使用回车（CR, ASCII 13, \r） + 换行（LF, ASCII 10, \n）作为换行符。
        /// </summary>
        CRLF = 0x4
    }
}
