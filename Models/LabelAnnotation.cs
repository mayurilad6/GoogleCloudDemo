namespace HCIHelp.Models
{
    public class LabelAnnotation
    {
        public string mid { get; set; }
        public string description { get; set; }
        public double score { get; set; }
        public double topicality { get; set; }
    }

    public class Response
    {
        public List<LabelAnnotation> labelAnnotations { get; set; }
    }

    public class Root
    {
        public List<Response> responses { get; set; }
    }
}
