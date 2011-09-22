using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Bao;
using System.Linq;
using MiniMaxTree;
using System.Collections.Generic;

namespace BaoGame
{
    public class AbsAI
    {
        private int [][] _probabilityDistribution;

        private int[][] _lookUp;

        private GTree<BaoMinimaxElement> _minimaxTree;

        private int _level;

        private int _pli;

        private Random _d100;

        Random randNum = new Random(DateTime.Now.Millisecond);
  
        // in relatà andrebbe l'oggetto andrebbe costruito con una factory che legga ad esempio da file xml, ora l'inizializzazione
        // la cablo dentro il costruttore. La classe dovrebbe anche essere un singleton
        public AbsAI ()
        {
            // TODO: solo la prima e seconda distribuzione hanno senso...
            _probabilityDistribution = new int[4][] { new int[]{100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                                                      new int[]{16, 16, 9, 9, 9, 7, 7, 7, 5, 3, 3, 3, 2, 2, 2, 0, 0, 0, 0, 0},
                                                      new int[]{16, 16, 9, 9, 9, 7, 7, 7, 5, 3, 3, 3, 2, 2, 2, 0, 0, 0, 0, 0},
                                                      new int[]{5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5  } };

            _lookUp = new int[4][];

            for (int i = 0; i < 4; i++)
            {
                int nLookUpIndex = 0;
                _lookUp[i] = new int [100];

                for (int j = 0; j < 20; j++)
                {
                    int nSamples = _probabilityDistribution[i][j];

                    for (int k = 0; k < nSamples; k++)
                    {
                        _lookUp[i][nLookUpIndex] = j;
                        nLookUpIndex++;
                    }
                }
            }

            // initialize dice seed
            _d100 = new Random(DateTime.Now.Millisecond);
        }

        public void InitializeTree(BaoGameState actualGameState)
        {
            _minimaxTree = new GTree<BaoMinimaxElement>(new BaoMinimaxElement(actualGameState));
        }

        public int Pli { get { return _pli; } set { _pli = value; } }

        public int Level { get { return _level; } set { _level = value; } }


        public BaoMove MiniMax()
        {
            // minimax forward tree creation
            int numOfStates = 1;    // for testing and debug and possible extension
            Byte currPlayer = 2;    // CPU move first, then player 1 countermove and so on, until pli depth

            bool newBranch;         // begins true if there is at least one branch from the current layer
            int depth = 0;          // depth of the tree (root is at depth 0)
            do
            {
                newBranch = false;

                foreach (Node<BaoMinimaxElement> node in _minimaxTree.Layers[depth])
                {
                    BaoGameState gs = node.Content.gameState;

                    switch (gs.WinLoseCondition(currPlayer))
                    {
                        case WinCondition.Player1:
                            node.Content.Score = float.MinValue;
                            break;

                        case WinCondition.Player2:
                            node.Content.Score = float.MaxValue;
                            break;

                        case WinCondition.none:
                            List<BaoGameState> derivedStates = gs.GenerateDerivedStates(currPlayer);

                            foreach (BaoGameState ds in derivedStates)
                            {
                                BaoMinimaxElement e = new BaoMinimaxElement(ds);
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
            while (depth < _pli && newBranch);

            // ok forward state is done, I evaluate all the nodes in the deepest layer
            foreach (Node<BaoMinimaxElement> n in _minimaxTree.Layers[_minimaxTree.Layers.Count - 1])
            {
                n.Content.Score = n.Content.gameState.EvaluateGameState();
            }

            // now I climb up the tree, resolving minimax
            for (int backDepth = depth - 1; backDepth >= 1; backDepth--)
            {
                // min or max?
                currPlayer = _minimaxTree.Layers[backDepth][0].Content.gameState.Player;

                if (currPlayer == 1)    // maximize
                {
                    foreach (var node in _minimaxTree.Layers[backDepth].Where(node => node.Children.Count > 0))
                    {
                        node.Content.Score = node.Children.Max(n => n.Content.Score);
                    }

                }
                else                    // minimize
                {
                    foreach (var node in _minimaxTree.Layers[backDepth].Where(node => node.Children.Count > 0))
                    {
                        node.Content.Score = node.Children.Min(n => n.Content.Score);
                    }
                }
            }

            // Now I am in the root, I calculate the last argmax, that will be the best possible move!
            /*float maxScore;
            BaoMove CPUMove = null;
            maxScore = float.MinValue;
            foreach (Node<BaoMinimaxElement> node in _minimaxTree.Root.Children)
            {
                if (node.Content.Score >= maxScore)
                {
                    CPUMove = node.Content.gameState.Move;
                    maxScore = node.Content.Score;
                }
            }

            return CPUMove;*/

            var sortedMoves = from Node<BaoMinimaxElement> nodes in _minimaxTree.Root.Children
                              orderby nodes.Content.Score descending
                              select nodes.Content;

            List<BaoMinimaxElement> sortedMovesList = sortedMoves.ToList();

            // prima di tirare il dado elimino le mosse palesemente sbagliate! (posso gestire questa funzione parametrizzata dal livello di difficoltà)

            int nPossibleMoves = sortedMoves.Count();
            int nDiceResult = _d100.Next(0, 100);   // 0 included - 100 excluded
            int nMoveScore = _lookUp[_level][nDiceResult] + 1;  // range is 1 - 20

            // nMoveScore : 20 = nMoveOrder : nPossibleMoves
            int nMoveOrder = (int)( (nMoveScore * nPossibleMoves) / 20) + 1;
            int nMoveIndex = nMoveOrder - 1;

            // bisogna esaminare questa mossa, se porta alla cattura della casa si deve escludere (solo per la kujifunza)
            while (nMoveIndex > 0 && sortedMovesList[nMoveIndex].gameState.MoveToAvoidFlag == 1)
            {
                nMoveIndex--;
            }

            return sortedMovesList[nMoveIndex].gameState.Move;
        }
            
    }
}
