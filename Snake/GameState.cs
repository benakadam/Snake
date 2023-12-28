namespace Snake
{
    public class GameState
    {
        public int Rows { get; set; }
        public int Cols { get; set; }
        public GridValue[,] Grid { get; set; }
        public Direction Dir { get; set; }
        public int Score { get; set; }
        public bool GameOver { get; set; }

        private readonly LinkedList<Position> snakePositions = new LinkedList<Position>();
        private readonly Random random = new Random();

        public GameState(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
            Grid = new GridValue[rows, cols];
            Dir = Direction.Right;

            AddSnake();
            AddFood();
        }

        private void AddSnake()
        {
            int r = Rows / 2;

            for (int c = 1; c <= 3; c++)
            {
                Grid[r, c] = GridValue.Snake;
                snakePositions.AddFirst(new Position(r, c));  
            }
        }

        private IEnumerable<Position> EmptyPositions()
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    if (Grid[r, c] == GridValue.Empty)
                    {
                        yield return new Position(r, c);
                    }
                }

            }
        }

        private void AddFood()
        {
            List<Position> empty = new List<Position>(EmptyPositions());

            if (!empty.Any()) return;

            Position pos = empty[random.Next(empty.Count)];
            Grid[pos.Row, pos.Col] = GridValue.Food;
        }

        public Position HeadPosition() => snakePositions.First.Value;

        public Position TailPosition() => snakePositions.Last.Value;

        public IEnumerable<Position> SnakePositions() => snakePositions;

        private void AddHead(Position pos)
        {
            snakePositions.AddFirst(pos);
            Grid[pos.Row, pos.Col] = GridValue.Snake;
        }

        private void RemoveTail()
        {
            Position tail = snakePositions.Last.Value;
            Grid[tail.Row, tail.Col] = GridValue.Empty;
            snakePositions.RemoveLast();
        }

        public void ChangeDirection(Direction dir)
        {
            Dir = dir;
        }

        private bool OutSideGrid(Position pos)
            => pos.Row < 0 || pos.Row >= Rows || pos.Col < 0 || pos.Col >= Cols;

        private GridValue WillHit(Position newHeadPos)
        {
            if (OutSideGrid(newHeadPos)) return GridValue.Outside;
            if (newHeadPos == TailPosition()) return GridValue.Empty;

            return Grid[newHeadPos.Row, newHeadPos.Col];
        }

        public void Move()
        {
            Position newHeadPos = HeadPosition().Translate(Dir);
            GridValue hit = WillHit(newHeadPos);

            switch (hit)
            {
                case GridValue.Outside:
                case GridValue.Snake:
                    GameOver = true;
                    break;
                case GridValue.Empty:
                    RemoveTail();
                    AddHead(newHeadPos);
                    break;
                case GridValue.Food:
                    AddHead(newHeadPos);
                    Score++;
                    AddFood();
                    break;
            }
        }
    }
}
