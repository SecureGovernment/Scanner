using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SecureGovernment.Domain.Models
{
    public class Truststore
    {
        public string Name { get; }
        public string Directory { get; }
        public List<X509Certificate2> Certificates { get; }

        public Truststore(string name, string directory)
        {
            this.Name = name;
            this.Directory = directory;
            this.Certificates = LoadCertificates();
        }

        private List<X509Certificate2> LoadCertificates()
        {
            var certificates = new List<X509Certificate2>();
            if (!System.IO.Directory.Exists(Directory)) throw new DirectoryNotFoundException($"Could not find directory for {this.Name} truststore!");

            var certificateFilePaths = System.IO.Directory.GetFiles(Directory, "*.pem", SearchOption.TopDirectoryOnly);
            foreach (var certificateFilePath in certificateFilePaths)
            {
                var text = File.ReadAllText(certificateFilePath);
                var certificateFileTextBytes = Encoding.ASCII.GetBytes(text);

                try
                {
                    var certificate = new X509Certificate2(certificateFileTextBytes);
                    certificates.Add(certificate);
                }
                catch { } //TODO: Log error
            }

            return certificates;
        }
    }
}
