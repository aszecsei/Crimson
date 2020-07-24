using System;
using Crimson.Collections;

namespace Crimson.AI.GOAP
{
    public class AStarStorage
    {
        private const int MAX_NODES = 128;

        private readonly AStarNode?[] _opened = new AStarNode[MAX_NODES];
        private readonly AStarNode?[] _closed = new AStarNode[MAX_NODES];
        private int _numOpened;
        private int _numClosed;
        private int _lastFoundOpened;
        private int _lastFoundClosed;
        
        internal AStarStorage() {}

        public void Clear()
        {
            for (var i = 0; i < _numOpened; ++i)
            {
                Pool<AStarNode>.Free(_opened[i]!);
                _opened[i] = null;
            }

            for (var i = 0; i < _numClosed; ++i)
            {
                Pool<AStarNode>.Free(_closed[i]!);
                _closed[i] = null;
            }

            _numOpened = _numClosed = 0;
            _lastFoundClosed = _lastFoundOpened = 0;
        }

        public AStarNode? FindOpened(AStarNode node)
        {
            for (var i = 0; i < _numOpened; i++)
            {
                if (_opened[i]!.Action!.IsSatisfied(node.WorldState))
                {
                    _lastFoundClosed = i;
                    return _closed[i];
                }
            }

            return null;
        }
        
        public AStarNode? FindClosed(AStarNode node)
        {
            for (var i = 0; i < _numClosed; i++)
            {
                if ((_closed[i]!.Action!.IsSatisfied(node.WorldState)))
                {
                    _lastFoundClosed = i;
                    return _closed[i];
                }
            }

            return null;
        }


        public bool HasOpened()
        {
            return _numOpened > 0;
        }


        public void RemoveOpened(AStarNode node)
        {
            if (_numOpened > 0)
                _opened[_lastFoundOpened] = _opened[_numOpened - 1];
            _numOpened--;
        }


        public void RemoveClosed(AStarNode node)
        {
            if (_numClosed > 0)
                _closed[_lastFoundClosed] = _closed[_numClosed - 1];
            _numClosed--;
        }


        public bool IsOpen(AStarNode node)
        {
            return Array.IndexOf(_opened, node) > -1;
        }


        public bool IsClosed(AStarNode node)
        {
            return Array.IndexOf(_closed, node) > -1;
        }


        public void AddToOpenList(AStarNode node)
        {
            _opened[_numOpened++] = node;
        }


        public void AddToClosedList(AStarNode node)
        {
            _closed[_numClosed++] = node;
        }


        public AStarNode? RemoveCheapestOpenNode()
        {
            var lowestVal = int.MaxValue;
            _lastFoundOpened = -1;
            for (var i = 0; i < _numOpened; i++)
            {
                if (_opened[i]!.CostSoFarAndHeuristicCost < lowestVal)
                {
                    lowestVal = _opened[i]!.CostSoFarAndHeuristicCost;
                    _lastFoundOpened = i;
                }
            }

            if (_lastFoundOpened > -1)
            {
                var val = _opened[_lastFoundOpened];
                RemoveOpened(val!);

                return val;
            }

            return null;
        }
    }
}