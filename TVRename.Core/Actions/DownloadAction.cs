using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename.Core.Actions
{
    public class DownloadAction : IAction
    {
        private readonly FileInfo file;
        private readonly string url;

        public string Type => "Download";

        public string Produces => this.file.FullName;

        public DownloadAction(FileInfo file, string url)
        {
            this.file = file;
            this.url = url;
        }

        public async Task Run(CancellationToken ct)
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.GetAsync(this.url, ct))
                {
                    if (!response.IsSuccessStatusCode) throw new HttpException((int)response.StatusCode, await response.Content.ReadAsStringAsync());

                    ct.ThrowIfCancellationRequested();

                    using (Stream stream = await response.Content.ReadAsStreamAsync())
                    using (FileStream file = new FileStream(this.file.FullName, FileMode.Create))
                    {
                        await stream.CopyToAsync(file, 81920, ct);
                    }
                }
            }
        }
    }
}
