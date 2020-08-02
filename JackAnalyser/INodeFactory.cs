namespace JackAnalyser
{
    public interface INodeFactory
    {
        Node Get(KeywordToken keyword);
    }
}
