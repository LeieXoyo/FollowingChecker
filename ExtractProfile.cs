using System.Text.RegularExpressions;

static class ExtractProfile
{

    private static readonly HttpClient httpClient = new HttpClient();
    
    internal static async Task<HashSet<string>> ExtractFollow(string url)
    {
        var responseBody = await getResponseBody(url);
        var results = new HashSet<string>();
        var followsPattern = new Regex(@"<span class=""Link--secondary.*?"">(.*?)</span>");
        var nextButtonPattern = new Regex(@"Previous</\w*?><a rel=""nofollow"" href=""(https://github.com/.*?)"">Next</a>");
        var matches = followsPattern.Matches(responseBody);
        foreach (Match match in matches)
        {
            results.Add(match.Groups[1].Value);
        }
        var nextButtonMatch = nextButtonPattern.Match(responseBody);
        while (nextButtonMatch.Success)
        {
            responseBody = await getResponseBody(nextButtonMatch.Groups[1].Value);
            matches = followsPattern.Matches(responseBody);
            foreach (Match match in matches)
            {
                results.Add(match.Groups[1].Value);
            }
            nextButtonMatch = nextButtonPattern.Match(responseBody);
        }
        return results;
    }
    
    private static async Task<string> getResponseBody(string url)
    {
        string responseBody = string.Empty;
        while (responseBody == string.Empty)
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                responseBody = await response.Content.ReadAsStringAsync();   
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine($"Message: {e.Message}");
                Thread.Sleep(1000);
            }
        }
        return responseBody;
    }
}