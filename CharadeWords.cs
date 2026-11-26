namespace asdfasdf;

[Serializable]
public class CharadeWords
{
    public List<CharadeWord> words { get; set; }
}

[Serializable]
public class CharadeWord(int id, string text)
{
    public int Id { get; set; } = id;
    public int Difficulty { get; set; }
    public string EN_US { get; set; } = text;
}