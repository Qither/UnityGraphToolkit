using System;

namespace RedDotSystem.Runtime
{
    public interface IRedDotRule
    {
        TrieNode<(RedDotData nodeData, int nodeValue)> Order { get; }

        void Setup(RedDotService service, TrieNode<(RedDotData nodeData, int nodeValue)> node);
        
        void OnStart();

        void Execute(TrieNode<(RedDotData nodeData, int nodeValue)> executeNode, RedDotExecuteDelegate nodeExecute);
    }
}