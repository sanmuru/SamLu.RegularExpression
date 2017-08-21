using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SamLu.IO
{
	public class StringLineReader : NodeReader<string, char>
	{
		private int line = 0;
		public int Line { get { return this.line + 1; } }

		private int column = 0;
        public int Column { get { return this.column + 1; } }

		public EndOfLineOptions EndOfLineOptions { get; set; } = EndOfLineOptions.Auto;

		protected List<List<char>> linesBuffer { get; } = new List<List<char>>() { new List<char>() };
		public string[] LinesBuffer { get { return this.linesBuffer.Select(chars => new string(chars.ToArray())).ToArray(); } }
		
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

		public StringLineReader(char[] charArray) : this(new string(charArray))
		{
			if (charArray == null) throw new ArgumentNullException(nameof(charArray));
		}

		public StringLineReader(string value) : base(value.GetEnumerator())
		{
			if (value == null) throw new ArgumentNullException(nameof(value));

			this.Reader = value;
		}

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

	[Flags]
	public enum EndOfLineOptions
	{
		Auto = 0,
		CR = 0x1,
		LF = 0x2,
		CRLF = 0x4
	}
}
