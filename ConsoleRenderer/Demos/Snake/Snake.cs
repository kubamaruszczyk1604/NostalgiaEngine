using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NostalgiaEngine.Core;

namespace NostalgiaEngine.Demos
{

	class Snake : NEScene
	{
		enum CellValue
		{
			Empty = 0,
			Snake = 1,
			Food = 2
		}

		struct Vec2
		{
			public int X;
			public int Y;

			public Vec2(int x, int y)
			{
				X = x;
				Y = y;
			}

			static public bool operator ==(Vec2 l, Vec2 r)
			{
				return (l.X == r.X) && (l.Y == r.Y);
			}

			static public bool operator !=(Vec2 l, Vec2 r)
			{
				return !(l == r);
			}

			static public Vec2 operator +(Vec2 l, Vec2 r)
			{
				return new Vec2(l.X + r.X, l.Y + r.Y);
			}

			static public Vec2 operator -(Vec2 dir)
			{
				return new Vec2(-dir.X, -dir.Y);
			}

		}

		static Vec2 DirLeft = new Vec2(-1, 0);
		static Vec2 DirRight = new Vec2(1, 0);
		static Vec2 DirUp = new Vec2(0, -1);
		static Vec2 DirDown = new Vec2(0, 1);

		class SnakeUtils
		{
			public static uint CoordToIndex(uint x, uint y, uint boardW)
			{
				return y * boardW + x;
			}

			private int CoordToIndexSigned(int x, int y, int boardW)
			{
				return y * boardW + x;
			}

			public static bool IndexToCoord(uint index, uint boardW, uint boardH, out uint x, out uint y)
			{
				x = 0;
				y = 0;

				if (index >= boardW * boardH)
				{
					return false;
				}

				x = index % boardW;
				y = index / boardW;

				return true;
			}
		}

		class SnakePlayer
		{
			public delegate void OnSnakeGrow(uint newSize);
			public delegate void OnSnakeSelfBite();

			private Queue<uint> m_snakeCells;
			uint m_BoardW;
			uint m_BoardH;
			uint m_InitialSnakeSize;

			Vec2 m_Direction;
			bool m_Active;

			OnSnakeGrow m_GrowCallbacks;
			OnSnakeSelfBite m_BiteCallbacks;

			public SnakePlayer(uint snakeSize, uint boardW, uint boardH)
			{
				//if(startIndex > boardW * m_boardH)
				//{
				//	throw new Exception("Start index out of bounds");
				//}

				if (snakeSize > 10)
				{
					throw new Exception("Initial snake size larger than 10 is not allowed");
				}

				if (snakeSize >= (boardW / 2))
				{
					throw new Exception("Snake size larger than half board width is not allowed");
				}
				m_InitialSnakeSize = snakeSize;
				m_BoardW = boardW;
				m_BoardH = boardH;

				ResetSnake();

			}

			public void AddGrowCallback(OnSnakeGrow growCallback)
			{
				m_GrowCallbacks += growCallback;
			}

			public void AddCollideCalback(OnSnakeSelfBite collideCallback)
			{
				m_BiteCallbacks += collideCallback;
			}

			public int GetSize()
			{
				return m_snakeCells.Count;
			}

			public void ResetSnake()
			{
				uint startSnakeX = (m_BoardW / 2) - 2;
				uint startSnakeY = m_BoardH / 2;
				m_snakeCells = new Queue<uint>();

				m_Direction = new Vec2(1, 0);
				m_Active = true;

				uint currentX = startSnakeX;
				for (uint i = 0; i < m_InitialSnakeSize; ++i)
				{
					m_snakeCells.Enqueue(SnakeUtils.CoordToIndex(currentX, startSnakeY, m_BoardW));
					currentX++;
				}
			}

			public void TurnCCW()
			{
				// 1, 0  ->  0, -1
				// 0, -1 ->  -1, 0
				// -1, 0 ->  0, 1
				// 0 , 1 -> 1, 0

				int sign = (m_Direction.X != 0) ? -1 : 1;

				int tempX = m_Direction.X;
				m_Direction.X = m_Direction.Y * sign;
				m_Direction.Y = tempX * sign;

			}

