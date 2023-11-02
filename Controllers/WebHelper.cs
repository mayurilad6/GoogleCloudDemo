using Google.Cloud.Vision.V1;
using HCIHelp.Models;
using Newtonsoft.Json;
using System.Net;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.AIPlatform.V1;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Text;

namespace HCIHelp.wwwroot
{
    public class WebHelper
    {
        public static bool GetImageLabels(string base64String, string bearerToken ,out string hashTags)
        {
            try
            {


                var imageParts = base64String.Split(',').ToList<string>();
                byte[] imageBytes = Convert.FromBase64String(base64String);

                using (var client = new WebClient())
                {
                    MainRequests mainrequests = new MainRequests()
                    {
                        requests = new List<requests>()
                {
                     new requests()
                {
                     image = new image()
                     {
                     content = base64String
                 },

                 features = new List<features>()
                 {
                     new features()
                     {
                         type = "LABEL_DETECTION",
                     }

                 }
             }

                    }

                    };

                    //var cc = ImageAnnotatorClient.Create();
                    //var im = Image.FromBytes(imageBytes);
                    //var labels = cc.DetectLabels(im);


                    var uri = "https://vision.googleapis.com/v1/images:annotate?key=" + "AIzaSyD0aIhUyxEU58YLcUcg0-1yvqsNOkOXk-E";
                    client.Headers.Add("Content-Type:application/json");
                    client.Headers.Add("Accept:application/json");
                    var jsonString = client.UploadString(uri, JsonConvert.SerializeObject(mainrequests));


                    Root root = JsonConvert.DeserializeObject<Root>(jsonString);
                    if (root == null)
                    {
                        hashTags = "";
                        return false;
                    }

                    string concatenatedLabels = "";
                    foreach (var response in root?.responses)
                    {
                        foreach (var labelAnnotation in response?.labelAnnotations)
                        {
                            concatenatedLabels += labelAnnotation.description?.ToLower() + ",";
                        }
                    }

                    List<string> AllowedTags = new List<string>();
                    AllowedTags = listofAllowedIssues();
                    string[] items = concatenatedLabels.Split(',');
                    bool IsEligible = false;
                    foreach (var item in items)
                    {
                        if (AllowedTags.Contains(item))
                        {
                            IsEligible = true;
                            break;
                        }
                    }
                    hashTags = "#" + concatenatedLabels?.Replace(",", "#");
                    return IsEligible;


                }
            }catch (Exception ex)
            {
                hashTags = "";
                return false;
            }
        }
        public static List<string> listofAllowedIssues()
        {
            List<string> list = new List<string>() {"lightening","tornado","thunder", "atmosphere", "water", "watercourse", "earthquake", "hazard", "pollution", "fire", "flame", "heat", "drought", "dry", "agriculture", "farm", "land" };
            return list;
        }
        public static async Task<string> GetHashTags(string bearerToken,string concatenatedLabels)
        {
            string promptConent = "Given the following text, generate an engaging and interesting tweet. Include popular names and famous person to increase coverage :\n\n " + concatenatedLabels;
            string hashTags = await GetCommentLabel(bearerToken, promptConent);
            return hashTags;
        }

        public static async Task<string> GetCommentLabel(string BearerToken, string PromptContent)
        {
            try
            {
                // Your bearer token
                string bearerToken = BearerToken;
                //await GetAccessToken(GSettingsPath);

                if (!String.IsNullOrEmpty(bearerToken))
                {

                    // Your API endpoint
                    string url = "https://us-central1-prediction-aiplatform.clients6.google.com/ui/projects/hcihelp/locations/us-central1/publishers/google/models/text-bison:predict?key=AIzaSyBWXhG3sG-bGVfNCwEP3OUpASd6bi1ckj4";

                    using (var client = new HttpClient())
                    {
                        // Set the bearer token in the Authorization header
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

                        Prompt p = new Prompt();
                        List<Instance> li = new List<Instance>();
                        Instance i = new Instance();
                        i.content = PromptContent;

                        li.Add(i);
                        p.instances = li;
                        Parameters parameterss = new Parameters();
                        parameterss.temperature = 0.2;
                        parameterss.maxOutputTokens = 1024;
                        parameterss.topP = 0.8;
                        parameterss.topK = 40;
                        parameterss.candidateCount = 1;
                        p.parameters = parameterss;
                        string jsonString = JsonConvert.SerializeObject(p);


                        // Your request body
                        var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                        // Send the POST request
                        var response = await client.PostAsync(url, content);
                        var responseString = await response.Content.ReadAsStringAsync();

                        VertexAIResponse vr = JsonConvert.DeserializeObject<VertexAIResponse>(responseString);

                        string label = vr?.predictions?.FirstOrDefault()?.content;

                        return label;
                    }
                }
                return "customer service";
            }
            catch (Exception ex)
            {
                return "customer service";
            }
        }

        public static async Task<string> GetAccessToken(string GSettingsPath)
        {
            try
            {
                // Load the service account key file
                var credential = GoogleCredential.FromFile(GSettingsPath);

                // Add the required scopes
                credential = credential.CreateScoped(new[] { "https://www.googleapis.com/auth/cloud-platform" });

                // Get an access token
                var response = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();

                return response;
            }
            catch (Exception ex)
            {
                string e = ex.Message;
                return null;
            }
        }




    }



    //public async Task MakePrediction()
    //{
    //    // Path to your service account key file
    //    string serviceAccountKeyFilePath = "path/to/your/service-account-key.json";

    //    // Authenticate with Google Cloud
    //    var credential = GoogleCredential.FromFile(serviceAccountKeyFilePath)
    //        .CreateScoped(PredictionServiceClient.DefaultScopes);

    //    // Create a client
    //    var client = await PredictionServiceClient.CreateAsync(credential);

    //    // TODO: Replace these with your own values
    //    string endpointId = "your-endpoint-id";
    //    string projectId = "your-project-id";
    //    string location = "your-location";  // e.g., "us-central1"

    //    // Create a request
    //    var request = new PredictRequest
    //    {
    //        EndpointAsEndpointName = EndpointName.FromProjectLocationEndpoint(projectId, location, endpointId),
    //        // TODO: Add your instances here
    //        Instances = { new ExampleInstance() }
    //    };

    //    // Make the prediction
    //    var response = await client.PredictAsync(request);

    //    // Handle the response
    //    // TODO: Add your own response handling logic here
    //}
}

