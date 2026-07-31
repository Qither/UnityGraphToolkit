namespace RedDotSystem.Editor
{
    public class RedDotExecuteNodeCodeInfo
    {
        public string FileName;
        
        public string FunctionName;

        public int StartLine;
        
        public int EndLine;
        
        public RedDotExecuteNodeCodeInfo(string fileName, string functionName, int startLine, int endLine)
        {
            this.FileName     = fileName;
            this.FunctionName = functionName;
            this.StartLine    = startLine;
            this.EndLine      = endLine;
        }
    }
}