			public void TurnCW()
			{
				// 1, 0  ->  0, 1
				// 0, 1 ->  -1, 0 swap
				// -1, 0 ->  0, -1
				// 0 , -1 -> 1, 0 swap

				int sign = (m_Direction.X == 0) ? -1 : 1;

				int tempX = m_Direction.X;
				m_Direction.X = m_Direction.Y * sign;
				m_Direction.Y = tempX * sign;

			}

			public bool Turn(Vec2 direction)
			{
				bool isOpposite = m_Direction == -direction;
				if (isOpposite)
				{
					return false;
				}

				m_Direction = direction;

				return true;
			}

			public void UpdateBoard(CellValue[] board, out bool wantFrameDelay)
			{
				wantFrameDelay = false;
				if (!m_Active) return;
				uint head = m_snakeCells.Last();

				uint headX;
				uint headY;
				SnakeUtils.IndexToCoord(head, c_BoardWidth, c_BoardHeight, out headX, out headY);

				Vec2 nextHeadCoord = new Vec2((int)headX, (int)headY);
				nextHeadCoord += m_Direction;

				if (nextHeadCoord.X >= c_BoardWidth) nextHeadCoord.X = 0;
				if (nextHeadCoord.Y >= c_BoardHeight) nextHeadCoord.Y = 0;
				if (nextHeadCoord.X < 0) nextHeadCoord.X = c_BoardWidth - 1;
				if (nextHeadCoord.Y < 0) nextHeadCoord.Y = c_BoardHeight - 1;

				uint newHeadIndex = SnakeUtils.CoordToIndex((uint)nextHeadCoord.X, (uint)nextHeadCoord.Y, c_BoardWidth);
				m_snakeCells.Enqueue(newHeadIndex);

				CellValue headFld = board[newHeadIndex];

				bool snakeHit = headFld == CellValue.Snake;
				if (snakeHit)
				{
					m_Active = false;
					m_BiteCallbacks?.Invoke();
					return;
					//foreach (uint boardIndex in m_snakeCells)
					//{
					//	board[boardIndex] = CellValue.Empty;
					//}
					//ResetSnake();
				}

				bool growSnake = (headFld == CellValue.Food);
				if (growSnake)
				{
					m_GrowCallbacks?.Invoke((uint)m_snakeCells.Count);
				}
				else
				{
					board[m_snakeCells.First()] = CellValue.Empty;
					m_snakeCells.Dequeue();
					wantFrameDelay = true;
				}

				foreach (uint boardIndex in m_snakeCells)
				{
					board[boardIndex] = CellValue.Snake;
				}
			}
		}

		private readonly Dictionary<NEKey, Vec2> c_keyMap = new Dictionary<NEKey, Vec2>
		{
			{NEKey.LeftArrow, DirLeft},
			{NEKey.RightArrow, DirRight},
			{NEKey.UpArrow, DirUp},
			{NEKey.DownArrow, DirDown}
		};

		private const int c_BoardWidth = 30;
		private const int c_BoardHeight = 20;

		private CellValue[] m_Board;
		private SnakePlayer m_Snake;
		private bool m_Paused;

		private void PlaceNewFood()
		{
			Random rnd = new Random();
			uint index;
			while (true)
			{
				index = (uint)rnd.Next(0, m_Board.Length);
				if (m_Board[index] == CellValue.Empty) break;
			}

			m_Board[index] = CellValue.Food;
		}

		void OnFoodCollected(uint newLength)
		{
			PlaceNewFood();
		}

