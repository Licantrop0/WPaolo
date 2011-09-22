using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaoUtil
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Insert pli");
            int pli = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("");
            Console.WriteLine("Let's go!");

            BaoGameState actualGameState = new BaoGameState(2);

            string[] sPlayerMove;
            bool playerMoveLeft;
            string cpuMoveLeft;
            BaoMove playerMove = null;
            BaoMove cpuMove = null;
            bool isPlayerMoveValid;
            List<BaoMove> playerPossibleMoves = null;
            List<BaoMove> cpuPossibleMoves = null;

            while (true)
            {
                // print actual board state
                actualGameState.Print();
                Console.WriteLine();

                // player move
                isPlayerMoveValid = false;
                while (!isPlayerMoveValid)
                {
                    playerPossibleMoves = actualGameState.GetPossibleMoves(1);
                    if(TestWinLoseCondition(actualGameState, 1, playerPossibleMoves))
                        goto ENDGAME;

                    Console.WriteLine("Player 1 insert your move");
                    sPlayerMove = Console.ReadLine().Split(' ');
                    playerMoveLeft = (sPlayerMove[2] == "l") ? true : false;
                    playerMove = new BaoMove((Byte)(Convert.ToByte(sPlayerMove[0]) - (Byte)1), Convert.ToByte(sPlayerMove[1]), playerMoveLeft);

                    foreach (BaoMove m in playerPossibleMoves)
                    {
                        if (m == playerMove)
                        {
                            isPlayerMoveValid = true;
                            break;
                        }
                    }

                    if (!isPlayerMoveValid)
                    {
                        Console.WriteLine("Incorrect move!");
                    }
                }

                // update board
                actualGameState.UpdateGameState(playerMove, 1);

                // Show board
                Console.WriteLine("Board after player move is:");
                Console.WriteLine();
                actualGameState.Print();
                Console.WriteLine();

                cpuPossibleMoves = actualGameState.GetPossibleMoves(2);
                if(TestWinLoseCondition(actualGameState, 2, cpuPossibleMoves))
                    goto ENDGAME;

                // now it's computer turn
                GTree<BaoMinimaxElement> minimaxTree = new GTree<BaoMinimaxElement>(new BaoMinimaxElement(actualGameState));
                GC.Collect();   // force the garbace collector to clean the memory
                cpuMove = MiniMax(minimaxTree, pli);

                cpuMoveLeft = cpuMove.left ? "l" : "r";

                Console.WriteLine("Computer move: "+ Convert.ToString(cpuMove.rowBox + 1) + " " + Convert.ToString(cpuMove.columnBox) + " " + cpuMoveLeft);

                // update board
                actualGameState.UpdateGameState(cpuMove, 2);

                Console.WriteLine("Board after cpu move is:");
            }

            ENDGAME:
            Console.WriteLine("Press any key to exit...");
            Console.Read();
        }

        /*public static BaoMove MiniMax(GTree<BaoMinimaxElement> minimaxTree, int pli)
        {
            float maxScore, minScore;

            // minimax forward tree creation
            int numOfStates = 1;    // for testing and debug
            Byte currPlayer = 2;    // CPU move first, then player 1 countermove and so on, until pli depth

            bool newBranch;         // begins true if there is at least one branch from the current layer
            int depth = 0;          // depth of the tree (root is at depth 0)
            do
            {
                newBranch = false;

                foreach (Node<BaoMinimaxElement> node in minimaxTree.Layers[depth])
                {
                    BaoGameState gs = node.Content.gameState;

                    List<BaoMove> possibleMoves = gs.GetPossibleMoves(currPlayer);

                    switch (gs.WinLoseCondition(currPlayer, possibleMoves))
                    {
                        case WinCondition.Player1:
                            node.Content.Score = float.MinValue;
                            break;

                        case WinCondition.Player2:
                            node.Content.Score = float.MaxValue;
                            break;

                        case WinCondition.none:
                            foreach (BaoMove m in possibleMoves)
                            {
                                BaoGameState ngs = gs.NewGameStateFromExisting(m, currPlayer);
                                BaoMinimaxElement e = new BaoMinimaxElement(ngs);
                                node.AddChild(e);
                                numOfStates++;
                            }

                            newBranch = true;
                            break;
                    }
                }

                if (currPlayer == 1)
                    currPlayer = 2;
                else
                    currPlayer = 1;

                depth++;
            }
            while (depth < pli && newBranch);

            // ok forward state is done, I evaluate all the nodes in the deepest layer
            foreach (Node<BaoMinimaxElement> n in minimaxTree.Layers[minimaxTree.Layers.Count - 1])
            {
                n.Content.Score = n.Content.gameState.EvaluateGameState();
            }

            // now I climb up the tree, resolving minimax
            for (int backDepth = depth - 1; backDepth >= 1; backDepth--)
            {
                // min or max?
                currPlayer = minimaxTree.Layers[backDepth][0].Content.gameState.Player;

                if (currPlayer == 1)    // maximize
                {
                    foreach (Node<BaoMinimaxElement> n in minimaxTree.Layers[backDepth])
                    {
                        maxScore = float.MinValue;
                        foreach (Node<BaoMinimaxElement> child in n.Children)
                        {
                            if (child.Content.Score >= maxScore)
                            {
                                maxScore = child.Content.Score;
                            }
                        }
                        n.Content.Score = maxScore;
                    }
                }
                else                // minimize
                {
                    foreach (Node<BaoMinimaxElement> n in minimaxTree.Layers[backDepth])
                    {
                        minScore = float.MaxValue;
                        foreach (Node<BaoMinimaxElement> child in n.Children)
                        {
                            if (child.Content.Score =< minScore)
                            {
                                minScore = child.Content.Score;
                            }
                        }
                        n.Content.Score = minScore;
                    }
                }
            }

            // Now I am in the root, I calculate the last argmax, that will be the best possible move!
            BaoMove CPUMove = null;
            maxScore = float.MinValue;
            foreach (Node<BaoMinimaxElement> node in minimaxTree.Root.Children)
            {
                if (node.Content.Score >= maxScore)
                {
                    CPUMove = node.Content.gameState.Move;
                    maxScore = node.Content.Score;
                }
            }

            return CPUMove;
        }*/

        public static BaoMove MiniMax(GTree<BaoMinimaxElement> minimaxTree, int pli)
        {
            float maxScore;

            // minimax forward tree creation
            int numOfStates = 1;    // for testing and debug
            Byte currPlayer = 2;    // CPU move first, then player 1 countermove and so on, until pli depth

            bool newBranch;         // begins true if there is at least one branch from the current layer
            int depth = 0;          // depth of the tree (root is at depth 0)
            do
            {
                newBranch = false;

                foreach (Node<BaoMinimaxElement> node in minimaxTree.Layers[depth])
                {
                    BaoGameState gs = node.Content.gameState;

                    List<BaoMove> possibleMoves = gs.GetPossibleMoves(currPlayer);

                    switch (gs.WinLoseCondition(currPlayer, possibleMoves))
                    {
                        case WinCondition.Player1:
                            node.Content.Score = float.MinValue;
                            break;

                        case WinCondition.Player2:
                            node.Content.Score = float.MaxValue;
                            break;

                        case WinCondition.none:
                            foreach (BaoMove m in possibleMoves)
                            {
                                BaoGameState ngs = gs.NewGameStateFromExisting(m, currPlayer);
                                BaoMinimaxElement e = new BaoMinimaxElement(ngs);
                                node.AddChild(e);
                                numOfStates++;
                            }

                            newBranch = true;
                            break;
                    }
                }

                currPlayer = (currPlayer == 1) ? (Byte)2 : (Byte)1;

                depth++;
            }
            while (depth < pli && newBranch);

            // ok forward state is done, I evaluate all the nodes in the deepest layer
            foreach (Node<BaoMinimaxElement> n in minimaxTree.Layers[minimaxTree.Layers.Count - 1])
            {
                n.Content.Score = n.Content.gameState.EvaluateGameState();
            }

            // now I climb up the tree, resolving minimax
            for (int backDepth = depth - 1; backDepth >= 1; backDepth--)
            {
                // min or max?
                currPlayer = minimaxTree.Layers[backDepth][0].Content.gameState.Player;

                if (currPlayer == 1)    // maximize
                {
                    foreach (var node in minimaxTree.Layers[backDepth].Where(node => node.Children.Count > 0))
                    {
                        node.Content.Score = node.Children.Max(n => n.Content.Score);
                    }
                        
                }
                else                    // minimize
                {
                    foreach (var node in minimaxTree.Layers[backDepth].Where(node => node.Children.Count >0))
                    {
                        node.Content.Score = node.Children.Min(n => n.Content.Score);
                    }
                }
            }

            // Now I am in the root, I calculate the last argmax, that will be the best possible move!
            BaoMove CPUMove = null;
            maxScore = float.MinValue;
            foreach (Node<BaoMinimaxElement> node in minimaxTree.Root.Children)
            {
                if (node.Content.Score >= maxScore)
                {
                    CPUMove = node.Content.gameState.Move;
                    maxScore = node.Content.Score;
                }
            }

            return CPUMove;
        }

        public static bool TestWinLoseCondition(BaoGameState actualGameState, Byte currPlayer, List<BaoMove> currPlayerPossibleMoves)
        {
            WinCondition wc = actualGameState.WinLoseCondition(currPlayer, currPlayerPossibleMoves);
            if (wc == WinCondition.Player1)
            {
                Console.WriteLine("Excellent, you beat me!");
                return true;
            }
            else if (wc == WinCondition.Player2)
            {
                Console.WriteLine("I won, you suck!!!");
                return true;
            }

            return false;
        }
    }


}
