using CommandLine;
using System.Text;

namespace NiohDecryptor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var arguments = CommandLine.Parser.Default.ParseArguments<Options>(args);
            RunOptions(arguments.Value);
        }

        static void RunOptions(Options options)
        {
            if (options.InFile != null)
            {
                if (options.Nioh1)
                {
                    byte[] bytes = Decrypt(File.ReadAllBytes(options.InFile), "u2ms9b6c954lz0hy", options.Trim);
                    string ext = GetExtention(bytes, options.Trim);
                    File.WriteAllBytes(Path.ChangeExtension(options.InFile, ext), bytes);
                    Console.WriteLine($"Wrote {bytes.Length} bytes to {Path.ChangeExtension(options.InFile, ext)}");
                }
                else if (options.Nioh2)
                {
                    byte[] bytes = File.ReadAllBytes(options.InFile);
                    string header = Encoding.UTF8.GetString(bytes[..32]);

                    if (header != "M$w$U4mbUZVm$x6&Nk%6.zpS6(kCJYpn" && options.NoCheck == false)
                    {
                        Console.WriteLine("File Header does not match Nioh 2's encryption header skipping, use \"--NoCheck\" to bypass this");
                        return;
                    }

                    bytes = Decrypt(bytes, "dqPXh2L2J5shiDV9", options.Trim);
                    string ext = GetExtention(bytes, options.Trim);
                    File.WriteAllBytes(Path.ChangeExtension(options.InFile, ext), bytes);
                    Console.WriteLine($"Wrote {bytes.Length} bytes to {Path.ChangeExtension(options.InFile, ext)}");

                }
                else
                {
                    if (options.Key != null)
                    {
                        byte[] bytes = Decrypt(File.ReadAllBytes(options.InFile), options.Key, options.Trim);
                        string ext = GetExtention(bytes, options.Trim);
                        File.WriteAllBytes(Path.ChangeExtension(options.InFile, ext), bytes);
                        Console.WriteLine($"Wrote {bytes.Length} bytes to {Path.ChangeExtension(options.InFile, ext)}");
                    }
                }
            }
        }

        static byte[] Decrypt(byte[] data, string key, bool notrim)
        {
            Console.WriteLine($"Decrypting file with key {key}...");
            List<byte> result = new List<byte>();
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] != 0x0)
                {
                    byte tmp = (byte)(data[i] ^ key.ToCharArray()[i%16]);
                    if (tmp != 0x0)
                        result.Add(tmp);
                    else
                        result.Add(data[i]);
                }
                else
                    result.Add(data[i]);
            }
            if (notrim == false)
            {
                Console.WriteLine("Trimming encyption header...");
                return result.ToArray()[32..];
            }
            else
                return result.ToArray();
        }

        static string GetExtention(byte[] data, bool notrim)
        {
            string longmagic = Encoding.UTF8.GetString(data[..8]);
            string magic = Encoding.UTF8.GetString(data[..4]);
            string ext;

            if (notrim == true)
            {
                longmagic = Encoding.UTF8.GetString(data[32..40]);
                magic = Encoding.UTF8.GetString(data[32..36]);
            }

            if (longmagic == "MDLRESPK" || longmagic == "MDLTEXPK")
                ext = ".pg1m";
            else if (longmagic == "G1M_PACK")
                ext = ".g1mpack";
            else if (magic == "_M1G")
                ext = ".g1m";
            else if (magic == "GT1G")
                ext = ".g1t";
            else
                ext = ".decbin";

            if (ext != ".decbin")
                Console.WriteLine($"Using extension {ext} from file magic...");
            return ext;
        }
    }

    public class Options
    {
        [Option('k', "key", HelpText = "custom XOR Key used for decryption")]
        public string? Key { get; set; }

        [Option("nioh1", HelpText = "Uses the default XOR key for Nioh 1 for decryption", SetName = "Game")]
        public bool Nioh1 { get; set; }

        [Option("nioh2", HelpText = "Uses the default XOR key for Nioh 2 for decryption", SetName = "Game")]
        public bool Nioh2 { get; set; }

        [Option('t', "NoTrim", HelpText = "Disables trimming the first 32 bytes used to tell the game the file is encrypt")]
        public bool Trim { get; set; }

        [Option('c', "NoCheck", HelpText = "Bypasses encyption header check and decypts file regardless")]
        public bool NoCheck { get; set; }

        [Option('i', "infile", HelpText = "Input File to decrypt")]
        public string? InFile { get; set; }
    }
}