		public override bool OnLoad()
		{
			ScreenWidth = 36;
			ScreenHeight = 22;
			PixelWidth = 28;
			PixelHeight = 28;

			NEConsoleColorDef baseCol = new NEConsoleColorDef(156, 205, 126);
			baseCol = baseCol * 0.95f;
			NEColorPalette pal = new NEColorPalette(NEColorPalette.NostalgiaPalette);
			pal.SetColor(1, baseCol);
			pal.SetColor(2, new NEConsoleColorDef(156, 205, 126));
			pal.SetColor(3, new NEConsoleColorDef(48, 60, 46));
			NEColorManagement.SetPalette(pal);

			m_Board = new CellValue[c_BoardWidth * c_BoardHeight];

			m_Board[SnakeUtils.CoordToIndex(2, 2, c_BoardWidth)] = CellValue.Food;
			m_Snake = new SnakePlayer(5, c_BoardWidth, c_BoardHeight);
			m_Snake.AddGrowCallback(OnFoodCollected);

			m_Paused = false;

			return true;
		}

		public override void OnUpdate(float deltaTime)
		{
			if (NEInput.CheckKeyPress(NEKey.Control))
			{
				NEScreenBuffer.SaveAsTxt("C:/test/test.txt");
			}
			if (NEInput.CheckKeyPress(NEKey.Key_P))
			{
				m_Paused = !m_Paused;
			}
			if (m_Paused)
			{
				Thread.Sleep(350);
				return;
			}
			bool dirUpdated = false;

			foreach(KeyValuePair<NEKey, Vec2> keyPress in c_keyMap)
			{
				if (NEInput.CheckKeyPress(keyPress.Key))
				{
					dirUpdated = m_Snake.Turn(keyPress.Value);	
				}
			}

			bool requestedFrameDelay;
			m_Snake.UpdateBoard(m_Board, out requestedFrameDelay);
			int frameDelayMask = requestedFrameDelay ? 1 : 0;
			Thread.Sleep(350 - (3 * m_Snake.GetSize() * frameDelayMask));
		}

		public override bool OnDraw()
		{
			NEScreenBuffer.ClearColor(2);
			base.OnDraw();
			NEColorSample sampleA = NEColorSample.MakeColFromBlocks5((ConsoleColor)1, (ConsoleColor)2, 0.2f);
			NEColorSample sampleB = NEColorSample.MakeColFromBlocks5((ConsoleColor)1, (ConsoleColor)2, 0.4f);

			uint yOffset = 1;
			uint xOffset = 3;
			for(uint x = xOffset; x < ScreenWidth; ++x)
			{
				uint xOnBoard = x - xOffset;
				if (xOnBoard >= c_BoardWidth) continue;

				for (uint y = yOffset; y < ScreenHeight; ++y)
				{
					uint yOnBoard = y - yOffset;
					if (yOnBoard >= c_BoardHeight) continue;

					uint index = SnakeUtils.CoordToIndex(xOnBoard, yOnBoard, c_BoardWidth);

					ref CellValue cell = ref m_Board[index];
					if (cell == CellValue.Empty)
					{
						NEColorSample sample = sampleA;
						if ((y + x) % 2 == 0)
						{
							sample = sampleB;
						}
						NEScreenBuffer.PutChar(sample.Character, sample.BitMask, (int)x, (int)y);
					}
					else
					{
						NEColorSample sample2 = NEColorSample.MakeColFromBlocks5(0, (ConsoleColor)3, 1.0f);
						NEScreenBuffer.PutChar(sample2.Character, sample2.BitMask, (int)x, (int)y);
					}
				}
			}

			string scoreDigits = m_Snake.GetSize().ToString();
			if(scoreDigits.Length < 2)
			{
				scoreDigits = "0" + scoreDigits;
			}

			string scoreStr = "SCORE:" + scoreDigits;
			for (int i = 0; i < scoreStr.Length; ++i)
			{
				NEScreenBuffer.PutChar(scoreStr[i], 1, 3 + i, 0);
			}

			if (m_Paused)
			{
				string pausedStr = "PAUSED";
				NEScreenBuffer.WriteXY(ScreenWidth / 2 - pausedStr.Length / 2, ScreenHeight / 2 - 1, 1, pausedStr);
			}
			return true;
		}
	
	}
}
