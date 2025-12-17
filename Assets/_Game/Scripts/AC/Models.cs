using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

[Serializable]
public struct TrackingData
{
    public int numOfBlock;
    public int numX1OfLine;
    public int numX2OfLine;
    public int numX3OfLine;
    public int numX4OfLine;
    public int numX5OfLine;
    public int numX6OfLine;
    public int expOfLuckyWheel;
    public int numDropBlock;
    public int mulScoreBlockCount;
    public int mulScoreExp;
    public int singleScoreExp;
    public int singleScoreBlockCount;
    public int expOfSection;

    public void Reset()
    {
        this.numDropBlock = 0;
        this.expOfLuckyWheel = 0;
        this.numX1OfLine = 0;
        this.numX2OfLine = 0;
        this.numX3OfLine = 0;
        this.numX4OfLine = 0;
        this.numX5OfLine = 0;
        this.numX6OfLine = 0;
        this.numOfBlock = 0;
        this.mulScoreExp = 0;
        this.mulScoreBlockCount = 0;
        this.expOfSection = 0;
        this.singleScoreExp = 0;
        this.singleScoreBlockCount = 0;
    }

    public TrackingData DeepClone()
    {
        return new TrackingData()
        {
            numOfBlock = this.numOfBlock,
            numX1OfLine = this.numX1OfLine,
            numX2OfLine = this.numX2OfLine,
            numX3OfLine = this.numX3OfLine,
            numX4OfLine = numX4OfLine,
            numX5OfLine = this.numX5OfLine,
            numX6OfLine = numX6OfLine,
            expOfLuckyWheel = expOfLuckyWheel,
            numDropBlock = numDropBlock,
            mulScoreBlockCount = mulScoreBlockCount,
            mulScoreExp = mulScoreExp,
            singleScoreExp = singleScoreExp,
            singleScoreBlockCount = singleScoreBlockCount,
            expOfSection = expOfSection
        };
    }

    public override string ToString()
    {
        return $"num Of Block: {numOfBlock}\n" +
               $"num Of Line: {numX1OfLine}: {numX2OfLine}: {numX3OfLine}: {numX4OfLine}: {numX5OfLine}: {numX6OfLine}\n" +
               $"exp Lucky Wheel: {expOfLuckyWheel}\n" +
               $"num Of Drop: {numDropBlock}\n";
    }

    [System.Reflection.Obfuscation(Exclude = false)]
    public string GetH()
    {
        var json = JsonConvert.SerializeObject(this, Formatting.None);
        using SHA256 sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(json));
        var sb = new StringBuilder();
        foreach (var b in bytes)
        {
            sb.Append(b.ToString("x2"));
        }

        return sb.ToString();
    }
}

[Serializable]
public struct TrackingAdData
{
    public double bannerRev;
    public int bannerCount;
    public double interRev;
    public int interCount;
    public double rewardRev;
    public int rewardCount;
    public double mrecRev;
    public int mrecCount;
    public double openRev;
    public int openCount;

    public void Reset()
    {
        this.bannerRev = 0;
        this.interRev = 0;
        this.rewardRev = 0;
        this.mrecRev = 0;
        this.openRev = 0;
        bannerCount = 0;
        interCount = 0;
        rewardCount = 0;
        mrecCount = 0;
        openCount = 0;
    }

    [System.Reflection.Obfuscation(Exclude = false)]
    public string GetH()
    {
        var dict = new Dictionary<string, object>()
        {
            ["bannerCount"] = this.bannerCount,
            ["interCount"] = this.interCount,
            ["rewardCount"] = this.rewardCount,
            ["mrecCount"] = this.mrecCount,
            ["openCount"] = this.openCount
        };
        var json = JsonConvert.SerializeObject(dict, Formatting.None);
        using SHA256 sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(json));
        var sb = new StringBuilder();
        foreach (var b in bytes)
        {
            sb.Append(b.ToString("x2"));
        }
        return sb.ToString();
    }
}