using FollowingChecker;

if (args.Length == 1)
{
    var userName = args[0];
    var followers = await ExtractHelper.ExtractFollow($"https://github.com/{userName}?tab=followers");
    var following = await ExtractHelper.ExtractFollow($"https://github.com/{userName}?tab=following");
    following.ExceptWith(followers);
    Console.WriteLine($"关注了对方而没被对方关注的:\n[ {string.Join(" || ", following)} ]");
}
else
{
    throw new Exception("参数个数错误,请检查!");
}