using System;
using System.Collections.Generic;
using System.IO;

namespace GeneticsProject
{
    public struct GeneticData
    {
        public string name;
        public string organism;
        public string formula;
    }

    class Program
    {
        static List<GeneticData> data = new List<GeneticData>();

        static string GetFormula(string proteinName)
        {
            foreach (GeneticData item in data)
            {
                if (item.name.Equals(proteinName)) return item.formula;
            }
            return null;
        }

        static void ReadGeneticData(string filename)
        {
            StreamReader reader = new StreamReader(filename);
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] fragments = line.Split('\t');
                GeneticData protein;
                protein.name = fragments[0];
                protein.organism = fragments[1];
                protein.formula = fragments[2];
                data.Add(protein);
            }
            reader.Close();
        }

        static void ReadHandleCommands(string filename, string outputFileName)
        {
            using (StreamWriter writer = new StreamWriter(outputFileName))
            {
                writer.WriteLine("Egor Ohremchuk");
                writer.WriteLine("Genetic Searching");
                writer.WriteLine("--------------------------------------------------------------------------");

                StreamReader reader = new StreamReader(filename);
                int counter = 0;
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    counter++;
                    string[] command = line.Split('\t');

                    if (command[0].Equals("search"))
                    {
                        writer.WriteLine($"{counter.ToString("D3")}   search   {Decoding(command[1])}");
                        int index = Search(command[1]);
                        if (index != -1)
                        {
                            writer.WriteLine("organism\t\t\tprotein");
                            writer.WriteLine($"{data[index].organism}\t\t\t{data[index].name}");
                        }
                        else
                        {
                            writer.WriteLine("organism\t\t\tprotein");
                            writer.WriteLine("NOT FOUND");
                        }
                    }
                    else if (command[0].Equals("diff"))
                    {
                        writer.WriteLine($"{counter.ToString("D3")}   diff   {command[1]}   {command[2]}");
                        int diffCount = Diff(command[1], command[2]);
                        if (diffCount >= 0)
                            writer.WriteLine($"amino-acids difference:\n{diffCount}");
                        else
                            writer.WriteLine("MISSING: one or both proteins not found");
                    }
                    else if (command[0].Equals("mode"))
                    {
                        writer.WriteLine($"{counter.ToString("D3")}   mode   {command[1]}");
                        string mostCommonAminoAcid = Mode(command[1]);
                        writer.WriteLine("amino-acid occurs:");
                        if (mostCommonAminoAcid != null)
                            writer.WriteLine($"{mostCommonAminoAcid}");
                        else
                            writer.WriteLine("MISSING");
                    }
                    writer.WriteLine("--------------------------------------------------------------------------");
                }
                reader.Close();
            }
        }

        static string Decoding(string formula)
        {
            string decoded = String.Empty;
            for (int i = 0; i < formula.Length; i++)
            {
                if (char.IsDigit(formula[i]))
                {
                    char letter = formula[i + 1];
                    int conversion = formula[i] - '0';
                    decoded += new string(letter, conversion);
                    i++;
                }
                else decoded += formula[i];
            }
            return decoded;
        }

        static int Search(string amino_acid)
        {
            string decoded = Decoding(amino_acid);
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].formula.Contains(decoded)) return i;
            }
            return -1;
        }

        static int Diff(string protein1, string protein2)
        {
            string formula1 = Decoding(GetFormula(protein1));
            string formula2 = Decoding(GetFormula(protein2));

            if (formula1 == null || formula2 == null) return -1;

            int diffCount = 0;
            int minLength = Math.Min(formula1.Length, formula2.Length);

            for (int i = 0; i < minLength; i++)
            {
                if (formula1[i] != formula2[i])
                {
                    diffCount++;
                }
            }
            diffCount += Math.Abs(formula1.Length - formula2.Length);

            return diffCount;
        }

        static string Mode(string proteinName)
        {
            string formula = GetFormula(proteinName);
            if (formula == null) return null;

            Dictionary<char, int> frequency = new Dictionary<char, int>();
            foreach (char ch in formula)
            {
                if (frequency.ContainsKey(ch)) frequency[ch]++;
                else frequency[ch] = 1;
            }

            int maxCount = 0;
            char mostCommonAminoAcid = ' ';
            foreach (var item in frequency)
            {
                if (item.Value > maxCount || (item.Value == maxCount && item.Key < mostCommonAminoAcid))
                {
                    maxCount = item.Value;
                    mostCommonAminoAcid = item.Key;
                }
            }

            return $"{mostCommonAminoAcid}\t\t  {maxCount}";
        }

        static void Main(string[] args)
        {
            ReadGeneticData("sequences.1.txt");
            ReadHandleCommands("commands.1.txt", "genedata.1.txt");
        }
    }
}
