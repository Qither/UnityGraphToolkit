namespace NPBehave
{
    public abstract class RuntimeTree
    {
        /// <summary>
        /// NP行为树根结点
        /// </summary>
        private Root m_RootNode;

        /// <summary>
        /// 所归属的数据块
        /// </summary>
        public NPBehaveData OwnerData { get; }

        /// <summary>
        /// 所归属的Unit
        /// </summary>
        public string OwnerUnitId { get; }

        public Clock GetClock { get; }

        public RuntimeTree(string unitId, NPBehaveData data, Clock clock)
        {
            this.OwnerData   = data;
            this.OwnerUnitId = unitId;
            this.GetClock    = clock;
        }

        /// <summary>
        /// 设置根结点
        /// </summary>
        /// <param name="rootNode"></param>
        public void SetRootNode(Root rootNode)
        {
            this.m_RootNode = rootNode;
        }

        public Root GetRoot()
        {
            return this.m_RootNode;
        }

        /// <summary>
        /// 获取黑板
        /// </summary>
        /// <returns></returns>
        public Blackboard GetBlackboard()
        {
            if (this.m_RootNode == null)
            {
                Log.Error($"behavior tree {this.OwnerUnitId} root node is null");
                return null;
            }
            if (this.m_RootNode.Blackboard == null)
            {
                Log.Error($"behavior tree {this.OwnerUnitId} blackboard instance is null");
                return null;
            }
            return this.m_RootNode.Blackboard;
        }

        /// <summary>
        /// 开始运行行为树
        /// </summary>
        public void Start()
        {
            this.m_RootNode.Start();
        }
        
        /// <summary>
        /// 结束运行行为树
        /// </summary>
        public void Stop()
        {
            if (this.m_RootNode.IsActive)
            {
                this.m_RootNode.Stop();
            }
        }
    }
}