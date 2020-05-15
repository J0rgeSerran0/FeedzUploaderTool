namespace FeedzUploader
{
    public class CheckArgumentsResult
    {
        public bool IsReady { get; }
        public bool ShowHelp { get; }
        public string Message { get; }

        public CheckArgumentsResult(bool isReady)
        {
            IsReady = isReady;
            ShowHelp = true;
        }

        public CheckArgumentsResult(bool isReady, bool showHelp, string message)
        {
            IsReady = isReady;
            ShowHelp = showHelp;
            Message = message;
        }
    }
}