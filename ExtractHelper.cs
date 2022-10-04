using System.Text.RegularExpressions;

namespace FollowingChecker;

internal static class ExtractHelper
{
    private static readonly HttpClient HttpClient = new HttpClient();
    
    internal static async Task<HashSet<string>> ExtractFollow(string url)
    {
        var responseBody = await GetResponseBody(url);
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
            responseBody = await GetResponseBody(nextButtonMatch.Groups[1].Value);
            matches = followsPattern.Matches(responseBody);
            foreach (Match match in matches)
            {
                results.Add(match.Groups[1].Value);
            }
            nextButtonMatch = nextButtonPattern.Match(responseBody);
        }
        return results;
    }
    
    private static async Task<string> GetResponseBody(string url)
    {
        var responseBody = string.Empty;
        while (responseBody == string.Empty)
        {
            try
            {
                HttpResponseMessage response = await HttpClient.GetAsync(url);
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