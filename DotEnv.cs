namespace token_generator_dotnet
{
  using System;
  using System.IO;
  using System.Collections.Generic;

  public static class DotEnv
  {
    public static List<string> test2 = new() {};
    public static void Load(string filePath)
    {
      if (!File.Exists(filePath))
      {
        return;
      }

      foreach (var line in File.ReadAllLines(filePath))
      {
        if (line.Contains('#'))
        {
          continue;
        }

        var parts = line.Split('=',StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 2)
        {
          continue;
        }

        Environment.SetEnvironmentVariable(parts[0], parts[1]);
        test2.Add(parts[1]);
      }
    }
  
    public static void testing()
    {
      Console.WriteLine("This is a test");
    }
  }
}