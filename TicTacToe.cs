namespace TicTacToe;

public class TicTacToe
{
	public static readonly string X = "X";
	public static readonly string O = "O";
	public static readonly int LOOK_AHEAD = 3;
	private string[] board;
	private bool isPlayerOneTurn;

	public TicTacToe()
	{
		board = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8" };
		isPlayerOneTurn = true;
	}

	public void Start()
	{
		int input;

		ShowGameStartScreen();

		do
		{
			ShowBoard();

			do
			{
				ShowInputOptions();
				input = GetInput();
			}
			while (!IsValidInput(input));

			ProcessInput(input);
			UpdateGameboard();
		}
		while (!IsGameOver());

		ShowGameOverScreen();
	}

	private void ShowGameStartScreen()
	{
		Console.WriteLine("Welcome to Tic-Tac-Toe!");
	}

	private void ShowBoard()
	{
		string output =
			$"{board[0]} | {board[1]} | {board[2]}\n" +
			$"---------\n" +
			$"{board[3]} | {board[4]} | {board[5]}\n" +
			$"---------\n" +
			$"{board[6]} | {board[7]} | {board[8]}\n";

		Console.Clear();
		Console.WriteLine(output);
	}

	private void ShowInputOptions()
	{
		int[] aiMove = new int[1];

		int bestEval = MiniMax(LOOK_AHEAD, isPlayerOneTurn, isPlayerOneTurn, aiMove);

		Console.WriteLine("AI suggested move is: {0} (Eval: {1})\n", aiMove[0], bestEval);
		Console.Write("Enter a number between 0 to 8: ");
	}

	private int GetInput()
	{
		return int.TryParse(Console.ReadLine(), out int input) ? input : -1;
	}

	private bool IsValidInput(int input)
	{
		return input >= 0 && input <= 8 && IsEmpty(input);
	}

	private bool IsEmpty(int input)
	{
		return board[input] != X && board[input] != O;
	}

	private void ProcessInput(int input)
	{
		board[input] = isPlayerOneTurn ? X : O;
	}

	private void UpdateGameboard()
	{
		isPlayerOneTurn = !isPlayerOneTurn;
	}

	private bool IsGameOver()
	{
		return IsWinner(X) || IsWinner(O) || IsDraw();
	}

	private bool IsWinner(string m)
	{
		return
			CheckLine(0, 1, 2, m) ||
			CheckLine(3, 4, 5, m) ||
			CheckLine(6, 7, 8, m) ||
			CheckLine(0, 3, 6, m) ||
			CheckLine(1, 4, 7, m) ||
			CheckLine(2, 5, 8, m) ||
			CheckLine(0, 4, 8, m) ||
			CheckLine(2, 4, 6, m);
	}

	private bool CheckLine(int a, int b, int c, string m)
	{
		return board[a] == m && board[b] == m && board[c] == m;
	}

	private bool IsDraw()
	{
		for (int i = 0; i < board.Length; i++)
		{
			if (IsEmpty(i)) { return false; }
		}

		return true;
	}

	private void ShowGameOverScreen()
	{
		ShowBoard();
		Console.WriteLine("Game Over!");

		if (IsWinner(X))
		{
			Console.WriteLine("X Won!");
		}
		else if (IsWinner(O))
		{
			Console.WriteLine("O Won!");
		}
		else
		{
			Console.WriteLine("Draw!");
		}
	}

	public int MiniMax(int lookAhead, bool playerOneTurn, bool maximizingPlayer, int[] bestMove)
	{
		if (lookAhead == 0 || IsGameOver())
		{
			return Evaluate(lookAhead, maximizingPlayer);
		}

		bool maximizing = (playerOneTurn == maximizingPlayer);
		int bestEval = maximizing ? int.MinValue : int.MaxValue;

		foreach (int m in GetNextPossibleMoves())
		{
			board[m] = playerOneTurn ? X : O;
			int value = MiniMax(lookAhead - 1, !playerOneTurn, maximizingPlayer, new int[1]);
			board[m] = m.ToString();

			if (maximizing ? value > bestEval : value < bestEval)
			{
				bestEval = value;
				bestMove[0] = m;
			}
		}

		return bestEval;
	}

	private int Evaluate(int lookAhead, bool maximizingPlayer)
	{
		int x3 = CheckAllThreeInALine(X);
		int o3 = CheckAllThreeInALine(O);

		int x2 = CheckAllTwoInALine(X);
		int o2 = CheckAllTwoInALine(O);

		int x1 = CheckAllOneInALine(X);
		int o1 = CheckAllOneInALine(O);

		int val = ((x3 - o3) * 100 * (lookAhead + 1)) + ((x2 - o2) * 10) + ((x1 - o1) * 1);

		return (maximizingPlayer) ? val : -val;
	}

	private int CheckAllThreeInALine(string m)
	{
		return
			CheckThreeInALine(m, 0, 1, 2) +
			CheckThreeInALine(m, 3, 4, 5) +
			CheckThreeInALine(m, 6, 7, 8) +
			CheckThreeInALine(m, 0, 3, 6) +
			CheckThreeInALine(m, 1, 4, 7) +
			CheckThreeInALine(m, 2, 5, 8) +
			CheckThreeInALine(m, 0, 4, 8) +
			CheckThreeInALine(m, 2, 4, 6);
	}

	public int CheckThreeInALine(string m, int a, int b, int c)
	{
		return board[a] == m && board[b] == m && board[c] == m ? 1 : 0;
	}

	private int CheckAllTwoInALine(string m)
	{
		return
			CheckTwoInALine(m, 0, 1, 2) +
			CheckTwoInALine(m, 3, 4, 5) +
			CheckTwoInALine(m, 6, 7, 8) +
			CheckTwoInALine(m, 0, 3, 6) +
			CheckTwoInALine(m, 1, 4, 7) +
			CheckTwoInALine(m, 2, 5, 8) +
			CheckTwoInALine(m, 0, 4, 8) +
			CheckTwoInALine(m, 2, 4, 6);
	}

	public int CheckTwoInALine(String m, int a, int b, int c)
	{
		return
			(board[a] == m && board[b] == m && IsEmpty(c)) ||
			(board[a] == m && IsEmpty(b) && board[c] == m) ||
			(IsEmpty(a) && board[b] == m && board[c] == m) ? 1 : 0;
	}

	private int CheckAllOneInALine(string m)
	{
		return CheckOneInALine(board, m, 0, 1, 2) +
					 CheckOneInALine(board, m, 3, 4, 5) +
					 CheckOneInALine(board, m, 6, 7, 8) +
					 CheckOneInALine(board, m, 0, 3, 6) +
					 CheckOneInALine(board, m, 1, 4, 7) +
					 CheckOneInALine(board, m, 2, 5, 8) +
					 CheckOneInALine(board, m, 0, 4, 8) +
					 CheckOneInALine(board, m, 2, 4, 6);
	}

	public int CheckOneInALine(String[] board, String mark, int a, int b, int c)
	{
		return
			(IsEmpty(a) && IsEmpty(b) && board[c] == mark) ||
			(IsEmpty(a) && board[b] == mark && IsEmpty(c)) ||
			(board[a] == mark && IsEmpty(b) && IsEmpty(c)) ? 1 : 0;
	}

	private List<int> GetNextPossibleMoves()
	{
		var ls = new List<int>();

		for (int i = 0; i < board.Length; i++)
		{
			if (IsEmpty(i)) { ls.Add(i); }
		}

		return ls;
	}
}
