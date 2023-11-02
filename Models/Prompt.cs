namespace HCIHelp.Models
{
    public class Prompt
    {
        public List<Instance> instances { get; set; }
        public Parameters parameters { get; set; }
    }

    public class Instance
    {
        public string content { get; set; }
    }

    public class Parameters
    {
        public double temperature { get; set; }
        public int maxOutputTokens { get; set; }
        public double topP { get; set; }
        public int topK { get; set; }
        public int candidateCount { get; set; }
    }

}

