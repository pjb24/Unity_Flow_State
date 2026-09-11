using System.Collections.Generic;

namespace FlowState.Runtime.Features
{
    public class InfinitePatternCatalog
    {
        private readonly Dictionary<string, InfinitePatternDefinition> _patterns =
            new Dictionary<string, InfinitePatternDefinition>();
        private readonly List<InfinitePatternDefinition> _orderedPatterns =
            new List<InfinitePatternDefinition>();

        public int Count => _patterns.Count;

        public bool Initialize(
            IReadOnlyList<InfinitePatternDefinition> definitions)
        {
            _patterns.Clear();
            _orderedPatterns.Clear();

            if (definitions == null || definitions.Count == 0)
            {
                return false;
            }

            for (int i = 0; i < definitions.Count; i++)
            {
                InfinitePatternDefinition definition = definitions[i];

                if (definition == null ||
                    !definition.IsInitialized ||
                    _patterns.ContainsKey(definition.Id))
                {
                    _patterns.Clear();
                    _orderedPatterns.Clear();
                    return false;
                }

                _patterns.Add(definition.Id, definition);
                _orderedPatterns.Add(definition);
            }

            return true;
        }

        public bool Contains(string patternId)
        {
            return !string.IsNullOrWhiteSpace(patternId) &&
                   _patterns.ContainsKey(patternId);
        }

        public bool TryGet(
            string patternId,
            out InfinitePatternDefinition definition)
        {
            definition = null;

            return !string.IsNullOrWhiteSpace(patternId) &&
                   _patterns.TryGetValue(patternId, out definition);
        }

        public bool CanConnect(string previousPatternId, string nextPatternId)
        {
            if (!TryGet(
                    previousPatternId,
                    out InfinitePatternDefinition previousPattern) ||
                !TryGet(
                    nextPatternId,
                    out InfinitePatternDefinition nextPattern))
            {
                return false;
            }

            return previousPattern.CanConnectTo(nextPattern);
        }

        public bool TryGetAt(
            int index,
            out InfinitePatternDefinition definition)
        {
            definition = null;

            if (index < 0 || index >= _orderedPatterns.Count)
            {
                return false;
            }

            definition = _orderedPatterns[index];
            return true;
        }
    }
}
